using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using com.centralaz.Accountability.Data;
using com.centralaz.Accountability.Model;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Communication;

namespace RockWeb.Plugins.com_centralaz.Accountability
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Accountability Report Form" )]
    [Category( "com_centralaz > Accountability" )]
    [Description( "Block for accountability group members to fill out and submit reports" )]
    public partial class AccountabilityReportForm : Rock.Web.UI.RockBlock
    {
        #region Fields

        GroupMember _groupMember = null;

        #endregion

        #region Properties

        public string emailReportIntroduction = "<p> The following is an Accountability Group Report from your team member. </p>";
        public string emailReportSubject = "Accountability Group Report";

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
                AssignReportDateOptions();
                ShowQuestions();
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

        /// <summary>
        /// Handles the OnClick event of the btnSubmit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbSubmit_OnClick( object sender, EventArgs e )
        {
            SaveReport();
            SendEmail();
        }

        /// <summary>
        /// Handles the OnClick event of the lbCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbCancel_OnClick( object sender, EventArgs e )
        {
            Dictionary<string, string> qryString = new Dictionary<string, string>();
            qryString["GroupId"] = PageParameter( "GroupId" );
            NavigateToParentPage( qryString );
        }
        #endregion

        #region Methods

        /// <summary>
        /// Binds the options for report dates to the report date drop down list.
        /// </summary>
        protected void AssignReportDateOptions()
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

            int daysElapsed = ( nextDueDate - recentReportDate ).Days;
            int daysUntilDueDate = ( nextDueDate - DateTime.Today ).Days;
            //Submit report for this week case
            if ( daysUntilDueDate < 6 )
            {
                ddlSubmitForDate.Items.Add( nextDueDate.ToShortDateString() );
            }
            //Report overdue case
            if ( daysElapsed > 7 )
            {
                ddlSubmitForDate.Items.Add( nextDueDate.AddDays( -7 ).ToShortDateString() );
            }
        }

        /// <summary>
        /// Populates the phQuestions placeholder with labels, dropdowns, and textboxes for the report questions.
        /// </summary>
        protected void ShowQuestions()
        {
            int groupId = int.Parse( PageParameter( "GroupId" ) );
            int personId = (int)CurrentPersonId;
            GroupMemberService groupMemberService = new GroupMemberService( new RockContext() );
            _groupMember = groupMemberService.GetByGroupIdAndPersonId( groupId, personId ).FirstOrDefault();
            List<Question> questions = new QuestionService( new AccountabilityContext() ).GetQuestionsFromGroupTypeID( _groupMember.Group.GroupTypeId );
            if ( questions.Count > 0 )
            {
                HtmlGenericControl questionRow = new HtmlGenericControl();
                Literal questionLongFormLiteral = new Literal();
                RockDropDownList responseDropDown = new RockDropDownList();
                RockTextBox responseTextBox = new RockTextBox();
                HtmlGenericControl gridCell = new HtmlGenericControl( "div" );

                //For each question
                for ( int i = 0; i < questions.Count; i++ )
                {
                    questionRow = new HtmlGenericControl( "div" );

                    //The question id
                    HiddenField questionIdHiddenField = new HiddenField();
                    questionIdHiddenField.ID = "hfQuestionId" + i.ToString();
                    questionRow.Controls.Add( questionIdHiddenField );

                    //The question long form
                    questionLongFormLiteral = new Literal();
                    questionLongFormLiteral.ID = "lblquestionLongForm" + i.ToString();
                    questionLongFormLiteral.Text = "<div class='col-md-7'>" + questions[i].LongForm + "</div>";
                    questionRow.Controls.Add( questionLongFormLiteral );

                    //The response yes/no dropdown
                    responseDropDown = new RockDropDownList();
                    responseDropDown.ID = "ddlResponseAnswer" + i.ToString();
                    responseDropDown.Items.Add( "yes" );
                    responseDropDown.Items.Add( "no" );
                    gridCell = new HtmlGenericControl( "div" );
                    gridCell.AddCssClass( "col-md-1" );
                    gridCell.Controls.Add( responseDropDown );
                    questionRow.Controls.Add( gridCell );

                    //The response comment
                    responseTextBox = new RockTextBox();
                    responseTextBox.ID = "txtResponseComment" + i.ToString();
                    responseTextBox.MaxLength = 150;
                    gridCell = new HtmlGenericControl( "div" );
                    gridCell.AddCssClass( "col-md-4" );
                    gridCell.Controls.Add( responseTextBox );
                    questionRow.Controls.Add( gridCell );

                    //And the row to phQuestions
                    phQuestions.Controls.Add( questionRow );
                }
            }
        }

        /// <summary>
        /// Saves the report to the database.
        /// </summary>
        protected void SaveReport()
        {
            AccountabilityContext dataContext = new AccountabilityContext();
            ResponseSetService responseSetService = new ResponseSetService( dataContext );
            ResponseSet myResponseSet = new ResponseSet();
            myResponseSet.PersonId = (int)CurrentPersonId;
            myResponseSet.GroupId = int.Parse( PageParameter( "GroupId" ) );
            myResponseSet.Comment = tbComments.Text;
            myResponseSet.SubmitForDate = DateTime.Parse( ddlSubmitForDate.SelectedValue );
            int correct = 0;

            int groupId = int.Parse( PageParameter( "GroupId" ) );
            int personId = (int)CurrentPersonId;
            GroupMemberService groupMemberService = new GroupMemberService( new RockContext() );
            _groupMember = groupMemberService.GetByGroupIdAndPersonId( groupId, personId ).FirstOrDefault();

            dataContext = new AccountabilityContext();
            ResponseService responseService = new ResponseService( dataContext );
            List<Question> questions = new QuestionService( new AccountabilityContext() ).GetQuestionsFromGroupTypeID( _groupMember.Group.GroupTypeId );
            Response myResponse = null;
            for ( int i = 0; i < questions.Count; i++ )
            {
                myResponse = new Response();
                myResponse.ResponseSetId = myResponseSet.Id;
                myResponse.QuestionId = questions[i].Id;
                String answerName = "ddlResponseAnswer" + i.ToString();
                Control control = this.FindControl( answerName );
                RockDropDownList questionAnswer = (RockDropDownList)control;
                if ( questionAnswer.SelectedValue == "yes" )
                {
                    myResponse.IsResponseYes = true;
                    correct++;
                }
                else
                {
                    myResponse.IsResponseYes = false;
                }
                myResponse.Comment = ( (RockTextBox)this.FindControl( "txtResponseComment" + i.ToString() ) ).Text;
                responseService.Add( myResponse );
            }
            myResponseSet.Score = correct / questions.Count;
            responseSetService.Add( myResponseSet );
            dataContext.SaveChanges();

        }

        /// <summary>
        /// Sends an email of the report to all group members
        /// </summary>
        protected void SendEmail()
        {
            Group group = new GroupService( new RockContext() ).Get( int.Parse( PageParameter( "GroupId" ) ) );
            if ( group.Members.Count > 0 )
            {
                string fromAddress = CurrentPerson.Email;
                string subject = string.Format( "{0} for {1} - ", emailReportSubject, group.Name, CurrentPerson.FullName );
                string body = CreateMessageBody( group );
                foreach ( GroupMember member in group.Members )
                {
                    if ( !string.IsNullOrWhiteSpace( member.Person.Email ) && member.Person.EmailPreference != EmailPreference.DoNotEmail )
                    {
                        Send( member.Person.Email, fromAddress, subject, body, new RockContext() );
                    }
                }
            }
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="recipient">The recipient's email address</param>
        /// <param name="from">The sender's email address</param>
        /// <param name="subject">The subject</param>
        /// <param name="body">The body</param>
        /// <param name="rockContext">The Rock Context</param>
        private void Send( string recipient, string from, string subject, string body, RockContext rockContext )
        {
            var recipients = new List<string>();
            recipients.Add( recipient );

            var channelData = new Dictionary<string, string>();
            channelData.Add( "From", from );
            channelData.Add( "Subject", subject );
            channelData.Add( "Body", System.Text.RegularExpressions.Regex.Replace( body, @"\[\[\s*UnsubscribeOption\s*\]\]", string.Empty ) );

            var channelEntity = EntityTypeCache.Read( Rock.SystemGuid.EntityType.COMMUNICATION_CHANNEL_EMAIL.AsGuid(), rockContext );
            if ( channelEntity != null )
            {
                var channel = ChannelContainer.GetComponent( channelEntity.Name );
                if ( channel != null && channel.IsActive )
                {
                    var transport = channel.Transport;
                    if ( transport != null && transport.IsActive )
                    {
                        var appRoot = GlobalAttributesCache.Read( rockContext ).GetValue( "PublicApplicationRoot" );
                        transport.Send( channelData, recipients, appRoot, string.Empty );
                    }
                }
            }
        }

        /// <summary>
        /// Creates the email message body.
        /// </summary>
        /// <param name="group">The group</param>
        /// <returns>Returns a string containing html for the email body</returns>
        protected string CreateMessageBody( Group group )
        {
            StringBuilder body = new StringBuilder();
            body.Append( emailReportIntroduction );

            //Get the  group type's questions
            List<Question> questions = new QuestionService( new AccountabilityContext() ).GetQuestionsFromGroupTypeID( _groupMember.Group.GroupTypeId );
            body.Append( string.Format( "<h2>{0} - {1} {2}</h2>", group.Name, CurrentPerson.FirstName, CurrentPerson.LastName ) );

            //Report week
            body.Append( string.Format( "<div class='row'><div class='col-md-4'><b>Report for week:</b><div><div class='col-md-4'>{0}</div></div>", ddlSubmitForDate.SelectedValue ) );

            //Question rows
            for ( int i = 0; i < questions.Count; i++ )
            {
                RockDropDownList dropDown = (RockDropDownList)this.FindControl( "ddlResponseAnswer" + i.ToString() );
                RockTextBox textBox = ( (RockTextBox)this.FindControl( "txtResponseComment" + i.ToString() ) );
                body.Append( "<div class='row'>" );
                body.Append( string.Format( "<div class='col-md-4'><b>{0}:</b></div>", questions[i].ShortForm ) );
                if ( dropDown.SelectedValue == "yes" )
                {
                    body.Append( "<div class='col-md-4'>yes" );

                }
                else
                {
                    body.Append( "<div class='col-md-4'><b>no</b>" );
                }
                if ( textBox.Text.Trim().Length == 0 )
                {
                    body.Append( "</div>" );
                }
                else
                {
                    body.Append( string.Format( " - {0}</div>", textBox.Text ) );
                }
                body.Append( "</div>" );
            }

            //ResponseSet Comment
            body.Append( "<div class='row'>" );
            body.Append( string.Format( "div class='col-md-12'>{0}</div>", tbComments.Text ) );
            body.Append( "</div>" );
            return body.ToString();
        }

        /// <summary>
        /// Returns the next report date for the group.
        /// </summary>
        /// <param name="reportStartDate">The group's report start date</param>
        /// <returns>A DateTime that is the next report due date.</returns>
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
        /// Gets a group of id groupId
        /// </summary>
        /// <param name="groupId">the id of the group to get</param>
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