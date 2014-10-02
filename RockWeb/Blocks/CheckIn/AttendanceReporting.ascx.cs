﻿// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Web.UI;

namespace RockWeb.Blocks.CheckIn
{
    /// <summary>
    /// Shows a graph of attendance statistics which can be configured for specific groups, date range, etc.
    /// </summary>
    [DisplayName( "Attendance Analysis" )]
    [Category( "Check-in" )]
    [Description( "Shows a graph of attendance statistics which can be configured for specific groups, date range, etc." )]

    [DefinedValueField( Rock.SystemGuid.DefinedType.CHART_STYLES, "Chart Style", DefaultValue = Rock.SystemGuid.DefinedValue.CHART_STYLE_ROCK )]
    [LinkedPage( "Detail Page", "Select the page to navigate to when the chart is clicked" )]
    [GroupTypeField( "Check-in Type", key: "GroupTypeTemplate", groupTypePurposeValueGuid: Rock.SystemGuid.DefinedValue.GROUPTYPE_PURPOSE_CHECKIN_TEMPLATE )]
    public partial class AttendanceReporting : RockBlock
    {
        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );

            gAttendance.GridRebind += gAttendance_GridRebind;

            // GroupTypesUI dynamically creates controls, so we need to rebuild it on every OnInit()
            BuildGroupTypesUI();
        }

        /// <summary>
        /// Handles the GridRebind event of the gAttendance control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gAttendance_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            var chartStyleDefinedValueGuid = this.GetAttributeValue( "ChartStyle" ).AsGuidOrNull();

            lcAttendance.Options.SetChartStyle( chartStyleDefinedValueGuid );

            if ( !Page.IsPostBack )
            {
                LoadDropDowns();
                LoadSettingsFromUserPreferences();
                LoadChart();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the detail page unique identifier.
        /// </summary>
        /// <value>
        /// The detail page unique identifier.
        /// </value>
        public Guid? DetailPageGuid
        {
            get
            {
                return ( GetAttributeValue( "DetailPage" ) ?? string.Empty ).AsGuidOrNull();
            }
        }

        #endregion

        #region Events

        // handlers called by the controls on your block

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            LoadChart();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the drop downs.
        /// </summary>
        public void LoadDropDowns()
        {
            cpCampuses.Campuses = CampusCache.All();
        }

        /// <summary>
        /// Builds the group types UI.
        /// </summary>
        private void BuildGroupTypesUI()
        {
            var rockContext = new RockContext();

            var groupTypeService = new GroupTypeService( rockContext );
            var groupTypeTemplateGuid = this.GetAttributeValue( "GroupTypeTemplate" ).AsGuidOrNull();

            nbGroupTypeWarning.Visible = !groupTypeTemplateGuid.HasValue;
            if ( groupTypeTemplateGuid.HasValue )
            {
                var groupType = groupTypeService.Get( groupTypeTemplateGuid.Value );
                var groupTypes = groupTypeService.GetChildGroupTypes( groupType.Id ).OrderBy( a => a.Order ).ThenBy( a => a.Name );
                rptGroupTypes.DataSource = groupTypes.ToList();
                rptGroupTypes.DataBind();
            }
        }

        /// <summary>
        /// Loads the chart.
        /// </summary>
        public void LoadChart()
        {
            lcAttendance.ShowTooltip = true;
            if ( this.DetailPageGuid.HasValue )
            {
                lcAttendance.ChartClick += lcAttendance_ChartClick;
            }

            var dataSourceUrl = "~/api/Attendances/GetChartData";
            var dataSourceParams = new Dictionary<string, object>();
            var dateRange = SlidingDateRangePicker.CalculateDateRangeFromDelimitedValues( drpSlidingDateRange.DelimitedValues );

            if ( dateRange.Start.HasValue )
            {
                dataSourceParams.AddOrReplace( "startDate", dateRange.Start.Value.ToString( "o" ) );
            }

            if ( dateRange.End.HasValue )
            {
                dataSourceParams.AddOrReplace( "endDate", dateRange.End.Value.ToString( "o" ) );
            }

            var groupBy = hfGroupBy.Value.ConvertToEnumOrNull<AttendanceGroupBy>() ?? AttendanceGroupBy.Week;
            lcAttendance.TooltipFormatter = null;
            switch ( groupBy )
            {
                case AttendanceGroupBy.Week:
                    {
                        lcAttendance.Options.xaxis.tickSize = new string[] { "7", "day" };
                        lcAttendance.TooltipFormatter = @"
function(item) { 
    var itemDate = new Date(item.series.chartData[item.dataIndex].DateTimeStamp);
    var dateText = 'Weekend of <br />' + itemDate.toLocaleDateString();
    var seriesLabel = item.series.label;
    var pointValue = item.series.chartData[item.dataIndex].YValue || item.series.chartData[item.dataIndex].YValueTotal;
    return dateText + '<br />' + seriesLabel + ': ' + pointValue;
}
";
                    }

                    break;
                case AttendanceGroupBy.Month:
                    {
                        lcAttendance.Options.xaxis.tickSize = new string[] { "1", "month" };
                        lcAttendance.TooltipFormatter = @"
function(item) { 
    var month_names = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    var itemDate = new Date(item.series.chartData[item.dataIndex].DateTimeStamp);
    var dateText = month_names[itemDate.getMonth()] + ' ' + itemDate.getFullYear();
    var seriesLabel = item.series.label;
    var pointValue = item.series.chartData[item.dataIndex].YValue || item.series.chartData[item.dataIndex].YValueTotal;
    return dateText + '<br />' + seriesLabel + ': ' + pointValue;
}
";
                    }

                    break;
                case AttendanceGroupBy.Year:
                    {
                        lcAttendance.Options.xaxis.tickSize = new string[] { "1", "year" };
                        lcAttendance.TooltipFormatter = @"
function(item) { 
    var itemDate = new Date(item.series.chartData[item.dataIndex].DateTimeStamp);
    var dateText = itemDate.getFullYear();
    var seriesLabel = item.series.label;
    var pointValue = item.series.chartData[item.dataIndex].YValue || item.series.chartData[item.dataIndex].YValueTotal;
    return dateText + '<br />' + seriesLabel + ': ' + pointValue;
}
";
                    }

                    break;
            }

            dataSourceParams.AddOrReplace( "groupBy", hfGroupBy.Value.AsInteger() );
            dataSourceParams.AddOrReplace( "graphBy", hfGraphBy.Value.AsInteger() );

            dataSourceParams.AddOrReplace( "campusIds", cpCampuses.SelectedCampusIds.AsDelimited( "," ) );
            dataSourceParams.AddOrReplace( "groupIds", GetSelectedGroupIds().AsDelimited( "," ) );

            SaveSettingsToUserPreferences();

            dataSourceUrl += "?" + dataSourceParams.Select( s => string.Format( "{0}={1}", s.Key, s.Value ) ).ToList().AsDelimited( "&" );

            lcAttendance.DataSourceUrl = this.ResolveUrl( dataSourceUrl );

            if ( pnlGrid.Visible )
            {
                BindGrid();
            }
        }

        /// <summary>
        /// Saves the attendance reporting settings to user preferences.
        /// </summary>
        private void SaveSettingsToUserPreferences()
        {
            string keyPrefix = string.Format( "attendance-reporting-{0}-", this.BlockId );

            this.SetUserPreference( keyPrefix + "SlidingDateRange", drpSlidingDateRange.DelimitedValues );
            this.SetUserPreference( keyPrefix + "GroupBy", hfGroupBy.Value );
            this.SetUserPreference( keyPrefix + "GraphBy", hfGraphBy.Value );
            this.SetUserPreference( keyPrefix + "CampusIds", cpCampuses.SelectedCampusIds.AsDelimited( "," ) );

            var selectedGroupIds = GetSelectedGroupIds();

            this.SetUserPreference( keyPrefix + "GroupIds", selectedGroupIds.AsDelimited( "," ) );
        }

        /// <summary>
        /// Gets the selected group ids.
        /// </summary>
        /// <returns></returns>
        private List<int> GetSelectedGroupIds()
        {
            var selectedGroupIds = new List<int>();
            var checkboxListControls = rptGroupTypes.ControlsOfTypeRecursive<RockCheckBoxList>();
            foreach ( var cblGroup in checkboxListControls )
            {
                selectedGroupIds.AddRange( cblGroup.SelectedValuesAsInt );
            }

            return selectedGroupIds;
        }

        /// <summary>
        /// Loads the attendance reporting settings from user preferences.
        /// </summary>
        private void LoadSettingsFromUserPreferences()
        {
            string keyPrefix = string.Format( "attendance-reporting-{0}-", this.BlockId );

            string slidingDateRangeSettings = this.GetUserPreference( keyPrefix + "SlidingDateRange" );
            if ( string.IsNullOrWhiteSpace( slidingDateRangeSettings ) )
            {
                // default to current year
                drpSlidingDateRange.SlidingDateRangeMode = SlidingDateRangePicker.SlidingDateRangeType.Current;
                drpSlidingDateRange.TimeUnit = SlidingDateRangePicker.TimeUnitType.Year;
            }
            else
            {
                drpSlidingDateRange.DelimitedValues = slidingDateRangeSettings;
            }

            hfGroupBy.Value = this.GetUserPreference( keyPrefix + "GroupBy" );
            hfGraphBy.Value = this.GetUserPreference( keyPrefix + "GraphBy" );

            var campusIdList = this.GetUserPreference( keyPrefix + "CampusIds" ).Split( ',' ).ToList();
            cpCampuses.SetValues( campusIdList );

            // if no campuses are selected, default to showing all of them
            if ( cpCampuses.SelectedCampusIds.Count == 0 )
            {
                foreach ( ListItem item in cpCampuses.Items )
                {
                    item.Selected = true;
                }
            }

            var groupIdList = this.GetUserPreference( keyPrefix + "GroupIds" ).Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).ToList();

            // if no groups are selected, default to showing all of them
            var selectAll = groupIdList.Count == 0;

            var checkboxListControls = rptGroupTypes.ControlsOfTypeRecursive<RockCheckBoxList>();
            foreach ( var cblGroup in checkboxListControls )
            {
                foreach ( ListItem item in cblGroup.Items )
                {
                    item.Selected = selectAll || groupIdList.Contains( item.Value );
                }
            }
        }

        /// <summary>
        /// Lcs the attendance_ chart click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        protected void lcAttendance_ChartClick( object sender, ChartClickArgs e )
        {
            if ( this.DetailPageGuid.HasValue )
            {
                Dictionary<string, string> qryString = new Dictionary<string, string>();
                qryString.Add( "YValue", e.YValue.ToString() );
                qryString.Add( "DateTimeValue", e.DateTimeValue.ToString( "o" ) );
                NavigateToPage( this.DetailPageGuid.Value, qryString );
            }
        }

        /// <summary>
        /// Handles the Click event of the lShowGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lShowGrid_Click( object sender, EventArgs e )
        {
            if ( pnlGrid.Visible )
            {
                pnlGrid.Visible = false;
                lShowGrid.Text = "Show Data <i class='fa fa-chevron-down'></i>";
                lShowGrid.ToolTip = "Show Data";
            }
            else
            {
                pnlGrid.Visible = true;
                lShowGrid.Text = "Hide Data <i class='fa fa-chevron-up'></i>";
                lShowGrid.ToolTip = "Hide Data";
                BindGrid();
            }
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            var dateRange = SlidingDateRangePicker.CalculateDateRangeFromDelimitedValues( drpSlidingDateRange.DelimitedValues );

            string groupIds = GetSelectedGroupIds().AsDelimited( "," );
            string campusIds = cpCampuses.SelectedCampusIds.AsDelimited( "," );

            SortProperty sortProperty = gAttendance.SortProperty;

            var chartData = new AttendanceService( new RockContext() ).GetChartData(
                hfGroupBy.Value.ConvertToEnumOrNull<AttendanceGroupBy>() ?? AttendanceGroupBy.Week,
                hfGraphBy.Value.ConvertToEnumOrNull<AttendanceGraphBy>() ?? AttendanceGraphBy.Total,
                dateRange.Start,
                dateRange.End,
                groupIds,
                campusIds );

            if ( sortProperty != null )
            {
                gAttendance.DataSource = chartData.AsQueryable().Sort( sortProperty ).ToList();
            }
            else
            {
                gAttendance.DataSource = chartData.OrderBy( a => a.DateTimeStamp ).ToList();
            }

            gAttendance.DataBind();
        }

        /// <summary>
        /// Handles the ItemDataBound event of the rptGroupTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void rptGroupTypes_ItemDataBound( object sender, RepeaterItemEventArgs e )
        {
            if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
            {
                var groupType = e.Item.DataItem as GroupType;

                var liGroupTypeItem = new HtmlGenericContainer( "li", "rocktree-item rocktree-folder" );
                liGroupTypeItem.ID = "liGroupTypeItem" + groupType.Id;
                e.Item.Controls.Add( liGroupTypeItem );
                AddGroupTypeControls( groupType, liGroupTypeItem );
            }
        }

        /// <summary>
        /// Adds the group type controls.
        /// </summary>
        /// <param name="groupType">Type of the group.</param>
        /// <param name="pnlGroupTypes">The PNL group types.</param>
        private void AddGroupTypeControls( GroupType groupType, HtmlGenericContainer liGroupTypeItem )
        {
            if ( groupType.Groups.Any() )
            {
                var cblGroupTypeGroups = new RockCheckBoxList { ID = "cblGroupTypeGroups" + groupType.Id };

                cblGroupTypeGroups.Label = groupType.Name;
                cblGroupTypeGroups.Items.Clear();
                foreach ( var group in groupType.Groups.OrderBy( a => a.Order ).ThenBy( a => a.Name ).Select( a => new { a.Id, a.Name } ).ToList() )
                {
                    cblGroupTypeGroups.Items.Add( new ListItem( group.Name, group.Id.ToString() ) );
                }

                liGroupTypeItem.Controls.Add( cblGroupTypeGroups );
            }
            else
            {
                if ( groupType.ChildGroupTypes.Any() )
                {
                    liGroupTypeItem.Controls.Add( new Label { Text = groupType.Name, ID = "lbl" + groupType.Name } );
                }
            }

            if ( groupType.ChildGroupTypes.Any() )
            {
                var ulGroupTypeList = new HtmlGenericContainer( "ul", "rocktree-children" );

                liGroupTypeItem.Controls.Add( ulGroupTypeList );
                foreach ( var childGroupType in groupType.ChildGroupTypes.OrderBy( a => a.Order ).ThenBy( a => a.Name ) )
                {
                    var liChildGroupTypeItem = new HtmlGenericContainer( "li", "rocktree-item rocktree-folder" );
                    liChildGroupTypeItem.ID = "liGroupTypeItem" + groupType.Id;
                    ulGroupTypeList.Controls.Add( liChildGroupTypeItem );
                    AddGroupTypeControls( childGroupType, liChildGroupTypeItem );
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnApply control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnApply_Click( object sender, EventArgs e )
        {
            LoadChart();
        }

        #endregion
    }
}