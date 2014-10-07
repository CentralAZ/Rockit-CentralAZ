﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditPerson.ascx.cs" Inherits="RockWeb.Blocks.Crm.PersonDetail.EditPerson" %>


<%--
    ******************************************************************************************************************************
    * NOTE: The Security/EditMyAccount.ascx block has very similiar functionality.  If updating this block, make sure to check
    * that block also.  It may need the same updates.
    ******************************************************************************************************************************
--%>


<asp:UpdatePanel ID="upEditPerson" runat="server">
    <ContentTemplate>
        
        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> <asp:Literal ID="lTitle" runat="server" /></h1>
            </div>

            <div class="panel-body">
                <asp:ValidationSummary ID="valValidation" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />

                <div class="row">

                    <div class="col-md-3">
                        <Rock:ImageEditor ID="imgPhoto" runat="server" Label="Photo" BinaryFileTypeGuid="03BD8476-8A9F-4078-B628-5B538F967AFC" />

                        <fieldset>
                            <Rock:RockDropDownList ID="ddlRecordStatus" runat="server" Label="Record Status" AutoPostBack="true" OnSelectedIndexChanged="ddlRecordStatus_SelectedIndexChanged" />
                            <Rock:RockDropDownList ID="ddlReason" runat="server" Label="Reason" Visible="false"></Rock:RockDropDownList>
                        </fieldset>
                    </div>

                    <div class="col-md-9">

                        <fieldset>
                            <Rock:RockDropDownList ID="ddlTitle" runat="server" CssClass="input-width-md" Label="Title"/>
                            <Rock:DataTextBox ID="tbFirstName" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="FirstName" />
                            <Rock:DataTextBox ID="tbNickName" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="NickName" />
                            <Rock:DataTextBox ID="tbMiddleName" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="MiddleName" />
                            <Rock:DataTextBox ID="tbLastName" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="LastName" />
                            <Rock:RockDropDownList ID="ddlSuffix" CssClass="input-width-md" runat="server" Label="Suffix"/>
                            <Rock:BirthdayPicker ID="bpBirthDay" runat="server" Label="Birthday" />

                            <div class="row">
                                <div class="col-sm-3">
                                    <Rock:RockDropDownList ID="ddlGrade" runat="server" CssClass="input-width-md" Label="Grade"/>
                                </div>
                                <div class="col-sm-3">
                                    <Rock:YearPicker ID="ypGraduation" runat="server" Label="Graduation Year" Help="High School Graduation Year." />
                                </div>
                                <div class="col-sm-6">
                                </div>
                            </div>
                            <Rock:DatePicker ID="dpAnniversaryDate" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="AnniversaryDate" StartView="decade" />

                            <Rock:RockRadioButtonList ID="rblGender" runat="server" RepeatDirection="Horizontal" Label="Gender">
                                <asp:ListItem Text="Male" Value="Male" />
                                <asp:ListItem Text="Female" Value="Female" />
                                <asp:ListItem Text="Unknown" Value="Unknown" />
                            </Rock:RockRadioButtonList>

                            <Rock:RockRadioButtonList ID="rblMaritalStatus" runat="server" RepeatDirection="Horizontal" Label="Marital Status" />
                            <Rock:RockRadioButtonList ID="rblConnectionStatus" runat="server" Label="Connection Status" />

                        </fieldset>

                        <fieldset>
                            <legend>Contact Info</legend>

                            <div class="form-horizontal">
                                <asp:Repeater ID="rContactInfo" runat="server">
                                    <ItemTemplate>
                                        <div class="form-group">
                                            <div class="control-label col-sm-2"><%# Eval("NumberTypeValue.Value")  %></div>
                                            <div class="controls col-sm-10">
                                                <div class="row">
                                                    <div class="col-sm-7">
                                                        <asp:HiddenField ID="hfPhoneType" runat="server" Value='<%# Eval("NumberTypeValueId")  %>' />
                                                        <Rock:PhoneNumberBox ID="pnbPhone" runat="server" CountryCode='<%# Eval("CountryCode") %>' Number='<%# Eval("NumberFormatted")  %>' />
                                                    </div>    
                                                    <div class="col-sm-5">
                                                        <div class="row">
                                                            <div class="col-xs-6">
                                                                <asp:CheckBox ID="cbSms" runat="server" Text="sms" Checked='<%# (bool)Eval("IsMessagingEnabled") %>' CssClass="js-sms-number" />
                                                            </div>
                                                            <div class="col-xs-6">
                                                                <asp:CheckBox ID="cbUnlisted" runat="server" Text="unlisted" Checked='<%# (bool)Eval("IsUnlisted") %>' />
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <div class="row">
                                <div class="col-sm-6">
                                    <Rock:DataTextBox ID="tbEmail" PrependText="<i class='fa fa-envelope'></i>" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="Email" />
                                </div>
                                <div class="col-sm-3">
                                    <Rock:RockCheckBox ID="cbIsEmailActive" runat="server" Label="Email Status" Text="Is Active" />
                                </div>
                            </div>

                            <Rock:RockRadioButtonList ID="rblEmailPreference" runat="server" RepeatDirection="Horizontal" Label="Email Preference">
                                <asp:ListItem Text="Email Allowed" Value="EmailAllowed" />
                                <asp:ListItem Text="No Mass Emails" Value="NoMassEmails" />
                                <asp:ListItem Text="Do Not Email" Value="DoNotEmail" />
                            </Rock:RockRadioButtonList>

                        </fieldset>

                        <fieldset>
                            <legend>Contribution Info</legend>
                            <Rock:RockDropDownList ID="ddlGivingGroup" runat="server" Label="Combine Giving With" Help="The family that this person's gifts should be combined with for contribution statements and reporting.  If left blank, their contributions will not be grouped with their family" /> 
                        </fieldset>

                        <div class="actions">
                            <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                            <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" OnClick="btnCancel_Click" />
                        </div>

                </div>

            </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
