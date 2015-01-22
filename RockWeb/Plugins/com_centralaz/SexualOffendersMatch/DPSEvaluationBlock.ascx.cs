
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using com.centralaz.DpsMatch.Model;
using com.centralaz.DpsMatch.Data;

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

        Dictionary<String, List<Match>> _matchList;
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
                if ( _matchList == null )
                {
                    PopulateMatchList();
                }
                BuildColumns();
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
                var keyCol = new BoundField();
                keyCol.DataField = "Key";
                keyCol.Visible = false;
                gValues.Columns.Add( keyCol );

                //Set this as SO
                var labelCol = new BoundField();
                labelCol.DataField = "Label";
                labelCol.HeaderStyle.CssClass = "merge-personselect";
                gValues.Columns.Add( labelCol );

                var personService = new PersonService( new RockContext() );
                foreach ( Match match in _matchList.ElementAt( _dictionaryIndex ).Value )
                {
                    var personCol = new BoundField();
                    personCol.HeaderStyle.CssClass = "merge-personselect";
                    gValues.Columns.Add( personCol );
                }
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
                _matchList[match.KeyString].Add( match );
            }
        }

        #endregion
    }
}