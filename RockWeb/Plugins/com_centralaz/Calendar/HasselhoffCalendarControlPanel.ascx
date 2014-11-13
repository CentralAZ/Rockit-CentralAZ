<%@ Control Language="C#" ClassName="ArenaWeb.UserControls.Custom.Cccev.Web2.HasselhoffCalendarControlPanel" Inherits="Arena.Portal.PortalControl" %>

<script runat="server">
    [ListFromSqlSetting("Primary Topic Area", "Set this to tie calendar to a specific topic or ministry.", false, "",
        "SELECT l.lookup_id, l.lookup_value FROM core_lookup l INNER JOIN core_lookup_type lt ON lt.lookup_type_id = l.lookup_type_id AND lt.guid = '1FE55E22-F67C-46BA-A6AE-35FD112AFD6D' WHERE active = 1 ORDER BY lookup_order", ListSelectionMode.Multiple )]
    public string TopicAreaSetting { get { return Setting("TopicArea", "", false); } }

    [TextSetting( "Calendar Prepend Title", "Name to prepend to calendar title when a topic area is specified. Default 'Ministry'.", false )]
    public string CalendarTitleSetting { get { return Setting( "CalendarTitle", "Ministry", false ); } }

    private Lookup[] campuses;
    
    private void Page_Load(object sender, EventArgs e)
    {
        campuses = new LookupType(SystemGuids.CAMPUS_LOOKUP_TYPE).Values.ToArray();
        
        // TODO: Make these module settings...
        BasePage.AddCssLink(Page, "~/Templates/cccev/007/css/jquery-ui-1.8.16.custom.css");
        BasePage.AddCssLink(Page, "~/UserControls/Custom/Cccev/Web2/css/calendar.css");
    }
</script>

<asp:ScriptManagerProxy ID="smpScripts" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/include/scripts/Custom/Cccev/lib/jquery-ui-1.8.13.min.js" />
        <asp:ScriptReference Path="~/UserControls/Custom/Cccev/Web2/js/eventCalendar-controls.min.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<% if (TopicAreaSetting.Trim() != string.Empty)
   {
       %> <h2 class="multi-line-heading"><%= string.Format( "{0}<br />", CalendarTitleSetting )%>Calendar</h2> <%
   }
   else
   {
       %> <h2 class="content-heading">Event Filters</h2> <%
   }
%>
<div class="control-wrap">
    
    <div id="slider-wrap">
        <h2>Date Range</h2>
        <div id="date-slider"></div>
        <div class="range">
            <span class="min"></span>
            <span class="max"></span>
        </div>
    </div>

    <h2>Keywords</h2>
    <asp:TextBox ID="tbKeywords" runat="server" CssClass="calendar-search" />

    <h2>View</h2>
    <ul id="calendar-views">
        <li><input type="radio" id="rbCalendar" name="calendar-view" checked="checked" rel="calendar" /><label for="rbCalendar">Calendar</label></li>
        <li><input type="radio" id="rbList" name="calendar-view" rel="list" /><label for="rbList">List</label></li>
        <li><input type="radio" id="rbCloud" name="calendar-view" rel="cloud" /><label for="rbCloud">Tag Cloud</label></li>
    </ul>

    <h2>Campus</h2>
    <ul id="campuses">
 <% foreach (var campus in campuses)
    {
        var id = "campus_" + campus.Qualifier; %>
        <li id="<%= id %>">
            <input type="checkbox" id="cb_<%= id %>" name="campus" data-id="<%= campus.Qualifier %>" />
            <label for="cb_<%= id %>"><%= campus.Value %></label>
        </li><%
    } %>
    </ul>
    <input type="hidden" id="ihTopicAreas" name="ihTopicAreas" value="<%= TopicAreaSetting %>" />
</div>