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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock;
using Rock.Attribute;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Security;

namespace RockWeb.Blocks.Core
{
    [DisplayName( "Category Detail" )]
    [Category( "Core" )]
    [Description( "Displays the details of a given category." )]

    [EntityTypeField( "Entity Type", "The type of entity to associate category with" )]
    [TextField( "Entity Type Qualifier Property", "", false )]
    [TextField( "Entity Type Qualifier Value", "", false )]
    public partial class CategoryDetail : RockBlock, IDetailBlock
    {
        #region Control Methods

        private int entityTypeId = 0;
        private string entityTypeQualifierProperty = string.Empty;
        private string entityTypeQualifierValue = string.Empty;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            Guid entityTypeGuid = Guid.Empty;
            if ( Guid.TryParse( GetAttributeValue( "EntityType" ), out entityTypeGuid ) )
            {
                entityTypeId = EntityTypeCache.Read( entityTypeGuid ).Id;
            }
            entityTypeQualifierProperty = GetAttributeValue( "EntityTypeQualifierProperty" );
            entityTypeQualifierValue = GetAttributeValue( "EntityTypeQualifierValue" );
            
            btnDelete.Attributes["onclick"] = string.Format( "javascript: return Rock.dialogs.confirmDelete(event, '{0}');", Category.FriendlyTypeName );
            btnSecurity.EntityTypeId = EntityTypeCache.Read( typeof( Rock.Model.Category ) ).Id;
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
                string itemId = PageParameter( "CategoryId" );
                string parentCategoryId = PageParameter( "ParentCategoryId" );
                if ( !string.IsNullOrWhiteSpace( itemId ) )
                {
                    if ( string.IsNullOrWhiteSpace( parentCategoryId ) )
                    {
                        ShowDetail( "CategoryId", int.Parse( itemId ) );
                    }
                    else
                    {
                        ShowDetail( "CategoryId", int.Parse( itemId ), int.Parse( parentCategoryId ) );
                    }
                }
                else
                {
                    pnlDetails.Visible = false;
                }
            }
        }

        #endregion

        #region Edit Events

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnCancel_Click( object sender, EventArgs e )
        {
            if ( hfCategoryId.Value.Equals( "0" ) )
            {
                int? parentCategoryId = PageParameter( "ParentCategoryId" ).AsInteger( false );
                if ( parentCategoryId.HasValue )
                {
                    // Cancelling on Add, and we know the parentCategoryId, so we are probably in treeview mode, so navigate to the current page
                    var qryParams = new Dictionary<string, string>();
                    qryParams["CategoryId"] = parentCategoryId.ToString();
                    NavigateToPage( RockPage.Guid, qryParams );
                }
                else
                {
                    // Cancelling on Add.  Return to Grid
                    NavigateToParentPage();
                }
            }
            else
            {
                // Cancelling on Edit.  Return to Details
                CategoryService service = new CategoryService( new RockContext() );
                Category category = service.Get( hfCategoryId.ValueAsInt() );
                ShowReadonlyDetails( category );
            }
        }

        /// <summary>
        /// Handles the Click event of the btnEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnEdit_Click( object sender, EventArgs e )
        {
            CategoryService service = new CategoryService( new RockContext() );
            Category category = service.Get( hfCategoryId.ValueAsInt() );
            ShowEditDetails( category );
        }

        /// <summary>
        /// Handles the Click event of the btnDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnDelete_Click( object sender, EventArgs e )
        {
            int? parentCategoryId = null;

            var rockContext = new RockContext();
            var categoryService = new CategoryService( rockContext );
            var category = categoryService.Get( int.Parse( hfCategoryId.Value ) );

            if ( category != null )
            {
                string errorMessage;
                if ( !categoryService.CanDelete( category, out errorMessage ) )
                {
                    ShowReadonlyDetails( category );
                    mdDeleteWarning.Show( errorMessage, ModalAlertType.Information );
                }
                else
                {
                    parentCategoryId = category.ParentCategoryId;

                    CategoryCache.Flush( category.Id );

                    categoryService.Delete( category );
                    rockContext.SaveChanges();

                    // reload page, selecting the deleted category's parent
                    var qryParams = new Dictionary<string, string>();
                    if ( parentCategoryId != null )
                    {
                        qryParams["CategoryId"] = parentCategoryId.ToString();
                    }

                    NavigateToPage( RockPage.Guid, qryParams );
                }
            }
        }

        /// <summary>
        /// Sets the edit mode.
        /// </summary>
        /// <param name="editable">if set to <c>true</c> [editable].</param>
        private void SetEditMode( bool editable )
        {
            pnlEditDetails.Visible = editable;
            fieldsetViewDetails.Visible = !editable;
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            Category category;

            var rockContext = new RockContext();
            CategoryService categoryService = new CategoryService( rockContext );

            int categoryId = hfCategoryId.ValueAsInt();

            if ( categoryId == 0 )
            {
                category = new Category();
                category.IsSystem = false;
                category.EntityTypeId = entityTypeId;
                category.EntityTypeQualifierColumn = entityTypeQualifierProperty;
                category.EntityTypeQualifierValue = entityTypeQualifierValue;
                category.Order = 0;
                categoryService.Add( category );
            }
            else
            {
                category = categoryService.Get( categoryId );
            }

            category.Name = tbName.Text;
            category.ParentCategoryId = cpParentCategory.SelectedValueAsInt();
            category.IconCssClass = tbIconCssClass.Text;

            List<int> orphanedBinaryFileIdList = new List<int>();

            if ( !Page.IsValid )
            {
                return;
            }

            if ( !category.IsValid )
            {
                // Controls will render the error messages                    
                return;
            }

            BinaryFileService binaryFileService = new BinaryFileService( rockContext );
            foreach ( int binaryFileId in orphanedBinaryFileIdList )
            {
                var binaryFile = binaryFileService.Get( binaryFileId );
                if ( binaryFile != null )
                {
                    // marked the old images as IsTemporary so they will get cleaned up later
                    binaryFile.IsTemporary = true;
                }
            }

            rockContext.SaveChanges();
            CategoryCache.Flush( category.Id );

            var qryParams = new Dictionary<string, string>();
            qryParams["CategoryId"] = category.Id.ToString();
            NavigateToPage( RockPage.Guid, qryParams );
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Shows the detail.
        /// </summary>
        /// <param name="itemKey">The item key.</param>
        /// <param name="itemKeyValue">The item key value.</param>
        public void ShowDetail( string itemKey, int itemKeyValue )
        {
            ShowDetail( itemKey, itemKeyValue, null );
        }

        /// <summary>
        /// Shows the detail.
        /// </summary>
        /// <param name="itemKey">The item key.</param>
        /// <param name="itemKeyValue">The item key value.</param>
        /// <param name="parentCategoryId">The parent category id.</param>
        public void ShowDetail( string itemKey, int itemKeyValue, int? parentCategoryId )
        {
            pnlDetails.Visible = false;
            if ( !itemKey.Equals( "CategoryId" ) )
            {
                return;
            }

            var categoryService = new CategoryService( new RockContext() );
            Category category = null;

            if ( !itemKeyValue.Equals( 0 ) )
            {
                category = categoryService.Get( itemKeyValue );
            }
            else
            {
                category = new Category { Id = 0, IsSystem = false, ParentCategoryId = parentCategoryId};
                category.EntityTypeId = entityTypeId;
                category.EntityTypeQualifierColumn = entityTypeQualifierProperty;
                category.EntityTypeQualifierValue = entityTypeQualifierValue;
            }

            if ( category == null || !category.IsAuthorized( Authorization.VIEW, CurrentPerson ) )
            {
                return;
            }

            pnlDetails.Visible = true;
            hfCategoryId.Value = category.Id.ToString();

            // render UI based on Authorized and IsSystem
            bool readOnly = false;

            nbEditModeMessage.Text = string.Empty;
            if ( !category.IsAuthorized( Authorization.EDIT, CurrentPerson ) )
            {
                readOnly = true;
                nbEditModeMessage.Text = EditModeMessage.ReadOnlyEditActionNotAllowed( Category.FriendlyTypeName );
            }

            if ( category.IsSystem )
            {
                readOnly = true;
                nbEditModeMessage.Text = EditModeMessage.ReadOnlySystem( Category.FriendlyTypeName );
            }

            btnSecurity.Visible = category.IsAuthorized( Authorization.ADMINISTRATE, CurrentPerson );
            btnSecurity.Title = category.Name;
            btnSecurity.EntityId = category.Id;

            if ( readOnly )
            {
                btnEdit.Visible = false;
                btnDelete.Visible = false;
                ShowReadonlyDetails( category );
            }
            else
            {
                btnEdit.Visible = true;
                string errorMessage = string.Empty;
                btnDelete.Visible = categoryService.CanDelete(category, out errorMessage);
                if ( category.Id > 0 )
                {
                    ShowReadonlyDetails( category );
                }
                else
                {
                    ShowEditDetails( category );
                }
            }
        }

        /// <summary>
        /// Shows the edit details.
        /// </summary>
        /// <param name="category">The category.</param>
        private void ShowEditDetails( Category category )
        {
            if ( category.Id > 0 )
            {
                lActionTitle.Text = ActionTitle.Edit( Category.FriendlyTypeName ).FormatAsHtmlTitle();
            }
            else
            {
                lActionTitle.Text = ActionTitle.Add( Category.FriendlyTypeName ).FormatAsHtmlTitle();
            }

            SetEditMode( true );
            
            tbName.Text = category.Name;

            if ( category.EntityTypeId != 0 )
            {
                var entityType = EntityTypeCache.Read( category.EntityTypeId );
                lblEntityTypeName.Text = entityType.Name;
            }
            else
            {
                lblEntityTypeName.Text = string.Empty;
            }

            cpParentCategory.EntityTypeId = category.EntityTypeId;
            cpParentCategory.EntityTypeQualifierColumn = category.EntityTypeQualifierColumn;
            cpParentCategory.EntityTypeQualifierValue = category.EntityTypeQualifierValue;
            cpParentCategory.SetValue( category.ParentCategoryId );

            lblEntityTypeQualifierColumn.Visible = !string.IsNullOrWhiteSpace( category.EntityTypeQualifierColumn );
            lblEntityTypeQualifierColumn.Text = category.EntityTypeQualifierColumn;
            lblEntityTypeQualifierValue.Visible = !string.IsNullOrWhiteSpace( category.EntityTypeQualifierValue );
            lblEntityTypeQualifierValue.Text = category.EntityTypeQualifierValue;
            tbIconCssClass.Text = category.IconCssClass;
        }

        /// <summary>
        /// Shows the readonly details.
        /// </summary>
        /// <param name="category">The category.</param>
        private void ShowReadonlyDetails( Category category )
        {
            SetEditMode( false );

            string categoryIconHtml = !string.IsNullOrWhiteSpace( category.IconCssClass ) ?
                categoryIconHtml = string.Format( "<i class='{0} fa-2x' ></i>", category.IconCssClass ) : "";

            hfCategoryId.SetValue( category.Id );
            lReadOnlyTitle.Text = category.Name.FormatAsHtmlTitle();

            lblMainDetails.Text = new DescriptionList()
                .Add("Entity Type", category.EntityType.Name)
                .Html;
            
        }

        #endregion
    }
}