<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountabilityGroupMemberDetail.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Accountability.AccountabilityGroupMemberDetail" %>
<asp:UpdatePanel ID="upnlContent" runat="server">

    <ContentTemplate>

        <asp:Panel ID="pnlContent" runat="server">

            <asp:HiddenField ID="hfGroupId" runat="server" />
            <asp:HiddenField ID="hfGroupMemberId" runat="server" />
            <asp:HiddenField ID="hfGroupTypeId" runat="server" />

            <div class="panel panel-block">
                <div class="panel-body">

                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                    <Rock:NotificationBox ID="nbErrorMessage" runat="server" NotificationBoxType="Danger" />

                    <div id="pnlViewDetails" runat="server">
                        <div class="row">
                            <div class="col-md-6">
                                <h1 class="title name">
                                    <asp:Literal ID="lName" runat="server" /></h1>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-2">
                                    <asp:Literal ID="lImage" runat="server" />
                            </div>
                            <div class="col-md-10">
                                <Rock:RockRadioButtonList ID="rblStatus" runat="server" Label="Status" RepeatDirection="Horizontal" Enabled="false" />
                                <h5><b>Member Start Date</b></h5>
                                <asp:Literal ID="lblStartDate" runat="server" />
                            </div>

                        </div>
                       
                        <div class="actions margin-v-md">
                            <asp:LinkButton ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click" CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
