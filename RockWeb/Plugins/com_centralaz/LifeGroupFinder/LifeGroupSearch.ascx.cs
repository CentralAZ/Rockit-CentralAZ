// <copyright>
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

using com.centralaz.LifeGroupFinder.Web.UI.Controls.GroupFinder;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;

namespace RockWeb.Plugins.com_centralaz.LifeGroupFinder
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Life Group Search" )]
    [Category( "com_centralaz > Groups" )]
    [Description( "Central custom group search block." )]
    [LinkedPage( "Life Group List Page", "The page to navigate to for group details.", false, "", "", 0 )]
    [LinkedPage( "Life Group Map Page", "The page to navigate to for group details.", false, "", "", 0 )]
    [LinkedPage( "Information Security Page", "The page to navigate to for group details.", false, "", "", 0 )]

    public partial class LifeGroupSearch : Rock.Web.UI.RockBlock
    {
        #region ViewState and Dynamic Controls

        public GroupSearchFilter GroupSearchFilterState
        {
            get
            {
                GroupSearchFilter groupSearchFilter = ViewState["GroupSearchFilterState"] as GroupSearchFilter;
                if ( groupSearchFilter == null )
                {
                    groupSearchFilter = new GroupSearchFilter();
                    groupSearchFilter.ID = "_groupSearchFilter";
                    groupSearchFilter.SearchClick += btnSearch_Click;
                }
                return groupSearchFilter;
            }
            set
            {
                ViewState["GroupSearchFilterState"] = value;
            }
        }

        #endregion

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

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
            phSearchFilter.Controls.Add( GroupSearchFilterState );
            //ScriptManager.GetCurrent( this.Page ).RegisterAsyncPostBackControl( groupSearchFilter );
            //ScriptManager scriptManager = ScriptManager.GetCurrent( Page );
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

                if ( CurrentPerson != null )
                {
                    pnlLogin.Visible = false;
                }
                ddlCampus.DataSource = CampusCache.All();
                ddlCampus.DataBind();
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

        }

        protected void btnSearch_Click( object sender, EventArgs e )
        {
            //GroupSearchFilter groupSearchFilter = (GroupSearchFilter)phSearchFilter.FindControl( "_groupSearchFilter_section" );
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add( "Children",  GroupSearchFilterState.ChildrenState);
            param.Add( "Days", GroupSearchFilterState.DaysState );
            param.Add( "Pets", GroupSearchFilterState.PetsState.ToTrueFalse() );
            param.Add( "Campus", ddlCampus.SelectedValue );
            param.Add( "StreetAddress1", acAddress.Street1 );
            param.Add( "StreetAddress2", acAddress.Street2 );
            param.Add( "City", acAddress.City );
            param.Add( "State", acAddress.State );
            param.Add( "PostalCode", acAddress.PostalCode );
            param.Add( "Country", acAddress.Country );
            NavigateToLinkedPage( "LifeGroupListPage", queryParams: param );
        }
        void cblChildren_SelectedIndexChanged( object sender, EventArgs e )
        {
            GroupSearchFilterState.ChildrenState = ( sender as RockCheckBoxList ).SelectedValues.AsDelimited( ";" );
        }

        void cblDays_SelectedIndexChanged( object sender, EventArgs e )
        {
            GroupSearchFilterState.DaysState = ( sender as RockCheckBoxList ).SelectedValues.AsDelimited( ";" );
        }

        void cbPets_CheckedChanged( object sender, EventArgs e )
        {
            GroupSearchFilterState.PetsState = ( sender as RockCheckBox ).Checked;
        }

        protected void lbLogin_Click( object sender, EventArgs e )
        {
            var site = RockPage.Layout.Site;
            if ( site.LoginPageId.HasValue )
            {
                site.RedirectToLoginPage( true );           
            }
        }

        protected void lbMap_Click( object sender, EventArgs e )
        {
            NavigateToLinkedPage( "LifeGroupMapPage", "Campus", ddlCampus.SelectedValue.AsInteger() );
        }

        protected void lbSecurity_Click( object sender, EventArgs e )
        {
            NavigateToLinkedPage( "InformationSecurityPage" );
        }

        #endregion

    }
}