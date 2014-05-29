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
using Rock.CheckIn;
using Rock.Model;

namespace RockWeb.Blocks.CheckIn
{
    [DisplayName("Time Select")]
    [Category("Check-in")]
    [Description("Displays a list of times to checkin for.")]
    public partial class TimeSelect : CheckInBlock
    {
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
                    var family = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault();
                    if ( family != null )
                    {
                        var person = family.People.Where( p => p.Selected ).FirstOrDefault();
                        if ( person != null )
                        {
                            var groupType = person.GroupTypes.Where( g => g.Selected ).FirstOrDefault();
                            if ( groupType != null )
                            {
                                var group = groupType.Groups.Where( g => g.Selected ).FirstOrDefault();
                                if ( group != null )
                                {
                                    var location = group.Locations.Where( l => l.Selected ).FirstOrDefault();
                                    if ( location != null )
                                    {
                                        lTitle.Text = person.ToString();
                                        lSubTitle.Text = group.ToString();

                                        if ( location.Schedules.Count == 1 )
                                        {
                                            foreach ( var schedule in location.Schedules )
                                            {
                                                schedule.Selected = true;
                                            }

                                            ProcessSelection( maWarning );
                                        }
                                        else
                                        {
                                            string script = string.Format(@"
    <script>
        function GetTimeSelection() {{
            var ids = '';
            $('div.checkin-timelist button.active').each( function() {{
                ids += $(this).attr('schedule-id') + ',';
            }});
            if (ids == '') {{
                alert('Please select at least one time');
                return false;
            }}
            else
            {{
                $('#{0}').val(ids);
                return true;
            }}
        }}
    </script>
", hfTimes.ClientID );
                                            Page.ClientScript.RegisterClientScriptBlock( this.GetType(), "SelectTime", script );

                                            rSelection.DataSource = location.Schedules.OrderBy( s => s.StartTime );
                                            rSelection.DataBind();
                                        }
                                    }
                                    else
                                    {
                                        GoBack();
                                    }
                                }
                                else
                                {
                                    GoBack();
                                }
                            }
                            else
                            {
                                GoBack();
                            }
                        }
                        else
                        {
                            GoBack();
                        }
                    }
                    else
                    {
                        GoBack();
                    }
                }
            }
        }

        protected void lbSelect_Click( object sender, EventArgs e )
        {
            if ( KioskCurrentlyActive )
            {
                var family = CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ).FirstOrDefault();
                if ( family != null )
                {
                    var person = family.People.Where( p => p.Selected ).FirstOrDefault();
                    if ( person != null )
                    {
                        var groupType = person.GroupTypes.Where( g => g.Selected ).FirstOrDefault();
                        if ( groupType != null )
                        {
                            var group = groupType.Groups.Where( g => g.Selected ).FirstOrDefault();
                            if ( group != null )
                            {
                                var location = group.Locations.Where( l => l.Selected ).FirstOrDefault();
                                if ( location != null )
                                {
                                    foreach( var scheduleId in hfTimes.Value.SplitDelimitedValues())
                                    {
                                        int id = Int32.Parse( scheduleId );
                                        var schedule = location.Schedules.Where( s => s.Schedule.Id == id).FirstOrDefault();
                                        if (schedule != null)
                                        {
                                            schedule.Selected = true;
                                        }
                                    }

                                    ProcessSelection( maWarning );
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void lbBack_Click( object sender, EventArgs e )
        {
            GoBack();
        }

        protected void lbCancel_Click( object sender, EventArgs e )
        {
            CancelCheckin();
        }
    }
}