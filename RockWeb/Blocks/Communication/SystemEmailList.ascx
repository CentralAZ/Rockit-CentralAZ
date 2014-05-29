<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SystemEmailList.ascx.cs" Inherits="RockWeb.Blocks.Communication.SystemEmailList" %>

<asp:UpdatePanel ID="upSettings" runat="server">
    <ContentTemplate>

        <Rock:NotificationBox ID="nbMessage" runat="server" Title="Error" NotificationBoxType="Danger" Visible="false" />

        <Rock:GridFilter ID="rFilter" runat="server">
            <Rock:RockDropDownList ID="ddlCategoryFilter" runat="server" Label="Category" />
        </Rock:GridFilter>
        <Rock:Grid ID="gEmailTemplates" runat="server" AllowSorting="true" OnRowSelected="gEmailTemplates_Edit">
            <Columns>
                <asp:BoundField DataField="Category" HeaderText="Category" SortExpression="Category" />
                <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
                <asp:BoundField DataField="FromName" HeaderText="From Name" SortExpression="FromName" />
                <asp:BoundField DataField="From" HeaderText="From Address" SortExpression="From" />
                <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" />
                <Rock:DeleteField OnClick="gEmailTemplates_Delete" />
            </Columns>
        </Rock:Grid>

    </ContentTemplate>
</asp:UpdatePanel>
