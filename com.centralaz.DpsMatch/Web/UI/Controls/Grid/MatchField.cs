using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rock.Web.UI.Controls;


namespace com.centralaz.DpsMatch.Web.UI.Controls.Grid
{
    public class MatchField : TemplateField, INotRowSelectedField
    {

        #region Properties

        /// <summary>
        /// Gets or sets the person identifier.
        /// </summary>
        /// <value>
        /// The person identifier.
        /// </value>
        public int PersonId
        {
            get { return ViewState["PersonId"] as int? ?? 0; }
            set { ViewState["PersonId"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the person.
        /// </summary>
        /// <value>
        /// The name of the person.
        /// </value>
        public string PersonName
        {
            get { return ViewState["PersonName"] as string; }
            set { ViewState["PersonName"] = value; }
        }

        public string HeaderImage
        {
            get
            {
                var headerImage = ViewState["HeaderImage"] as string;
                if ( headerImage == null )
                {
                    headerImage = String.Empty;
                    HeaderImage = headerImage;
                }
                return headerImage;
            }

            set
            {
                ViewState["HeaderImage"] = value;
            }
        }

        public int? MatchPercentage
        {
            get
            {
                var matchPercentage = ViewState["MatchPercentage"] as int?;
                if ( matchPercentage == null )
                {
                    matchPercentage = null;
                    MatchPercentage = matchPercentage;
                }
                return matchPercentage;
            }

            set
            {
                ViewState["MatchPercentage"] = value;
            }
        }

        public bool? MatchIsMatch
        {
            get
            {
                var matchIsMatch = ViewState["MatchIsMatch"] as bool?;
                if ( matchIsMatch == null )
                {
                    matchIsMatch = null;
                    MatchIsMatch = matchIsMatch;
                }
                return matchIsMatch;
            }

            set
            {
                ViewState["MatchIsMatch"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the data field.
        /// </summary>
        /// <value>
        /// The data field.
        /// </value>
        public string DataTextField
        {
            get
            {
                return ViewState["DataTextField"] as string;
            }

            set
            {
                ViewState["DataTextField"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the column.
        /// </summary>
        /// <value>
        /// The index of the column.
        /// </value>
        public int ColumnIndex
        {
            get
            {
                return ViewState["ColumnIndex"] as int? ?? 0;
            }
            set
            {
                ViewState["ColumnIndex"] = value;
            }
        }

        /// <summary>
        /// Gets the parent grid.
        /// </summary>
        /// <value>
        /// The parent grid.
        /// </value>
        public Rock.Web.UI.Controls.Grid ParentGrid { get; internal set; }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Performs basic instance initialization for a data control field.
        /// </summary>
        /// <param name="sortingEnabled">A value that indicates whether the control supports the sorting of columns of data.</param>
        /// <param name="control">The data control that owns the <see cref="T:System.Web.UI.WebControls.DataControlField" />.</param>
        /// <returns>
        /// Always returns false.
        /// </returns>
        public override bool Initialize( bool sortingEnabled, Control control )
        {
            base.Initialize( sortingEnabled, control );
            this.HeaderTemplate = new MatchFieldHeaderTemplate();
            this.ItemTemplate = new MatchFieldTemplate();
            MatchFieldFooterTemplate footerTemplate = new MatchFieldFooterTemplate();
            footerTemplate.RadioButtonClick += FooterTemplate_RadioButtonClick;
            this.FooterTemplate = footerTemplate;
            this.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            this.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            this.ParentGrid = control as Rock.Web.UI.Controls.Grid;

            return false;
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the LinkButtonClick event of the HeaderTemplate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void FooterTemplate_RadioButtonClick( object sender, EventArgs e )
        {
            //Do Something
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class MatchFieldHeaderTemplate : ITemplate
    {
        /// <summary>
        /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control" /> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
        /// </summary>
        /// <param name="container">The <see cref="T:System.Web.UI.Control" /> object to contain the instances of controls from the inline template.</param>
        public void InstantiateIn( Control container )
        {
            var cell = container as DataControlFieldCell;
            if ( cell != null )
            {
                var matchField = cell.ContainingField as MatchField;
                if ( matchField != null )
                {
                    HtmlGenericContainer headerSummary = new HtmlGenericContainer( "div", "merge-header-summary" );
                    headerSummary.Attributes.Add( "data-col", matchField.ColumnIndex.ToString() );
                    headerSummary.Controls.Add( new LiteralControl(matchField.HeaderImage ));
                    headerSummary.Controls.Add( new LiteralControl( String.Format( "<div class='merge-heading-family'>{0}</div>", matchField.PersonName ) ) );

                    cell.Controls.Add( headerSummary );
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MatchFieldTemplate : ITemplate
    {

        #region Properties

        /// <summary>
        /// Gets the data text field.
        /// </summary>
        /// <value>
        /// The data text field.
        /// </value>
        public string DataTextField { get; private set; }

        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        /// <value>
        /// The index of the column.
        /// </value>
        public int ColumnIndex { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control" /> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
        /// </summary>
        /// <param name="container">The <see cref="T:System.Web.UI.Control" /> object to contain the instances of controls from the inline template.</param>
        public void InstantiateIn( Control container )
        {
            var cell = container as DataControlFieldCell;
            if ( cell != null )
            {
                var matchField = cell.ContainingField as MatchField;
                if ( matchField != null )
                {
                    DataTextField = matchField.DataTextField;
                    ColumnIndex = matchField.ColumnIndex;

                    Literal lText = new Literal();
                    lText.ID = "lText_" + ColumnIndex.ToString();
                    lText.DataBinding += lText_DataBinding;
                    lText.Visible = true;
                    cell.Controls.Add( lText );
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the DataBinding event of the cb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void lText_DataBinding( object sender, EventArgs e )
        {
            var lText = sender as Literal;
            if ( lText != null )
            {
                GridViewRow gridViewRow = lText.NamingContainer as GridViewRow;

                if ( gridViewRow.DataItem != null )
                {
                    if ( !string.IsNullOrWhiteSpace( DataTextField ) )
                    {
                        object dataValue = DataBinder.Eval( gridViewRow.DataItem, DataTextField );
                        lText.Text = dataValue.ToString();
                    }
                    else
                    {
                        lText.Text = "Something Went Wrong";
                    }
                    lText.Visible = true;
                }
            }
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class MatchFieldFooterTemplate : ITemplate
    {
        /// <summary>
        /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control" /> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
        /// </summary>
        /// <param name="container">The <see cref="T:System.Web.UI.Control" /> object to contain the instances of controls from the inline template.</param>
        public void InstantiateIn( Control container )
        {
            var cell = container as DataControlFieldCell;
            if ( cell != null )
            {
                var matchField = cell.ContainingField as MatchField;
                if ( matchField != null )
                {
                    //Percentage
                    HtmlGenericContainer footerSummary = new HtmlGenericContainer( "div", "merge-header-summary" );
                    footerSummary.Attributes.Add( "data-col", matchField.ColumnIndex.ToString() );

                    LiteralControl lcPercentage = new LiteralControl();
                    lcPercentage.Text = String.Format( "<div class='alert alert-info'><center><h1>{0}</h1></center></div>", matchField.MatchPercentage.Value.ToString( "0.0%" ) );

                    footerSummary.Controls.Add( lcPercentage );
                    cell.Controls.Add( footerSummary );

                   // //Radio Button List
                   // var lbDelete = new LinkButton();
                   // lbDelete.CausesValidation = false;
                   // lbDelete.CssClass = "btn btn-danger btn-xs pull-right";
                   // lbDelete.ToolTip = "Remove Person";
                   // cell.Controls.Add( lbDelete );

                   // HtmlGenericControl buttonIcon = new HtmlGenericControl( "i" );
                   // buttonIcon.Attributes.Add( "class", "fa fa-times" );
                   // lbDelete.Controls.Add( buttonIcon );

                   // lbDelete.Click += lbDelete_Click;

                   // // make sure delete button is registered for async postback (needed just in case the grid was created at runtime)
                   // //var sm = ScriptManager.GetCurrent( matchField.ParentGrid.Page );
                   //// sm.RegisterAsyncPostBackControl( lbDelete );
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the lbDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void lbDelete_Click( object sender, EventArgs e )
        {
            if ( RadioButtonClick != null )
            {
                RadioButtonClick( sender, e );
            }
        }

        /// <summary>
        /// Occurs when [link button click].
        /// </summary>
        internal event EventHandler RadioButtonClick;
    }

}
