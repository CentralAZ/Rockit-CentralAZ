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
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;

namespace RockWeb.Blocks.Core
{
    /// <summary>
    /// Block that can be used to set the default campus context for the site
    /// </summary>
    [DisplayName( "Campus Context Setter" )]
    [Category( "Core" )]
    [Description( "Block that can be used to set the default campus context for the site." )]
    public partial class CampusContextSetter : RockBlock
    {
        #region Base Control Methods
        
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                LoadDropDowns();
            }
        }

        /// <summary>
        /// Loads the drop downs.
        /// </summary>
        private void LoadDropDowns()
        {
            var campusService =new CampusService( new RockContext() );

            // default campus to the whatever the context cookie has for it
            string defaultCampusPublicKey = string.Empty;
            var contextCookie = Request.Cookies["Rock:context"];
            if ( contextCookie != null )
            {
                var cookieValue = contextCookie.Values[typeof( Rock.Model.Campus ).FullName];

                string contextItem = Rock.Security.Encryption.DecryptString( cookieValue );
                string[] contextItemParts = contextItem.Split( '|' );
                if ( contextItemParts.Length == 2 )
                {
                    defaultCampusPublicKey = contextItemParts[1];
                }
            }

            var defaultCampus = campusService.GetByPublicKey( defaultCampusPublicKey );
            var campuses = campusService.Queryable().OrderBy( a => a.Name ).ToList();
            foreach ( var campus in campuses )
            {
                var listItem = new ListItem( campus.Name, HttpUtility.UrlDecode( campus.ContextKey ) );
                listItem.Selected = campus.Guid == defaultCampus.Guid;
                ddlCampus.Items.Add( listItem );
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlCampus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ddlCampus_SelectedIndexChanged( object sender, EventArgs e )
        {
            var contextCookie = Request.Cookies["Rock:context"];
            if ( contextCookie == null )
            {
                contextCookie = new HttpCookie( "Rock:context" );
            }

            contextCookie.Values[typeof( Rock.Model.Campus ).FullName] = ddlCampus.SelectedValue;
            contextCookie.Expires = RockDateTime.Now.AddYears( 1 );

            Response.Cookies.Add( contextCookie );
        }

        #endregion
    }
}