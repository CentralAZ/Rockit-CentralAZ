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
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Web.Cache;
using System.Collections.Generic;
using Rock.Security;

namespace RockWeb.Blocks.Finance
{
    /// <summary>
    /// Lists scheduled transactions for current or selected user (if context for person is not configured, will display for currently logged in person).
    /// </summary>
    [DisplayName( "Giving Profile List" )]
    [Category( "Finance" )]
    [Description( "Lists scheduled transactions for current or selected user (if context for person is not configured, will display for currently logged in person)." )]

    [LinkedPage( "Edit Page" )]
    [LinkedPage( "Add Page" )]
    [ContextAware( typeof( Person ) )]
    public partial class GivingProfileList : Rock.Web.UI.RockBlock
    {
        #region Properties

        /// <summary>
        /// Gets the target person.
        /// </summary>
        /// <value>
        /// The target person.
        /// </value>
        protected Person TargetPerson { get; private set; }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            gfSettings.ApplyFilterClick += gfSettings_ApplyFilterClick;
            gfSettings.DisplayFilterValue += gfSettings_DisplayFilterValue;

            bool canEdit = IsUserAuthorized( Authorization.EDIT );

            rGridGivingProfile.DataKeyNames = new string[] { "id" };
            rGridGivingProfile.Actions.ShowAdd = canEdit && !string.IsNullOrWhiteSpace( GetAttributeValue( "AddPage" ) );
            rGridGivingProfile.IsDeleteEnabled = canEdit;

            rGridGivingProfile.Actions.AddClick += rGridGivingProfile_Add;
            rGridGivingProfile.GridRebind += rGridGivingProfile_GridRebind;

            TargetPerson = ContextEntity<Person>();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                cbIncludeInactive.Checked = !string.IsNullOrWhiteSpace( gfSettings.GetUserPreference( "Include Inactive" ) );

                BindGrid();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the ApplyFilterClick event of the gfSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gfSettings_ApplyFilterClick( object sender, EventArgs e )
        {
            gfSettings.SaveUserPreference( "Include Inactive", cbIncludeInactive.Checked ? "Yes" : string.Empty );
            BindGrid();
        }

        /// <summary>
        /// Gfs the settings_ display filter value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        protected void gfSettings_DisplayFilterValue( object sender, GridFilter.DisplayFilterValueArgs e )
        {
            if ( e.Key != "Include Inactive" )
            {
                e.Value = string.Empty;
            }
        }

        /// <summary>
        /// Handles the RowSelected event of the rGridGivingProfile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void rGridGivingProfile_Edit( object sender, RowEventArgs e )
        {
            string urlEncodedKey = string.Empty;
            if ( TargetPerson != null )
            {
                urlEncodedKey = TargetPerson.UrlEncodedKey;
            }
            else
            {
                var txn = new FinancialScheduledTransactionService( new RockContext() ).Get( (int)e.RowKeyValue );
                if ( txn != null && txn.AuthorizedPerson != null )
                {
                    urlEncodedKey = txn.AuthorizedPerson.UrlEncodedKey;
                }
            }

            var parms = new Dictionary<string, string>();
            parms.Add( "Txn", rGridGivingProfile.DataKeys[e.RowIndex]["id"].ToString() );
            if ( !string.IsNullOrWhiteSpace( urlEncodedKey ) )
            {
                parms.Add( "Person", urlEncodedKey );
            }

            NavigateToLinkedPage( "EditPage", parms );
        }

        /// <summary>
        /// Handles the Add event of the gridFinancialGivingProfile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void rGridGivingProfile_Add( object sender, EventArgs e )
        {
            var parms = new Dictionary<string, string>();
            parms.Add( "Person", TargetPerson.UrlEncodedKey );
            NavigateToLinkedPage( "AddPage", parms );
        }

        /// <summary>
        /// Handles the Delete event of the grdFinancialGivingProfile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void rGridGivingProfile_Delete( object sender, RowEventArgs e )
        {
            // TODO: Can't just delete profile, need to inactivate it on gateway
            //var scheduledTransactionService = new FinancialScheduledTransactionService();

            //FinancialScheduledTransaction profile = scheduledTransactionService.Get( (int)rGridGivingProfile.DataKeys[e.RowIndex]["id"] );
            //if ( profile != null )
            //{
            //    scheduledTransactionService.Delete( profile, CurrentPersonId );
            //    scheduledTransactionService.Save( profile, CurrentPersonId );
            //}

            BindGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the grdFinancialGivingProfile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void rGridGivingProfile_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            bool includeInactive = !string.IsNullOrWhiteSpace( gfSettings.GetUserPreference( "Include Inactive" ) );

            int? personId = null;
            int? givingGroupId = null;
            if ( TargetPerson != null )
            {
                personId = TargetPerson.Id;
                givingGroupId = TargetPerson.GivingGroupId;
            }

            rGridGivingProfile.DataSource = new FinancialScheduledTransactionService( new RockContext() )
                .Get( personId, givingGroupId, includeInactive ).ToList();

            rGridGivingProfile.DataBind();
        }

        /// <summary>
        /// Shows the detail form.
        /// </summary>
        /// <param name="id">The id.</param>
        protected void ShowDetailForm( int id )
        {
            var parms = new Dictionary<string, string>();
            parms.Add( "Txn", id.ToString() );
            parms.Add( "Person", TargetPerson.UrlEncodedKey );
            NavigateToLinkedPage( "DetailPage", parms );
        }

        #endregion
    }
}
