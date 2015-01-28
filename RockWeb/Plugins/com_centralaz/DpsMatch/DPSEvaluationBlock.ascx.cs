
using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using com.centralaz.DpsMatch.Model;
using com.centralaz.DpsMatch.Data;
using com.centralaz.DpsMatch.Web.UI.Controls.Grid;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;

namespace RockWeb.Plugins.com_centralaz.DpsMatch
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "DPS Evaluation Block" )]
    [Category( "com_centralaz > DpsMatch" )]
    [Description( "Block to manually evaluate Person entries similar to known sexual offenders" )]
    public partial class DPSEvaluationBlock : Rock.Web.UI.RockBlock
    {
        #region Fields

        Dictionary<int, List<Match>> _matchList = new Dictionary<int, List<Match>>();
        OffenderService _offenderService = new OffenderService( new DpsMatchContext() );
        int _dictionaryIndex = 0;

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
                if ( _matchList.Count == 0 )
                {
                    PopulateMatchList();
                }
                BuildColumns();
                BindGrid();
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

        /// <summary>
        /// Handles the Click event of the lbMerge control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbNext_Click( object sender, EventArgs e )
        {
            //Save changes to PMs
            //Update any Person attributes
            _dictionaryIndex++;
            if ( ( _matchList.Count - 1 ) >= _dictionaryIndex )
            {
                BuildColumns();
            }
            else
            {
                _dictionaryIndex = 0;
                //display "Completed" view
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Builds the values columns.
        /// </summary>
        private void BuildColumns()
        {
            gValues.Columns.Clear();
            if ( _matchList.ElementAt( _dictionaryIndex ).Value != null )
            {

                Offender offender = _offenderService.Get( _matchList.ElementAt( _dictionaryIndex ).Key );
                var offenderCol = new MatchField();
                offenderCol.SelectionMode = SelectionMode.Single;
                offenderCol.PersonId = offender.Id;
                offenderCol.PersonName = String.Format( "{0} {1}", offender.FirstName, offender.LastName );
                gValues.Columns.Add( offenderCol );

                var personService = new PersonService( new RockContext() );
                List<Match> matchSubList = _matchList.ElementAt( _dictionaryIndex ).Value;
                Person person = new Person();
                foreach ( Match match in _matchList.ElementAt( _dictionaryIndex ).Value )
                {
                    person = match.PersonAlias.Person;
                    var personCol = new MatchField();
                    personCol.SelectionMode = SelectionMode.Single;
                    personCol.PersonId = person.Id;
                    if ( person.NickName != person.FirstName )
                    {
                        personCol.PersonName = String.Format( "{0}({1}) {2}", person.FirstName, person.NickName, person.LastName );
                    }
                    else
                    {
                        personCol.PersonName = person.FullName;
                    }
                    string imgTag = Rock.Model.Person.GetPhotoImageTag( person.PhotoId, person.Age, person.Gender, 200, 200 );
                    if ( person.PhotoId.HasValue )
                    {
                        personCol.HeaderImage = string.Format( "<a href='{0}'>{1}</a>", person.PhotoUrl, imgTag );
                    }
                    gValues.Columns.Add( personCol );
                }
            }
        }


        /// <summary>
        /// Binds the values.
        /// </summary>
        private void BindGrid()
        {
            if ( _matchList.ElementAt( _dictionaryIndex ).Value != null )
            {
                Offender offender = _offenderService.Get( _matchList.ElementAt( _dictionaryIndex ).Key );
                List<Match> matchSubList = _matchList.ElementAt( _dictionaryIndex ).Value;
                DataTable dt = GetDataTable( offender, matchSubList );
                gValues.DataSource = dt;
                gValues.DataBind();
            }
        }
        /// <summary>
        /// Gets the values column header.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns></returns>
        private string GetValuesColumnHeader( int personId )
        {
            Guid familyGuid = new Guid( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY );

            var groupMemberService = new GroupMemberService( new RockContext() );
            var families = groupMemberService.Queryable()
                .Where( m => m.PersonId == personId && m.Group.GroupType.Guid == familyGuid )
                .Select( m => m.Group )
                .Distinct();

            StringBuilder sbHeaderData = new StringBuilder();

            foreach ( var family in families )
            {
                sbHeaderData.Append( "<div class='merge-heading-family'>" );

                var nickNames = groupMemberService.Queryable( "Person" )
                    .Where( m => m.GroupId == family.Id )
                    .OrderBy( m => m.GroupRole.Order )
                    .ThenBy( m => m.Person.BirthDate ?? DateTime.MinValue )
                    .ThenByDescending( m => m.Person.Gender )
                    .Select( m => m.Person.NickName )
                    .ToList();
                if ( nickNames.Any() )
                {
                    sbHeaderData.AppendFormat( "{0} ({1})", family.Name, nickNames.AsDelimited( ", " ) );
                }
                else
                {
                    sbHeaderData.Append( family.Name );
                }

                bool showType = family.GroupLocations.Count() > 1;
                foreach ( var loc in family.GroupLocations )
                {
                    sbHeaderData.AppendFormat( " <span class='merge-heading-location'>{0}{1}</span>",
                        loc.Location.ToStringSafe(),
                        ( showType ? " (" + loc.GroupLocationTypeValue.Value + ")" : "" ) );
                }

                sbHeaderData.Append( "</div>" );
            }

            return sbHeaderData.ToString();

        }

        protected void PopulateMatchList()
        {
            List<Match> matchList = new MatchService( new DpsMatchContext() ).Queryable().ToList();
            foreach ( Match match in matchList )
            {
                if ( _matchList.ContainsKey( match.OffenderId ) )
                {
                    _matchList[match.OffenderId].Add( match );
                }
                else
                {
                    _matchList.Add( match.OffenderId, new List<Match>() );
                    _matchList[match.OffenderId].Add( match );
                }
            }
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="headingKeys">The heading keys.</param>
        /// <returns></returns>
        public DataTable GetDataTable( Offender offender, List<Match> matchList )
        {
            var tbl = new DataTable();

            tbl.Columns.Add( "Offender" );

            foreach ( Match match in matchList )
            {
                tbl.Columns.Add( string.Format( "{0}", match.Id ) );
            }

            var rowValues = new List<object>();
            //Address
            rowValues.Add( String.Format( "{0} {1},{2} {3}", offender.ResidentialAddress, offender.ResidentialCity, offender.ResidentialState, offender.ResidentialZip ) );
            foreach ( Match match in matchList )
            {
                rowValues.Add( match.PersonAlias.Person.GetFamilies().FirstOrDefault().GroupLocations.FirstOrDefault().Location.GetFullStreetAddress() );
            }
            tbl.Rows.Add( rowValues.ToArray() );

            //Age
            rowValues = new List<object>();
            rowValues.Add( String.Format( "{0}", offender.Age ) );
            foreach ( Match match in matchList )
            {
                rowValues.Add( String.Format( "{0}", match.PersonAlias.Person.Age ) );
            }

            //Gender
            rowValues = new List<object>();
            rowValues.Add( String.Format( "{0}", offender.Age ) );
            foreach ( Match match in matchList )
            {
                rowValues.Add( String.Format( "{0}", match.PersonAlias.Person.Age ) );
            }

            return tbl;
        }
        #endregion
    }
}