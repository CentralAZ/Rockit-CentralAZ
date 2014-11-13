using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;

namespace RockWeb.Plugins.com_centralaz.Calendar
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Event Calendar" )]
    [Category( "com_centralaz > EventCalendar" )]
    [Description( "A calendar for a church to manage events." )]
    public partial class EventCalendar : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables

        #endregion

        #region Properties

        // used for public / protected properties

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

            RockPage.AddCSSLink( ResolveRockUrl( "~/Plugins/com_centralaz/Calendar/Styles/fullcalendar.css" ) );
            RockPage.AddCSSLink( ResolveRockUrl( "~/Plugins/com_centralaz/Calendar/Styles/calendar.css" ) );

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
                // added for your convenience
            }
            InsertTemplates();
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

        #endregion

        #region Methods

        /// <summary>
        /// Responsible for injecting the jQuery templates into the header.
        /// </summary>
        private void InsertTemplates()
        {
            if ( !Page.Header.Controls.OfType<LiteralControl>().Any( c => c.Text.Contains( "id=\"event-featured-list-template\"" ) ) )
            {
                string template = @"
<script type=""text/html"" id=""event-featured-list-template"">
<li class=""item"">
	<div class=""date {%= className %}"">{%= date %}<div class=""month"">{%= month %}</div></div>
	<div class=""photo""><a href=""{%= url %}""><img src=""{%= imageUrl %}"" /></a></div>
	<h3><a href=""{%= url %}"">{%= title %}</a></h3>
</li>
</script>

<script type=""text/html"" id=""event-list-template"">
<li class=""item"">
	<div class=""date {%= className %}"">{%= date %}<div class=""month"">{%= month %}</div></div>
	<h3><a href=""{%= url %}"">{%= title %}</a></h3>
	<h4>{%= title %}</h4>
	<p class=""summary"">{%= description %}</p>
</li>
</script>";
                Page.Header.Controls.Add( new LiteralControl( template ) );
            }
        }


        #endregion
    }
}