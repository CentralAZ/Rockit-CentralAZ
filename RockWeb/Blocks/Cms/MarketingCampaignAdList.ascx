<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MarketingCampaignAdList.ascx.cs" Inherits="RockWeb.Blocks.Cms.MarketingCampaignAdList" %>

<asp:UpdatePanel ID="upList" runat="server">
    <ContentTemplate>
        <asp:Literal ID="lTitle" runat="server">
            <h4>Ads</h4>
        </asp:Literal>
        <Rock:GridFilter ID="rFilter" runat="server" >
            <%-- Approval Status, Priority Range, Ad Type, Date Range --%>
            <Rock:RockDropDownList ID="ddlApprovalStatus" runat="server" Label="Approval Status" />
            <Rock:NumberRangeEditor ID="pPriorityRange" runat="server" Label="Priority Range" />
            <Rock:RockDropDownList ID="ddlAdType" runat="server" Label="Ad Type" />
            <Rock:DateRangePicker ID="pDateRange" runat="server" Label="Date Range" />
        </Rock:GridFilter>
        <div id="pnlMarketingCampaignAds" runat="server">
            <asp:HiddenField ID="hfMarketingCampaignId" runat="server" />
            <Rock:ModalAlert ID="mdGridWarning" runat="server" />
            <Rock:Grid ID="gMarketingCampaignAds" runat="server" DisplayType="Full" OnRowSelected="gMarketingCampaignAds_Edit" AllowSorting="true">
                <Columns>
                    <asp:BoundField DataField="MarketingCampaign.Title" HeaderText="Campaign" SortExpression="MarketingCampaign.Title" />
                    <asp:BoundField DataField="MarketingCampaignAdType.Name" HeaderText="Ad Type" SortExpression="MarketingCampaignAdType.Name" />
                    <Rock:DateField DataField="StartDate" HeaderText="Date" SortExpression="StartDate" />
                    <Rock:EnumField DataField="MarketingCampaignAdStatus" HeaderText="Approval Status" SortExpression="MarketingCampaignAdStatus" />
                    <asp:BoundField DataField="Priority" HeaderText="Priority" SortExpression="Priority" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                    <Rock:DeleteField OnClick="gMarketingCampaignAds_Delete" />
                </Columns>
            </Rock:Grid>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
