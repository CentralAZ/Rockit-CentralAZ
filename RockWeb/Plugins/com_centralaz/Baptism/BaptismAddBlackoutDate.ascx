<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BaptismAddBlackoutDate.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Baptism.BaptismAddBlackoutDate" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title">Add Blackout Date</h1>
            </div>
            <div class="panel-body">
                <Rock:DatePicker ID="dpBlackOutDate" runat="server" Label="Blackout Date" />
                <Rock:RockTextBox ID="tbDescription" Label="Description" TextMode="MultiLine" Rows="5" runat="server" />
                <Rock:BootstrapButton ID="btnSave" Text="Save" runat="server" />
                <Rock:BootstrapButton ID="btnDelete" Text="Delete" runat="server" />
                <Rock:BootstrapButton ID="btnEdit" Text="Edit" runat="server" />


            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
