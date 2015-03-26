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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.com_centralaz.LifeGroupFinder
{
    [DisplayName( "Life Group List" )]
    [Category( "com_centralaz > Groups" )]
    [Description( "Lists all groups for the configured group types." )]

    [LinkedPage( "Detail Page", "", true, "", "", 0 )]
    [CustomDropdownListField( "Limit to Active Status", "Select which groups to show, based on active status. Select [All] to let the user filter by active status.", "all^[All], active^Active, inactive^Inactive", false, "all", Order = 10 )]
    [ContextAware]
    public partial class LifeGroupList : RockBlock
    {
        private int _groupTypesCount = 0;

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            this.BlockUpdated += GroupList_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlGroupList );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                LoadList();
            }

            base.OnLoad( e );
        }

        /// <summary>
        /// Handles the BlockUpdated event of the GroupList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GroupList_BlockUpdated( object sender, EventArgs e )
        {

        }

        #endregion

        #region Internal Methods

        private void LoadList()
        {
            // Build qry
            int smallGroupTypeId = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_SMALL_GROUP ).Id;
            RockContext rockContext = new RockContext();
            AttributeValueService attributeValueService = new AttributeValueService( rockContext );
            var qry = new GroupService( rockContext ).Queryable()
                .Where( g => smallGroupTypeId == g.GroupTypeId );
            if ( PageParameter( "Campus" ).AsIntegerOrNull() != null )
            {
                qry = qry.Where( g => g.CampusId == PageParameter( "Campus" ).AsIntegerOrNull() );
            }
            if ( !String.IsNullOrWhiteSpace( PageParameter( "Days" ) ) )
            {
                List<String> daysList = PageParameter( "Days" ).Split( ';' ).ToList();
                if ( daysList.Any() )
                {
                    //TODO: find out how days works
                    qry = qry.Where( g => daysList.Contains( g.Schedule.WeeklyDayOfWeek.Value.ConvertToInt().ToString() ) );
                }
            }
            bool? hasPets = PageParameter( "Pets" ).AsBooleanOrNull();
            if ( hasPets != null || hasPets.Value )
            {
                var attributeValues = attributeValueService
                                        .Queryable()
                                        .Where( v => v.Attribute.Key == "HasPets" && v.Value == "False");

                qry = qry.Where( g => attributeValues.Select( v => v.EntityId ).Contains( g.Id ) );
            }
            if ( !String.IsNullOrWhiteSpace( PageParameter( "Children" ) ) )
            {
                List<String> childrenList = PageParameter( "Children" ).Split( ';' ).ToList();
                if ( childrenList.Any() )
                {
                    var attributeValues = attributeValueService
                                        .Queryable()
                                        .Where( v => v.Attribute.Key == "HasChildren" );

                    qry = qry.Where( g => childrenList.Intersect( attributeValues.FirstOrDefault( v => v.EntityId == g.Id ).Value.Split( ',' ).ToList() ).Any() );
                }
            }
            if ( PageParameter( "StreetAddress1" ).AsIntegerOrNull() != null )
            {
                qry = qry.Where( g => g.CampusId == PageParameter( "StreetAddress1" ).AsIntegerOrNull() );
            }

            bool shadeRow = false;
            // Construct List
            foreach ( Group group in qry )
            {
                if ( shadeRow )
                {
                    phGroups.Controls.Add( new LiteralControl( "<div class='row' >" ) );
                }
                else
                {
                    phGroups.Controls.Add( new LiteralControl( "<div class='row'>" ) );
                }
                phGroups.Controls.Add( new LiteralControl( "<div class='col-md-1'>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='photo'>" ) );
                GroupMember leader = group.Members.FirstOrDefault( m => m.GroupRole.IsLeader == true );
                if ( leader != null )
                {
                    Person person = leader.Person;
                    string imgTag = Rock.Model.Person.GetPhotoImageTag( person.PhotoId, person.Age, person.Gender, 200, 200 );
                    if ( person.PhotoId.HasValue )
                    {
                        phGroups.Controls.Add( new LiteralControl( String.Format( "<a href='{0}'>{1}</a>", person.PhotoUrl, imgTag ) ) );
                    }
                    else
                    {
                        phGroups.Controls.Add( new LiteralControl( "imgTag" ) );
                    }
                }

                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='col-md-1'>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='row'>" ) );
                group.LoadAttributes();
                if ( group.GetAttributeValue( "HasPets" ).AsBoolean() )
                {
                    phGroups.Controls.Add( new LiteralControl( "<i class='fa fa-paw fa-4x text-primary fa-fw'></i>" ) );
                }
                else
                {
                    phGroups.Controls.Add( new LiteralControl( "<i class='fa fa-paw fa-4x text-muted fa-fw'></i>" ) );
                }

                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='row'>" ) );

                if ( group.GetAttributeValues( "HasChildren" ).Any() )
                {
                    phGroups.Controls.Add( new LiteralControl( "<i class='fa fa-child fa-4x text-primary fa-fw'></i>" ) );
                }
                else
                {
                    phGroups.Controls.Add( new LiteralControl( "<i class='fa fa-child fa-4x text-muted fa-fw'></i>" ) );
                }

                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='col-md-10'>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='row'>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='pull-left'>" ) );
                phGroups.Controls.Add( new LiteralControl( String.Format( "<u><b><p style='font-size:24px'>{0}</p></b></u>", group.Name ) ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='pull-right'>" ) );
                phGroups.Controls.Add( new LiteralControl( string.Format( "<a href='{0}'>View>></a>", ResolveUrl( string.Format( "~/LifeGroup/{0}", group.Id ) ) ) ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='row'>" ) );
                phGroups.Controls.Add( new LiteralControl( group.Description ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "<hr />" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='row'>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='pull-left'>" ) );
                phGroups.Controls.Add( new LiteralControl( group.Schedule.ToString() ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "<center>" ) );
                phGroups.Controls.Add( new LiteralControl( group.GetAttributeValue( "Crossroads" ) ) );
                phGroups.Controls.Add( new LiteralControl( "</center>" ) );
                phGroups.Controls.Add( new LiteralControl( "<div class='pull-right'>" ) );
                phGroups.Controls.Add( new LiteralControl( GetDistance( group ) ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "</div>" ) );
                phGroups.Controls.Add( new LiteralControl( "</br>" ) );
                phGroups.Controls.Add( new LiteralControl( "</br>" ) );
                shadeRow = !shadeRow;
            }

        }
        protected string GetDistance( Group group )
        {
            Location personLocation = new LocationService( new RockContext() )
                        .Get( PageParameter( "StreetAddress1" ), PageParameter( "StreetAddress2" ), PageParameter( "City" ),
                            PageParameter( "State" ), PageParameter( "PostalCode" ), PageParameter( "Country" ) );
            double? closestLocation = null;
            foreach ( var groupLocation in group.GroupLocations
                        .Where( gl => gl.Location.GeoPoint != null ) )
            {

                if ( personLocation != null && personLocation.GeoPoint != null )
                {
                    double meters = groupLocation.Location.GeoPoint.Distance( personLocation.GeoPoint ) ?? 0.0D;
                    double miles = meters * ( 1 / 1609.344 );//TODO: replace with Location.MilesperMeter

                    // If this group already has a distance calculated, see if this location is closer and if so, use it instead
                    if ( closestLocation != null )
                    {
                        if ( closestLocation < miles )
                        {
                            closestLocation = miles;
                        }
                    }
                    else
                    {
                        closestLocation = miles;
                    }
                }
            }
            if ( closestLocation != null )
            {
                return closestLocation.Value.ToString( "0.0" );
            }
            else
            {
                return String.Empty;
            }
        }
        #endregion
    }
}