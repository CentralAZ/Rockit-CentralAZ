﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DefinedValueList.ascx.cs" Inherits="RockWeb.Blocks.Core.DefinedValueList" %>

<asp:UpdatePanel ID="upnlSettings" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlContent" runat="server">

            <asp:HiddenField ID="hfDefinedTypeId" runat="server" />

            <asp:Panel ID="pnlList" runat="server" Visible="false">

                <div class="row-fluid">
                    <h4>Values</h4>
                    <asp:Panel ID="pnlValues" runat="server">
                        <Rock:ModalAlert ID="mdGridWarningValues" runat="server" />
                        <Rock:Grid ID="gDefinedValues" runat="server" AllowPaging="true" DisplayType="Full" OnRowSelected="gDefinedValues_Edit" AllowSorting="False">
                            <Columns>
                                <Rock:ReorderField/>
                                <asp:BoundField DataField="Name" HeaderText="Value"/>
                                <asp:BoundField DataField="Description" HeaderText="Description"/>
                            </Columns>
                        </Rock:Grid>

                    </asp:Panel>
                </div>

            </asp:Panel>

            <Rock:ModalDialog ID="modalValue" runat="server" Title="Defined Value" ValidationGroup="Value" >
                <Content>

                <asp:HiddenField ID="hfDefinedValueId" runat="server" />
                <asp:ValidationSummary ID="valSummaryValue" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" ValidationGroup="Value" />
                <legend>
                    <asp:Literal ID="lActionTitleDefinedValue" runat="server" />
                </legend>
                <fieldset>                
                    <div class="row-fluid">
                        <div class="span12">
                            <Rock:DataTextBox ID="tbValueName" runat="server" SourceTypeName="Rock.Model.DefinedValue, Rock" PropertyName="Name" ValidationGroup="Value" Label="Value"/>
                            <Rock:DataTextBox ID="tbValueDescription" runat="server" SourceTypeName="Rock.Model.DefinedValue, Rock" PropertyName="Description" TextMode="MultiLine" Rows="3" ValidationGroup="Value"/>
                        </div>
                    </div>
                    <div class="attributes">
                        <asp:PlaceHolder ID="phDefinedValueAttributes" runat="server" EnableViewState="false"></asp:PlaceHolder>
                    </div>
                </fieldset>

                </Content>
            </Rock:ModalDialog>

        </asp:Panel>
        
    </ContentTemplate>
</asp:UpdatePanel>
