﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AttendanceHistoryList.ascx.cs" Inherits="RockWeb.Blocks.Checkin.AttendanceHistoryList" %>

<asp:UpdatePanel ID="upAttendanceHistory" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlContent" CssClass="panel panel-block" runat="server">

            <Rock:ModalAlert ID="mdGridWarning" runat="server" />

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-check-square"></i> Attendance History</h1>
            </div>
            <div class="panel-body">

                <div class="grid grid-panel">
                    <Rock:GridFilter ID="rFilter" runat="server">
                        <Rock:DateRangePicker ID="drpDates" runat="server" Label="Date Range" />
                        <Rock:RockDropDownList ID="ddlPeople" runat="server" Label="Person" />
                        <Rock:RockDropDownList ID="ddlGroups" runat="server" Label="Group" />
                        <Rock:RockDropDownList ID="ddlSchedules" runat="server" Label="Schedule" />
                    </Rock:GridFilter>
                    <Rock:Grid ID="gHistory" runat="server" DisplayType="Full" AllowSorting="true" EmptyDataText="No Attendance Found" OnRowDataBound="gHistory_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="Location" HeaderText="Location" SortExpression="Location" />
                            <asp:BoundField DataField="Schedule" HeaderText="Schedule" SortExpression="Schedule" />
                            <asp:BoundField DataField="FullName" HeaderText="Person" SortExpression="FullName" />
                            <asp:BoundField DataField="Group" HeaderText="Group" SortExpression="Group" />
                            <asp:BoundField DataField="StartDateTime" HeaderText="Start Date Time" SortExpression="StartDateTime" />
                            <asp:BoundField DataField="EndDateTime" HeaderText="End Date Time" SortExpression="EndDateTime" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>              

        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
