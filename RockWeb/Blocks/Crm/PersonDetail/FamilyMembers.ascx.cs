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
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Rock;
using Rock.Attribute;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Data;

namespace RockWeb.Blocks.Crm.PersonDetail
{
    [DisplayName( "Family Members" )]
    [Category( "CRM > Person Detail" )]
    [Description( "Allows you to view the members of a family." )]
    public partial class FamilyMembers : Rock.Web.UI.PersonBlock
    {

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            rptrFamilies.ItemDataBound += rptrFamilies_ItemDataBound;
 	        base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            //if (!Page.IsPostBack)
            //{
                BindFamilies();
            //}
        }

        #endregion

        #region Events

        void rptrFamilies_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
            {
                var group = e.Item.DataItem as Group;
                if (group != null)
                {
                    HyperLink hlEditFamily = e.Item.FindControl( "hlEditFamily" ) as HyperLink;
                    if ( hlEditFamily != null )
                    {
                        hlEditFamily.NavigateUrl = string.Format( "~/EditFamily/{0}/{1}", Person.Id, group.Id );
                    }

                    var rockContext = new RockContext();

                    Repeater rptrMembers = e.Item.FindControl( "rptrMembers" ) as Repeater;
                    if (rptrMembers != null)
                    {
                        var members = new GroupMemberService( rockContext ).Queryable( "GroupRole,Person" )
                            .Where( m => 
                                m.GroupId == group.Id &&
                                m.PersonId != Person.Id )
                            .OrderBy( m => m.GroupRole.Order )
                            .ToList();

                        var orderedMembers = new List<GroupMember>();
                        
                        // Add adult males
                        orderedMembers.AddRange(members
                            .Where( m => m.GroupRole.Guid.Equals(new Guid(Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT)) &&
                                m.Person.Gender == Gender.Male)
                            .OrderByDescending( m => m.Person.Age));
                        
                        // Add adult females
                        orderedMembers.AddRange(members
                            .Where( m => m.GroupRole.Guid.Equals(new Guid(Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT)) &&
                                m.Person.Gender != Gender.Male)
                            .OrderByDescending( m => m.Person.Age));

                        // Add non-adults
                        orderedMembers.AddRange(members
                            .Where( m => !m.GroupRole.Guid.Equals(new Guid(Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT)))
                            .OrderByDescending( m => m.Person.Age));

                        rptrMembers.ItemDataBound += rptrMembers_ItemDataBound;
                        rptrMembers.DataSource = orderedMembers;
                        rptrMembers.DataBind();
                    }

                    Repeater rptrAddresses =  e.Item.FindControl("rptrAddresses") as Repeater;
                    {
                        rptrAddresses.ItemDataBound += rptrAddresses_ItemDataBound;
                        rptrAddresses.ItemCommand += rptrAddresses_ItemCommand;
                        rptrAddresses.DataSource = new GroupLocationService( rockContext ).Queryable( "Location,GroupLocationTypeValue" )
                            .Where( l => l.GroupId == group.Id)
                            .OrderBy( l => l.GroupLocationTypeValue.Order)
                            .ToList();
                        rptrAddresses.DataBind();
                    }
                }
            }
        }

        void rptrMembers_ItemDataBound( object sender, RepeaterItemEventArgs e )
        {
            if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
            {
                var groupMember = e.Item.DataItem as GroupMember;
                if ( groupMember != null && groupMember.Person != null )
                {
                    Person fm = groupMember.Person;

                    // very similar code in EditFamily.ascx.cs
                    HtmlControl divPersonImage = e.Item.FindControl( "divPersonImage" ) as HtmlControl;
                    if ( divPersonImage != null )
                    {
                        divPersonImage.Style.Add( "background-image", @String.Format( @"url({0})", Person.GetPhotoUrl( fm.PhotoId, fm.Gender ) + "&width=65" ) );
                        divPersonImage.Style.Add("background-size",  "cover");
                        divPersonImage.Style.Add("background-position", "50%");
                    }
                }
            }
        }

        void rptrAddresses_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
            {
                var groupLocation = e.Item.DataItem as GroupLocation;
                if (groupLocation != null && groupLocation.Location != null)
                {
                    Location loc = groupLocation.Location;

                    HtmlAnchor aMap = e.Item.FindControl( "aMap" ) as HtmlAnchor;
                    if ( aMap != null )
                    {
                        aMap.HRef = loc.GoogleMapLink( Person.FullName );
                    }

                    LinkButton lbVerify = e.Item.FindControl( "lbVerify" ) as LinkButton;
                    if ( lbVerify != null )
                    {
                        if ( Rock.Address.VerificationContainer.Instance.Components.Any( c => c.Value.Value.IsActive ) )
                        {
                            lbVerify.Visible = true;
                            lbVerify.CommandName = "verify";
                            lbVerify.CommandArgument = loc.Id.ToString();

                            if ( loc.GeoPoint != null )
                            {
                                lbVerify.ToolTip = string.Format( "{0} {1}",
                                    loc.GeoPoint.Latitude,
                                    loc.GeoPoint.Longitude );
                            }
                            else
                            {
                                lbVerify.ToolTip = "Verify Address";
                            }
                        }
                        else
                        {
                            lbVerify.Visible = false;
                        }
                    }

                    if ( !string.IsNullOrWhiteSpace( loc.Street2 ) )
                    {
                        PlaceHolder phStreet2 = e.Item.FindControl( "phStreet2" ) as PlaceHolder;
                        if ( phStreet2 != null )
                        {
                            phStreet2.Controls.Add( new LiteralControl( string.Format( "{0}</br>", loc.Street2 ) ) );
                        }
                    }
                }
            }
        }

        void rptrAddresses_ItemCommand( object source, RepeaterCommandEventArgs e )
        {
            int locationId = int.MinValue;
            if ( int.TryParse( e.CommandArgument.ToString(), out locationId ) )
            {
                var rockContext = new RockContext();
                var service = new LocationService( rockContext );
                var location = service.Get( locationId );

                switch ( e.CommandName )
                {
                    case "verify":
                        service.Verify( location, true );
                        break;
                }

                rockContext.SaveChanges();
            }

            BindFamilies();
        }

        #endregion

        #region Methods

        private void BindFamilies()
        {
            if ( Person != null && Person.Id > 0 )
            {
                Guid familyGroupGuid = new Guid( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY );

                var rockContext = new RockContext();

                var memberService = new GroupMemberService( rockContext );
                var families = memberService.Queryable()
                    .Where( m =>
                        m.PersonId == Person.Id &&
                        m.Group.GroupType.Guid == familyGroupGuid
                    )
                    .Select( m => m.Group )
                    .ToList();

                if ( !families.Any() )
                {
                    var familyGroupType = GroupTypeCache.GetFamilyGroupType();
                    if ( familyGroupType != null )
                    {
                        var groupMember = new GroupMember();
                        groupMember.PersonId = Person.Id;
                        groupMember.GroupRoleId = familyGroupType.DefaultGroupRoleId.Value;

                        var family = new Group();
                        family.Name = Person.LastName;
                        family.GroupTypeId = familyGroupType.Id;
                        family.Members.Add( groupMember );

                        var groupService = new GroupService( rockContext );
                        groupService.Add( family );
                        rockContext.SaveChanges();

                        families.Add( groupService.Get( family.Id ) );
                    }
                }

                rptrFamilies.DataSource = families;
                rptrFamilies.DataBind();
            }
        }

        protected string FormatAddressType(object addressType)
        {
            string type = addressType.ToString();
            return type.EndsWith("Address", StringComparison.CurrentCultureIgnoreCase) ? type : type + " Address";
        }

        protected string FormatPersonLink(string personId)
        {
            return ResolveRockUrl( string.Format( "~/Person/{0}", personId ) );
        }

        protected string FormatAsHtmlTitle(string str)
        {
            return str.FormatAsHtmlTitle();
        }

        #endregion

    }
}