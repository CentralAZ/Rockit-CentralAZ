<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LifeGroupSearch.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.LifeGroupFinder.LifeGroupSearch" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">
            <div class="panel-body">
                <h2>
                    <center>Finding a Life Group</center>
                </h2>
                </br>
                    <h4>
                        <center>Only two things are needed to get started</center>
                    </h4>

                <div class="row">
                    <div class="col-md-4">
                        Please select primary campus
                    </div>
                    <div class="col-md-4">
                        <Rock:RockDropDownList ID="ddlCampus" runat="server" />
                    </div>
                    <div class="col-md-4">
                        View a
                        <asp:LinkButton ID="lbMap" runat="server" Text="map" OnClick="lbMap_Click" />
                        from all groups on this campus.                   
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="pull-right">
                            <asp:Panel ID="pnlLogin" runat="server">
                                <asp:LinkButton ID='lbLogin' runat='server' Text='Sign in' OnClick='lbLogin_Click' /> to autocomplete forms. 
                            </asp:Panel>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-10">
                            <Rock:AddressControl ID="acAddress" runat="server" Required="true" RequiredErrorMessage="Your Address is Required" />
                        </div>
                        <div class="col-md-2">
                            <asp:LinkButton ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                        </div>
                    </div>

                    <div class="col-md-12">
                        <div class="pull-right">
                            <asp:LinkButton ID="lbSecurity" runat="server" Text="Why" OnClick="lbSecurity_Click" />
                            your information is safe.
                        </div>
                    </div>
                </div>
                <asp:PlaceHolder ID="phSearchFilter" runat="server" />
            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
