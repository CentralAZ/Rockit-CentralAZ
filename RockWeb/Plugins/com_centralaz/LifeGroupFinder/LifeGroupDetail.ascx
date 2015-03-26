﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LifeGroupDetail.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.LifeGroupFinder.LifeGroupDetail" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <Rock:NotificationBox ID="nbNotice" runat="server" Visible="false" NotificationBoxType="Danger" />

        <asp:Panel ID="pnlView" runat="server" Visible="true">
            <div class="row">
                <h2>
                    <center>
                    <Rock:RockLiteral ID="lGroupName" runat="server"/>
                    </center>
                </h2>
            </div>
            <hr />
            <div class="row">
                <div class="pull-left">
                    <asp:LinkButton ID="lbGoBack" runat="server" Text="Back" OnClick="lbGoBack_Click" />
                </div>
                <div class="pull-right">
                    <Rock:RockLiteral ID="lSignin" runat="server" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <center>
                    <asp:Literal ID="lImage" runat="server" />
                    </center>
                </div>
                <div class="col-md-4">
                    <div class="row">
                        <center>
                            <asp:LinkButton ID="lbRegister" runat="server" Text="Sign up!" CssClass="btn btn-primary" OnClick="lbRegister_Click" />
                        </center>
                    </div>
                    <div class="row">
                        <center>                
                            <asp:LinkButton ID="lbEmail" runat="server" Text="Email" OnClick="lbEmail_Click" />
                        </center>
                    </div>
                    <div class="row">
                        <center>                
                            <asp:LinkButton ID="lbPhone" runat="server" OnClick="lbPhone_Click" />
                        </center>
                    </div>
                </div>
                <div class="col-md-4 location-maps">
                    <center>
                    <asp:PlaceHolder ID="phMaps" runat="server" EnableViewState="true" />
                    </center>
                </div>
            </div>
            <div class="row">
                <asp:Literal ID="lDescription" runat="server" />
            </div>
            <hr />
            <asp:Literal ID="lLavaOverview" runat="server" />
            <asp:Literal ID="lLavaOutputDebug" runat="server" />

            <asp:ValidationSummary ID="valSummary" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />

            <div class="row">
                <asp:Panel ID="pnlSignup" runat="server" Visible="true">
                    <div class="row">
                        <div class="col-md-6">
                            We treat Life Groups like family, and to us family uses real names.
                        </div>
                        <div class="col-md-6">
                            <Rock:RockTextBox ID="tbFirstName" runat="server" Label="First Name" Required="true"></Rock:RockTextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            What is yours?                       
                        </div>
                        <div class="col-md-6">
                            <Rock:RockTextBox ID="tbLastName" runat="server" Label="Last Name" Required="true"></Rock:RockTextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            Please provide your email and phone.                       
                        </div>
                        <div class="col-md-6">
                            <Rock:PhoneNumberBox ID="pnHome" runat="server" Label="Home Phone" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            To connect you to your selected group we would like to contact you.                       
                        </div>
                        <div class="col-md-6">
                            <Rock:EmailBox ID="tbEmail" runat="server" Label="Email" Required="true"></Rock:EmailBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            Will some one else be attending with you, such as a spouse, friend, or neighbor?                       
                        </div>
                        <div class="col-md-6">
                            <Rock:RockCheckBox ID="cbSecondSignup" runat="server" Text="Yes" OnCheckedChanged="cbSecondSignup_CheckedChanged" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <div class="row">
                <asp:Panel ID="pnlSecondSignup" runat="server" Visible="false">
                    <div class="row">
                        <div class="col-md-6">
                        </div>
                        <div class="col-md-6">
                            <Rock:RockTextBox ID="tbSecondFirstName" runat="server" Label="First Name" Required="true"></Rock:RockTextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                        </div>
                        <div class="col-md-6">
                            <Rock:RockTextBox ID="tbSecondLastName" runat="server" Label="Last Name" Required="true"></Rock:RockTextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                        </div>
                        <div class="col-md-6">
                            <Rock:PhoneNumberBox ID="pnSecondHome" runat="server" Label="Home Phone" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                        </div>
                        <div class="col-md-6">
                            <Rock:EmailBox ID="tbSecondEmail" runat="server" Label="Email" Required="true"></Rock:EmailBox>
                        </div>
                    </div>
                </asp:Panel>
            </div>

            <div class="pull-right">
                <div class="actions">
                    <asp:LinkButton ID="btnRegister" runat="server" Text="Sign up!" CssClass="btn btn-primary" OnClick="btnRegister_Click" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlResult" runat="server" Visible="true">
            <center>
            <h3>Success!</h3>
            <hr />
            <h3>
                Thank you for submitting your info!
                <br />
                You should recieve a confirmation email shortly.
            </h3>
            </center>
            <div class="col-md-6">
                <div class="row">
                    This is the information that was submitted, is this correct?
                </div>
                <div class="row">
                    <asp:LinkButton ID="lbChange" runat="server" Text="Nope, it needs changes" CssClass="btn btn-primary" OnClick="lbChange_Click" />
                </div>
                <div class="row">
                    <asp:LinkButton ID="lbExit" runat="server" Text="Looks perfect!" CssClass="btn btn-primary" OnClick="lbExit_Click" />
                </div>
            </div>
            <div class="col-md-6">
                <asp:Panel runat="server">
                    <div class="row">
                        <Rock:RockTextBox ID="tbResultFirstName" runat="server" Enabled="false"></Rock:RockTextBox>
                    </div>
                    <div class="row">
                        <Rock:RockTextBox ID="tbResultLastName" runat="server" Enabled="false"></Rock:RockTextBox>
                    </div>
                    <div class="row">
                        <Rock:EmailBox ID="tbResultEmail" runat="server" Enabled="false"></Rock:EmailBox>
                    </div>
                    <div class="row">
                        <Rock:PhoneNumberBox ID="pnResultHome" runat="server" Enabled="false" />
                    </div>
                </asp:Panel>
                <br />
                <asp:Panel ID="pnlSecondResult" runat="server" Visible="false">
                    <div class="row">
                        <Rock:RockTextBox ID="tbSecondResultFirstName" runat="server" Enabled="false"></Rock:RockTextBox>
                    </div>
                    <div class="row">
                        <Rock:RockTextBox ID="tbSecondResultLastName" runat="server" Enabled="false"></Rock:RockTextBox>
                    </div>
                    <div class="row">
                        <Rock:EmailBox ID="tbSecondResultEmail" runat="server" Enabled="false"></Rock:EmailBox>
                    </div>
                    <div class="row">
                        <Rock:PhoneNumberBox ID="pnSecondResultHome" runat="server" Enabled="false" />
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
