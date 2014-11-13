<%@ Control Language="C#" ClassName="ArenaWeb.UserControls.Custom.Cccev.Web2.HasselhoffEventCalendar" Inherits="Arena.Portal.PortalControl" %>
<%@ Import Namespace="Arena.Portal" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/Include/scripts/jquery.hoverIntent.min.js" />
        <asp:ScriptReference Path="~/UserControls/Custom/Cccev/Web2/js/fullcalendar.min.js" />
        <asp:ScriptReference Path="~/UserControls/Custom/Cccev/Web2/js/eventcalendar.min.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<script runat="server">
    [PageSetting("Event Detail Page", "Reference to Event Details page.", true)]
    public string EventDetailPageSetting { get { return Setting("EventDetailPage", "", true); } }
    
    private void Page_Load(object sender, EventArgs e)
    {
        BasePage.AddCssLink(Page, "~/UserControls/Custom/Cccev/Web2/css/fullcalendar.css");
    }
</script>

<input type="hidden" id="ihCalendarEventDetails" value="<%= EventDetailPageSetting %>" />
<div id="event-calendar" class="event-view"></div>
<div id="calendar-overlay" class="spinner"></div>