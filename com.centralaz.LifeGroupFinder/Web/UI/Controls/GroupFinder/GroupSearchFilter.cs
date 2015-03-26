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
using System.Linq;
using System.Runtime.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock.Data;
using Rock.Model;
using Rock;
using Rock.Web.UI.Controls;
using Rock.Web.Cache;


namespace com.centralaz.LifeGroupFinder.Web.UI.Controls.GroupFinder
{
    /// <summary>
    /// Report Filter control
    /// </summary>
    [ToolboxData( "<{0}:GroupSearchFilter runat=server></{0}:GroupSearchFilter>" )]
    public class GroupSearchFilter : CompositeControl
    {

        public string ChildrenState
        {
            get
            {
                return ViewState["ChildrenState"] as string;
            }

            set
            {
                ViewState["ChildrenState"] = value;
            }
        }

        public bool PetsState
        {
            get
            {
                bool? hasPets = ViewState["PetsState"] as bool?;
                if ( hasPets != null )
                {
                    return hasPets.Value;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                ViewState["PetsState"] = value;
            }
        }

        public string DaysState
        {
            get
            {
                return ViewState["DaysState"] as string;
            }

            set
            {
                ViewState["DaysState"] = value;
            }
        }
        private HiddenFieldWithClass _hfExpanded;
        public RockCheckBox _cbPets;
        public RockCheckBoxList _cblDays;
        public RockCheckBoxList _cblChildren;
        public Label _lblName;
        public Label _lblChildren;
        public Label _lblDays;
        public Label _lblPets;
        public LinkButton _lbSearch;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WorkflowActivityTypeEditor"/> is expanded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if expanded; otherwise, <c>false</c>.
        /// </value>
        public bool Expanded
        {
            get
            {
                EnsureChildControls();
                return _hfExpanded.Value.AsBooleanOrNull() ?? false;
            }

            set
            {
                EnsureChildControls();
                _hfExpanded.Value = value.ToString();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            string script = @"
// groupsearch-filter animation
$('.groupsearch-filter > header').click(function () {
    $(this).siblings('.panel-body').slideToggle();

    $expanded = $(this).children('input.filter-expanded');
    $expanded.val($expanded.val() == 'True' ? 'False' : 'True');

    $('i.groupsearch-filter-state', this).toggleClass('fa-chevron-right');
    $('i.groupsearch-filter-state', this).toggleClass('fa-chevron-down');
});

// fix so that the Remove button will fire its event, but not the parent event 
$('.groupsearch-filter a.btn-danger').click(function (event) {
    event.stopImmediatePropagation();
});

$('.groupsearch-filter > .panel-body').on('validation-error', function() {
    var $header = $(this).siblings('header');
    $(this).slideDown();

    $expanded = $header.children('input.filter-expanded');
    $expanded.val('True');

    $('i.groupsearch-filter-state', $header).removeClass('fa-chevron-right');
    $('i.groupsearch-filter-state', $header).addClass('fa-chevron-down');

    return false;
});

";

            ScriptManager.RegisterStartupScript( this.Page, this.Page.GetType(), "GroupSearchFilterScript", script, true );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( Page.IsPostBack )
            {
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();

            _hfExpanded = new HiddenFieldWithClass();
            Controls.Add( _hfExpanded );
            _hfExpanded.ID = this.ID + "_hfExpanded";
            _hfExpanded.CssClass = "filter-expanded";
            _hfExpanded.Value = "False";

            _lbSearch = new LinkButton();
            _lbSearch.CausesValidation = false;
            _lbSearch.ID = "_lbSearch";
            _lbSearch.CssClass = "btn btn-primary";
            _lbSearch.Click += Search_Click;
            _lbSearch.Text = "Search";

            _lblName = new Label();
            _lblName.Text = "Add options to enhance your search.";

            _lblChildren = new Label();
            _lblChildren.Text = "If you would have children and want to find groups with children their age please select all that apply";

            _lblDays = new Label();
            _lblDays.Text = "Day the group meets?";

            _lblPets = new Label();
            _lblPets.Text = "Would pets take away from your experience?";

            _cbPets = new RockCheckBox();
            _cbPets.ID = "_cbPets";
            _cbPets.Text = "Yes";
            _cbPets.Help = "This will remove groups that have pets from your search.";
            _cbPets.Checked = PetsState;
            _cbPets.CheckedChanged += _cbPets_CheckedChanged;

            _cblDays = new RockCheckBoxList();
            _cblDays.ID = "_cblDays";
            _cblDays.RepeatDirection = RepeatDirection.Horizontal;
            _cblDays.Items.Clear();
            foreach ( var dow in Enum.GetValues( typeof( DayOfWeek ) ).OfType<DayOfWeek>().ToList() )
            {
                _cblDays.Items.Add( new ListItem( dow.ConvertToString().Substring( 0, 3 ), dow.ConvertToInt().ToString() ) );
            }
            if ( !string.IsNullOrWhiteSpace( DaysState ) )
            {
                _cblDays.SetValues( DaysState.Split( ';' ).ToList() );
            }
            _cblDays.SelectedIndexChanged += _cblDays_SelectedIndexChanged;
            _cblDays.AutoPostBack = false;

            _cblChildren = new RockCheckBoxList();
            _cblChildren.ID = "_cblChildren";
            _cblChildren.Items.Clear();
            _cblChildren.BindToDefinedType( DefinedTypeCache.Read( "512F355E-9441-4C47-BE47-7FFE19209496".AsGuid() ) );
            _cblChildren.RepeatColumns = 2;
            _cblChildren.RepeatDirection = RepeatDirection.Horizontal;
            if ( !string.IsNullOrWhiteSpace( ChildrenState ) )
            {
                _cblChildren.SetValues( ChildrenState.Split( ';' ).ToList() );
            }
            _cblChildren.TextChanged += _cblChildren_SelectedIndexChanged;
            _cblChildren.AutoPostBack = false;

            Controls.Add( _lbSearch );
            Controls.Add( _lblPets );
            Controls.Add( _lblName );
            Controls.Add( _lblDays );
            Controls.Add( _lblChildren );
            Controls.Add( _cblChildren );
            Controls.Add( _cblDays );
            Controls.Add( _cbPets );
        }

        void _cblChildren_SelectedIndexChanged( object sender, EventArgs e )
        {
            ChildrenState = ( sender as RockCheckBoxList ).SelectedValues.AsDelimited( ";" );
        }

        void _cblDays_SelectedIndexChanged( object sender, EventArgs e )
        {
            DaysState = ( sender as RockCheckBoxList ).SelectedValues.AsDelimited( ";" );
        }

        void _cbPets_CheckedChanged( object sender, EventArgs e )
        {
            PetsState = ( sender as RockCheckBox ).Checked;
        }

        /// <summary>
        /// Handles the Click event of the AddCheckinLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Search_Click( object sender, EventArgs e )
        {
            if ( SearchClick != null )
            {
                SearchClick( sender, e );
            }
        }

        /// <summary>
        /// Writes the <see cref="T:System.Web.UI.WebControls.CompositeControl" /> content to the specified <see cref="T:System.Web.UI.HtmlTextWriter" /> object, for display on the client.
        /// </summary>
        /// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter" /> that represents the output stream to render HTML content on the client.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel panel-widget groupsearch-filter" );
            //writer.AddAttribute( "data-key", _hfGroupTypeGuid.Value );
            writer.AddAttribute( HtmlTextWriterAttribute.Id, this.ID + "_section" );
            writer.RenderBeginTag( "section" );

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel-heading clearfix clickable" );
            writer.RenderBeginTag( "header" );

            // Hidden Field to track expansion
            _hfExpanded.RenderControl( writer );

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "pull-left panel-actions" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            writer.WriteLine( string.Format( "<a class='btn btn-xs btn-link'><i class='groupsearch-filter-state fa {0}'></i></a>",
                Expanded ? "fa fa-chevron-down" : "fa fa-chevron-right" ) );

            // Add/ChevronUpDown/Delete div
            writer.RenderEndTag();

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "filter-toggle pull-left" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel-title" );
            writer.RenderBeginTag( HtmlTextWriterTag.H3 );
            _lblName.RenderControl( writer );

            // H3 tag
            writer.RenderEndTag();

            // Name/Description div
            writer.RenderEndTag();

            // header div
            writer.RenderEndTag();

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel-body" );

            if ( !Expanded )
            {
                writer.AddStyleAttribute( "display", "none" );
            }

            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            // make two span6 columns: Left Column for Children label. Right Column for children checkboxlist
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "row" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-sm-6" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            _lblChildren.RenderControl( writer );
            writer.RenderEndTag();

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-sm-6" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            _cblChildren.RenderControl( writer );
            writer.RenderEndTag();

            // rowfluid
            writer.RenderEndTag();

            writer.RenderBeginTag( HtmlTextWriterTag.Hr );
            writer.RenderEndTag();

            // make two span6 columns: Left Column for days label. Right Column for days checkboxlist
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "row" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-sm-6" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            _lblDays.RenderControl( writer );
            writer.RenderEndTag();

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-sm-6" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            _cblDays.RenderControl( writer );
            writer.RenderEndTag();

            // rowfluid
            writer.RenderEndTag();

            writer.RenderBeginTag( HtmlTextWriterTag.Hr );
            writer.RenderEndTag();

            // make three span4 columns: Left Column for pets label. Middle for pets checkbox. Right Column for searchButton
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "row" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-sm-6" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            _lblPets.RenderControl( writer );
            writer.RenderEndTag();

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-sm-3" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            _cbPets.RenderControl( writer );
            writer.RenderEndTag();

            writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-sm-3" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            _lbSearch.RenderControl( writer );
            writer.RenderEndTag();

            // rowfluid
            writer.RenderEndTag();

            // widget-content div
            writer.RenderEndTag();

            // section tag
            writer.RenderEndTag();
        }

        /// <summary>
        /// Occurs when [search click].
        /// </summary>
        public event EventHandler SearchClick;
    }
}