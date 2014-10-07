using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using com.centralaz.Baptism.Model;
using com.centralaz.Baptism.Data;


using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;

namespace RockWeb.Plugins.com_centralaz.Baptism
{

    [DisplayName( "Baptism Campus Detail Block" )]
    [Category( "com_centralaz > Baptism" )]
    [Description( "Detail block for Baptism scheduling" )]
    [LinkedPage( "Add Baptism Page", "", true, "", "", 0 )]
    [LinkedPage( "Add Blackout Day Page", "", true, "", "", 0 )]
    public partial class BaptismCampusDetail : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables

        #endregion

        #region Properties

        protected List<Schedule> blackoutDates;
        protected List<Baptizee> baptizeeList;

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
            GetBlackoutDates();
            BindCalendar();
            if ( !Page.IsPostBack )
            {
                // added for your convenience
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

        protected void lbAddBaptism_Click( object sender, EventArgs e )
        {
            Dictionary<string, string> dictionaryInfo = new Dictionary<string, string>();
            dictionaryInfo.Add( "GroupId", PageParameter( "GroupId" ) );
            dictionaryInfo.Add( "SelectedDate", calBaptism.SelectedDate.ToShortDateString() );
            NavigateToLinkedPage( "AddBaptismPage", dictionaryInfo );
        }

        protected void lbAddBlackout_Click( object sender, EventArgs e )
        {
            Dictionary<string, string> dictionaryInfo = new Dictionary<string, string>();
            dictionaryInfo.Add( "GroupId", PageParameter( "GroupId" ) );
            dictionaryInfo.Add( "SelectedDate", calBaptism.SelectedDate.ToShortDateString() );
            NavigateToLinkedPage( "AddBlackoutDayPage", dictionaryInfo );
        }

        protected void lbPrintReport_Click( object sender, EventArgs e )
        {

        }

        protected void calBaptism_SelectionChanged( object sender, EventArgs e )
        {
            UpdateScheduleList();
        }

        protected void calBaptisms_DayRender( object sender, DayRenderEventArgs e )
        {
            DateTime day = e.Day.Date;
 
            if ( baptizeeList.Any(b=>b.BaptismDateTime.Day==day.Day ))
            {
                e.Cell.Style.Add( "font-weight", "bold" );
            }
            //foreach(Schedule i in blackoutDates){
            //    if(i.iCalendarContent.Any( s => s.ScheduleItemDate.Date == day.Date ) )
            //    {
            //        e.Cell.Style.Add( "background-color", "#ffcfcf" );
            //    }            
            //}
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

        protected void BindCalendar()
        {
            calBaptism.SelectedDate = DateTime.Today;
            UpdateScheduleList();
        }
        protected void UpdateScheduleList()
        {
            DateTime[] dateRange = GetTheDateRange( calBaptism.SelectedDate );
            Group group = new GroupService( new RockContext() ).Get( PageParameter( "GroupId" ).AsInteger() );
            if ( group == null )
            {
                RockPage.Layout.Site.RedirectToDefaultPage();
            }
            else
            {
                lPanelHeadingDateRange.Text = String.Format( "{0}: {1} - {2}", group.Name, dateRange[0].ToString( "MMMM d" ), dateRange[1].ToString( "MMMM d" ) );
                //bool for if blackout date
                nbBlackOutWeek.Visible = false;
                baptizeeList = new BaptizeeService( new BaptismContext() ).GetBaptizeesByDateRange( dateRange[0], dateRange[1] );
                if ( baptizeeList.Count == 0 )
                {
                    nbNoBaptisms.Text = "No baptisms scheduled for the selected week!";
                    nbNoBaptisms.Visible = true;
                }
                else
                {
                    nbNoBaptisms.Visible = false;
                    PopulateScheduleList( baptizeeList );
                }
            }
        }
        protected DateTime[] GetTheDateRange( DateTime daySelected )
        {
            DateTime[] dateRange = new DateTime[2];
            int delta = DayOfWeek.Monday - daySelected.DayOfWeek;
            dateRange[0] = daySelected.AddDays( delta );
            dateRange[1] = dateRange[0].AddDays( 6 );
            return dateRange;
        }
        protected void PopulateScheduleList( List<Baptizee> baptizeeList )
        {

        }
        #endregion
    }
}