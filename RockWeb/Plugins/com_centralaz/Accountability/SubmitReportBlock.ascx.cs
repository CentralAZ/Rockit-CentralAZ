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
    [DisplayName("Submit Report")]
    [Category("com_centralaz > Accountability")]
    [Description("The Submit Report Block")]

    [LinkedPage("Detail Page", "", true, "", "", 0)]
    public partial class SubmitReportBlock : Rock.Web.UI.RockBlock
    {

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            lblStatusMessage.Text = "Submit Report by 12/4/14";
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
            WriteMessage();
            base.OnLoad(e);
        }

        #endregion


        #region Internal Methods

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Dictionary<String, String> parameters = new Dictionary<string,string>(){
                {"GroupId", PageParameter("GroupId")},
                {"PersonId", CurrentPersonId.ToString()}

            };
            NavigateToLinkedPage("DetailPage", parameters);
        }
        protected void WriteMessage()
        {
            bool canSubmit = IsGroupMember();
            if (!canSubmit)
            {
                pnlContent.Visible = false;
            }
            else
            {

                int groupId = int.Parse(PageParameter("GroupId"));
                DateTime recentReportDate;
                Group group = GetGroup(groupId);
                group.LoadAttributes();
                DateTime reportStartDate = DateTime.Parse(group.GetAttributeValue("ReportStartDate"));
                try
                {
                    ResponseSet recentReport = new ResponseSetService(new AccountabilityContext()).GetMostRecentReport(CurrentPersonId, groupId);
                    recentReportDate = recentReport.SubmitForDate; 
                }
                catch (Exception e)
                {
                    recentReportDate =reportStartDate;
                }
                DateTime nextDueDate = NextReportDate(reportStartDate);

                IsReportOverdue(nextDueDate, recentReportDate);

            }

        }
        protected void IsReportOverdue(DateTime nextDueDate, DateTime lastReportDate)
        {
            int daysElapsed = (nextDueDate - lastReportDate).Days;
            int daysUntilDueDate = (nextDueDate - DateTime.Today).Days;
            //All caught up case
            if (daysElapsed <= 7 && daysUntilDueDate >= 6)
            {
                lblStatusMessage.Text = "Report Submitted";
            }
            //Submit report for this week case
            if (daysElapsed <= 7 && daysUntilDueDate < 6)
            {
                lblStatusMessage.Text = "Report due in " + daysUntilDueDate + " days";
            }
            //Report overdue case
            if (daysElapsed > 7)
            {
                lblStatusMessage.Text = "Report Overdue for week of " + nextDueDate.AddDays(-7);
            }
        }
        protected DateTime NextReportDate(DateTime reportStartDate)
        {
            DateTime today = DateTime.Now;

            int daysElapsed = (today - reportStartDate).Days;
            int remainder = daysElapsed % 7;
            int daysUntil = 7 - remainder;

            DateTime reportDue = today.AddDays(daysUntil);

            return reportDue;
        }

        protected bool IsGroupMember()
        {

            int groupId = int.Parse(PageParameter("GroupId"));
            bool isMember = false;
            var qry = new GroupMemberService(new RockContext()).Queryable()
            .Where(gm => (gm.PersonId == CurrentPersonId) && (gm.GroupId == groupId))
            .FirstOrDefault();
            if (qry != null)
            {
                isMember = true;
            }
            return isMember;
        }
        private Group GetGroup(int groupId)
        {
            string key = string.Format("Group:{0}", groupId);
            Group group = RockPage.GetSharedItem(key) as Group;
            if (group == null)
            {
                group = new GroupService(new RockContext()).Queryable("GroupType,GroupLocations.Schedules")
                    .Where(g => g.Id == groupId)
                    .FirstOrDefault();
                RockPage.SaveSharedItem(key, group);
            }

            return group;
        }
        #endregion
    }
}