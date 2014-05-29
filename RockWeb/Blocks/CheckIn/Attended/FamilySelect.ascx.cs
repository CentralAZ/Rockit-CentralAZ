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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using System.Runtime.InteropServices;
using Rock.Data;

namespace RockWeb.Blocks.CheckIn.Attended
{
    /// <summary>
    /// Family Select block for Attended Check-in
    /// </summary>
    [DisplayName("Family Select")]
    [Category("Check-in > Attended")]
    [Description( "Attended Check-In Family Select Block" )]
    [BooleanField( "Enable Add Buttons", "Show the add people/visitor/family buttons on the family select page?", true )]
    [TextField("Not Found Text", "What text should display when the nothing is found?", true, "Please add them using one of the buttons on the right")]
    public partial class FamilySelect : CheckInBlock
    {
        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( CurrentWorkflow == null || CurrentCheckInState == null )
            {
                NavigateToHomePage();
            }

            if ( !Page.IsPostBack )
            {
                if ( CurrentCheckInState.CheckIn.Families.Count > 0 )
                {
                    // TODO: make ordering smarter, possibly by campus then by caption
                    var familyList = CurrentCheckInState.CheckIn.Families.OrderBy( f => f.Caption ).ToList();
                    if ( !UserBackedUp )
                    {
                        familyList.FirstOrDefault().Selected = true;
                    }

                    ProcessFamily();
                    lvFamily.DataSource = familyList;
                    lvFamily.DataBind();
                }
                else
                {
                    bool showAddButtons = bool.Parse( GetAttributeValue( "EnableAddButtons" ) );
                    string nothingFoundText = GetAttributeValue( "NotFoundText" );
                    lblFamilyTitle.InnerText = "No Search Results";
                    lbNext.Enabled = false;
                    lbNext.Visible = false;
                    pnlFamily.Visible = false;
                    pnlPerson.Visible = false;
                    pnlVisitor.Visible = false;
                    actions.Visible = false;
                    divNothingFound.Visible = true;
                    divNothingFound.InnerText = nothingFoundText;
                    lbAddPerson.Visible = showAddButtons;
                    lbAddVisitor.Visible = showAddButtons;
                    lbAddFamily.Visible = showAddButtons;                    
                }

                rGridPersonResults.PageSize = 4;
            }
        }

        /// <summary>
        /// Handles the Click event of the lbBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbBack_Click( object sender, EventArgs e )
        {
            if ( CurrentCheckInState != null && CurrentCheckInState.CheckIn != null )
            {
                CurrentCheckInState.CheckIn.Families = new List<CheckInFamily>();
            }
            GoBack();
        }

        /// <summary>
        /// Handles the Click event of the lbNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbNext_Click( object sender, EventArgs e )
        {
            var selectedPeopleList = ( hfSelectedPerson.Value + hfSelectedVisitor.Value ).SplitDelimitedValues();
            var family = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault();
            if ( family == null )
            {
                maWarning.Show( "Please pick a family.", ModalAlertType.Warning );
                return;
            }
            else if ( family.People.Count == 0 )
            {
                string errorMsg = "No one in this family is eligible to check-in.";
                maWarning.Show( errorMsg, Rock.Web.UI.Controls.ModalAlertType.Warning );
                return;
            }

            if ( selectedPeopleList.Count() > 0 )
            {
                var selectedPeopleIds = selectedPeopleList.Select( int.Parse ).ToList();
                family.People.ForEach( p => p.Selected = false );
                foreach ( var person in family.People.Where( p => selectedPeopleIds.Contains( p.Person.Id ) ).ToList() )
                {
                    person.Selected = true;
                }

                var errors = new List<string>();
                if ( ProcessActivity( "Activity Search", out errors ) )
                {
                    SaveState();
                    NavigateToNextPage();
                }
                else
                {
                    string errorMsg = "<ul><li>" + errors.AsDelimited( "</li><li>" ) + "</li></ul>";
                    maWarning.Show( errorMsg, Rock.Web.UI.Controls.ModalAlertType.Warning );
                }
            }
            else
            {
                maWarning.Show( "Please pick at least one person.", ModalAlertType.Warning );
                return;
            }
        }

        #endregion

        #region Load Methods

        /// <summary>
        /// Handles the DataBound event of the lvFamily control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lvFamily_ItemDataBound( object sender, ListViewItemEventArgs e )
        {
            if ( e.Item.ItemType == ListViewItemType.DataItem )
            {
                if ( ( (CheckInFamily)e.Item.DataItem ).Selected )
                {
                    ( (LinkButton)e.Item.FindControl( "lbSelectFamily" ) ).AddCssClass( "active" );
                }
            }
        }

        /// <summary>
        /// Handles the ItemDataBound event of the lvPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void lvPerson_ItemDataBound( object sender, ListViewItemEventArgs e )
        {
            if ( e.Item.ItemType == ListViewItemType.DataItem )
            {
                var person = (CheckInPerson)e.Item.DataItem;
                if ( person.Selected )
                {
                    var lbSelectPerson = (LinkButton)e.Item.FindControl( "lbSelectPerson" );
                    lbSelectPerson.AddCssClass( "active" );
                    lbSelectPerson.CommandArgument = person.Person.Id.ToString();
                }
            }
        }

        /// <summary>
        /// Handles the ItemDataBound event of the lvVisitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void lvVisitor_ItemDataBound( object sender, ListViewItemEventArgs e )
        {
            if ( e.Item.ItemType == ListViewItemType.DataItem )
            {
                var person = (CheckInPerson)e.Item.DataItem;
                if ( person.Selected )
                {
                    var lbSelectVisitor = (LinkButton)e.Item.FindControl( "lbSelectVisitor" );
                    lbSelectVisitor.AddCssClass( "active" );
                    lbSelectVisitor.CommandArgument = person.Person.Id.ToString();
                }
            }
        }

        /// <summary>
        /// Handles the ItemDataBound event of the lvAddFamily control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ListViewItemEventArgs"/> instance containing the event data.</param>
        protected void lvAddFamily_ItemDataBound( object sender, ListViewItemEventArgs e )
        {
            var tbFirstName = (RockTextBox)e.Item.FindControl( "tbFirstName" );
            var tbLastName = (RockTextBox)e.Item.FindControl( "tbLastName" );
            var dpBirthDate = (DatePicker)e.Item.FindControl( "dpBirthDate" );            
            var ddlGender = (RockDropDownList)e.Item.FindControl( "ddlGender" );
            ddlGender.BindToEnum( typeof( Gender ) );
            BindAbilityGrade( (RockDropDownList)e.Item.FindControl( "ddlAbilityGrade" ) );            
        }

        #endregion

        #region Select People Events

        /// <summary>
        /// Handles the ItemCommand event of the lvFamily control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ListViewCommandEventArgs"/> instance containing the event data.</param>
        protected void lvFamily_ItemCommand( object sender, ListViewCommandEventArgs e )
        {
            int id = int.Parse( e.CommandArgument.ToString() );
            var family = CurrentCheckInState.CheckIn.Families.Where( f => f.Group.Id == id ).FirstOrDefault();
            
            foreach ( ListViewDataItem li in lvFamily.Items )
            {
                ( (LinkButton)li.FindControl( "lbSelectFamily" ) ).RemoveCssClass( "active" );
            }

            if ( !family.Selected )
            {
                CurrentCheckInState.CheckIn.Families.ForEach( f => f.Selected = false );
                ( (LinkButton)e.Item.FindControl( "lbSelectFamily" ) ).AddCssClass( "active" );
                family.Selected = true;
                ProcessFamily();
            }
            else
            {
                family.Selected = false;               
                lvPerson.DataSource = null;
                lvPerson.DataBind();
                lvVisitor.DataSource = null;
                lvVisitor.DataBind();  
            }

            dpPersonPager.Visible = true;
            dpPersonPager.SetPageProperties( 0, dpPersonPager.MaximumRows, false );
            if ( lvPerson.DataSource == null )
            {
                dpPersonPager.Visible = false;
            }
            
            dpVisitorPager.Visible = true;
            dpVisitorPager.SetPageProperties( 0, dpVisitorPager.MaximumRows, false );
            if ( lvVisitor.DataSource == null )
            {
                dpVisitorPager.Visible = false;
            }
            
        }

        /// <summary>
        /// Handles the PagePropertiesChanging event of the lvFamily control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
        protected void lvFamily_PagePropertiesChanging( object sender, PagePropertiesChangingEventArgs e )
        {
            dpFamilyPager.SetPageProperties( e.StartRowIndex, e.MaximumRows, false );

            // rebind List View
            lvFamily.DataSource = CurrentCheckInState.CheckIn.Families.OrderBy( f => f.Caption ).ToList();
            lvFamily.DataBind();
            pnlFamily.Update();
        }

        /// <summary>
        /// Handles the PagePropertiesChanging event of the dpPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
        protected void lvPerson_PagePropertiesChanging( object sender, PagePropertiesChangingEventArgs e )
        {
            dpPersonPager.SetPageProperties( e.StartRowIndex, e.MaximumRows, false );

            // rebind List View
            lvPerson.DataSource = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault()
                .People.Where( f => f.FamilyMember ).OrderBy( p => p.Person.FullNameReversed ).ToList();
            lvPerson.DataBind();
            pnlPerson.Update();
        }

        /// <summary>
        /// Handles the PagePropertiesChanging event of the dpVisitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
        protected void lvVisitor_PagePropertiesChanging( object sender, PagePropertiesChangingEventArgs e )
        {
            dpVisitorPager.SetPageProperties( e.StartRowIndex, e.MaximumRows, false );

            // rebind List View
            lvVisitor.DataSource = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault()
                .People.Where( f => !f.FamilyMember ).OrderBy( p => p.Person.FullNameReversed ).ToList();
            lvVisitor.DataBind();
            pnlVisitor.Update();
        }

        #endregion

        #region Add People Events

        /// <summary>
        /// Handles the Click event of the lbAddPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbAddPerson_Click( object sender, EventArgs e )
        {

            lblAddPersonHeader.Text = "Add Person";
            personVisitorType.Value = "Person";
            SetAddPersonFields();
        }

        /// <summary>
        /// Handles the Click event of the lbAddVisitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbAddVisitor_Click( object sender, EventArgs e )
        {
            lblAddPersonHeader.Text = "Add Visitor";
            personVisitorType.Value = "Visitor";
            SetAddPersonFields();
        }

        /// <summary>
        /// Handles the Click event of the lbAddSearchedForPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbSavePerson_Click( object sender, EventArgs e )
        {
            if ( string.IsNullOrEmpty( tbFirstNameSearch.Text ) || string.IsNullOrEmpty( tbLastNameSearch.Text ) || string.IsNullOrEmpty( dpDOBSearch.Text ) || ddlGenderSearch.SelectedValueAsInt() == 0 )
            {   // modal takes care of the validation
                Page.Validate( "Person" );
                mpeAddPerson.Show();
            }
            else
            {
                var checkInFamily = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault();
                if ( checkInFamily == null )
                {
                    checkInFamily = new CheckInFamily();
                    var familyGroup = CreateFamily( tbLastNameSearch.Text );

                    checkInFamily.Group = familyGroup;
                    checkInFamily.Caption = familyGroup.Name;
                }

                var checkInPerson = new CheckInPerson();
                checkInPerson.Person = CreatePerson( tbFirstNameSearch.Text, tbLastNameSearch.Text, dpDOBSearch.SelectedDate, (int)ddlGenderSearch.SelectedValueAsEnum<Gender>(),
                    ddlAbilitySearch.SelectedValue, ddlAbilitySearch.SelectedItem.Attributes["optiongroup"] );

                if ( personVisitorType.Value != "Visitor" )
                {   // Family Member
                    var groupMember = AddGroupMember( checkInFamily.Group.Id, checkInPerson.Person );
                    checkInPerson.FamilyMember = true;
                    hfSelectedPerson.Value += checkInPerson.Person.Id + ",";
                }
                else
                {   // Visitor
                    AddVisitorGroupMemberRoles( checkInFamily, checkInPerson.Person.Id );
                    checkInPerson.FamilyMember = false;
                    hfSelectedVisitor.Value += checkInPerson.Person.Id + ",";
                }

                checkInPerson.Selected = true;
                checkInFamily.People.Add( checkInPerson );
                checkInFamily.SubCaption = string.Join( ",", checkInFamily.People.Select( p => p.Person.FirstName ) );
                checkInFamily.Selected = true;
                CurrentCheckInState.CheckIn.Families.Add( checkInFamily );

                tbFirstNameSearch.Required = false;
                tbLastNameSearch.Required = false;
                ddlGenderSearch.Required = false;
                dpDOBSearch.Required = false;
                
                ProcessFamily();
            }
        }

        /// <summary>
        /// Handles the PagePropertiesChanging event of the lvAddFamily control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
        protected void lvAddFamily_PagePropertiesChanging( object sender, PagePropertiesChangingEventArgs e )
        {
            var newFamilyList = new List<NewPerson>();
            if ( ViewState["newFamily"] != null )
            {
                newFamilyList = (List<NewPerson>)ViewState["newFamily"];
                int personOffset = 0;
                foreach ( ListViewItem item in lvAddFamily.Items )
                {
                    var rowPerson = new NewPerson();
                    rowPerson.FirstName = ( (TextBox)item.FindControl( "tbFirstName" ) ).Text;
                    rowPerson.LastName = ( (TextBox)item.FindControl( "tbLastName" ) ).Text;
                    rowPerson.BirthDate = ( (DatePicker)item.FindControl( "dpBirthDate" ) ).SelectedDate;
                    rowPerson.Gender = ( (RockDropDownList)item.FindControl( "ddlGender" ) ).SelectedValueAsEnum<Gender>();
                    rowPerson.Ability = ( (RockDropDownList)item.FindControl( "ddlAbilityGrade" ) ).SelectedValue;
                    rowPerson.AbilityGroup = ( (RockDropDownList)item.FindControl( "ddlAbilityGrade" ) ).SelectedItem.Attributes["optiongroup"];
                    newFamilyList[System.Math.Abs( e.StartRowIndex - e.MaximumRows ) + personOffset] = rowPerson;
                    personOffset++;

                    // check if the list should be expanded
                    if ( e.MaximumRows + e.StartRowIndex + personOffset >= newFamilyList.Count )
                    {
                        newFamilyList.AddRange( Enumerable.Repeat( new NewPerson(), e.MaximumRows ) );
                    }
                }
            }

            ViewState["newFamily"] = newFamilyList;
            dpAddFamily.SetPageProperties( e.StartRowIndex, e.MaximumRows, false );
            lvAddFamily.DataSource = newFamilyList;
            lvAddFamily.DataBind();
            mpeAddFamily.Show();
        }

        /// <summary>
        /// Handles the Click event of the lbAddPersonSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbPersonSearch_Click( object sender, EventArgs e )
        {
            lbSavePerson.Visible = true;            
            rGridPersonResults.PageIndex = 0;
            rGridPersonResults.Visible = true;
            rGridPersonResults.PageSize = 4;
            BindPersonGrid();            
            mpeAddPerson.Show();
        }

        /// <summary>
        /// Handles the Click event of the lbAddFamily control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbAddFamily_Click( object sender, EventArgs e )
        {
            var newFamilyList = new List<NewPerson>();
            newFamilyList.AddRange( Enumerable.Repeat( new NewPerson(), 10 ) );
            ViewState["newFamily"] = newFamilyList;
            lvAddFamily.DataSource = newFamilyList;
            lvAddFamily.DataBind();
            
            mpeAddFamily.Show();
        }

        /// <summary>
        /// Handles the Click event of the lbAddFamilySave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbSaveFamily_Click( object sender, EventArgs e )
        {
            var newFamilyList = (List<NewPerson>)ViewState["newFamily"];
            var checkInFamily = new CheckInFamily();
            CheckInPerson checkInPerson;
            NewPerson newPerson;

            // add the new people
            foreach ( ListViewItem item in lvAddFamily.Items )
            {
                newPerson = new NewPerson();
                newPerson.FirstName = ( (TextBox)item.FindControl( "tbFirstName" ) ).Text;
                newPerson.LastName = ( (TextBox)item.FindControl( "tbLastName" ) ).Text;
                newPerson.BirthDate = ( (DatePicker)item.FindControl( "dpBirthDate" ) ).SelectedDate;
                newPerson.Gender = ( (RockDropDownList)item.FindControl( "ddlGender" ) ).SelectedValueAsEnum<Gender>();
                newPerson.Ability = ( (RockDropDownList)item.FindControl( "ddlAbilityGrade" ) ).SelectedValue;
                newPerson.AbilityGroup = ( (RockDropDownList)item.FindControl( "ddlAbilityGrade" ) ).SelectedItem.Attributes["optiongroup"];
                newFamilyList.Add( newPerson );
            }

            var lastName = newFamilyList.Where( p => p.BirthDate.HasValue ).OrderBy( p => p.BirthDate ).Select( p => p.LastName ).FirstOrDefault();
            var familyGroup = CreateFamily( lastName );

            // create people and add to checkin
            foreach ( NewPerson np in newFamilyList.Where( np => np.IsValid() ) )
            {
                var person = CreatePerson( np.FirstName, np.LastName, np.BirthDate, (int)np.Gender, np.Ability, np.AbilityGroup );
                var groupMember = AddGroupMember( familyGroup.Id, person );
                familyGroup.Members.Add( groupMember );
                checkInPerson = new CheckInPerson();
                checkInPerson.Person = person;
                checkInPerson.Selected = true;
                checkInPerson.FamilyMember = true;
                checkInFamily.People.Add( checkInPerson );
            }

            checkInFamily.Group = familyGroup;
            checkInFamily.Caption = familyGroup.Name;
            checkInFamily.SubCaption = string.Join( ",", checkInFamily.People.Select( p => p.Person.FirstName ) );
            checkInFamily.Selected = true;

            CurrentCheckInState.CheckIn.Families.Clear();
            CurrentCheckInState.CheckIn.Families.Add( checkInFamily );

            ProcessFamily();
            RefreshFamily();
        }

        /// <summary>
        /// Handles the RowCommand event of the grdPersonSearchResults control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridViewCommandEventArgs"/> instance containing the event data.</param>
        protected void rGridPersonResults_AddExistingPerson( object sender, GridViewCommandEventArgs e )
        {
            if ( e.CommandName == "Add" )
            {
                var rockContext = new RockContext();
                GroupMemberService groupMemberService = new GroupMemberService( rockContext );
                int index = int.Parse( e.CommandArgument.ToString() );
                int personId = int.Parse( rGridPersonResults.DataKeys[index].Value.ToString() );

                var family = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault();
                if ( family != null )
                {
                    var checkInPerson = new CheckInPerson();
                    checkInPerson.Person = new PersonService( rockContext ).Get( personId ).Clone( false );
                    var isPersonInFamily = family.People.Any( p => p.Person.Id == checkInPerson.Person.Id );
                    if ( !isPersonInFamily )
                    {
                        if ( personVisitorType.Value != "Visitor" )
                        {
                            // TODO: DT: Is this right?  Not sure this is the best way to set family id
                            var groupMember = groupMemberService.GetByPersonId( personId ).FirstOrDefault();
                            groupMember.GroupId = family.Group.Id;

                            rockContext.SaveChanges();

                            checkInPerson.FamilyMember = true;
                            hfSelectedPerson.Value += personId + ",";
                        }
                        else
                        {
                            AddVisitorGroupMemberRoles( family, personId );
                            checkInPerson.FamilyMember = false;
                            hfSelectedVisitor.Value += personId + ",";
                        }
                        checkInPerson.Selected = true;
                        family.People.Add( checkInPerson );
                        ProcessFamily();
                    }
                    
                    mpeAddPerson.Hide();
                }
                else
                {
                    string errorMsg = "<ul><li>You have to pick a family to add this person to.</li></ul>";
                    maWarning.Show( errorMsg, Rock.Web.UI.Controls.ModalAlertType.Warning );
                }
            }
            else
            {
                mpeAddPerson.Show();
                BindPersonGrid();
            }
        }

        /// <summary>
        /// Handles the GridRebind event of the rGridPersonResults control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void rGridPersonResults_GridRebind( object sender, EventArgs e )
        {
            BindPersonGrid();
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Binds the person search results grid.
        /// </summary>
        private void BindPersonGrid()
        {
            var personService = new PersonService( new RockContext() );
            var people = personService.Queryable();

            if ( !string.IsNullOrEmpty( tbFirstNameSearch.Text ) && !string.IsNullOrEmpty( tbLastNameSearch.Text ) )
            {
                people = personService.GetByFullName( tbFirstNameSearch.Text + " " + tbLastNameSearch.Text, false );
            }
            else if ( !string.IsNullOrEmpty( tbLastNameSearch.Text ) )
            {
                people = people.Where( p => p.LastName.ToLower().StartsWith( tbLastNameSearch.Text ) );
            }
            else if ( !string.IsNullOrEmpty( tbFirstNameSearch.Text ) )
            {
                people = people.Where( p => p.FirstName.ToLower().StartsWith( tbFirstNameSearch.Text ) );
            }

            if ( !string.IsNullOrEmpty( dpDOBSearch.Text ) )
            {
                DateTime searchDate;
                if ( DateTime.TryParse( dpDOBSearch.Text, out searchDate ) )
                {
                    people = people.Where( p => p.BirthYear == searchDate.Year
                        && p.BirthMonth == searchDate.Month && p.BirthDay == searchDate.Day );
                }
            }

            if ( ddlGenderSearch.SelectedValueAsEnum<Gender>() != 0 )
            {
                var gender = ddlGenderSearch.SelectedValueAsEnum<Gender>();
                people = people.Where( p => p.Gender == gender );
            }

            // get the list of people so we can filter by grade and ability level
            var peopleList = people.OrderBy( p => p.LastName ).ThenBy( p => p.FirstName ).ToList();
            var abilityLevelValues = DefinedTypeCache.Read( new Guid( Rock.SystemGuid.DefinedType.PERSON_ABILITY_LEVEL_TYPE ) ).DefinedValues;
            peopleList.ForEach( p => p.LoadAttributes() );
            if ( ddlAbilitySearch.SelectedIndex != 0 )
            {
                var optionGroup = ddlAbilitySearch.SelectedItem.Attributes["optiongroup"];
                if ( optionGroup.Equals( "Grade" ) )
                {
                    var grade = ddlAbilitySearch.SelectedValueAsEnum<GradeLevel>();
                    if ( (int)grade <= 12 )
                    {
                        peopleList = peopleList.Where( p => p.Grade == (int)grade ).ToList();
                    }
                }
                else if ( optionGroup.Equals( "Ability" ) )
                {
                    var abilityLevelGuid = ddlAbilitySearch.SelectedValue;                    
                    peopleList = peopleList.Where( p => p.Attributes.ContainsKey( "AbilityLevel" ) 
                        && p.GetAttributeValue( "AbilityLevel" ) == abilityLevelGuid ).ToList();
                }
            }
            
            var matches = peopleList.Select( p => new 
            {
                p.Id, p.FirstName, p.LastName, p.BirthDate, p.Age, p.Gender,
                Attribute = p.Grade.HasValue 
                    ? ( (GradeLevel) p.Grade ).GetDescription() 
                    : abilityLevelValues.Where( dv => dv.Guid.ToString()
                        .Equals( p.GetAttributeValue( "AbilityLevel" ), StringComparison.OrdinalIgnoreCase ) )
                        .Select( dv => dv.Name ).FirstOrDefault()
            } ).OrderByDescending( p => p.BirthDate ).ToList();

            rGridPersonResults.DataSource = matches;
            rGridPersonResults.DataBind();
        }

        /// <summary>
        /// Processes the family.
        /// </summary>
        private void ProcessFamily()
        {
            var errors = new List<string>();
            if ( ProcessActivity( "Person Search", out errors ) )
            {
                var family = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault();
                if ( family != null )
                {
                    IEnumerable<CheckInPerson> memberDataSource = null;
                    IEnumerable<CheckInPerson> visitorDataSource = null;
                    if ( family.People.Any() )
                    {
                        if ( family.People.Where( f => f.FamilyMember ).Any() )
                        {
                            var familyMembers = family.People.Where( f => f.FamilyMember ).ToList();
                            familyMembers.ForEach( p => p.Selected = true );
                            hfSelectedPerson.Value = string.Join( ",", familyMembers.Select( f => f.Person.Id ) ) + ",";
                            memberDataSource = familyMembers.OrderBy( p => p.Person.FullNameReversed ).ToList();
                        }

                        if ( family.People.Where( f => !f.FamilyMember ).Any() )
                        {
                            var familyVisitors = family.People.Where( f => !f.FamilyMember ).ToList();
                            hfSelectedVisitor.Value = string.Join( ",", familyVisitors.Select( f => f.Person.Id ) ) + ",";
                            visitorDataSource = familyVisitors.OrderBy( p => p.Person.FullNameReversed ).ToList();
                        }
                    }

                    lvPerson.DataSource = memberDataSource;
                    lvPerson.DataBind();
                    pnlPerson.Update();
                    lvVisitor.DataSource = visitorDataSource;
                    lvVisitor.DataBind();
                    pnlVisitor.Update();
                }
            }
            else
            {
                string errorMsg = "<ul><li>" + errors.AsDelimited( "</li><li>" ) + "</li></ul>";
                maWarning.Show( errorMsg, Rock.Web.UI.Controls.ModalAlertType.Warning );
            }
        }        

        /// <summary>
        /// Refreshes the family.
        /// </summary>
        protected void RefreshFamily()
        {
            lvFamily.DataSource = CurrentCheckInState.CheckIn.Families.OrderBy( f => f.Caption ).ToList();
            lvFamily.DataBind();
            pnlFamily.Update();

            if ( divNothingFound.Visible )
            {
                lblFamilyTitle.InnerText = "Search Results";
                lbNext.Enabled = true;
                lbNext.Visible = true;
                pnlFamily.Visible = true;
                pnlPerson.Visible = true;
                pnlVisitor.Visible = true;
                actions.Visible = true;
                divNothingFound.Visible = false;
            }
        }

        /// <summary>
        /// Sets the add person fields.
        /// </summary>
        protected void SetAddPersonFields()
        {
            ddlGenderSearch.BindToEnum( typeof( Gender ) );
            ddlGenderSearch.SelectedIndex = 0;
            BindAbilityGrade( ddlAbilitySearch );
            ddlAbilitySearch.SelectedIndex = 0;
            rGridPersonResults.Visible = false;
            lbSavePerson.Visible = false;

            tbFirstNameSearch.Required = true;
            tbLastNameSearch.Required = true;
            ddlGenderSearch.Required = true;
            dpDOBSearch.Required = true;

            mpeAddPerson.Show();
        }

        /// <summary>
        /// Binds the dropdown to a list of ability levels and grades.
        /// </summary>
        /// <returns>List Items</returns>
        protected void BindAbilityGrade( DropDownList thisDDL )
        {
            thisDDL.Items.Clear();
            thisDDL.DataTextField = "Text";
            thisDDL.DataValueField = "Value";
            thisDDL.Items.Add( new ListItem( Rock.Constants.None.Text, Rock.Constants.None.Id.ToString() ) );

            var dtAbility = DefinedTypeCache.Read( new Guid( Rock.SystemGuid.DefinedType.PERSON_ABILITY_LEVEL_TYPE ) );
            if ( dtAbility != null && dtAbility.DefinedValues.Count > 0 )
            {
                foreach ( var ability in dtAbility.DefinedValues.Select( dv => new ListItem( dv.Name, dv.Guid.ToString() ) ).ToList() )
                {
                    ability.Attributes.Add( "optiongroup", "Ability" );
                    thisDDL.Items.Add( ability );
                }
            }

            var gradeList = Enum.GetValues( typeof( GradeLevel ) ).Cast<GradeLevel>().OrderBy( gl => (int)gl )
                .Select( g => new ListItem( g.GetDescription(), g.ConvertToString() ) ).ToList();
            foreach ( var grade in gradeList )
            {
                grade.Attributes.Add( "optiongroup", "Grade" );
                thisDDL.Items.Add( grade );
            }
        }
        
        /// <summary>
        /// Adds a new person.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="DOB">The DOB.</param>
        /// <param name="gender">The gender</param>
        /// <param name="attribute">The attribute.</param>
        protected Person CreatePerson( string firstName, string lastName, DateTime? dob, int? gender, string ability, string abilityGroup )
        {
            Person person = new Person();
            person.FirstName = firstName;
            person.LastName = lastName;
            person.BirthDate = dob;

            if ( gender != null )
            {
                person.Gender = (Gender)gender;
            }

            if ( !string.IsNullOrWhiteSpace( ability ) && abilityGroup == "Grade" )
            {
                person.Grade = (int)ability.ConvertToEnum<GradeLevel>();
            }

            var rockContext = new RockContext();
            PersonService ps = new PersonService( rockContext );
            ps.Add( person );
            rockContext.SaveChanges();

            if ( !string.IsNullOrWhiteSpace( ability ) &&  abilityGroup == "Ability" )
            {
                rockContext = new RockContext();
                Person p = new PersonService( rockContext ).Get( person.Id );
                if ( p != null )
                {
                    p.LoadAttributes( rockContext );
                    p.SetAttributeValue( "AbilityLevel", ability );
                    p.SaveAttributeValues(  );
                }
            }

            return person;
        }

        /// <summary>
        /// Creates the family.
        /// </summary>
        /// <param name="FamilyName">Name of the family.</param>
        /// <returns></returns>
        protected Group CreateFamily( string FamilyName )
        {
            var familyGroup = new Group();
            familyGroup.Name = FamilyName + " Family";
            familyGroup.GroupTypeId = GroupTypeCache.GetFamilyGroupType().Id;
            familyGroup.IsSecurityRole = false;
            familyGroup.IsSystem = false;
            familyGroup.IsActive = true;

            var rockContext = new RockContext();
            var gs = new GroupService(rockContext);
            gs.Add( familyGroup );
            rockContext.SaveChanges();

            return familyGroup;
        }

        /// <summary>
        /// Adds the group member.
        /// </summary>
        /// <param name="familyGroup">The family group.</param>
        /// <param name="person">The person.</param>
        /// <returns></returns>
        protected GroupMember AddGroupMember( int familyGroupId, Person person )
        {
            var rockContext = new RockContext();

            GroupMember groupMember = new GroupMember();
            groupMember.IsSystem = false;
            groupMember.GroupId = familyGroupId;
            groupMember.PersonId = person.Id;
            if ( person.Age >= 18 )
            {
                groupMember.GroupRoleId = new GroupTypeRoleService( rockContext ).Get( new Guid( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT ) ).Id;
            }
            else
            {
                groupMember.GroupRoleId = new GroupTypeRoleService( rockContext ).Get( new Guid( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD ) ).Id;
            }

            GroupMemberService groupMemberService = new GroupMemberService( rockContext );
            groupMemberService.Add( groupMember );
            rockContext.SaveChanges();

            return groupMember;
        }

        /// <summary>
        /// Adds the visitor group member roles.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <param name="personId">The person id.</param>
        protected void AddVisitorGroupMemberRoles( CheckInFamily family, int personId )
        {
            var rockContext = new RockContext();
            var groupService = new GroupService( rockContext );
            var groupMemberService = new GroupMemberService( rockContext );
            var groupRoleService = new GroupTypeRoleService( rockContext );
            
            int ownerRoleId = groupRoleService.Get( new Guid( Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_OWNER ) ).Id;
            int canCheckInId = groupRoleService.Get( new Guid( Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_CAN_CHECK_IN ) ).Id;
            
            foreach ( var familyMember in family.People )
            {
                var group = groupMemberService.Queryable()
                .Where( m =>
                    m.PersonId == familyMember.Person.Id &&
                    m.GroupRoleId == ownerRoleId )
                .Select( m => m.Group )
                .FirstOrDefault();

                if ( group == null )
                {
                    var role = new GroupTypeRoleService( rockContext ).Get( ownerRoleId );
                    if ( role != null && role.GroupTypeId.HasValue )
                    {
                        var groupMember = new GroupMember();
                        groupMember.PersonId = familyMember.Person.Id;
                        groupMember.GroupRoleId = role.Id;

                        group = new Group();
                        group.Name = role.GroupType.Name;
                        group.GroupTypeId = role.GroupTypeId.Value;
                        group.Members.Add( groupMember );

                        groupService.Add( group );
                    }
                }

                // add the visitor to this group with CanCheckIn
                Person.CreateCheckinRelationship( familyMember.Person.Id, personId, CurrentPersonAlias );
            }

            rockContext.SaveChanges();
        }
               
        #endregion

        #region NewPerson Class
        /// <summary>
        /// Lightweight Person model to quickly add people during Check-in
        /// </summary>
        [Serializable()]
        protected class NewPerson
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime? BirthDate { get; set; }
            public Gender Gender { get; set; }
            public string Ability { get; set; }
            public string AbilityGroup { get; set; }

            public bool IsValid()
            {
                return !( string.IsNullOrWhiteSpace( FirstName ) || string.IsNullOrWhiteSpace( LastName )
                    || !BirthDate.HasValue || Gender == Gender.Unknown );
            }

            public NewPerson()
            {
                FirstName = string.Empty;
                LastName = string.Empty;
                BirthDate = new DateTime?();
                Gender = new Gender();
                Ability = string.Empty;
                AbilityGroup = string.Empty;
            }
        }

        #endregion

        
}
}