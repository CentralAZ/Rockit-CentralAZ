<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountabilityReportList.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Accountability.AccountabilityReportList" %>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">
        
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-star"></i> Report List</h1>
            </div>
            <div class="panel-body">

                <div class="grid grid-panel">
                    <Rock:Grid ID="gList" runat="server" AllowSorting="true">
                        <Columns>
                            <asp:BoundField DataField="SubmitForDate" HeaderText="Week of" SortExpression="FullName" />
                            <asp:BoundField DataField="Score" HeaderText="Score" SortExpression="Gender" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>
        
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>