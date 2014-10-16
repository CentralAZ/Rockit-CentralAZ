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
        protected List<Baptizee> baptizees;
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
            GetBlackoutDates();
            UpdateBaptizees();


            if ( !Page.IsPostBack )
            {
                if ( PageParameter( "SelectedDate" ).AsDateTime().HasValue )
                {
                    BindCalendar( PageParameter( "SelectedDate" ).AsDateTime().Value );
                }
                else
                {
                    BindCalendar();
                }
            }
            else
            {
                //  UpdateScheduleList();
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

        protected void lbEditBlackout_Click( object sender, EventArgs e )
        {
            blackoutDate = blackoutDates.Where( b => b.EffectiveStartDate.Value.Date == calBaptism.SelectedDate.Date ).FirstOrDefault();
            Dictionary<string, string> dictionaryInfo = new Dictionary<string, string>();
            dictionaryInfo.Add( "GroupId", PageParameter( "GroupId" ) );
            dictionaryInfo.Add( "SelectedDate", calBaptism.SelectedDate.ToShortDateString() );
            dictionaryInfo.Add( "BlackoutId", blackoutDate.Id.ToString() );
            NavigateToLinkedPage( "AddBlackoutDayPage", dictionaryInfo );
        }

        protected void lbPrintReport_Click( object sender, EventArgs e )
        {

        }

        protected void calBaptism_SelectionChanged( object sender, EventArgs e )
        {
            //    PageParameter( "SelectedDate" ) = calBaptism.SelectedDate.ToShortDateString();
            UpdateScheduleList();
        }

        protected void calBaptisms_DayRender( object sender, DayRenderEventArgs e )
        {
            DateTime day = e.Day.Date;
            if ( baptizees != null )
            {
                if ( baptizees.Any( b => b.BaptismDateTime.Date == day.Date ) )
                {
                    e.Cell.Style.Add( "font-weight", "bold" );
                }
            }
            if ( blackoutDates.Any( b => b.EffectiveStartDate.Value.Date == day.Date ) )
            {
                e.Cell.Style.Add( "background-color", "#ffcfcf" );
            }

        }

        #endregion

        #region Methods

        protected void UpdateBaptizees()
        {
            baptizees = new BaptizeeService( new BaptismContext() ).GetAllBaptizees();
        }

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

        protected void BindCalendar()
        {

            calBaptism.SelectedDate = DateTime.Today;
            UpdateScheduleList();
        }

        protected void BindCalendar( DateTime selectedDate )
        {

            calBaptism.SelectedDate = selectedDate;
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
                blackoutDate = blackoutDates.Where( b => b.EffectiveStartDate.Value.Date == calBaptism.SelectedDate.Date ).FirstOrDefault();
                nbNoBaptisms.Visible = false;
                if ( blackoutDate != null )
                {
                    nbBlackOutWeek.Title = String.Format( "{0} has been blacked out!</br>", calBaptism.SelectedDate.ToLongDateString() );
                    nbBlackOutWeek.Text = String.Format( "{0}", blackoutDate.Description );
                    lbEditBlackout.Visible = true;
                    nbBlackOutWeek.Visible = true;
                    //PopulateWithBlackoutMessage( blackoutDate );
                }
                else
                {
                    lbEditBlackout.Visible = false;
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
        }

        protected DateTime[] GetTheDateRange( DateTime daySelected )
        {
            DateTime[] dateRange = new DateTime[2];
            DayOfWeek x1 = DayOfWeek.Monday;
            DayOfWeek x2 = daySelected.DayOfWeek;
            int delta;
            if ( x2 == DayOfWeek.Sunday )
            {
                delta = -6;
            }
            else
            {
                delta = x1 - x2;
            }
            dateRange[0] = daySelected.AddDays( delta );
            dateRange[1] = dateRange[0].AddDays( 6 );
            return dateRange;
        }

        protected void PopulateScheduleList( List<Baptizee> baptizeeList )
        {
            DateTime current = DateTime.MinValue;
            foreach ( Baptizee b in baptizeeList )
            {
                if ( current != b.BaptismDateTime )
                {
                    current = b.BaptismDateTime;
                    BuildItemListHeader( current );
                }

                BuildListItem( b );
            }
        }

        protected void BuildItemListHeader( DateTime date )
        {
            Literal lHeader = new Literal();
            lHeader.Text = string.Format( "<h3>Service: {0} - {1} {2}</h3>",
                date.ToShortTimeString(), date.DayOfWeek, date.ToString( "MM/d" ) );
            plBaptismList.Controls.Add( lHeader );
            plBaptismList.Controls.Add( new LiteralControl( "<div class='row'>" ) );
            plBaptismList.Controls.Add( new LiteralControl( "<div class='col-md-2'><h4>Attendee</h4></div>" ) );
            plBaptismList.Controls.Add( new LiteralControl( "<div class='col-md-2'><h4>Baptized By</h4></div>" ) );
            plBaptismList.Controls.Add( new LiteralControl( "<div class='col-md-2'><h4>Phone Number</h4></div>" ) );
            plBaptismList.Controls.Add( new LiteralControl( "<div class='col-md-2'><h4>Approved By</h4></div>" ) );
            plBaptismList.Controls.Add( new LiteralControl( "<div class='col-md-2'><h4>Confirmed</h4></div>" ) );
            plBaptismList.Controls.Add( new LiteralControl( "</div>" ) );
            plBaptismList.Controls.Add( new LiteralControl( "<hr>" ) );
        }

        protected void BuildListItem( Baptizee baptizee )
        {
            plBaptismList.Controls.Add( new LiteralControl( "<div class='row'>" ) );
            string url = ResolveUrl( string.Format( "~/Person/{0}", baptizee.Person.Id ) );
            String theString = String.Format( "<div class='col-md-2'><a href=\"{0}\">{1}</a></div>", url, baptizee.Person.FullName );
            plBaptismList.Controls.Add( new LiteralControl( theString ) );

            plBaptismList.Controls.Add( new LiteralControl( "<div class='col-md-2'>" ) );
            if ( baptizee.Baptizer1 != null )
            {
                url = ResolveUrl( string.Format( "~/Person/{0}", baptizee.Baptizer1.Id ) );
                plBaptismList.Controls.Add( new LiteralControl( string.Format( "<li><a href=\"{0}\">{1}</a></li>", url, baptizee.Baptizer1.FullName ?? "" ) ) );
            }
            else
            {
                plBaptismList.Controls.Add( new LiteralControl( "" ) );
            }
            if ( baptizee.Baptizer2 != null )
            {
                url = ResolveUrl( string.Format( "~/Person/{0}", baptizee.Baptizer2.Id ) );
                plBaptismList.Controls.Add( new LiteralControl( string.Format( "<li><a href=\"{0}\">{1}</a></li>", url, baptizee.Baptizer2.FullName ?? "" ) ) );
            }
            else
            {
                plBaptismList.Controls.Add( new LiteralControl( "" ) );
            } plBaptismList.Controls.Add( new LiteralControl( "</div>" ) );

            theString = String.Format( "<div class='col-md-2'>{0}</div>", baptizee.Person.PhoneNumbers.FirstOrDefault() );
            plBaptismList.Controls.Add( new LiteralControl( theString ) );

            if ( baptizee.Approver != null )
            {
                url = ResolveUrl( string.Format( "~/Person/{0}", baptizee.Approver.Id ) );
                theString = String.Format( "<div class='col-md-2'><a href=\"{0}\">{1}</a></div>", url, baptizee.Approver.FullName ?? "" );
            }
            else
            {
                theString = "<div class='col-md-2'></div>";
            }
            plBaptismList.Controls.Add( new LiteralControl( theString ) );

            CheckBox cb = new CheckBox
            {
                ID = string.Format( "cbConfirmed_{0}", baptizee.Id ),
                Checked = baptizee.IsConfirmed,
                Enabled = false
            };

            Dictionary<string, string> dictionaryInfo = new Dictionary<string, string>();
            dictionaryInfo.Add( "GroupId", PageParameter( "GroupId" ) );
            dictionaryInfo.Add( "SelectedDate", calBaptism.SelectedDate.ToShortDateString() );
            dictionaryInfo.Add( "BaptizeeId", baptizee.Id.ToString() );
            theString = LinkedPageUrl( "AddBaptismPage", dictionaryInfo );

            LinkButton lbEdit = new LinkButton
            {
                Text = "<i class='fa fa-pencil'></i>",
                PostBackUrl = theString
            };
            //  lbEdit.Click += lbEdit_Click;
            plBaptismList.Controls.Add( new LiteralControl( "<div class='col-md-2'>" ) );
            plBaptismList.Controls.Add( cb );
            plBaptismList.Controls.Add( new LiteralControl( "  </div>" ) );
            plBaptismList.Controls.Add( new LiteralControl( "<div class='col-md-2'>" ) );
            plBaptismList.Controls.Add( lbEdit );
            plBaptismList.Controls.Add( new LiteralControl( "</div>" ) );
            plBaptismList.Controls.Add( new LiteralControl( "</div>" ) );

            ScriptManager.GetCurrent( this.Page ).RegisterAsyncPostBackControl( lbEdit );
        }

        #endregion
    }
}