using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using com.centralaz.Accountability.Data;
using com.centralaz.Accountability.Model;

using Rock;
using Rock.Web;
using Rock.Attribute;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.com_centralaz.Accountability
{
    [DisplayName("Accountability Group Member Detail")]
    [Category("com_centralaz > Accountability")]
    [Description("Shows the detail for a group Member")]
    [LinkedPage("Detail Page")]

    public partial class AccountabilityGroupMemberDetail : Rock.Web.UI.RockBlock
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
                ShowDetail(PageParameter("GroupMemberId").AsInteger(), PageParameter("GroupId").AsIntegerOrNull());
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Returns breadcrumbs specific to the block that should be added to navigation
        /// based on the current page reference.  This function is called during the page's
        /// oninit to load any initial breadcrumbs
        /// </summary>
        /// <param name="pageReference">The page reference.</param>
        /// <returns></returns>
        public override List<BreadCrumb> GetBreadCrumbs(PageReference pageReference)
        {
            var breadCrumbs = new List<BreadCrumb>();

            int? groupMemberId = PageParameter(pageReference, "GroupMemberId").AsIntegerOrNull();
            if (groupMemberId != null)
            {
                GroupMember groupMember = new GroupMemberService(new RockContext()).Get(groupMemberId.Value);
                if (groupMember != null)
                {
                    breadCrumbs.Add(new BreadCrumb(groupMember.Person.FullName, pageReference));
                }
                else
                {
                    breadCrumbs.Add(new BreadCrumb("New Group Member", pageReference));
                }
            }
            else
            {
                // don't show a breadcrumb if we don't have a pageparam to work with
            }

            return breadCrumbs;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
        #endregion

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            NavigateToLinkedPage("DetailPage", "GroupMemberId", PageParameter("GroupMemberId").AsInteger());

        }

        #region Internal Methods

        /// <summary>
        /// Shows the detail.
        /// </summary>
        /// <param name="groupMemberId">The group member identifier.</param>
        public void ShowDetail(int groupMemberId)
        {
            ShowDetail(groupMemberId, null);
        }

        /// <summary>
        /// Shows the detail.
        /// </summary>
        /// <param name="groupMemberId">The group member identifier.</param>
        /// <param name="groupId">The group id.</param>
        public void ShowDetail(int groupMemberId, int? groupId)
        {
            var rockContext = new RockContext();
            GroupMember groupMember = null;

            if (!groupMemberId.Equals(0))
            {
                groupMember = new GroupMemberService(rockContext).Get(groupMemberId);
                if (groupMember != null)
                {
                    groupMember.LoadAttributes();
                }
            }

            if (groupMember == null)
            {
                nbErrorMessage.Title = "Invalid Request";
                nbErrorMessage.Text = "An incorrect querystring parameter was used.  A valid GroupMemberId or GroupId parameter is required.";
                pnlViewDetails.Visible = false;
                return;
            }

            pnlViewDetails.Visible = true;

            hfGroupId.Value = groupMember.GroupId.ToString();
            hfGroupMemberId.Value = groupMember.Id.ToString();

            //Load name and image
            Person person = groupMember.Person;
            if (person != null && person.Id != 0)
            {
                if (person.NickName == person.FirstName)
                {
                    lName.Text = person.FullName.FormatAsHtmlTitle();
                }
                else
                {
                    lName.Text = String.Format("{0} {2} <span class='full-name'>({1})</span>", person.NickName.FormatAsHtmlTitle(), person.FirstName, person.LastName);
                }

                // Setup Image
                string imgTag = Rock.Model.Person.GetPhotoImageTag(person.PhotoId, person.Age, person.Gender, 120, 120);
                if (person.PhotoId.HasValue)
                {
                    lImage.Text = string.Format("<a href='{0}'>{1}</a>", person.PhotoUrl, imgTag);
                }
                else
                {
                    lImage.Text = imgTag;
                }
            }

            // render UI based on Authorized and IsSystem
            var group = groupMember.Group;
            rblStatus.BindToEnum(typeof(GroupMemberStatus));
            rblStatus.SetValue((int)groupMember.GroupMemberStatus);
            rblStatus.Enabled = false;
            rblStatus.Label = string.Format("Status");

            groupMember.LoadAttributes();
            lblStartDate.Text = groupMember.GetAttributeValue("MemberStartDate");

            //Determines visibility of the edit button
            bool readOnly=false;
            if (!IsUserAuthorized(Authorization.EDIT) || !group.IsAuthorized(Authorization.EDIT, this.CurrentPerson))
            {
                readOnly = true;
            }

            if (groupMember.IsSystem)
            {
                readOnly = true;
            }
            if (readOnly)
            {
                btnEdit.Visible = false;
            }
            else
            {
                btnEdit.Visible = true;
            }
        }

        /// <summary>
        /// Clears the error message title and text.
        /// </summary>
        private void ClearErrorMessage()
        {
            nbErrorMessage.Title = string.Empty;
            nbErrorMessage.Text = string.Empty;
        }
        #endregion

    }
}