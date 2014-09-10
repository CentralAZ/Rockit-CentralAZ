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
                int groupId = int.Parse(PageParameter( "GroupId" ));
                ResponseSet recentReport=new ResponseSetService(new RockContext()).GetMostRecentReport(CurrentPersonId, groupId);
                DateTime ReportStartDate=new GroupTypeService(new RockContext()).Queryable("Group, GroupType")
                    .Where(g=> (g.Id==groupId))
                    .Select(g=> g.GroupType.InheritedGroupType)
                    .First();
                DateTime NextDueDate=NextReportDate(ReportStartDate);
                IsReportOverdue(NextDueDate, recentReport.SubmitForDate);

                }

            }
        protected void IsReportOverdue(DateTime NextDueDate, DateTime LastReportDate)
        {
            int daysElapsed = (NextDueDate - LastReportDate).Days;
            int daysUntilDueDate = (NextDueDate - DateTime.Today).Days;
            //All caught up case
            if (daysElapsed <= 7 && daysUntilDueDate >= 5)
            {
                lblStatusMessage.Text = "Report Submitted";
            }
            //Submit report for this week case
            if(daysElapsed<=7 && daysUntilDueDate<5){
                lblStatusMessage.Text="Report due in {0} days", daysUntilDueDate;
            }
            //Report overdue case
            if (daysElapsed > 7)
            {
                lblStatusMessage.Text = "Report Overdue";
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
            .First();
            if (qry != null)
            {
                isMember = true;
            }
            return isMember;
        }
        #endregion
    }
}