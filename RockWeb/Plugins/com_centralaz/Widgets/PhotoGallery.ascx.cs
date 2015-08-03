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

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;

namespace RockWeb.Plugins.com_centralaz.Widgets
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Photo Gallery" )]
    [Category( "com_centralaz > Widgets" )]
    [Description( "Template block for developers to use to start a new detail block." )]
    [EmailField( "Email" )]
    [TextField("Image Folder Path")]
    public partial class PhotoGallery : Rock.Web.UI.RockBlock
    {
        #region Fields

        private string virtualPath;
        private string physicalPath;

        #endregion

        #region Properties

        /// <summary>
        /// Relative path to the Images Folder
        /// </summary>
        public string ImageFolderPath { get; set; }

        /// <summary>
        /// Get or Set the Admin Mode 
        /// </summary>
        public bool AdminMode { get; set; }

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

            RockPage.AddCSSLink( ResolveRockUrl( "~/Plugins/com_centralaz/Widgets/Styles/dropzone.css" ) );
            RockPage.AddScriptLink( "~/Plugins/com_centralaz/Widgets/Scripts/dropzone.js" );
            ImageFolderPath = GetAttributeValue( "ImageFolderPath" );

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

            //Update the path
            //use a default path
            virtualPath = "~/Content/ExternalSite/Gallery";
            physicalPath = Server.MapPath( virtualPath );

            //If ImageFolderPath is specified then use that path
            if ( !string.IsNullOrEmpty( ImageFolderPath ) )
            {
                physicalPath = Server.MapPath( ImageFolderPath );
                virtualPath = ImageFolderPath;
            }

            //Show AdminMode specific controls
            if ( AdminMode )
            {
                lvImages.InsertItemPosition = InsertItemPosition.FirstItem;
            }

            if ( !Page.IsPostBack )
            {
                // added for your convenience
            }
        }

        /// <summary>
        /// Pre render operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender( object sender, EventArgs e )
        {
            //Binds the Data Before Rendering
            BindData();
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {

        }

        protected void lvImages_ItemDataBound( object sender, ListViewItemEventArgs e )
        {
            //In case of AdminMode, we would want to show the delete button 
            //which is not visible by iteself for Non-Admin users
            if ( AdminMode )
            {
                var lbDelete = e.Item.FindControl( "lbDelete" ) as LinkButton;
                if ( lbDelete == null ) return;

                lbDelete.Visible = true;
            }

            //Get the required controls
            var fupImage = e.Item.FindControl( "fupImage" ) as FileUpload;
            if ( fupImage != null )
            {
                var parent = fupImage.Parent;
                if ( parent != null )
                {
                    var lblImageUploadStatus = parent.FindControl( "lblImageUploadStatus" ) as Label;
                    if ( lblImageUploadStatus != null )
                    {
                        //If a file is posted, save it
                        if ( this.IsPostBack )
                        {
                            if ( fupImage.PostedFile != null && fupImage.PostedFile.ContentLength > 0 )
                            {
                                try
                                {
                                    fupImage.PostedFile.SaveAs( string.Format( "{0}\\{1}",
                                        physicalPath, GetFileName( fupImage.PostedFile.FileName ) ) );
                                    lblImageUploadStatus.Text = string.Format(
                                        "Image {0} successfully uploaded!",
                                        fupImage.PostedFile.FileName );
                                }
                                catch ( Exception ex )
                                {
                                    lblImageUploadStatus.Text = string.Format( "Error uploading {0}!",
                                        fupImage.PostedFile.FileName );
                                }
                            }
                            else
                            {
                                lblImageUploadStatus.Text = string.Empty;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Redirects to the full image when the image is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imbItem_Command( object sender, CommandEventArgs e )
        {
            Response.Redirect( e.CommandArgument as string );
        }

        /// <summary>
        /// Performs commands for bound buttons in the ImageListView. In this case 
        /// 'Remove (Delete)'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lvImages_ItemCommand( object sender, ListViewCommandEventArgs e )
        {
            /* We have not bound the control to any DataSource derived controls, 
            nor do we use any key to identify the image. Hence, it makes more sense not to 
            use 'delete' but to use a custom command 'Remove' which can be fired as a 
            generic ItemCommand, and the ListViewCommandEventArgs e will have 
            the CommandArgument passed by the 'Remove' button In this case, it is the bound 
            ImageUrl that we are passing, and making use it of to delete the image.*/
            switch ( e.CommandName )
            {
                case "Remove":
                    var path = e.CommandArgument as string;
                    if ( path != null )
                    {
                        try
                        {
                            FileInfo fi = new FileInfo( Server.MapPath( path ) );
                            fi.Delete();

                            //Display message
                            Parent.Controls.Add( new Label()
                            {
                                Text = GetFileName( path ) + " deleted successfully!"
                            } );

                        }
                        catch ( Exception ex )
                        {
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get File Name
        /// </summary>
        /// <param name="path">full path</param>
        /// <returns>string containing the file name</returns>
        private string GetFileName( string path )
        {
            DateTime timestamp = DateTime.Now;
            string fileName = string.Empty;
            try
            {
                if ( path.Contains( '\\' ) ) fileName = path.Split( '\\' ).Last();
                if ( path.Contains( '/' ) ) fileName = path.Split( '/' ).Last();
            }
            catch ( Exception ex )
            {
            }
            return fileName;
        }

        /// <summary>
        /// Binds the ImageListView to current DataSource
        /// </summary>
        private void BindData()
        {
            var images = new List<string>();

            try
            {
                var imagesFolder = new DirectoryInfo( physicalPath );
                foreach ( var item in imagesFolder.EnumerateFiles() )
                {
                    if ( item is FileInfo )
                    {
                        //add virtual path of the image to the images list
                        images.Add( string.Format( "{0}/{1}", virtualPath, item.Name ) );
                    }
                }
            }
            catch ( Exception ex )
            {
                //log exception
            }

            lvImages.DataSource = images;
            lvImages.DataBind();

        }

        #endregion

    }
}