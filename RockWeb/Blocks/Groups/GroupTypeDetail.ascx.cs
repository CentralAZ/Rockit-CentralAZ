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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Attribute = Rock.Model.Attribute;

namespace RockWeb.Blocks.Groups
{
    [DisplayName( "Group Type Detail" )]
    [Category( "Groups" )]
    [Description( "Displays the details of the given group type for editing." )]
    public partial class GroupTypes : RockBlock, IDetailBlock
    {
        #region Child Grid Dictionarys

        /// <summary>
        /// Gets the child group types dictionary.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> ChildGroupTypesDictionary
        {
            get
            {
                Dictionary<int, string> childGroupTypesDictionary = ViewState["ChildGroupTypesDictionary"] as Dictionary<int, string>;
                return childGroupTypesDictionary;
            }

            set
            {
                ViewState["ChildGroupTypesDictionary"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the location types dictionary.
        /// </summary>
        /// <value>
        /// The location types dictionary.
        /// </value>
        private Dictionary<int, string> LocationTypesDictionary
        {
            get
            {
                Dictionary<int, string> locationTypesDictionary = ViewState["LocationTypesDictionary"] as Dictionary<int, string>;
                return locationTypesDictionary;
            }

            set
            {
                ViewState["LocationTypesDictionary"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the group type inherited attributes.
        /// </summary>
        /// <value>
        /// The group type inherited attributes.
        /// </value>
        private List<InheritedAttribute> GroupTypeAttributesInheritedState
        {
            get
            {
                return ViewState["GroupTypeAttributesInheritedState"] as List<InheritedAttribute>;
            }

            set
            {
                ViewState["GroupTypeAttributesInheritedState"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the state of the group type attributes.
        /// </summary>
        /// <value>
        /// The state of the group type attributes.
        /// </value>
        private ViewStateList<Attribute> GroupTypeAttributesState
        {
            get
            {
                return ViewState["GroupTypeAttributesState"] as ViewStateList<Attribute>;
            }

            set
            {
                ViewState["GroupTypeAttributesState"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the group inherited attributes.
        /// </summary>
        /// <value>
        /// The group inherited attributes.
        /// </value>
        private List<InheritedAttribute> GroupAttributesInheritedState
        {
            get
            {
                return ViewState["GroupAttributesInheritedState"] as List<InheritedAttribute>;
            }

            set
            {
                ViewState["GroupAttributesInheritedState"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the state of the group attributes.
        /// </summary>
        /// <value>
        /// The state of the group attributes.
        /// </value>
        private ViewStateList<Attribute> GroupAttributesState
        {
            get
            {
                return ViewState["GroupAttributesState"] as ViewStateList<Attribute>;
            }

            set
            {
                ViewState["GroupAttributesState"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the group member inherited attributes.
        /// </summary>
        /// <value>
        /// The group member inherited attributes.
        /// </value>
        private List<InheritedAttribute> GroupMemberAttributesInheritedState
        {
            get
            {
                return ViewState["GroupMemberAttributesInheritedState"] as List<InheritedAttribute>;
            }

            set
            {
                ViewState["GroupMemberAttributesInheritedState"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the state of the group member attributes.
        /// </summary>
        /// <value>
        /// The state of the group member attributes.
        /// </value>
        private ViewStateList<Attribute> GroupMemberAttributesState
        {
            get
            {
                return ViewState["GroupMemberAttributesState"] as ViewStateList<Attribute>;
            }

            set
            {
                ViewState["GroupMemberAttributesState"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the state of the group type roles.
        /// </summary>
        /// <value>
        /// The state of the group type roles.
        /// </value>
        private ViewStateList<GroupTypeRole> GroupTypeRolesState
        {
            get
            {
                return ViewState["GroupTypeRolesState"] as ViewStateList<GroupTypeRole>;
            }

            set
            {
                ViewState["GroupTypeRolesState"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the default role unique identifier.
        /// </summary>
        /// <value>
        /// The default role unique identifier.
        /// </value>
        protected Guid DefaultRoleGuid
        {
            get
            {
                object value = ViewState["DefaultRoleGuid"];
                if ( value != null )
                {
                    return (Guid)value;
                }
                else
                {
                    return Guid.Empty;
                }
            }

            set
            {
                ViewState["DefaultRoleGuid"] = value;
            }
        }

        #endregion

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            gGroupTypeRoles.DataKeyNames = new string[] { "Guid" };
            gGroupTypeRoles.Actions.ShowAdd = true;
            gGroupTypeRoles.Actions.AddClick += gGroupTypeRoles_Add;
            gGroupTypeRoles.EmptyDataText = Server.HtmlEncode( None.Text );
            gGroupTypeRoles.GridRebind += gGroupTypeRoles_GridRebind;
            gGroupTypeRoles.GridReorder += gGroupTypeRoles_GridReorder;

            gChildGroupTypes.DataKeyNames = new string[] { "key" };
            gChildGroupTypes.Actions.ShowAdd = true;
            gChildGroupTypes.Actions.AddClick += gChildGroupTypes_Add;
            gChildGroupTypes.GridRebind += gChildGroupTypes_GridRebind;
            gChildGroupTypes.EmptyDataText = Server.HtmlEncode( None.Text );

            gLocationTypes.DataKeyNames = new string[] { "key" };
            gLocationTypes.Actions.ShowAdd = true;
            gLocationTypes.Actions.AddClick += gLocationTypes_Add;
            gLocationTypes.GridRebind += gLocationTypes_GridRebind;
            gLocationTypes.EmptyDataText = Server.HtmlEncode( None.Text );

            gGroupTypeAttributesInherited.Actions.ShowAdd = false;
            gGroupTypeAttributesInherited.EmptyDataText = Server.HtmlEncode( None.Text );
            gGroupTypeAttributesInherited.GridRebind += gGroupTypeAttributesInherited_GridRebind;

            gGroupTypeAttributes.DataKeyNames = new string[] { "Guid" };
            gGroupTypeAttributes.Actions.ShowAdd = true;
            gGroupTypeAttributes.Actions.AddClick += gGroupTypeAttributes_Add;
            gGroupTypeAttributes.EmptyDataText = Server.HtmlEncode( None.Text );
            gGroupTypeAttributes.GridRebind += gGroupTypeAttributes_GridRebind;
            gGroupTypeAttributes.GridReorder += gGroupTypeAttributes_GridReorder;

            gGroupAttributesInherited.Actions.ShowAdd = false;
            gGroupAttributesInherited.EmptyDataText = Server.HtmlEncode( None.Text );
            gGroupAttributesInherited.GridRebind += gGroupAttributesInherited_GridRebind;

            gGroupAttributes.DataKeyNames = new string[] { "Guid" };
            gGroupAttributes.Actions.ShowAdd = true;
            gGroupAttributes.Actions.AddClick += gGroupAttributes_Add;
            gGroupAttributes.EmptyDataText = Server.HtmlEncode( None.Text );
            gGroupAttributes.GridRebind += gGroupAttributes_GridRebind;
            gGroupAttributes.GridReorder += gGroupAttributes_GridReorder;

            gGroupMemberAttributesInherited.Actions.ShowAdd = false;
            gGroupMemberAttributesInherited.EmptyDataText = Server.HtmlEncode( None.Text );
            gGroupMemberAttributesInherited.GridRebind += gGroupMemberAttributesInherited_GridRebind;

            gGroupMemberAttributes.DataKeyNames = new string[] { "Guid" };
            gGroupMemberAttributes.Actions.ShowAdd = true;
            gGroupMemberAttributes.Actions.AddClick += gGroupMemberAttributes_Add;
            gGroupMemberAttributes.EmptyDataText = Server.HtmlEncode( None.Text );
            gGroupMemberAttributes.GridRebind += gGroupMemberAttributes_GridRebind;
            gGroupMemberAttributes.GridReorder += gGroupMemberAttributes_GridReorder;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                string itemId = PageParameter( "groupTypeId" );
                if ( !string.IsNullOrWhiteSpace( itemId ) )
                {
                    ShowDetail( "groupTypeId", int.Parse( itemId ) );
                }
                else
                {
                    pnlDetails.Visible = false;
                }
            }
            else
            {
                Guid newDefaultRole = Guid.Empty;
                if ( Guid.TryParse( Request.Form["GroupTypeDefaultRole"], out newDefaultRole ) )
                {
                    DefaultRoleGuid = newDefaultRole;
                }

                ShowDialog();
            }
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            // Persist any changes that might have been made to objects in list
            if ( GroupTypeAttributesState != null )
            {
                GroupTypeAttributesState.SaveViewState();
            }

            if ( GroupAttributesState != null )
            {
                GroupAttributesState.SaveViewState();
            }

            if ( GroupMemberAttributesState != null )
            {
                GroupMemberAttributesState.SaveViewState();
            }

            if ( GroupTypeRolesState != null )
            {
                GroupTypeRolesState.SaveViewState();
            }

            return base.SaveViewState();
        }

        /// <summary>
        /// Returns breadcrumbs specific to the block that should be added to navigation
        /// based on the current page reference.  This function is called during the page's
        /// oninit to load any initial breadcrumbs
        /// </summary>
        /// <param name="pageReference">The page reference.</param>
        /// <returns></returns>
        public override List<BreadCrumb> GetBreadCrumbs( PageReference pageReference )
        {
            var breadCrumbs = new List<BreadCrumb>();

            int? groupTypeId = PageParameter( pageReference, "groupTypeId" ).AsIntegerOrNull();
            if ( groupTypeId != null )
            {
                GroupType groupType = new GroupTypeService( new RockContext() ).Get( groupTypeId.Value );
                if ( groupType != null )
                {
                    breadCrumbs.Add( new BreadCrumb( groupType.Name, pageReference ) );
                }
                else
                {
                    breadCrumbs.Add( new BreadCrumb( "New Group Type", pageReference ) );
                }
            }
            else
            {
                // don't show a breadcrumb if we don't have a pageparam to work with
            }

            return breadCrumbs;
        }

        #endregion

        #region Events

        #region Action Events

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            GroupType groupType;
            var rockContext = new RockContext();
            GroupTypeService groupTypeService = new GroupTypeService( rockContext );
            GroupTypeRoleService groupTypeRoleService = new GroupTypeRoleService( rockContext );
            AttributeService attributeService = new AttributeService( rockContext );
            AttributeQualifierService qualifierService = new AttributeQualifierService( rockContext );
            CategoryService categoryService = new CategoryService( rockContext );

            int groupTypeId = int.Parse( hfGroupTypeId.Value );

            if ( groupTypeId == 0 )
            {
                groupType = new GroupType();
                groupTypeService.Add( groupType );
            }
            else
            {
                groupType = groupTypeService.Get( groupTypeId );

                // selected roles
                var selectedRoleGuids = GroupTypeRolesState.Select( r => r.Guid );
                foreach ( var role in groupType.Roles.Where( r => !selectedRoleGuids.Contains( r.Guid ) ).ToList() )
                {
                    groupType.Roles.Remove( role );
                    groupTypeRoleService.Delete( role );
                }
            }

            foreach ( var roleState in GroupTypeRolesState )
            {
                GroupTypeRole role = groupType.Roles.Where( r => r.Guid == roleState.Guid ).FirstOrDefault();
                if ( role == null )
                {
                    role = new GroupTypeRole();
                    groupType.Roles.Add( role );
                }
                else
                {
                    roleState.Id = role.Id;
                    roleState.Guid = role.Guid;
                }

                role.CopyPropertiesFrom( roleState );
            }

            GroupLocationPickerMode locationSelectionMode = GroupLocationPickerMode.None;
            foreach ( ListItem li in cblLocationSelectionModes.Items )
            {
                if ( li.Selected )
                {
                    locationSelectionMode = locationSelectionMode | (GroupLocationPickerMode)li.Value.AsInteger();
                }
            }

            groupType.Name = tbName.Text;
            groupType.Description = tbDescription.Text;
            groupType.GroupTerm = tbGroupTerm.Text;
            groupType.GroupMemberTerm = tbGroupMemberTerm.Text;
            groupType.ShowInGroupList = cbShowInGroupList.Checked;
            groupType.ShowInNavigation = cbShowInNavigation.Checked;
            groupType.IconCssClass = tbIconCssClass.Text;
            groupType.TakesAttendance = cbTakesAttendance.Checked;
            groupType.AttendanceRule = ddlAttendanceRule.SelectedValueAsEnum<AttendanceRule>();
            groupType.AttendancePrintTo = ddlPrintTo.SelectedValueAsEnum<PrintTo>();
            groupType.LocationSelectionMode = locationSelectionMode;
            groupType.GroupTypePurposeValueId = ddlGroupTypePurpose.SelectedValueAsInt();
            groupType.AllowMultipleLocations = cbAllowMultipleLocations.Checked;
            groupType.InheritedGroupTypeId = gtpInheritedGroupType.SelectedGroupTypeId;

            groupType.ChildGroupTypes = new List<GroupType>();
            groupType.ChildGroupTypes.Clear();
            foreach ( var item in ChildGroupTypesDictionary )
            {
                var childGroupType = groupTypeService.Get( item.Key );
                if ( childGroupType != null )
                {
                    groupType.ChildGroupTypes.Add( childGroupType );
                }
            }

            DefinedValueService definedValueService = new DefinedValueService( rockContext );

            groupType.LocationTypes = new List<GroupTypeLocationType>();
            groupType.LocationTypes.Clear();
            foreach ( var item in LocationTypesDictionary )
            {
                var locationType = definedValueService.Get( item.Key );
                if ( locationType != null )
                {
                    groupType.LocationTypes.Add( new GroupTypeLocationType { LocationTypeValueId = locationType.Id } );
                }
            }

            if ( !groupType.IsValid )
            {
                // Controls will render the error messages                    
                return;
            }

            // need WrapTransaction due to Attribute saves    
            RockTransactionScope.WrapTransaction( () =>
            {
                rockContext.SaveChanges();

                /* Save Attributes */
                string qualifierValue = groupType.Id.ToString();
                SaveAttributes( new GroupType().TypeId, "Id", qualifierValue, GroupTypeAttributesState, rockContext );
                SaveAttributes( new Group().TypeId, "GroupTypeId", qualifierValue, GroupAttributesState, rockContext );
                SaveAttributes( new GroupMember().TypeId, "GroupTypeId", qualifierValue, GroupMemberAttributesState, rockContext );

                // Reload to save default role
                groupType = groupTypeService.Get( groupType.Id );
                groupType.DefaultGroupRole = groupType.Roles.FirstOrDefault( r => r.Guid.Equals( DefaultRoleGuid ) );
                if ( groupType.DefaultGroupRole == null )
                {
                    groupType.DefaultGroupRole = groupType.Roles.FirstOrDefault();
                }

                rockContext.SaveChanges();

                // Reload the roles and apply their attribute values
                foreach ( var role in groupTypeRoleService.GetByGroupTypeId( groupType.Id ).ToList() )
                {
                    role.LoadAttributes( rockContext );
                    var roleState = GroupTypeRolesState.Where( r => r.Guid.Equals( role.Guid ) ).FirstOrDefault();
                    if ( roleState != null && roleState.AttributeValues != null )
                    {
                        foreach ( var attributeValue in roleState.AttributeValues )
                        {
                            role.SetAttributeValue( attributeValue.Key, roleState.GetAttributeValue( attributeValue.Key ) );
                        }

                        role.SaveAttributeValues( rockContext );
                    }
                }
            } );

            GroupTypeCache.Flush( groupType.Id );

            NavigateToParentPage();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnCancel_Click( object sender, EventArgs e )
        {
            NavigateToParentPage();
        }

        #endregion

        #region Control Events

        /// <summary>
        /// Handles the SelectedIndexChanged event of the gtpInheritedGroupType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gtpInheritedGroupType_SelectedIndexChanged( object sender, EventArgs e )
        {
            var rockContext = new RockContext();
            var groupTypeService = new GroupTypeService( rockContext );
            var attributeService = new AttributeService( rockContext );
            BindInheritedAttributes( gtpInheritedGroupType.SelectedValueAsInt(), groupTypeService, attributeService );
        }

        #endregion

        #endregion

        #region Internal Methods

        /// <summary>
        /// Shows the edit.
        /// </summary>
        /// <param name="itemKey">The item key.</param>
        /// <param name="itemKeyValue">The item key value.</param>
        public void ShowDetail( string itemKey, int itemKeyValue )
        {
            pnlDetails.Visible = false;

            if ( !itemKey.Equals( "groupTypeId" ) )
            {
                return;
            }

            bool editAllowed = true;

            GroupType groupType = null;

            if ( !itemKeyValue.Equals( 0 ) )
            {
                groupType = new GroupTypeService( new RockContext() ).Get( itemKeyValue );
                editAllowed = groupType.IsAuthorized( Authorization.EDIT, CurrentPerson );
            }
            else
            {
                groupType = new GroupType { Id = 0, ShowInGroupList = true, GroupTerm = "Group", GroupMemberTerm = "Member" };
                groupType.ChildGroupTypes = new List<GroupType>();
                groupType.LocationTypes = new List<GroupTypeLocationType>();

                Guid defaultRoleGuid = Guid.NewGuid();
                var memberRole = new GroupTypeRole { Guid = defaultRoleGuid, Name = "Member" };
                groupType.Roles.Add( memberRole );
                groupType.DefaultGroupRole = memberRole;

                groupType.LocationSelectionMode = GroupLocationPickerMode.None;
            }

            if ( groupType == null )
            {
                return;
            }

            DefaultRoleGuid = groupType.DefaultGroupRole != null ? groupType.DefaultGroupRole.Guid : Guid.Empty;

            pnlDetails.Visible = true;
            hfGroupTypeId.Value = groupType.Id.ToString();

            bool readOnly = false;

            nbEditModeMessage.Text = string.Empty;
            if ( !editAllowed || !IsUserAuthorized( Authorization.EDIT ) )
            {
                readOnly = true;
                nbEditModeMessage.Text = EditModeMessage.ReadOnlyEditActionNotAllowed( GroupType.FriendlyTypeName );
            }

            if ( groupType.IsSystem )
            {
                nbEditModeMessage.Text = EditModeMessage.System( GroupType.FriendlyTypeName );
            }

            if ( readOnly )
            {
                ShowReadonlyDetails( groupType );
            }
            else
            {
                ShowEditDetails( groupType );
            }
        }

        /// <summary>
        /// Shows the edit details.
        /// </summary>
        /// <param name="groupType">Type of the group.</param>
        private void ShowEditDetails( GroupType groupType )
        {
            hlType.Visible = false;
            if ( groupType.Id == 0 )
            {
                lReadOnlyTitle.Text = ActionTitle.Add( GroupType.FriendlyTypeName ).FormatAsHtmlTitle();
                if ( groupType.GroupTypePurposeValue != null )
                {
                    hlType.Text = groupType.GroupTypePurposeValue.Name;
                    hlType.Visible = true;
                }
            }
            else
            {
                lReadOnlyTitle.Text = groupType.Name.FormatAsHtmlTitle();
            }

            SetEditMode( true );

            var rockContext = new RockContext();

            var groupTypeService = new GroupTypeService( rockContext );
            var attributeService = new AttributeService( rockContext );

            LoadDropDowns( groupType.Id );

            // General
            tbName.ReadOnly = groupType.IsSystem;
            tbName.Text = groupType.Name;

            tbDescription.ReadOnly = groupType.IsSystem;
            tbDescription.Text = groupType.Description;

            tbGroupTerm.ReadOnly = groupType.IsSystem;
            tbGroupTerm.Text = groupType.GroupTerm;

            tbGroupMemberTerm.ReadOnly = groupType.IsSystem;
            tbGroupMemberTerm.Text = groupType.GroupMemberTerm;

            ddlGroupTypePurpose.Enabled = !groupType.IsSystem;
            ddlGroupTypePurpose.SetValue( groupType.GroupTypePurposeValueId );

            ChildGroupTypesDictionary = new Dictionary<int, string>();
            groupType.ChildGroupTypes.ToList().ForEach( a => ChildGroupTypesDictionary.Add( a.Id, a.Name ) );
            BindChildGroupTypesGrid();

            // Display
            cbShowInGroupList.Checked = groupType.ShowInGroupList;
            cbShowInNavigation.Checked = groupType.ShowInNavigation;
            tbIconCssClass.Text = groupType.IconCssClass;

            // Locations
            cbAllowMultipleLocations.Enabled = !groupType.IsSystem;
            cbAllowMultipleLocations.Checked = groupType.AllowMultipleLocations;

            cblLocationSelectionModes.Enabled = !groupType.IsSystem;
            cblLocationSelectionModes.Enabled = true;
            foreach ( ListItem li in cblLocationSelectionModes.Items )
            {
                GroupLocationPickerMode mode = (GroupLocationPickerMode)li.Value.AsInteger();
                li.Selected = ( groupType.LocationSelectionMode & mode ) == mode;
            }

            LocationTypesDictionary = new Dictionary<int, string>();
            groupType.LocationTypes.ToList().ForEach( a => LocationTypesDictionary.Add( a.LocationTypeValueId, a.LocationTypeValue.Name ) );
            BindLocationTypesGrid();

            // Check In
            cbTakesAttendance.Checked = groupType.TakesAttendance;
            ddlAttendanceRule.SetValue( (int)groupType.AttendanceRule );
            ddlPrintTo.SetValue( (int)groupType.AttendancePrintTo );

            // Attributes
            gtpInheritedGroupType.Enabled = !groupType.IsSystem;
            gtpInheritedGroupType.SelectedGroupTypeId = groupType.InheritedGroupTypeId;

            var groupTypeRoles = new List<GroupTypeRole>();
            foreach ( var role in groupType.Roles )
            {
                role.LoadAttributes();
                groupTypeRoles.Add( role );
            }
            GroupTypeRolesState = new ViewStateList<GroupTypeRole>();
            GroupTypeRolesState.AddAll( groupTypeRoles );

            BindGroupTypeRolesGrid();

            string qualifierValue = groupType.Id.ToString();

            GroupTypeAttributesState = new ViewStateList<Attribute>();
            GroupTypeAttributesState.AddAll( attributeService.GetByEntityTypeId( new GroupType().TypeId ).AsQueryable()
                .Where( a =>
                    a.EntityTypeQualifierColumn.Equals( "Id", StringComparison.OrdinalIgnoreCase ) &&
                    a.EntityTypeQualifierValue.Equals( qualifierValue ) )
                .OrderBy( a => a.Order )
                .ThenBy( a => a.Name )
                .ToList() );
            BindGroupTypeAttributesGrid();

            GroupAttributesState = new ViewStateList<Attribute>();
            GroupAttributesState.AddAll( attributeService.GetByEntityTypeId( new Group().TypeId ).AsQueryable()
                .Where( a =>
                    a.EntityTypeQualifierColumn.Equals( "GroupTypeId", StringComparison.OrdinalIgnoreCase ) &&
                    a.EntityTypeQualifierValue.Equals( qualifierValue ) )
                .OrderBy( a => a.Order )
                .ThenBy( a => a.Name )
                .ToList() );
            BindGroupAttributesGrid();

            GroupMemberAttributesState = new ViewStateList<Attribute>();
            GroupMemberAttributesState.AddAll( attributeService.GetByEntityTypeId( new GroupMember().TypeId ).AsQueryable()
                .Where( a =>
                    a.EntityTypeQualifierColumn.Equals( "GroupTypeId", StringComparison.OrdinalIgnoreCase ) &&
                    a.EntityTypeQualifierValue.Equals( qualifierValue ) )
                .OrderBy( a => a.Order )
                .ThenBy( a => a.Name )
                .ToList() );
            BindGroupMemberAttributesGrid();

            BindInheritedAttributes( groupType.InheritedGroupTypeId, groupTypeService, attributeService );
        }

        /// <summary>
        /// Shows the readonly details.
        /// </summary>
        /// <param name="group">The group.</param>
        private void ShowReadonlyDetails( GroupType groupType )
        {
            SetEditMode( false );

            hfGroupTypeId.SetValue( groupType.Id );
            lReadOnlyTitle.Text = groupType.Name.FormatAsHtmlTitle();
            if ( groupType.GroupTypePurposeValue != null )
            {
                hlType.Text = groupType.GroupTypePurposeValue.Name;
                hlType.Visible = true;
            }
            else
            {
                hlType.Visible = false;
            }

            lGroupTypeDescription.Text = groupType.Description;

            DescriptionList descriptionList = new DescriptionList();
            descriptionList.Add( string.Empty, string.Empty );
            lblMainDetails.Text = descriptionList.Html;
        }

        /// <summary>
        /// Sets the edit mode.
        /// </summary>
        /// <param name="editable">if set to <c>true</c> [editable].</param>
        private void SetEditMode( bool editable )
        {
            pnlEditDetails.Visible = editable;
            pnlViewDetails.Visible = !editable;

            this.HideSecondaryBlocks( editable );
        }

        /// <summary>
        /// Loads the drop downs.
        /// </summary>
        private void LoadDropDowns( int? groupTypeId )
        {
            ddlAttendanceRule.BindToEnum( typeof( Rock.Model.AttendanceRule ) );

            cblLocationSelectionModes.Items.Clear();
            cblLocationSelectionModes.Items.Add( new ListItem( "Named", "2" ) );
            cblLocationSelectionModes.Items.Add( new ListItem( "Address", "1" ) );
            cblLocationSelectionModes.Items.Add( new ListItem( "Point", "4" ) );
            cblLocationSelectionModes.Items.Add( new ListItem( "Geo-fence", "8" ) );
            cblLocationSelectionModes.Items.Add( new ListItem( "Group Member Address", "16" ) );

            var rockContext = new RockContext();
            gtpInheritedGroupType.GroupTypes = new GroupTypeService( rockContext ).Queryable()
                .Where( g => g.Id != groupTypeId )
                .ToList();

            var groupTypePurposeList = new DefinedValueService( rockContext ).GetByDefinedTypeGuid( new Guid( Rock.SystemGuid.DefinedType.GROUPTYPE_PURPOSE ) ).OrderBy( a => a.Name ).ToList();

            ddlGroupTypePurpose.Items.Clear();
            ddlGroupTypePurpose.Items.Add( Rock.Constants.None.ListItem );
            foreach ( var item in groupTypePurposeList )
            {
                ddlGroupTypePurpose.Items.Add( new ListItem( item.Name, item.Id.ToString() ) );
            }
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void ShowDialog( string dialog, bool setValues = false )
        {
            hfActiveDialog.Value = dialog.ToUpper().Trim();
            ShowDialog( setValues );
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void ShowDialog( bool setValues = false )
        {
            switch ( hfActiveDialog.Value )
            {
                case "GROUPTYPEROLES":

                    var role = GroupTypeRolesState.FirstOrDefault( r => r.Guid.Equals( hfRoleGuid.Value.AsGuid() ) );
                    if ( role == null )
                    {
                        role = new GroupTypeRole();
                        role.GroupTypeId = hfGroupTypeId.ValueAsInt();
                        role.LoadAttributes();
                    }

                    Helper.AddEditControls( role, phGroupTypeRoleAttributes, setValues );
                    SetValidationGroup( phGroupTypeRoleAttributes.Controls, dlgGroupTypeRoles.ValidationGroup );

                    dlgGroupTypeRoles.Show();
                    break;

                case "CHILDGROUPTYPES":
                    dlgChildGroupType.Show();
                    break;
                case "LOCATIONTYPE":
                    dlgLocationType.Show();
                    break;
                case "GROUPTYPEATTRIBUTES":
                    dlgGroupTypeAttribute.Show();
                    break;
                case "GROUPATTRIBUTES":
                    dlgGroupAttribute.Show();
                    break;
                case "GROUPMEMBERATTRIBUTES":
                    dlgGroupMemberAttribute.Show();
                    break;
            }
        }

        /// <summary>
        /// Hides the dialog.
        /// </summary>
        private void HideDialog()
        {
            switch ( hfActiveDialog.Value )
            {
                case "GROUPTYPEROLES":
                    dlgGroupTypeRoles.Hide();
                    break;
                case "CHILDGROUPTYPES":
                    dlgChildGroupType.Hide();
                    break;
                case "LOCATIONTYPE":
                    dlgLocationType.Hide();
                    break;
                case "GROUPTYPEATTRIBUTES":
                    dlgGroupTypeAttribute.Hide();
                    break;
                case "GROUPATTRIBUTES":
                    dlgGroupAttribute.Hide();
                    break;
                case "GROUPMEMBERATTRIBUTES":
                    dlgGroupMemberAttribute.Hide();
                    break;
            }

            hfActiveDialog.Value = string.Empty;
        }

        /// <summary>
        /// Binds the inherited attributes.
        /// </summary>
        /// <param name="inheritedGroupTypeId">The inherited group type identifier.</param>
        /// <param name="groupTypeService">The group type service.</param>
        /// <param name="attributeService">The attribute service.</param>
        private void BindInheritedAttributes( int? inheritedGroupTypeId, GroupTypeService groupTypeService, AttributeService attributeService )
        {
            GroupTypeAttributesInheritedState = new List<InheritedAttribute>();
            GroupAttributesInheritedState = new List<InheritedAttribute>();
            GroupMemberAttributesInheritedState = new List<InheritedAttribute>();

            while ( inheritedGroupTypeId.HasValue )
            {
                var inheritedGroupType = groupTypeService.Get( inheritedGroupTypeId.Value );
                if ( inheritedGroupType != null )
                {
                    string qualifierValue = inheritedGroupType.Id.ToString();

                    foreach ( var attribute in attributeService.GetByEntityTypeId( new GroupType().TypeId ).AsQueryable()
                        .Where( a =>
                            a.EntityTypeQualifierColumn.Equals( "Id", StringComparison.OrdinalIgnoreCase ) &&
                            a.EntityTypeQualifierValue.Equals( qualifierValue ) )
                        .OrderBy( a => a.Order )
                        .ThenBy( a => a.Name )
                        .ToList() )
                    {
                        GroupTypeAttributesInheritedState.Add( new InheritedAttribute(
                            attribute.Name,
                            attribute.Key,
                            attribute.Description,
                            Page.ResolveUrl( "~/GroupType/" + attribute.EntityTypeQualifierValue ),
                            inheritedGroupType.Name ) );
                    }

                    foreach ( var attribute in attributeService.GetByEntityTypeId( new Group().TypeId ).AsQueryable()
                        .Where( a =>
                            a.EntityTypeQualifierColumn.Equals( "GroupTypeId", StringComparison.OrdinalIgnoreCase ) &&
                            a.EntityTypeQualifierValue.Equals( qualifierValue ) )
                        .OrderBy( a => a.Order )
                        .ThenBy( a => a.Name )
                        .ToList() )
                    {
                        GroupAttributesInheritedState.Add( new InheritedAttribute(
                            attribute.Name,
                            attribute.Key,
                            attribute.Description,
                            Page.ResolveUrl( "~/GroupType/" + attribute.EntityTypeQualifierValue ),
                            inheritedGroupType.Name ) );
                    }

                    foreach ( var attribute in attributeService.GetByEntityTypeId( new GroupMember().TypeId ).AsQueryable()
                        .Where( a =>
                            a.EntityTypeQualifierColumn.Equals( "GroupTypeId", StringComparison.OrdinalIgnoreCase ) &&
                            a.EntityTypeQualifierValue.Equals( qualifierValue ) )
                        .OrderBy( a => a.Order )
                        .ThenBy( a => a.Name )
                        .ToList() )
                    {
                        GroupMemberAttributesInheritedState.Add( new InheritedAttribute(
                            attribute.Name,
                            attribute.Key,
                            attribute.Description,
                            Page.ResolveUrl( "~/GroupType/" + attribute.EntityTypeQualifierValue ),
                            inheritedGroupType.Name ) );
                    }

                    inheritedGroupTypeId = inheritedGroupType.InheritedGroupTypeId;
                }
                else
                {
                    inheritedGroupTypeId = null;
                }
            }

            BindGroupTypeAttributesInheritedGrid();
            BindGroupAttributesInheritedGrid();
            BindGroupMemberAttributesInheritedGrid();
        }

        /// <summary>
        /// Sets the group type role list order.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        private void SetGroupTypeRoleListOrder( ViewStateList<GroupTypeRole> itemList )
        {
            int order = 0;
            itemList.OrderBy( a => a.Order ).ToList().ForEach( a => a.Order = order++ );
        }

        /// <summary>
        /// Reorders the group type role list.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        private void ReorderGroupTypeRoleList( ViewStateList<GroupTypeRole> itemList, int oldIndex, int newIndex )
        {
            var movedItem = itemList.Where( a => a.Order == oldIndex ).FirstOrDefault();
            if ( movedItem != null )
            {
                if ( newIndex < oldIndex )
                {
                    // Moved up
                    foreach ( var otherItem in itemList.Where( a => a.Order < oldIndex && a.Order >= newIndex ) )
                    {
                        otherItem.Order = otherItem.Order + 1;
                    }
                }
                else
                {
                    // Moved Down
                    foreach ( var otherItem in itemList.Where( a => a.Order > oldIndex && a.Order <= newIndex ) )
                    {
                        otherItem.Order = otherItem.Order - 1;
                    }
                }

                movedItem.Order = newIndex;
            }
        }

        /// <summary>
        /// Sets the attribute list order.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        private void SetAttributeListOrder( ViewStateList<Attribute> itemList )
        {
            int order = 0;
            itemList.OrderBy( a => a.Order ).ToList().ForEach( a => a.Order = order++ );
        }

        /// <summary>
        /// Reorders the attribute list.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        private void ReorderAttributeList( ViewStateList<Attribute> itemList, int oldIndex, int newIndex )
        {
            var movedItem = itemList.Where( a => a.Order == oldIndex ).FirstOrDefault();
            if ( movedItem != null )
            {
                if ( newIndex < oldIndex )
                {
                    // Moved up
                    foreach ( var otherItem in itemList.Where( a => a.Order < oldIndex && a.Order >= newIndex ) )
                    {
                        otherItem.Order = otherItem.Order + 1;
                    }
                }
                else
                {
                    // Moved Down
                    foreach ( var otherItem in itemList.Where( a => a.Order > oldIndex && a.Order <= newIndex ) )
                    {
                        otherItem.Order = otherItem.Order - 1;
                    }
                }

                movedItem.Order = newIndex;
            }
        }

        /// <summary>
        /// Saves the attributes.
        /// </summary>
        /// <param name="entityTypeId">The entity type identifier.</param>
        /// <param name="qualifierColumn">The qualifier column.</param>
        /// <param name="qualifierValue">The qualifier value.</param>
        /// <param name="viewStateAttributes">The view state attributes.</param>
        /// <param name="attributeService">The attribute service.</param>
        /// <param name="qualifierService">The qualifier service.</param>
        /// <param name="categoryService">The category service.</param>
        private void SaveAttributes( int entityTypeId, string qualifierColumn, string qualifierValue, ViewStateList<Attribute> viewStateAttributes, RockContext rockContext )
        {
            // Get the existing attributes for this entity type and qualifier value
            var attributeService = new AttributeService( rockContext );
            var attributes = attributeService.Get( entityTypeId, qualifierColumn, qualifierValue );

            // Delete any of those attributes that were removed in the UI
            var selectedAttributeGuids = viewStateAttributes.Select( a => a.Guid );
            foreach ( var attr in attributes.Where( a => !selectedAttributeGuids.Contains( a.Guid ) ) )
            {
                attributeService.Delete( attr );
                rockContext.SaveChanges();
                Rock.Web.Cache.AttributeCache.Flush( attr.Id );
            }

            // Update the Attributes that were assigned in the UI
            foreach ( var attributeState in viewStateAttributes )
            {
                Helper.SaveAttributeEdits( attributeState, entityTypeId, qualifierColumn, qualifierValue, rockContext );
            }
        }

        #endregion

        #region GroupTypeRoles Grid and Picker

        /// <summary>
        /// Handles the Add event of the gGroupTypeRoles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupTypeRoles_Add( object sender, EventArgs e )
        {
            gGroupTypeRoles_ShowEdit( Guid.Empty );
        }

        /// <summary>
        /// Handles the Edit event of the gGroupTypeRoles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gGroupTypeRoles_Edit( object sender, RowEventArgs e )
        {
            Guid attributeGuid = (Guid)e.RowKeyValue;
            gGroupTypeRoles_ShowEdit( attributeGuid );
        }

        /// <summary>
        /// Gs the group attributes_ show edit.
        /// </summary>
        /// <param name="attributeGuid">The attribute GUID.</param>
        protected void gGroupTypeRoles_ShowEdit( Guid groupTypeRoleGuid )
        {
            GroupTypeRole groupTypeRole = GroupTypeRolesState.FirstOrDefault( a => a.Guid.Equals( groupTypeRoleGuid ) );
            if (groupTypeRole == null)
            {
                groupTypeRole = new GroupTypeRole();
                dlgGroupTypeRoles.Title = "Add Role";
            }
            else
            {
                dlgGroupTypeRoles.Title = "Edit Role";
            }

            hfRoleGuid.Value = groupTypeRole.Guid.ToString();
            tbRoleName.Text = groupTypeRole.Name;
            tbRoleDescription.Text = groupTypeRole.Description;

            string groupTerm = string.IsNullOrWhiteSpace( tbGroupTerm.Text ) ? "Group" : tbGroupTerm.Text;
            string memberTerm = string.IsNullOrWhiteSpace( tbGroupMemberTerm.Text ) ? "Member" : tbGroupMemberTerm.Text;

            nbMinimumRequired.Text = groupTypeRole.MinCount.HasValue ? groupTypeRole.MinCount.ToString() : string.Empty;
            nbMinimumRequired.Help = string.Format(
                "The minimum number of {0} in this {1} that are required to have this role.",
                memberTerm.Pluralize(),
                groupTerm );

            nbMaximumAllowed.Text = groupTypeRole.MaxCount.HasValue ? groupTypeRole.MaxCount.ToString() : string.Empty;
            nbMaximumAllowed.Help = string.Format(
                "The maximum number of {0} in this {1} that are allowed to have this role.",
                memberTerm.Pluralize(),
                groupTerm );

            ShowDialog( "GroupTypeRoles", true );
        }

        /// <summary>
        /// Handles the GridReorder event of the gGroupTypeRoles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridReorderEventArgs"/> instance containing the event data.</param>
        protected void gGroupTypeRoles_GridReorder( object sender, GridReorderEventArgs e )
        {
            ReorderGroupTypeRoleList( GroupTypeRolesState, e.OldIndex, e.NewIndex );
            BindGroupTypeRolesGrid();
        }

        /// <summary>
        /// Handles the Delete event of the gGroupTypeRoles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected void gGroupTypeRoles_Delete( object sender, RowEventArgs e )
        {
            Guid rowGuid = (Guid)e.RowKeyValue;
            GroupTypeRolesState.RemoveEntity( rowGuid );

            BindGroupTypeRolesGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gGroupTypeRoles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupTypeRoles_GridRebind( object sender, EventArgs e )
        {
            BindGroupTypeRolesGrid();
        }

        /// <summary>
        /// Handles the SaveClick event of the dlgGroupMemberAttribute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gGroupTypeRoles_SaveClick( object sender, EventArgs e )
        {
            var groupTypeRole = new GroupTypeRole();

            var groupTypeRoleState = GroupTypeRolesState.FirstOrDefault( r => r.Guid.Equals( hfRoleGuid.Value.AsGuid() ) );
            if ( groupTypeRoleState != null )
            {
                groupTypeRole.CopyPropertiesFrom( groupTypeRoleState );
                GroupTypeRolesState.RemoveEntity( groupTypeRoleState.Guid );
            }
            else
            {
                groupTypeRole.Order = GroupTypeRolesState.Any() ? GroupTypeRolesState.Max( a => a.Order ) + 1 : 0;
                groupTypeRole.GroupTypeId = hfGroupTypeId.ValueAsInt();
            }

            groupTypeRole.Name = tbRoleName.Text;
            groupTypeRole.Description = tbRoleDescription.Text;

            int result;

            groupTypeRole.MinCount = null;
            if ( int.TryParse( nbMinimumRequired.Text, out result ) )
            {
                groupTypeRole.MinCount = result;
            }

            groupTypeRole.MaxCount = null;
            if ( int.TryParse( nbMaximumAllowed.Text, out result ) )
            {
                groupTypeRole.MaxCount = result;
            }

            groupTypeRole.LoadAttributes();
            Helper.GetEditValues( phGroupTypeRoleAttributes, groupTypeRole );

            // Controls will show warnings
            if ( !groupTypeRole.IsValid )
            {
                return;
            }

            GroupTypeRolesState.Add( groupTypeRole );

            BindGroupTypeRolesGrid();
            HideDialog();
        }

        /// <summary>
        /// Binds the group type attributes grid.
        /// </summary>
        private void BindGroupTypeRolesGrid()
        {
            SetGroupTypeRoleListOrder( GroupTypeRolesState );
            gGroupTypeRoles.DataSource = GroupTypeRolesState.OrderBy( a => a.Order ).ToList();
            gGroupTypeRoles.DataBind();
        }

        protected void cvAllowed_ServerValidate( object source, System.Web.UI.WebControls.ServerValidateEventArgs args )
        {
            int? lowerValue = null;
            int value = int.MinValue;
            if ( int.TryParse( nbMinimumRequired.Text, out value ) )
            {
                lowerValue = value;
            }

            int? upperValue = null;
            value = int.MinValue;
            if ( int.TryParse( nbMaximumAllowed.Text, out value ) )
            {
                upperValue = value;
            }

            if ( lowerValue.HasValue && upperValue.HasValue && upperValue.Value < lowerValue.Value )
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        #endregion

        #region Child GroupType Grid and Picker

        /// <summary>
        /// Handles the Add event of the gChildGroupTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gChildGroupTypes_Add( object sender, EventArgs e )
        {
            GroupTypeService groupTypeService = new GroupTypeService( new RockContext() );
            int currentGroupTypeId = int.Parse( hfGroupTypeId.Value );

            // populate dropdown with all grouptypes that aren't already childgroups
            var qry = from gt in groupTypeService.Queryable()
                      where !( from cgt in ChildGroupTypesDictionary.Keys
                               select cgt ).Contains( gt.Id )
                      select gt;

            List<GroupType> list = qry.ToList();
            if ( list.Count == 0 )
            {
                modalAlert.Show( "There are not any other group types that can be added", ModalAlertType.Warning );
            }
            else
            {
                ddlChildGroupType.DataSource = list;
                ddlChildGroupType.DataBind();
                ShowDialog( "ChildGroupTypes" );
            }
        }

        /// <summary>
        /// Handles the Delete event of the gChildGroupTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gChildGroupTypes_Delete( object sender, RowEventArgs e )
        {
            int childGroupTypeId = e.RowKeyId;
            ChildGroupTypesDictionary.Remove( childGroupTypeId );
            BindChildGroupTypesGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gChildGroupTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gChildGroupTypes_GridRebind( object sender, EventArgs e )
        {
            BindChildGroupTypesGrid();
        }

        /// <summary>
        /// Binds the child group types grid.
        /// </summary>
        private void BindChildGroupTypesGrid()
        {
            gChildGroupTypes.DataSource = ChildGroupTypesDictionary.OrderBy( a => a.Value ).ToList();
            gChildGroupTypes.DataBind();
        }

        /// <summary>
        /// Handles the SaveClick event of the dlgChildGroupType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void dlgChildGroupType_SaveClick( object sender, EventArgs e )
        {
            ChildGroupTypesDictionary.Add( int.Parse( ddlChildGroupType.SelectedValue ), ddlChildGroupType.SelectedItem.Text );
            BindChildGroupTypesGrid();
            HideDialog();
        }

        #endregion

        #region LocationType Grid and Picker

        /// <summary>
        /// Handles the Add event of the gLocationTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gLocationTypes_Add( object sender, EventArgs e )
        {
            DefinedValueService definedValueService = new DefinedValueService( new RockContext() );

            // populate dropdown with all locationtypes that aren't already locationtypes
            var qry = from dlt in definedValueService.GetByDefinedTypeGuid( new Guid( Rock.SystemGuid.DefinedType.GROUP_LOCATION_TYPE ) )
                      where !( from lt in LocationTypesDictionary.Keys
                               select lt ).Contains( dlt.Id )
                      select dlt;

            List<DefinedValue> list = qry.ToList();
            if ( list.Count == 0 )
            {
                modalAlert.Show( "There are not any location types defined.  Before you can add location types to a group type, you will first need to add them using Defined Type/Values", ModalAlertType.Warning );
            }
            else
            {
                ddlLocationType.DataSource = list;
                ddlLocationType.DataBind();
                ShowDialog( "LocationType" );
            }
        }

        /// <summary>
        /// Handles the Delete event of the gLocationTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gLocationTypes_Delete( object sender, RowEventArgs e )
        {
            int locationTypeId = e.RowKeyId;
            LocationTypesDictionary.Remove( locationTypeId );
            BindLocationTypesGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gLocationTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gLocationTypes_GridRebind( object sender, EventArgs e )
        {
            BindLocationTypesGrid();
        }

        /// <summary>
        /// Binds the location types grid.
        /// </summary>
        private void BindLocationTypesGrid()
        {
            gLocationTypes.DataSource = LocationTypesDictionary.OrderBy( a => a.Value ).ToList();
            gLocationTypes.DataBind();
        }

        /// <summary>
        /// Handles the SaveClick event of the dlgLocationType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void dlgLocationType_SaveClick( object sender, EventArgs e )
        {
            LocationTypesDictionary.Add( int.Parse( ddlLocationType.SelectedValue ), ddlLocationType.SelectedItem.Text );
            BindLocationTypesGrid();
            HideDialog();
        }

        #endregion

        #region GroupTypeAttributes Grid and Picker

        /// <summary>
        /// Handles the Add event of the gGroupTypeAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupTypeAttributes_Add( object sender, EventArgs e )
        {
            gGroupTypeAttributes_ShowEdit( Guid.Empty );
        }

        /// <summary>
        /// Handles the Edit event of the gGroupTypeAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gGroupTypeAttributes_Edit( object sender, RowEventArgs e )
        {
            Guid attributeGuid = (Guid)e.RowKeyValue;
            gGroupTypeAttributes_ShowEdit( attributeGuid );
        }

        /// <summary>
        /// Gs the group type attributes_ show edit.
        /// </summary>
        /// <param name="attributeGuid">The attribute GUID.</param>
        protected void gGroupTypeAttributes_ShowEdit( Guid attributeGuid )
        {
            Attribute attribute;
            if ( attributeGuid.Equals( Guid.Empty ) )
            {
                attribute = new Attribute();
                attribute.FieldTypeId = FieldTypeCache.Read( Rock.SystemGuid.FieldType.TEXT ).Id;
                edtGroupTypeAttributes.ActionTitle = ActionTitle.Add( "attribute for group type " + tbName.Text );
            }
            else
            {
                attribute = GroupTypeAttributesState.First( a => a.Guid.Equals( attributeGuid ) );
                edtGroupTypeAttributes.ActionTitle = ActionTitle.Edit( "attribute for group type " + tbName.Text );
            }

            var reservedKeyNames = new List<string>();
            GroupTypeAttributesInheritedState.Select( a => a.Key ).ToList().ForEach( a => reservedKeyNames.Add( a ) );
            GroupTypeAttributesState.Where( a => !a.Guid.Equals( attributeGuid ) ).Select( a => a.Key ).ToList().ForEach( a => reservedKeyNames.Add( a ) );
            edtGroupTypeAttributes.ReservedKeyNames = reservedKeyNames.ToList();

            edtGroupTypeAttributes.SetAttributeProperties( attribute, typeof( GroupType ) );

            ShowDialog( "GroupTypeAttributes" );
        }

        /// <summary>
        /// Handles the GridReorder event of the gGroupTypeAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridReorderEventArgs"/> instance containing the event data.</param>
        protected void gGroupTypeAttributes_GridReorder( object sender, GridReorderEventArgs e )
        {
            ReorderAttributeList( GroupTypeAttributesState, e.OldIndex, e.NewIndex );
            BindGroupTypeAttributesGrid();
        }

        /// <summary>
        /// Handles the Delete event of the gGroupTypeAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected void gGroupTypeAttributes_Delete( object sender, RowEventArgs e )
        {
            Guid attributeGuid = (Guid)e.RowKeyValue;
            GroupTypeAttributesState.RemoveEntity( attributeGuid );

            BindGroupTypeAttributesGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gGroupTypeAttributesInherited control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupTypeAttributesInherited_GridRebind( object sender, EventArgs e )
        {
            BindGroupTypeAttributesInheritedGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gGroupTypeAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupTypeAttributes_GridRebind( object sender, EventArgs e )
        {
            BindGroupTypeAttributesGrid();
        }

        /// <summary>
        /// Handles the SaveClick event of the dlgGroupTypeAttribute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void dlgGroupTypeAttribute_SaveClick( object sender, EventArgs e )
        {
            Rock.Model.Attribute attribute = new Rock.Model.Attribute();
            edtGroupTypeAttributes.GetAttributeProperties( attribute );

            // Controls will show warnings
            if ( !attribute.IsValid )
            {
                return;
            }

            if ( GroupTypeAttributesState.Any( a => a.Guid.Equals( attribute.Guid ) ) )
            {
                attribute.Order = GroupTypeAttributesState.Where( a => a.Guid.Equals( attribute.Guid ) ).FirstOrDefault().Order;
                GroupTypeAttributesState.RemoveEntity( attribute.Guid );
            }
            else
            {
                attribute.Order = GroupTypeAttributesState.Any() ? GroupTypeAttributesState.Max( a => a.Order ) + 1 : 0;
            }

            GroupTypeAttributesState.Add( attribute );

            BindGroupTypeAttributesGrid();
            HideDialog();
        }

        /// <summary>
        /// Binds the group type attributes inherited grid.
        /// </summary>
        private void BindGroupTypeAttributesInheritedGrid()
        {
            gGroupTypeAttributesInherited.AddCssClass( "inherited-attribute-grid" );
            gGroupTypeAttributesInherited.DataSource = GroupTypeAttributesInheritedState;
            gGroupTypeAttributesInherited.DataBind();
            rcGroupTypeAttributesInherited.Visible = GroupTypeAttributesInheritedState.Any();
        }

        /// <summary>
        /// Binds the group type attributes grid.
        /// </summary>
        private void BindGroupTypeAttributesGrid()
        {
            gGroupTypeAttributes.AddCssClass( "attribute-grid" );
            SetAttributeListOrder( GroupTypeAttributesState );
            gGroupTypeAttributes.DataSource = GroupTypeAttributesState.OrderBy( a => a.Order ).ThenBy( a => a.Name ).ToList();
            gGroupTypeAttributes.DataBind();
        }

        #endregion

        #region GroupAttributes Grid and Picker

        /// <summary>
        /// Handles the Add event of the gGroupAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupAttributes_Add( object sender, EventArgs e )
        {
            gGroupAttributes_ShowEdit( Guid.Empty );
        }

        /// <summary>
        /// Handles the Edit event of the gGroupAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gGroupAttributes_Edit( object sender, RowEventArgs e )
        {
            Guid attributeGuid = (Guid)e.RowKeyValue;
            gGroupAttributes_ShowEdit( attributeGuid );
        }

        /// <summary>
        /// Gs the group attributes_ show edit.
        /// </summary>
        /// <param name="attributeGuid">The attribute GUID.</param>
        protected void gGroupAttributes_ShowEdit( Guid attributeGuid )
        {
            Attribute attribute;
            if ( attributeGuid.Equals( Guid.Empty ) )
            {
                attribute = new Attribute();
                attribute.FieldTypeId = FieldTypeCache.Read( Rock.SystemGuid.FieldType.TEXT ).Id;
                edtGroupAttributes.ActionTitle = ActionTitle.Add( "attribute for groups of group type " + tbName.Text );
            }
            else
            {
                attribute = GroupAttributesState.First( a => a.Guid.Equals( attributeGuid ) );
                edtGroupAttributes.ActionTitle = ActionTitle.Edit( "attribute for groups of group type " + tbName.Text );
            }

            var reservedKeyNames = new List<string>();
            GroupAttributesInheritedState.Select( a => a.Key ).ToList().ForEach( a => reservedKeyNames.Add( a ) );
            GroupAttributesState.Where( a => !a.Guid.Equals( attributeGuid ) ).Select( a => a.Key ).ToList().ForEach( a => reservedKeyNames.Add( a ) );
            edtGroupAttributes.ReservedKeyNames = reservedKeyNames.ToList();

            edtGroupAttributes.SetAttributeProperties( attribute, typeof( Group ) );

            ShowDialog( "GroupAttributes" );
        }

        /// <summary>
        /// Handles the GridReorder event of the gGroupAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridReorderEventArgs"/> instance containing the event data.</param>
        protected void gGroupAttributes_GridReorder( object sender, GridReorderEventArgs e )
        {
            ReorderAttributeList( GroupAttributesState, e.OldIndex, e.NewIndex );
            BindGroupAttributesGrid();
        }

        /// <summary>
        /// Handles the Delete event of the gGroupAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected void gGroupAttributes_Delete( object sender, RowEventArgs e )
        {
            Guid attributeGuid = (Guid)e.RowKeyValue;
            GroupAttributesState.RemoveEntity( attributeGuid );

            BindGroupAttributesGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gGroupAttributesInherited control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupAttributesInherited_GridRebind( object sender, EventArgs e )
        {
            BindGroupAttributesInheritedGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gGroupAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupAttributes_GridRebind( object sender, EventArgs e )
        {
            BindGroupAttributesGrid();
        }

        /// <summary>
        /// Handles the SaveClick event of the dlgGroupAttribute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void dlgGroupAttribute_SaveClick( object sender, EventArgs e )
        {
            Rock.Model.Attribute attribute = new Rock.Model.Attribute();
            edtGroupAttributes.GetAttributeProperties( attribute );

            // Controls will show warnings
            if ( !attribute.IsValid )
            {
                return;
            }

            if ( GroupAttributesState.Any( a => a.Guid.Equals( attribute.Guid ) ) )
            {
                attribute.Order = GroupAttributesState.Where( a => a.Guid.Equals( attribute.Guid ) ).FirstOrDefault().Order;
                GroupAttributesState.RemoveEntity( attribute.Guid );
            }
            else
            {
                attribute.Order = GroupAttributesState.Any() ? GroupAttributesState.Max( a => a.Order ) + 1 : 0;
            }
            GroupAttributesState.Add( attribute );

            BindGroupAttributesGrid();
            HideDialog();
        }

        /// <summary>
        /// Binds the group attributes inherited grid.
        /// </summary>
        private void BindGroupAttributesInheritedGrid()
        {
            gGroupAttributesInherited.AddCssClass( "inherited-attribute-grid" );
            gGroupAttributesInherited.DataSource = GroupAttributesInheritedState;
            gGroupAttributesInherited.DataBind();
            rcGroupAttributesInherited.Visible = GroupAttributesInheritedState.Any();
        }

        /// <summary>
        /// Binds the group type attributes grid.
        /// </summary>
        private void BindGroupAttributesGrid()
        {
            gGroupAttributes.AddCssClass( "attribute-grid" );
            SetAttributeListOrder( GroupAttributesState );
            gGroupAttributes.DataSource = GroupAttributesState.OrderBy( a => a.Order ).ThenBy( a => a.Name ).ToList();
            gGroupAttributes.DataBind();
        }

        #endregion

        #region GroupMemberAttributes Grid and Picker

        /// <summary>
        /// Handles the Add event of the gGroupMemberAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupMemberAttributes_Add( object sender, EventArgs e )
        {
            gGroupMemberAttributes_ShowEdit( Guid.Empty );
        }

        /// <summary>
        /// Handles the Edit event of the gGroupMemberAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gGroupMemberAttributes_Edit( object sender, RowEventArgs e )
        {
            Guid attributeGuid = (Guid)e.RowKeyValue;
            gGroupMemberAttributes_ShowEdit( attributeGuid );
        }

        /// <summary>
        /// Gs the group attributes_ show edit.
        /// </summary>
        /// <param name="attributeGuid">The attribute GUID.</param>
        protected void gGroupMemberAttributes_ShowEdit( Guid attributeGuid )
        {
            Attribute attribute;
            if ( attributeGuid.Equals( Guid.Empty ) )
            {
                attribute = new Attribute();
                attribute.FieldTypeId = FieldTypeCache.Read( Rock.SystemGuid.FieldType.TEXT ).Id;
                edtGroupMemberAttributes.ActionTitle = ActionTitle.Add( "attribute for members in groups of group type " + tbName.Text );
            }
            else
            {
                attribute = GroupMemberAttributesState.First( a => a.Guid.Equals( attributeGuid ) );
                edtGroupMemberAttributes.ActionTitle = ActionTitle.Edit( "attribute for members in groups of group type " + tbName.Text );
            }

            var reservedKeyNames = new List<string>();
            GroupMemberAttributesInheritedState.Select( a => a.Key ).ToList().ForEach( a => reservedKeyNames.Add( a ) );
            GroupMemberAttributesState.Where( a => !a.Guid.Equals( attributeGuid ) ).Select( a => a.Key ).ToList().ForEach( a => reservedKeyNames.Add( a ) );
            edtGroupMemberAttributes.ReservedKeyNames = reservedKeyNames.ToList();

            edtGroupMemberAttributes.SetAttributeProperties( attribute, typeof( GroupMember ) );

            ShowDialog( "GroupMemberAttributes" );
        }

        /// <summary>
        /// Handles the GridReorder event of the gGroupMemberAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridReorderEventArgs"/> instance containing the event data.</param>
        protected void gGroupMemberAttributes_GridReorder( object sender, GridReorderEventArgs e )
        {
            ReorderAttributeList( GroupMemberAttributesState, e.OldIndex, e.NewIndex );
            BindGroupMemberAttributesGrid();
        }

        /// <summary>
        /// Handles the Delete event of the gGroupMemberAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected void gGroupMemberAttributes_Delete( object sender, RowEventArgs e )
        {
            Guid attributeGuid = (Guid)e.RowKeyValue;
            GroupMemberAttributesState.RemoveEntity( attributeGuid );

            BindGroupMemberAttributesGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gGroupMemberAttributesInherited control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupMemberAttributesInherited_GridRebind( object sender, EventArgs e )
        {
            BindGroupMemberAttributesInheritedGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gGroupMemberAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gGroupMemberAttributes_GridRebind( object sender, EventArgs e )
        {
            BindGroupMemberAttributesGrid();
        }

        /// <summary>
        /// Handles the SaveClick event of the dlgGroupMemberAttribute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void dlgGroupMemberAttribute_SaveClick( object sender, EventArgs e )
        {
            Rock.Model.Attribute attribute = new Rock.Model.Attribute();
            edtGroupMemberAttributes.GetAttributeProperties( attribute );

            // Controls will show warnings
            if ( !attribute.IsValid )
            {
                return;
            }

            if ( GroupMemberAttributesState.Any( a => a.Guid.Equals( attribute.Guid ) ) )
            {
                attribute.Order = GroupMemberAttributesState.Where( a => a.Guid.Equals( attribute.Guid ) ).FirstOrDefault().Order;
                GroupMemberAttributesState.RemoveEntity( attribute.Guid );
            }
            else
            {
                attribute.Order = GroupMemberAttributesState.Any() ? GroupMemberAttributesState.Max( a => a.Order ) + 1 : 0;
            }
            GroupMemberAttributesState.Add( attribute );

            BindGroupMemberAttributesGrid();
            HideDialog();
        }

        /// <summary>
        /// Binds the group member attributes inherited grid.
        /// </summary>
        private void BindGroupMemberAttributesInheritedGrid()
        {
            gGroupMemberAttributesInherited.AddCssClass( "inherited-attribute-grid" );
            gGroupMemberAttributesInherited.DataSource = GroupMemberAttributesInheritedState;
            gGroupMemberAttributesInherited.DataBind();
            rcGroupMemberAttributesInherited.Visible = GroupMemberAttributesInheritedState.Any();
        }

        /// <summary>
        /// Binds the group type attributes grid.
        /// </summary>
        private void BindGroupMemberAttributesGrid()
        {
            gGroupMemberAttributes.AddCssClass( "attribute-grid" );
            SetAttributeListOrder( GroupMemberAttributesState );
            gGroupMemberAttributes.DataSource = GroupMemberAttributesState.OrderBy( a => a.Order ).ThenBy( a => a.Name ).ToList();
            gGroupMemberAttributes.DataBind();
        }

        #endregion
    }
}