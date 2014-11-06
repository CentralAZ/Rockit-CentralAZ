using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using DDay.iCal;

using com.centralaz.Baptism.Model;
using com.centralaz.Baptism.Data;

using Rock;
using Rock.Web;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Constants;
using Rock.Security;
using Rock.Web.UI;
namespace RockWeb.Plugins.com_centralaz.Baptism
{

    [DisplayName( "Baptism Add Baptism Block" )]
    [Category( "com_centralaz > Baptism" )]
    [Description( "Block for adding a baptism" )]
    [BooleanField( "Limit To Valid Service Times" )]
    public partial class BaptismAddBaptism : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables

        #endregion

        #region Properties
        List<DateTime> serviceTimes;
        List<DateTime> specialEvents;
        List<Schedule> blackoutDates;
        Baptizee baptizee = null;

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


                if ( PageParameter( "BaptizeeId" ).AsIntegerOrNull() == null )
                {
                    btnDelete.Visible = false;
                    dtpBaptismDate.SelectedDateTime = PageParameter( "SelectedDate" ).AsDateTime();
                    lPanelTitle.Text = "Add Baptism";
                }
                else
                {
                    BindValues( PageParameter( "BaptizeeId" ).AsInteger() );
                }
            }
        }

        /// <summary>
        /// Returns breadcrumbs specific to the block that should be added to navigation
        /// based on the current page reference.  This function is called during the page's
        /// oninit to load any initial breadcrumbs
        /// </summary>
        /// <param name="pageReference">The page reference.</param>
        /// <returns></returns>
        public override List<BreadCrumb> GetBreadCrumbs( PageReference pageReference )
        {
            var breadCrumbs = new List<BreadCrumb>();

            int? baptizeeId = PageParameter( pageReference, "BaptizeeId" ).AsIntegerOrNull();
            if ( baptizeeId != null )
            {
                Baptizee baptizee = new BaptizeeService( new BaptismContext() ).Get( baptizeeId.Value );
                if ( baptizee != null )
                {
                    breadCrumbs.Add( new BreadCrumb( String.Format( "Edit {0}", baptizee.Person.FullName ), pageReference ) );
                }
                else
                {
                    breadCrumbs.Add( new BreadCrumb( "New Baptism", pageReference ) );
                }
            }
            else
            {
                // don't show a breadcrumb if we don't have a pageparam to work with
            }

            return breadCrumbs;
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
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSave_OnClick( object sender, EventArgs e )
        {

            GetBlackoutDates();
            nbErrorWarning.Visible = false;
            if ( !dtpBaptismDate.SelectedDateTime.HasValue )
            {
                nbErrorWarning.Text = "Please select a date and time";
                nbErrorWarning.Visible = true;
                return;
            }
            if ( ppBaptizee.PersonId == null )
            {
                nbErrorWarning.Text = "Please select a person to be baptized";
                nbErrorWarning.Visible = true;
                return;
            }
            if ( blackoutDates.Any( b => b.EffectiveStartDate.Value == dtpBaptismDate.SelectedDateTime.Value.Date ) )
            {
                nbErrorWarning.Text = "The date you selected is a blackout date";
                nbErrorWarning.Visible = true;
                return;
            }

            if ( GetAttributeValue( "LimitToValidServiceTimes" ).AsBoolean() )
            {
                GetServiceTimes();
                if ( !serviceTimes.Any( s => ( s.DayOfWeek == dtpBaptismDate.SelectedDateTime.Value.DayOfWeek ) && ( s.TimeOfDay == dtpBaptismDate.SelectedDateTime.Value.TimeOfDay ) ) )
                {
                    if ( !specialEvents.Any( s => s == dtpBaptismDate.SelectedDateTime.Value ) )
                    {
                        nbErrorWarning.Title = "Please enter a valid service time: <br>";
                        nbErrorWarning.Text = BuildInvalidServiceTimeString();
                        nbErrorWarning.Visible = true;
                        return;
                    }
                }
            }


            BaptismContext baptismContext = new BaptismContext();
            BaptizeeService baptizeeService = new BaptizeeService( baptismContext );
            if ( PageParameter( "BaptizeeId" ).AsIntegerOrNull() == null )
            {
                baptizee = new Baptizee { Id = 0 };
                baptizee.GroupId = PageParameter( "GroupId" ).AsInteger();
            }
            else
            {
                baptizee = baptizeeService.Get( PageParameter( "BaptizeeId" ).AsInteger() );
            }

            baptizee.BaptismDateTime = (DateTime)dtpBaptismDate.SelectedDateTime;
            int theId = (int)new PersonAliasService( new RockContext() ).GetPrimaryAliasId( (int)ppBaptizee.PersonId );
            baptizee.PersonAliasId = theId;

            if ( ppBaptizer1.PersonId != null )
            {
                theId = (int)new PersonAliasService( new RockContext() ).GetPrimaryAliasId( (int)ppBaptizer1.PersonId );
                baptizee.Baptizer1AliasId = theId;
            }
            if ( ppBaptizer2.PersonId != null )
            {
                theId = (int)new PersonAliasService( new RockContext() ).GetPrimaryAliasId( (int)ppBaptizer2.PersonId );
                baptizee.Baptizer2AliasId = theId;
            }
            if ( ppApprover.PersonId != null )
            {
                theId = (int)new PersonAliasService( new RockContext() ).GetPrimaryAliasId( (int)ppApprover.PersonId );
                baptizee.ApproverAliasId = theId;
            }
            baptizee.IsConfirmed = cbIsConfirmed.Checked;
            if ( baptizee.Id.Equals( 0 ) )
            {
            baptizeeService.Add( baptizee );

            }
            baptismContext.SaveChanges();
            ReturnToParentPage();
        }

        /// <summary>
        /// Handles the Click event of the btnDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnDelete_OnClick( object sender, EventArgs e )
        {
            BaptismContext baptismContext = new BaptismContext();
            BaptizeeService baptizeeService = new BaptizeeService( baptismContext );
            if ( baptizee == null )
            {
                baptizee = baptizeeService.Get( PageParameter( "BaptizeeId" ).AsInteger() );
            }
            if ( baptizee != null )
            {
                baptizeeService.Delete( baptizee );
                baptismContext.SaveChanges();
            }
            ReturnToParentPage();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnCancel_OnClick( object sender, EventArgs e )
        {
            ReturnToParentPage();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Grabs the service times for the campus
        /// </summary>
        protected void GetServiceTimes()
        {
            serviceTimes = new List<DateTime>();
            specialEvents = new List<DateTime>();
            Group group = new GroupService( new RockContext() ).Get( PageParameter( "GroupId" ).AsInteger() );
            group.LoadAttributes();

            Guid categoryguid = group.GetAttributeValue( "ServiceTimes" ).AsGuid();
            CategoryCache category = CategoryCache.Read( categoryguid );
            List<Schedule> serviceSchedules = new ScheduleService( new RockContext() ).Queryable()
                .Where( s => s.CategoryId == category.Id )
                .ToList();
            //What happens in the case of a special service
            foreach ( Schedule s in serviceSchedules )
            {
                iCalendar calendar = iCalendar.LoadFromStream( new StringReader( s.iCalendarContent ) ).First() as iCalendar;
                if ( calendar.RecurringItems.FirstOrDefault().RecurrenceRules.Count == 0 )
                {
                    var specialEvent = calendar.Events[0].DTStart;
                    if ( calendar.Events[0].DTStart != null )
                    {
                        specialEvents.Add( specialEvent.Value );
                    }
                }
                else
                {
                    DateTime serviceTime = calendar.Events[0].DTStart.Value;
                    if ( serviceTime != null )
                    {
                        DayOfWeek dayOfWeek = calendar.RecurringItems.FirstOrDefault().RecurrenceRules.FirstOrDefault().ByDay[0].DayOfWeek;
                        double daysToAdd = dayOfWeek - serviceTime.DayOfWeek;
                        serviceTime = serviceTime.AddDays( daysToAdd );
                        serviceTimes.Add( serviceTime );
                    }
                }

            }
        }

        /// <summary>
        /// Grabs the blackout dates for the campus
        /// </summary>
        protected void GetBlackoutDates()
        {
            Group group = new GroupService( new RockContext() ).Get( PageParameter( "GroupId" ).AsInteger() );
            group.LoadAttributes();
            Guid categoryguid = group.GetAttributeValue( "BlackoutDates" ).AsGuid();
            CategoryCache category = CategoryCache.Read( categoryguid );
            blackoutDates = new ScheduleService( new RockContext() ).Queryable()
                .Where( s => s.CategoryId == category.Id )
                .ToList();
        }

        /// <summary>
        /// Returns the user to the schedule page
        /// </summary>
        protected void ReturnToParentPage()
        {
            Dictionary<string, string> dictionaryInfo = new Dictionary<string, string>();
            dictionaryInfo.Add( "GroupId", PageParameter( "GroupId" ) );
            dictionaryInfo.Add( "SelectedDate", PageParameter( "SelectedDate" ) );
            NavigateToParentPage( dictionaryInfo );
        }

        /// <summary>
        /// For existing baptisms, binds the values to the controls
        /// </summary>
        /// <param name="baptizeeId"></param>
        protected void BindValues( int baptizeeId )
        {
            Baptizee baptizee = new BaptizeeService( new BaptismContext() ).Get( baptizeeId );
            lPanelTitle.Text = String.Format( "Edit {0}", baptizee.Person.FullName.FormatAsHtmlTitle() );
            dtpBaptismDate.SelectedDateTime = baptizee.BaptismDateTime;
            ppBaptizee.SetValue( baptizee.Person );
            if ( baptizee.Baptizer1 != null )
            {
                ppBaptizer1.SetValue( baptizee.Baptizer1 );
            }
            if ( baptizee.Baptizer2 != null )
            {
                ppBaptizer2.SetValue(baptizee.Baptizer2);

            }
            if ( baptizee.Approver != null )
            {
                ppApprover.SetValue( baptizee.Approver );

            }
            cbIsConfirmed.Checked = baptizee.IsConfirmed;
        }

        /// <summary>
        /// If the user entered an invalid service time, and the baptism times are bound to the service times, returns a string with a list of valid service times
        /// </summary>
        /// <returns>returns an unordered list of valid service times</returns>
        protected string BuildInvalidServiceTimeString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append( "<ul>" );
            foreach ( DateTime d in serviceTimes )
            {

                stringBuilder.AppendLine( String.Format( "<li>{0}</li>", d.ToString( "dddd h:mm tt" ) ) );
            }
            stringBuilder.Append( "</ul>" );
            return stringBuilder.ToString();
        }

        #endregion
    }
}