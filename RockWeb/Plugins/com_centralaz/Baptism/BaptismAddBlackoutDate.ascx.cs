using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;

namespace RockWeb.Plugins.com_centralaz.Baptism
{

    [DisplayName( "Baptism Add Blackout Date Block" )]
    [Category( "com_centralaz > Baptism" )]
    [Description( "Block for adding blackout dates to baptism schedules" )]
    public partial class BaptismAddBlackoutDate : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables

        #endregion

        #region Properties

        protected List<Schedule> blackoutDates;
        protected Schedule blackoutDate;

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
                if ( PageParameter( "BlackoutId" ).AsIntegerOrNull() == null )
                {
                    dpBlackOutDate.SelectedDate = PageParameter( "SelectedDate" ).AsDateTime();
                    btnDelete.Visible = false;
                }
                else
                {
                    BindValues( PageParameter( "BlackoutId" ).AsInteger() );
                }
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
        protected void btnSave_OnClick( object sender, EventArgs e )
        {
            nbNotification.Visible = false;
            //baptisms exist for blackout date
            //blackout date already exists
            GetBlackoutDates();
            if ( blackoutDates.Any( b => ( b.EffectiveStartDate.Value.Date == dpBlackOutDate.SelectedDate.Value.Date ) && ( b.CategoryId == GetCategoryId() ) && ( b.Id != PageParameter( "BlackoutId" ).AsIntegerOrNull() ) ) )
            {
                nbNotification.Text = "Blackout already exists for that date";
                nbNotification.Visible = true;
                return;
            }
            //check that group is valid
            int categoryId = GetCategoryId();
            if ( categoryId == -1 )
            {
                nbNotification.Text = "Error loading campus";
                nbNotification.Visible = true;
                return;
            }
            //save blackout date to db
            RockContext rockContext = new RockContext();
            ScheduleService scheduleService = new ScheduleService( rockContext );
            if ( PageParameter( "BlackoutId" ).AsIntegerOrNull() == null )
            {
                blackoutDate = new Schedule { Id = 0 };
                blackoutDate.CategoryId = categoryId;
            }
            else
            {
                blackoutDate = scheduleService.Get( PageParameter( "BlackoutId" ).AsInteger() );
            }
            iCalendar calendar = new iCalendar();
            DDay.iCal.IDateTime datetime = new iCalDateTime();
            Event theEvent = new Event();
            calendar.Events.Add( theEvent );
            var x1 = theEvent.DTStart;
            datetime.Value = dpBlackOutDate.SelectedDate.Value;
            calendar.Events[0].DTStart = datetime;
            iCalendarSerializer calSerializer = new iCalendarSerializer( calendar );
            blackoutDate.iCalendarContent = calSerializer.SerializeToString();
            blackoutDate.Description = tbDescription.Text;
            if ( blackoutDate.Id.Equals( 0 ) )
            {
                scheduleService.Add( blackoutDate );

            }
            rockContext.SaveChanges();
            ReturnToParentPage();
        }
        protected void btnDelete_OnClick( object sender, EventArgs e )
        {
            RockContext rockContext = new RockContext();
            ScheduleService scheduleService = new ScheduleService( rockContext );
            if ( blackoutDate == null )
            {
                blackoutDate = scheduleService.Get( PageParameter( "BlackoutId" ).AsInteger() );
            }
            if ( blackoutDate != null )
            {
                scheduleService.Delete( blackoutDate );
                rockContext.SaveChanges();
            }
            ReturnToParentPage();
        }
        protected void btnCancel_OnClick( object sender, EventArgs e )
        {
            ReturnToParentPage();
        }
        #endregion

        #region Methods

        protected void GetBlackoutDates()
        {
            Category category = new CategoryService( new RockContext() ).Queryable()
                .Where( c => c.Name == "Mesa Blackout" )
                .FirstOrDefault();
            blackoutDates = new ScheduleService( new RockContext() ).Queryable()
                .Where( s => s.CategoryId == category.Id )
                .ToList();
        }
        protected void ReturnToParentPage()
        {
            Dictionary<string, string> dictionaryInfo = new Dictionary<string, string>();
            dictionaryInfo.Add( "GroupId", PageParameter( "GroupId" ) );
            dictionaryInfo.Add( "SelectedDate", dpBlackOutDate.SelectedDate.Value.ToShortDateString() );
            NavigateToParentPage( dictionaryInfo );
        }

        protected void BindValues( int blackoutId )
        {
            Schedule blackoutDate = new ScheduleService( new RockContext() ).Get( blackoutId );
            dpBlackOutDate.SelectedDate = blackoutDate.EffectiveStartDate;
            tbDescription.Text = blackoutDate.Description;
        }
        protected int GetCategoryId()
        {
            Group group = new GroupService( new RockContext() ).Get( PageParameter( "GroupId" ).AsInteger() );
            group.LoadAttributes();
            Guid categoryguid = group.GetAttributeValue( "BlackoutDates" ).AsGuid();
            CategoryCache category = CategoryCache.Read( categoryguid );
            if ( category == null )
            {
                return -1;
            }
            else
            {
                return category.Id;
            }
        }

        #endregion
    }
}