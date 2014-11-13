<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HasselhoffEventList.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Web2.HasselhoffEventList" %>
<%@ Import Namespace="Arena.Portal" %>
<asp:ScriptManagerProxy ID="smpScripts" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/UserControls/Custom/Cccev/Web2/js/eventcalendar.min.js" />
    </Scripts>
</asp:ScriptManagerProxy>
<script runat="server">
    [PageSetting("Event Detail Page", "Reference to Event Details page.", true)]
    public string EventDetailPageSetting { get { return Setting("EventDetailPage", "", true); } }
</script>
<input type="hidden" id="ihListEventDetails" value="<%= EventDetailPageSetting %>" />
<div id="event-list-wrapper" class="event-view">
    <div id="event-featured-list"></div>
    <div id="event-list"></div>
    <div id="event-list-overlay" class="spinner"></div>
</div>