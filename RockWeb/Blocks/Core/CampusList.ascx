﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CampusList.ascx.cs" Inherits="RockWeb.Blocks.Core.Campuses" %>

<asp:UpdatePanel ID="upCampusList" runat="server">
    <ContentTemplate>
        <Rock:ModalAlert ID="mdGridWarning" runat="server" />
        <Rock:Grid ID="gCampuses" runat="server" AllowSorting="true" OnRowSelected="gCampuses_Edit">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="ShortCode" HeaderText="Short Code" SortExpression="ShortCode" />
                <asp:BoundField DataField="PhoneNumber" HeaderText="Phone Number" SortExpression="PhoneNumber" />
                <asp:BoundField DataField="LeaderPersonAlias.Person.FullName" HeaderText="Campus Leader" SortExpression="LeaderPersonAlias.Person.FullName" />
                <Rock:BoolField DataField="IsActive" HeaderText="Active" />
                <Rock:DeleteField OnClick="gCampuses_Delete" />
            </Columns>
        </Rock:Grid>
    </ContentTemplate>
</asp:UpdatePanel>

