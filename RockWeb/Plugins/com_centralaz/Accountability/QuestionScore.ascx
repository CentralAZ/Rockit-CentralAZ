<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuestionScore.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Accountability.QuestionScore" %>
<asp:UpdatePanel ID="upnlContent" runat="server">

    <ContentTemplate>
        <asp:Panel ID="pnlContent" runat="server">
            <div class="panel panel-block">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-4">
                            <center>
                                <h4>
                                Overall Score
                            </h4>
                            </center>
                        </div>
                        <div class="col-md-4">
                            <center>
                                <h4>
                                Weak Spot
                            </h4>
                            </center>
                        </div>
                        <div class="col-md-4">
                            <center>
                                <h4>
                                Strong Spot
                            </h4>
                            </center>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-2 col-md-offset-1">
                            <div class="alert alert-info">
                                <center>
                                    <h1>
                                <asp:Literal ID="lblOverallScore" runat="server" />

                                    </h1>
                            </center>
                            </div>
                        </div>
                        <div class="col-md-2 col-md-offset-2">
                            <div class="alert alert-warning">
                                <center>
                                    <h1>
                                <asp:Literal ID="lblWeakScore" runat="server" />

                                    </h1>
                                <asp:Literal ID="lblWeakScoreQuestion" runat="server" />
                            </center>
                            </div>
                        </div>
                        <div class="col-md-2 col-md-offset-2">
                            <div class="alert alert-success">
                                <center>
                                    <h1>
                                <asp:Literal ID="lblStrongScore" runat="server" />

                                    </h1>
                                <asp:Literal ID="lblStrongScoreQuestion" runat="server" />
                            </center>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
