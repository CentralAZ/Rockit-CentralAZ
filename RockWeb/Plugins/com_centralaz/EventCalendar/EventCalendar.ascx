<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EventCalendar.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.EventCalendar.EventCalendar" %>
<%--<script src="<%# ResolveRockUrl("~/Plugins/com_centralaz/Calendar/js/fullcalendar.js") %>"></script>
<script src="<%# ResolveRockUrl("~/Plugins/com_centralaz/Calendar/js/eventcalendar.js") %>"></script>
<script src="<%# ResolveRockUrl("~/Plugins/com_centralaz/Calendar/js/jquery.hoverIntent.js") %>"></script>--%>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <div class="col-md-3">
            <asp:Panel ID="pnlControl" runat="server" CssClass="panel panel-block">

                <div class="panel-body">

                    <div class="alert alert-info">
                        <h2>Event Filters</h2>
                        <Rock:DateRangePicker ID="drpDateRange" runat="server" Label="<h3>Date Range</h3>" Visible="false" />

                        <Rock:RockTextBox ID="tbKeyword" runat="server" CssClass="input-width-md" Label="<h3>Keywords</h3>" />

                        <Rock:RockRadioButtonList ID="rblView" runat="server" Label="<h3>View</h3>" OnSelectedIndexChanged="rblView_ViewChange" AutoPostBack="true">
                            <asp:ListItem Value="Calendar" Text="Calendar" Selected="True" />
                            <asp:ListItem Value="List" Text="List" />
                        </Rock:RockRadioButtonList>

                        <Rock:RockCheckBoxList ID="cblCampus" runat="server" Label="<h3>Campus</h3>" />
                        <Rock:NumberBox ID="numbNumberOfItems" runat="server" Label="<h3>Number of Events</h3>" Visible="false" />
                    </div>

                </div>

            </asp:Panel>
        </div>

        <div class="col-md-9">
            <asp:Panel ID="pnlCalendar" runat="server" CssClass="panel panel-block">

                <div class="panel-body">

                    <div class="alert alert-info">
                        <asp:Calendar ID="calBaptism" runat="server" DayNameFormat="FirstLetter" SelectionMode="Day" BorderColor="#999999"
                            TitleStyle-BackColor="#e5e5e5" NextPrevStyle-ForeColor="#333333" FirstDayOfWeek="Monday" Width="100%" Height="100%" CssClass="calendar">
                            <DayStyle CssClass="calendar-day" />
                            <TodayDayStyle CssClass="calendar-today" />
                            <SelectedDayStyle CssClass="calendar-selected" />
                            <OtherMonthDayStyle CssClass="calendar-last-month" ForeColor="#999999" />
                            <DayHeaderStyle CssClass="calendar-day-header" />
                            <NextPrevStyle CssClass="calendar-next-prev" ForeColor="#777777" />
                            <TitleStyle CssClass="calendar-title" />
                        </asp:Calendar>
                        <asp:PlaceHolder ID="phCalendar" runat="server" />
                        <%--                        <input type="hidden" id="ihCalendarEventDetails" />
                        <div id="event-calendar" class="event-view"></div>
                        <div id="calendar-overlay" class="spinner"></div>--%>
                    </div>

                </div>

            </asp:Panel>

            <asp:Panel ID="pnlList" runat="server" CssClass="panel panel-block" Visible="false">

                <div class="panel-body">

                    <div class="alert alert-info">
                        <input type="hidden" id="ihListEventDetails" />
                        <asp:PlaceHolder ID="phList" runat="server" />
                        <%--                        <div id="event-list-wrapper" class="event-view">
                            <div id="event-featured-list"></div>
                            <div id="event-list"></div>
                            <div id="event-list-overlay" class="spinner"></div>--%>
                        <%--</div>--%>
                    </div>
                </div>

            </asp:Panel>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
