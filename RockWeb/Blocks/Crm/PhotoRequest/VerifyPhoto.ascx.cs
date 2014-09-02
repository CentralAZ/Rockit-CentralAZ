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
using System.Web.UI.HtmlControls;

namespace RockWeb.Blocks.Crm.PhotoRequest
{
    /// <summary>
    /// Allows uploaded photos to be verified.
    /// </summary>
    [DisplayName( "Verify Photo" )]
    [Category( "CRM > PhotoRequest" )]
    [Description( "Allows uploaded photos to be verified." )]

    [IntegerField( "Photo Size", "The size of the preview photo. Default is 65.", false, 65 )]

    public partial class VerifyPhoto : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables
        string size = "65";
        string sizepx = "65px";

        /// <summary>
        /// Group that manages the people using the Photo Request system.
        /// </summary>
        private Group _photoRequestGroup = null;

        #endregion

        #region Properties

        // used for public / protected properties

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

            size = GetAttributeValue( "PhotoSize" );
            sizepx = string.Format( "{0}px", size );

            if ( _photoRequestGroup == null )
            {
                var guid = Rock.SystemGuid.Group.GROUP_PHOTO_REQUEST.AsGuid();
                _photoRequestGroup = new GroupService( new RockContext() ).Queryable( "Members" )
                    .Where( g => g.Guid == guid )
                    .FirstOrDefault();
            }

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            nbMessage.Visible = false;

            if ( !Page.IsPostBack )
            {
                BindGrid();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Builds the image html for the person in the perticular row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gList_RowDataBound( object sender, GridViewRowEventArgs e )
        {
            if ( e.Row.RowType == DataControlRowType.DataRow )
            {
                var groupMember = e.Row.DataItem as GroupMember;
                if ( groupMember != null )
                {
                    HtmlImage imgPersonImage = e.Row.FindControl( "imgPersonImage" ) as HtmlImage;
                    if ( imgPersonImage != null )
                    {
                        imgPersonImage.Src = String.Format( @"{0}&width={1}", Person.GetPhotoUrl( groupMember.Person.PhotoId, groupMember.Person.Gender ), size );
                        imgPersonImage.Style.Add( "width", sizepx );
                    }

                    if ( groupMember.GroupMemberStatus != GroupMemberStatus.Pending )
                    {
                        var cb = e.Row.FindControl( "cbSelected" ) as CheckBox;
                        cb.Visible = false;
                    }

                    HtmlControl iStatus = e.Row.FindControl( "iStatus" ) as HtmlControl;
                    var bStatus = e.Row.FindControl( "bStatus" ) as Badge;

                    switch ( groupMember.GroupMemberStatus )
                    {
                        case GroupMemberStatus.Inactive:
                            iStatus.AddCssClass( "fa fa-lg fa-ban text-danger" );
                            iStatus.Attributes.Add( "data-original-title", "User has asked to never receive photo requests." );
                            break;
                        case GroupMemberStatus.Active:
                            iStatus.AddCssClass( "fa fa-lg fa-check-square-o" );
                            iStatus.Attributes.Add( "title", string.Format( "Verified on {0} by {1}", groupMember.ModifiedDateTime.Value.ToString( "MM/dd/yy" ), groupMember.ModifiedByPersonAlias.Person.FullName ) );

                            break;
                        case GroupMemberStatus.Pending:
                            iStatus.AddCssClass( "fa fa-lg fa-flag text-warning" );
                            iStatus.Attributes.Add( "title", "Pending verification." );
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            size = GetAttributeValue( "PhotoSize" );
            sizepx = string.Format( "{0}px", size );
            BindGrid();
        }

        /// <summary>
        /// Marks the selected people/photos as verified by setting their group member status Active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bbtnVerify_Click( object sender, EventArgs e )
        {
            nbMessage.Visible = true;

            int count = 0;
            RockContext rockContext = new RockContext();
            GroupService groupService = new GroupService( rockContext );
            Group group = groupService.Get( Rock.SystemGuid.Group.GROUP_PHOTO_REQUEST.AsGuid() );

            GroupMember groupMember = null;
            foreach ( GridViewRow row in gList.Rows )
            {
                var cb = row.FindControl( "cbSelected" ) as CheckBox;

                if ( cb != null && cb.Checked )
                {
                    int currentRowsPersonId = (int)gList.DataKeys[row.RowIndex].Value;
                    groupMember = group.Members.Where( m => m.PersonId == currentRowsPersonId ).FirstOrDefault();
                    if ( groupMember != null )
                    {
                        count++;
                        groupMember.GroupMemberStatus = GroupMemberStatus.Active;
                    }
                }
            }

            if ( count > 0 )
            {
                nbMessage.NotificationBoxType = NotificationBoxType.Success;
                nbMessage.Text = string.Format( "Verified {0} photo{1}.", count, count > 1 ? "s" : "" );
            }
            else
            {
                nbMessage.NotificationBoxType = NotificationBoxType.Warning;
                nbMessage.Text = "No changes were made.";
            }
            rockContext.SaveChanges();
            _photoRequestGroup = group;

            BindGrid();
        }

        /// <summary>
        /// Handle repainting grid when the show all checkbox is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbShowAll_CheckedChanged( object sender, EventArgs e )
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
            if ( _photoRequestGroup != null )
            {
                nbConfigError.Visible = false;
                var members = _photoRequestGroup.Members.Where( m => m.GroupMemberStatus == GroupMemberStatus.Pending || cbShowAll.Checked ).ToList();
                gList.DataSource = members;
                if ( members == null || members.Count == 0 )
                {
                    bbtnVerify.Visible = false;
                }
                else if ( members.Where( m => m.GroupMemberStatus == GroupMemberStatus.Pending ).ToList().Count > 0 )
                {
                    bbtnVerify.Visible = true;
                }
                gList.DataBind();
            }
            else
            {
                nbConfigError.Visible = true;
            }
        }

        /// <summary>
        /// Builds a link to the person's details page.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        protected string FormatPersonLink( string personId )
        {
            return ResolveRockUrl( string.Format( "~/Person/{0}", personId ) );
        }

        #endregion
    }
}