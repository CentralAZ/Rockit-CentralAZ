using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rock.Web.UI.Controls;


namespace com.centralaz.DpsMatch.Web.UI.Controls.Grid
{
    public class MatchField : SelectField, INotRowSelectedField
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

        public string PersonAddress
        {
            get
            {
                var personAddress = ViewState["PersonAddress"] as string;
                if ( personAddress == null )
                {
                    personAddress = string.Empty;
                    PersonAddress = personAddress;
                }
                return personAddress;
            }

            set
            {
                ViewState["PersonAddress"] = value;
            }
        }

        public string PersonAge
        {
            get
            {
                var personAge = ViewState["PersonAge"] as string;
                if ( personAge == null )
                {
                    personAge = string.Empty;
                    PersonAge = personAge;
                }
                return personAge;
            }

            set
            {
                ViewState["PersonAge"] = value;
            }
        }

        public int? PersonGender
        {
            get
            {
                var personGender = ViewState["PersonGender"] as int?;
                if ( personGender == null )
                {
                    personGender = null;
                    PersonGender = personGender;
                }
                return personGender;
            }

            set
            {
                ViewState["PersonGender"] = value;
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
           // MatchFieldTemplate matchFieldTemplate = new MatchFieldTemplate();
            MatchFieldHeaderTemplate headerTemplate = new MatchFieldHeaderTemplate();
            this.HeaderTemplate = headerTemplate;
           // this.ItemTemplate = matchFieldTemplate;
            this.ParentGrid = control as Rock.Web.UI.Controls.Grid;

            return false;
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
    /*
    /// <summary>
    /// Template used by the <see cref="DeleteField"/> control
    /// </summary>
    public class MatchFieldTemplate : ITemplate
    {
        /// <summary>
        /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control"/> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
        /// </summary>
        /// <param name="container">The <see cref="T:System.Web.UI.Control"/> object to contain the instances of controls from the inline template.</param>
        public void InstantiateIn( Control container )
        {
            DataControlFieldCell cell = container as DataControlFieldCell;
            if ( cell != null )
            {
                MatchField matchField = cell.ContainingField as MatchField;
                ParentGrid = matchField.ParentGrid;

                LiteralControl matchAddress = new LiteralControl();
                matchAddress.Text = matchField.PersonAddress;
                cell.Controls.Add( matchAddress );

                LiteralControl matchAge = new LiteralControl();
                matchAge.Text = matchField.PersonAge;
                cell.Controls.Add( matchAge );

                LiteralControl matchGender = new LiteralControl();
                matchGender.Text = String.Format("{0}",matchField.PersonGender);
                cell.Controls.Add( matchGender );
            }
        }

        /// <summary>
        /// Gets or sets the parent grid.
        /// </summary>
        /// <value>
        /// The parent grid.
        /// </value>
        private Rock.Web.UI.Controls.Grid ParentGrid { get; set; }
    }*/

}
