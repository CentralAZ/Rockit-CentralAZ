﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Components.ascx.cs" Inherits="RockWeb.Blocks.Core.Components" %>

<script type="text/javascript">
    function clearActiveDialog() {
        $('#<%=hfActiveDialog.ClientID %>').val('');
    }
</script>

<asp:UpdatePanel runat="server">
    <ContentTemplate>

        <Rock:ModalAlert ID="mdAlert" runat="server" />

        <Rock:GridFilter ID="rFilter" runat="server">
            <Rock:RockTextBox ID="tbName" runat="server" Label="Name" />
            <Rock:RockTextBox ID="tbDescription" runat="server" Label="Description" />
            <Rock:RockRadioButtonList ID="rblActive" runat="server" Label="Active" RepeatDirection="Horizontal">
                <asp:ListItem Value="" Text="All" />
                <asp:ListItem Value="Yes" Text="Yes" />
                <asp:ListItem Value="No" Text="No" />
            </Rock:RockRadioButtonList>
        </Rock:GridFilter>

        <Rock:Grid ID="rGrid" runat="server" EmptyDataText="No Components Found" OnRowSelected="rGrid_Edit">
            <Columns>
                <Rock:ReorderField />
                <asp:BoundField DataField="Name" HeaderText="Name" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <Rock:BoolField DataField="IsActive" HeaderText="Active" />
                <asp:TemplateField>
                    <HeaderStyle CssClass="span1" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <a id="aSecure" runat="server" class="btn btn-sm btn-security" height="500px"><i class="fa fa-lock"></i></a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </Rock:Grid>

        <asp:HiddenField ID="hfActiveDialog" runat="server" />

        <Rock:ModalDialog ID="mdEditComponent" runat="server" Title="Attribute" OnCancelScript="clearActiveDialog();">
            <Content>

                <asp:ValidationSummary runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                <fieldset>
                    <asp:PlaceHolder ID="phProperties" runat="server" EnableViewState="false" ></asp:PlaceHolder>
                </fieldset>

            </Content>
        </Rock:ModalDialog>


    </ContentTemplate>
</asp:UpdatePanel>
