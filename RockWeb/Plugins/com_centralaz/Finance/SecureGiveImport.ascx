<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SecureGiveImport.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Finance.SecureGiveImport" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title">Import SecureGive</h1>
            </div>
            <div class="panel-body">

                <p>
                    <asp:FileUpload runat="server" ID="fuImport" CssClass="input-small" />
                </p>

                <p>
                    <asp:LinkButton runat="server" ID="lbImport" CssClass="btn btn-default btn-sm" OnClick="lbImport_Click">
                                    <i class="fa fa-arrow-up"></i> Import
                    </asp:LinkButton>
                </p>

                <Rock:NotificationBox ID="nbMessage" runat="server" NotificationBoxType="Danger" />

                <Rock:NotificationBox ID="nbBatch" runat="server" NotificationBoxType="Success" />
                <div class="grid grid-panel">
                    <Rock:Grid ID="gContributions" runat="server" AllowSorting="true" Visible="false" OnRowSelected="gContributions_View">
                        <Columns>
                            <asp:BoundField DataField="FinancialTransactionDetail.Transaction.Id" HeaderText="Transaction Id" SortExpression="TransactionId" />
                            <asp:BoundField DataField="FinancialTransactionDetail.Transaction.ProcessedDateTime" HeaderText="Transaction Date" SortExpression="TransactionDate" />
                            <asp:BoundField DataField="FinancialTransactionDetail.Transaction.AuthorizedPersonAlias.Person.FullName" HeaderText="Full Name" SortExpression="FullName" />
                            <asp:BoundField DataField="FinancialTransactionDetail.TransactionTypeValue" HeaderText="Transaction Type" SortExpression="TransactionType" />
                            <asp:BoundField DataField="FinancialTransactionDetail.Account" HeaderText="Fund Name" SortExpression="FundName" />
                            <asp:BoundField DataField="FinancialTransactionDetail.Amount" HeaderText="Amount" SortExpression="Amount" />

                        </Columns>
                    </Rock:Grid>
                </div>
            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
