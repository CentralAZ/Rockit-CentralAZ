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
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.CheckIn.Attended
{
    /// <summary>
    /// Confirmation block for Attended Check-in
    /// </summary>
    [DisplayName("Confirmation Block")]
    [Category("Check-in > Attended")]
    [Description( "Attended Check-In Confirmation Block" )]
    [LinkedPage("Activity Select Page")]
    public partial class Confirm : CheckInBlock
    {
        /// <summary>
        /// Check-In information class used to bind the selected grid.
        /// </summary>
        protected class CheckIn
        {
            public int PersonId { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
            public int LocationId { get; set; }
            public string Schedule { get; set; }
            public int ScheduleId { get; set; }

            public CheckIn()
            {
                PersonId = 0;
                Name = string.Empty;
                Location = string.Empty;
                LocationId = 0;
                Schedule = string.Empty;
                ScheduleId = 0;
            }
        }

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            RockPage.AddScriptLink( this.Page, "http://www.sparkdevnetwork.org/public/js/cordova-2.4.0.js", false );
            RockPage.AddScriptLink( this.Page, "http://www.sparkdevnetwork.org/public/js/ZebraPrint.js", false );
        }

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
            else
            {
                if ( !Page.IsPostBack )
                {
                    BindGrid();
                }
            }
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        protected void BindGrid()
        {
            var selectedPeopleList = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault()
                .People.Where( p => p.Selected ).OrderBy( p => p.Person.FullNameReversed ).ToList();

            var checkInList = new List<CheckIn>();
            foreach ( var person in selectedPeopleList )
            {
                var locations = person.GroupTypes.Where( gt => gt.Selected )
                    .SelectMany( gt => gt.Groups ).Where( g => g.Selected )
                    .SelectMany( g => g.Locations ).Where( l => l.Selected ).ToList();

                if ( locations.Any() )
                {
                    foreach ( var location in locations )
                    {
                        foreach ( var schedule in location.Schedules.Where( s => s.Selected ) )
                        {
                            var checkIn = new CheckIn();
                            checkIn.PersonId = person.Person.Id;
                            checkIn.Name = person.Person.FullName;
                            checkIn.Location = location.Location.Name;
                            checkIn.LocationId = location.Location.Id;
                            checkIn.Schedule = schedule.Schedule.Name;
                            checkIn.ScheduleId = schedule.Schedule.Id;
                            checkInList.Add( checkIn );
                        }                    
                    }                    
                }
                else
                {   // auto assignment didn't select anything
                    checkInList.Add( new CheckIn { PersonId = person.Person.Id, Name = person.Person.FullName } );
                }
            }

            gPersonList.DataSource = checkInList.OrderBy( c => c.Schedule ).ToList();
            gPersonList.DataBind();
        }
        
        #endregion

        #region Edit Events

        /// <summary>
        /// Handles the Click event of the lbBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbBack_Click( object sender, EventArgs e )
        {
            GoBack();
        }

        /// <summary>
        /// Handles the Click event of the lbDone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbDone_Click( object sender, EventArgs e )
        {
            CurrentCheckInState.CheckIn.SearchType = null;
            CurrentCheckInState.CheckIn.SearchValue = string.Empty;
            SaveState();
            NavigateToNextPage();
        }
                
        /// <summary>
        /// Handles the Click event of the lbPrintAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbPrintAll_Click( object sender, EventArgs e )
        {
            SaveAttendance();
            foreach ( DataKey dataKey in gPersonList.DataKeys )
            {               
                var personId = Convert.ToInt32( dataKey["PersonId"] );
                var locationId = Convert.ToInt32( dataKey["LocationId"] );
                var scheduleId = Convert.ToInt32( dataKey["ScheduleId"] );
                PrintLabel( personId, locationId, scheduleId );               
            }
        }

        /// <summary>
        /// Handles the Edit event of the gPersonList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gPersonList_Edit( object sender, RowEventArgs e )
        {
            var dataKeyValues = gPersonList.DataKeys[e.RowIndex].Values;
            var queryParams = new Dictionary<string, string>();
            queryParams.Add( "personId", dataKeyValues["PersonId"].ToString() );
            queryParams.Add( "locationId", dataKeyValues["LocationId"].ToString() );
            queryParams.Add( "scheduleId", dataKeyValues["ScheduleId"].ToString() );
            NavigateToLinkedPage( "ActivitySelectPage", queryParams);
        }

        /// <summary>
        /// Handles the Delete event of the gPersonList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gPersonList_Delete( object sender, RowEventArgs e )
        {
            var dataKeyValues = gPersonList.DataKeys[e.RowIndex].Values;
            var personId = Convert.ToInt32( dataKeyValues["PersonId"] );
            var locationId = Convert.ToInt32( dataKeyValues["LocationId"] );
            var scheduleId = Convert.ToInt32( dataKeyValues["ScheduleId"] );
            
            var selectedPerson = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault()
                .People.Where( p => p.Person.Id == personId ).FirstOrDefault();
            var selectedGroups = selectedPerson.GroupTypes.Where( gt => gt.Selected )
                .SelectMany( gt => gt.Groups.Where( g => g.Selected ) );
            CheckInGroup selectedGroup = selectedGroups.Where( g => g.Selected
                && g.Locations.Any( l => l.Location.Id == locationId
                    && l.Schedules.Any( s => s.Schedule.Id == scheduleId ) ) ).FirstOrDefault();
            CheckInLocation selectedLocation = selectedGroup.Locations.Where( l => l.Selected 
                && l.Location.Id == locationId 
                    && l.Schedules.Any( s => s.Schedule.Id == scheduleId ) ).FirstOrDefault();
            CheckInSchedule selectedSchedule = selectedLocation.Schedules.Where( s => s.Selected 
                && s.Schedule.Id == scheduleId ).FirstOrDefault();
            
            selectedSchedule.Selected = false;
            selectedSchedule.PreSelected = false;

            // clear checkin rows without anything selected
            if ( !selectedLocation.Schedules.Any( s => s.Selected ) )
            {
                selectedLocation.Selected = false;
                selectedLocation.PreSelected = false;                
            }
            
            if ( !selectedGroup.Locations.Any( l => l.Selected ) )
            {
                selectedGroup.Selected = false;
                selectedGroup.PreSelected = false;
            }       
     
            if ( !selectedGroups.Any() )
            {
                selectedPerson.Selected = false;
                selectedPerson.PreSelected = false;
            }
            
            BindGrid();
        }

        /// <summary>
        /// Handles the RowCommand event of the gPersonList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridViewCommandEventArgs"/> instance containing the event data.</param>
        protected void gPersonList_Print( object sender, GridViewCommandEventArgs e )
        {
            if ( e.CommandName == "Print" )
            {
                SaveAttendance();                
                int index = Convert.ToInt32( e.CommandArgument );                
                var dataKeyValues = gPersonList.DataKeys[index].Values;
                var personId = Convert.ToInt32( dataKeyValues["PersonId"] );
                var locationId = Convert.ToInt32( dataKeyValues["LocationId"] );
                var scheduleId = Convert.ToInt32( dataKeyValues["ScheduleId"] );
                PrintLabel( personId, locationId, scheduleId );
            }
        }

        /// <summary>
        /// Handles the GridRebind event of the gPersonList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gPersonList_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }
        #endregion

        #region Internal Methods

        /// <summary>
        /// Saves the attendance and loads the labels.
        /// </summary>
        private void SaveAttendance()
        {
            var errors = new List<string>();
            if ( ProcessActivity( "Save Attendance", out errors ) )
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
        
        /// <summary>
        /// Prints the label.
        /// </summary>
        /// <param name="person">The person.</param>
        private void PrintLabel( int personId, int locationId, int scheduleId )
        {
            CheckInPerson selectedPerson = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault()
                    .People.Where( p => p.Person.Id == personId ).FirstOrDefault();
            List<CheckInGroupType> selectedGroupTypes = selectedPerson.GroupTypes.Where( gt => gt.Selected 
                && gt.Groups.Any( g => g.Selected && g.Locations.Any( l => l.Location.Id == locationId 
                    && l.Schedules.Any( s => s.Schedule.Id == scheduleId ) ) ) ).ToList();
            
            foreach ( var groupType in selectedGroupTypes )
            {
                var printFromClient = groupType.Labels.Where( l => l.PrintFrom == Rock.Model.PrintFrom.Client);
                if ( printFromClient.Any() )
                {
                    AddLabelScript( printFromClient.ToJson() );
                }

                var printFromServer = groupType.Labels.Where( l => l.PrintFrom == Rock.Model.PrintFrom.Server );
                if ( printFromServer.Any() )
                {
                    Socket socket = null;
                    string currentIp = string.Empty;

                    foreach ( var label in printFromServer )
                    {
                        var labelCache = KioskLabel.Read( label.FileId );
                        if ( labelCache != null )
                        {
                            if ( label.PrinterAddress != currentIp )
                            {
                                if ( socket != null && socket.Connected )
                                {
                                    socket.Shutdown( SocketShutdown.Both );
                                    socket.Close();
                                }

                                currentIp = label.PrinterAddress;
                                var printerIp = new IPEndPoint( IPAddress.Parse( currentIp ), 9100 );

                                socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
                                IAsyncResult result = socket.BeginConnect( printerIp, null, null );
                                bool success = result.AsyncWaitHandle.WaitOne( 5000, true );
                            }

                            string printContent = labelCache.FileContent;
                            foreach ( var mergeField in label.MergeFields )
                            {
                                var rgx = new Regex( string.Format( @"(?<=\^FD){0}(?=\^FS)", mergeField.Key ) );
                                printContent = rgx.Replace( printContent, mergeField.Value );
                            }

                            if ( socket.Connected )
                            {
                                var ns = new NetworkStream( socket );
                                byte[] toSend = System.Text.Encoding.ASCII.GetBytes( printContent );
                                ns.Write( toSend, 0, toSend.Length );
                            }
                            else
                            {
                                maWarning.Show( "Could not connect to printer.", ModalAlertType.Warning );
                            }
                        }
                    }

                    if ( socket != null && socket.Connected )
                    {
                        socket.Shutdown( SocketShutdown.Both );
                        socket.Close();
                    }
                }
            }            
        }

        /// <summary>
        /// Adds the label script.
        /// </summary>
        /// <param name="jsonObject">The json object.</param>
        private void AddLabelScript( string jsonObject )
        {
            string script = string.Format( @"

            // setup deviceready event to wait for cordova
	        document.addEventListener('deviceready', onDeviceReady, false);

	        // label data
            var labelData = {0};

		    function onDeviceReady() {{
	
			    //navigator.notification.alert('Oh boy! It's going to be a good day!, alertDismissed, 'Success', 'Continue');
			    printLabels();
		    }}
		
		    function alertDismissed() {{
		        // do something
		    }}
		
		    function printLabels() {{
		        ZebraPrintPlugin.printTags(
            	    JSON.stringify(labelData), 
            	    function(result) {{ 
			            console.log('I printed that tag like a champ!!!');
			        }},
			        function(error) {{   
				        // error is an array where:
				        // error[0] is the error message
				        // error[1] determines if a re-print is possible (in the case where the JSON is good, but the printer was not connected)
			            console.log('An error occurred: ' + error[0]);
			        }}
                );
	        }}", jsonObject );
            ScriptManager.RegisterStartupScript( this, this.GetType(), "addLabelScript", script, true );
        }

        #endregion        
}
}