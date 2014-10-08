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

    [DisplayName( "Baptism Add Baptism Block" )]
    [Category( "com_centralaz > Baptism" )]
    [Description( "Block for adding a baptism" )]
    public partial class BaptismAddBaptism : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables

        #endregion

        #region Properties
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
            GetBlackoutDates();
            if ( PageParameter( "BaptizeeId" ).AsIntegerOrNull() == null )
            {
                btnDelete.Visible = false;
                dtpBaptismDate.SelectedDateTime = PageParameter( "SelectedDate" ).AsDateTime();
            }
            else
            {
                BindValues( PageParameter( "BaptizeeId" ).AsInteger() );
            }
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
        protected void btnSave_OnClick( object sender, EventArgs e )
        {
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
            if ( blackoutDates.Any( b => b.EffectiveStartDate.Value.Day == dtpBaptismDate.SelectedDateTime.Value.Day ) )
            {
                nbErrorWarning.Text = "The date you selected is a blackout date";
                nbErrorWarning.Visible = true;
                return;
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
            baptizee.PersonAliasId = (int)ppBaptizee.PersonId;
            if ( ppBaptizer1.PersonId != null )
            {
                baptizee.Baptizer1AliasId = (int)ppBaptizer1.PersonId;
            }
            if ( ppBaptizer2.PersonId != null )
            {
                baptizee.Baptizer2AliasId = (int)ppBaptizer2.PersonId;
            }
            if ( ppApprover.PersonId != null )
            {
                baptizee.ApproverAliasId = (int)ppApprover.PersonId;
            }
            baptizee.IsConfirmed = cbIsConfirmed.Checked;
            baptizeeService.Add( baptizee );
            baptismContext.SaveChanges();
            ReturnToParentPage();
        }
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
            dictionaryInfo.Add( "SelectedDate", PageParameter( "SelectedDate" ) );
            NavigateToParentPage( dictionaryInfo );
        }

        protected void BindValues(int baptizeeId)
        {
            Baptizee baptizee = new BaptizeeService(new BaptismContext()).Get(baptizeeId);
            dtpBaptismDate.SelectedDateTime = baptizee.BaptismDateTime;
            ppBaptizee.PersonId = baptizee.PersonAliasId;
            ppBaptizer1.PersonId = baptizee.Baptizer1AliasId;
            ppBaptizer2.PersonId = baptizee.Baptizer2AliasId;
            ppApprover.PersonId = baptizee.ApproverAliasId;
            cbIsConfirmed.Checked = baptizee.IsConfirmed;
        }
        #endregion
    }
}