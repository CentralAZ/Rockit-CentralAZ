<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BaptismAddBaptism.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Baptism.BaptismAddBaptism" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title">Add Baptism</h1>

            </div>
            <div class="panel-body">
                <Rock:DateTimePicker ID="dtpBaptismDate" runat="server" Label="Date" />
                <Rock:NotificationBox ID="nbErrorWarning" runat="server" NotificationBoxType="Danger" />
                <Rock:PersonPicker ID="ppBaptizee" runat="server" Label="Person being baptized" />
                <Rock:PersonPicker ID="ppBaptizer1" runat="server" Label="Primary Baptizer" />
                <Rock:PersonPicker ID="ppBaptizer2" runat="server" Label="Secondary Baptizer (Optional)" />
                <Rock:PersonPicker ID="ppApprover" runat="server" Label="Approved By" />
                <Rock:RockCheckBox ID="cbIsConfirmed" runat="server" Label="Confirmed" />
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_OnClick" />
                <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_OnClick" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_OnClick" />
            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
