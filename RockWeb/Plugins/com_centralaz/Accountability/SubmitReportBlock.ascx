<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SubmitReportBlock.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Accountability.SubmitReportBlock" %>
<asp:UpdatePanel ID="upnlContent" runat="server">

    <ContentTemplate>
        <asp:Panel ID="pnlContent" runat="server">
            <asp:HiddenField ID="hfGroupTypeId" runat="server" />
            <div class="panel panel-block">
                <div class="panel-body">
                    <h1>
                        <asp:Literal ID="lblStatusMessage" runat="server">

                        </asp:Literal>
                        <span class="pull-right">
                        <asp:LinkButton ID="btnSubmitReport" runat="server" Text="Submit Report" CssClass="btn btn-lg btn-primary" OnClick="btnSubmit_Click" CausesValidation="false">

                        </asp:LinkButton>
                    </span>
                    </h1>
                    
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
+
