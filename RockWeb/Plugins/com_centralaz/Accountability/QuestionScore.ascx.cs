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
    [DisplayName("Accountability Question Score")]
    [Category("com_centralaz > Accountability")]
    [Description("Shows the score for each question")]

    public partial class QuestionScore : Rock.Web.UI.RockBlock
    {

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

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
            LoadScores();
            base.OnLoad(e);
        }

        #endregion


        #region Internal Methods

        protected void LoadScores()
        {
            GroupMember groupMember = new GroupMemberService(new RockContext()).Get(int.Parse(PageParameter("GroupMemberId")));
            int groupId = groupMember.GroupId;
            int personId = groupMember.PersonId;
            ResponseSetService responseSetService = new ResponseSetService(new AccountabilityContext());
            QuestionService questionService = new QuestionService(new AccountabilityContext());

            double overallScore = responseSetService.GetOverallScore(personId, groupId);
            double[] weakScore = responseSetService.GetWeakScore(personId, groupId);
            double[] strongScore = responseSetService.GetStrongScore(personId, groupId);


            if (overallScore == null || overallScore == -1)
            {
                lblOverallScore.Text = "N/A";
            }
            else
            {
                lblOverallScore.Text = overallScore.ToString();
            }

            if (weakScore[0] == -1)
            {
                lblWeakScore.Text = "N/A";
            }
            else
            {
                lblWeakScore.Text = weakScore[0].ToString();
                lblWeakScoreQuestion.Text = questionService.GetShortForm(weakScore[1]);
            }

            if (strongScore[0] == -1)
            {
                lblStrongScore.Text = "N/A";
            }
            else
            {
                lblStrongScore.Text = strongScore[0].ToString();
                lblStrongScoreQuestion.Text = questionService.GetShortForm(strongScore[1]);
            }


        }
        #endregion
    }
}