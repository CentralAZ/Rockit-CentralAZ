﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LineChartDashboardWidget.ascx.cs" Inherits="RockWeb.Blocks.Reporting.Dashboard.LineChartDashboardWidget" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <Rock:NotificationBox ID="nbMetricWarning" runat="server" NotificationBoxType="Warning" Text="Please select a metric in the block settings." />
        <Rock:LineChart ID="lcChart" runat="server" />

    </ContentTemplate>
</asp:UpdatePanel>
