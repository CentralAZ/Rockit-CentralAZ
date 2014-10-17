<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BaptismAddBlackoutDate.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Baptism.BaptismAddBlackoutDate" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title">Add Blackout Date</h1>
            </div>

            <div class="panel-body">
                <Rock:NotificationBox ID="nbNotification" runat="server" NotificationBoxType="Danger" />
                <Rock:DatePicker ID="dpBlackOutDate" runat="server" Label="Blackout Date" />
                <Rock:RockTextBox ID="tbDescription" Label="Description" TextMode="MultiLine" Rows="5" runat="server" />
                <asp:Button ID="btnSave" Text="Save" runat="server" OnClick="btnSave_OnClick" />
                <asp:Button ID="btnDelete" Text="Delete" runat="server" OnClick="btnDelete_OnClick" />
                <asp:Button ID="btnCancel" Text="Cancel" runat="server" OnClick="btnCancel_OnClick" />

            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
