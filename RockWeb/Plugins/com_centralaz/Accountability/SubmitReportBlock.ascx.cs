using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using com.centralaz.Accountability.Data;
using com.centralaz.Accountability.Model;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.com_centralaz.Accountability
{
    [DisplayName( "Submit Report" )]
    [Category( "com_centralaz > Accountability" )]
    [Description( "The Submit Report Block" )]

    [LinkedPage( "Detail Page", "", true, "", "", 0 )]
    public partial class SubmitReportBlock : Rock.Web.UI.RockBlock
    {

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
            }
            GetDatesForMessage();
            base.OnLoad( e );
        }

        #endregion


        #region Internal Methods

        /// <summary>
        /// Handles the OnClick event of the lbSubmit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void lbSubmit_Click( object sender, EventArgs e )
        {
            int groupId = int.Parse( PageParameter( "GroupId" ) );
            NavigateToLinkedPage( "DetailPage", "GroupId", groupId );
        }

        /// <summary>
        /// Grabs the dates for the due date message and passes them to the WriteDueDateMessage method.
        /// </summary>
        protected void GetDatesForMessage()
        {
            bool canSubmit = IsGroupMember();
            if ( !canSubmit )
            {
                pnlContent.Visible = false;
            }
            else
            {

                int groupId = int.Parse( PageParameter( "GroupId" ) );
                DateTime recentReportDate;
                Group group = GetGroup( groupId );
                group.LoadAttributes();
                DateTime reportStartDate = DateTime.Parse( group.GetAttributeValue( "ReportStartDate" ) );
                try
                {
                    ResponseSet recentReport = new ResponseSetService( new AccountabilityContext() ).GetMostRecentReport( CurrentPersonId, groupId );
                    recentReportDate = recentReport.SubmitForDate;
                }
                catch ( Exception e )
                {
                    recentReportDate = reportStartDate;
                }
                DateTime nextDueDate = NextReportDate( reportStartDate );

                WriteDueDateMessage( nextDueDate, recentReportDate );
            }
        }

        /// <summary>
        /// Populates the lStatesMessage literal with the due date message.
        /// </summary>
        /// <param name="nextDueDate">The next due date</param>
        /// <param name="lastReportDate">The last report date</param>
        protected void WriteDueDateMessage( DateTime nextDueDate, DateTime lastReportDate )
        {
            int daysElapsed = ( nextDueDate - lastReportDate ).Days;
            int daysUntilDueDate = ( nextDueDate - DateTime.Today ).Days;
            //All caught up case
            if ( daysElapsed <= 7 && daysUntilDueDate >= 6 )
            {
                lStatusMessage.Text = "Report Submitted";
                lbSubmitReport.Enabled = false;
            }
            //Submit report for this week case
            if ( daysElapsed <= 7 && daysUntilDueDate < 6 )
            {
                if ( daysUntilDueDate == 1 )
                {
                    lStatusMessage.Text = "Report due in 1 day";
                }
                else
                {
                    lStatusMessage.Text = "Report due in " + daysUntilDueDate + " days";
                }
            }
            //Report overdue case
            if ( daysElapsed > 7 )
            {
                lStatusMessage.Text = "Report Overdue for week of " + nextDueDate.AddDays( -7 ).ToShortDateString();
            }
        }

        /// <summary>
        /// Returns the next report due date.
        /// </summary>
        /// <param name="reportStartDate">The group's report start date</param>
        /// <returns>The next report due date</returns>
        protected DateTime NextReportDate( DateTime reportStartDate )
        {
            DateTime today = DateTime.Now;

            int daysElapsed = ( today - reportStartDate ).Days;
            int remainder = daysElapsed % 7;
            int daysUntil = 7 - remainder;

            DateTime reportDue = today.AddDays( daysUntil );

            return reportDue;
        }

        /// <summary>
        /// Checks if the current person is a group member
        /// </summary>
        /// <returns>A bool of whether the current person is a member or not</returns>
        protected bool IsGroupMember()
        {

            int groupId = int.Parse( PageParameter( "GroupId" ) );
            bool isMember = false;
            var qry = new GroupMemberService( new RockContext() ).Queryable()
            .Where( gm => ( gm.PersonId == CurrentPersonId ) && ( gm.GroupId == groupId ) )
            .FirstOrDefault();
            if ( qry != null )
            {
                isMember = true;
            }
            return isMember;
        }

        /// <summary>
        /// Returns the group with id groupId.
        /// </summary>
        /// <param name="groupId">the groupId.</param>
        /// <returns>Returns the group</returns>
        private Group GetGroup( int groupId )
        {
            string key = string.Format( "Group:{0}", groupId );
            Group group = RockPage.GetSharedItem( key ) as Group;
            if ( group == null )
            {
                group = new GroupService( new RockContext() ).Queryable( "GroupType,GroupLocations.Schedules" )
                    .Where( g => g.Id == groupId )
                    .FirstOrDefault();
                RockPage.SaveSharedItem( key, group );
            }

            return group;
        }
        #endregion
    }
}