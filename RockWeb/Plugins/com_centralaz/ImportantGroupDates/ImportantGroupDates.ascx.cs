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
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Lava;

namespace RockWeb.Plugins.com_centralaz.ImportantGroupDates
{
    [DisplayName( "Group List Personalized Lava" )]
    [Category( "Groups" )]
    [Description( "Lists all group that the person is a member of using a Lava template." )]

    [LinkedPage( "Detail Page", "", true, "", "", 0 )]
    [CodeEditorField( "Lava Template", "The lava template to use to format the group list.", CodeEditorMode.Liquid, CodeEditorTheme.Rock, 400, true, "{% include '~/Plugins/com_centralaz/ImportantGroupDates/Assets/ImportantGroupDates.lava' %}", "", 6 )]
    [BooleanField( "Enable Debug", "Shows the fields available to merge in lava.", false, "", 7 )]
    public partial class ImportantGroupDates : RockBlock
    {

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            this.BlockUpdated += Block_BlockUpdated;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                ListGroups();
            }

            base.OnLoad( e );
        }

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            ListGroups();
        }

        #endregion

        #region Internal Methods

        private void ListGroups()
        {
            RockContext rockContext = new RockContext();

            var qry = new GroupMemberService( rockContext )
                        .Queryable( "Person" )
                        .Where( m => m.PersonId == CurrentPersonId
                                && m.GroupMemberStatus == GroupMemberStatus.Active
                                && m.Group.IsActive == true );

            //List<Guid> includeGroupRolesGuids = GetAttributeValue( "IncludeGroupRoles" ).SplitDelimitedValues().Select( a => Guid.Parse( a ) ).ToList();
            //if ( includeGroupRolesGuids.Count > 0 )
            //{
            //    qry = qry.Where( m => includeGroupRolesGuids.Contains( m.GroupRole.Guid) );
            //}


            var qryBirthday = qry.Where( m => GetAttributeValue( "UpcomingDays" ).AsInteger() >= m.Person.DaysToBirthday );

            var qryAnniversary = qry.Where( m => m.Person.AnniversaryDate.HasValue && m.Person.AnniversaryDate.Value.DayOfYear >= DateTime.Now.DayOfYear && m.Person.AnniversaryDate.Value.DayOfYear <= DateTime.Now.AddDays( GetAttributeValue( "UpcomingDays" ).AsInteger() ).DayOfYear );

           // var dates = qry.Select( m => new GroupMemberImportantDateSummary { GroupMember = m.Group, Date = m.GroupRole.Name, Occasion = m.GroupRole.IsLeader } ).ToList();

            var mergeFields = new Dictionary<string, object>();
           // mergeFields.Add( "Dates", dates );
            mergeFields.Add( "CurrentPerson", CurrentPerson );
            var globalAttributeFields = Rock.Web.Cache.GlobalAttributesCache.GetMergeFields( CurrentPerson );
            globalAttributeFields.ToList().ForEach( d => mergeFields.Add( d.Key, d.Value ) );

            Dictionary<string, object> linkedPages = new Dictionary<string, object>();
            linkedPages.Add( "DetailPage", LinkedPageUrl( "DetailPage", null ) );
            mergeFields.Add( "LinkedPages", linkedPages );

            string template = GetAttributeValue( "LavaTemplate" );

            // show debug info
            bool enableDebug = GetAttributeValue( "EnableDebug" ).AsBoolean();
            if ( enableDebug && IsUserAuthorized( Authorization.EDIT ) )
            {
                lDebug.Visible = true;
                lDebug.Text = mergeFields.lavaDebugInfo();
            }

            lContent.Text = template.ResolveMergeFields( mergeFields );
        }

        [DotLiquid.LiquidType( "GroupMember", "Date", "Occasion" )]
        public class GroupMemberImportantDateSummary
        {
            public GroupMember GroupMember { get; set; }
            public DateTime Date { get; set; }
            public string Occasion { get; set; }
        }

        #endregion
    }
}