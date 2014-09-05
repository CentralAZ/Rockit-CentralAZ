<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountabilityQuestionList.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Accountability.AccountabilityQuestionList" %>
<asp:UpdatePanel ID="upnlContent" runat="server">

    <ContentTemplate>
        <asp:HiddenField ID="hfGroupTypeId" runat="server" />
        <div class="banner">
            <h1>Questions</h1>
        </div>

        <Rock:ModalAlert ID="mdGridWarning" runat="server" />

        <Rock:Grid ID="gAccountabilityQuestions" runat="server" AllowSorting="true" OnRowSelected="gAccountabilityQuestions_Edit" TooltipField="Description">
            <Columns>
                <asp:BoundField DataField="ShortForm" HeaderText="Short Form" SortExpression="ShortForm" />
                <asp:BoundField DataField="LongForm" HeaderText="Long Form" SortExpression="LongForm" />
                <Rock:DeleteField OnClick="gAccountabilityQuestions_Delete" />
            </Columns>
        </Rock:Grid>

        <Rock:ModalDialog ID="mdDialog" runat="server" Title="Add Question" OnSaveClick="mdDialog_SaveClick">
            <Content>
                <asp:ValidationSummary ID="valSummaryValue" runat="server" CssClass="alert alert-error" />
                <asp:HiddenField ID="hfQuestionId" runat="server" />
                <fieldset>
                    <Rock:RockTextBox ID="tbMdShortForm" runat="server" Label="Short Form" Required="true" Placeholder="Read Bible" />
                    <Rock:RockTextBox ID="tbMdLongForm" runat="server" Label="Long Form" Required="true" Placeholder="Did you read the bible at least 3 days this week?" />
                </fieldset>
            </Content>
        </Rock:ModalDialog>
    </ContentTemplate>
</asp:UpdatePanel>
