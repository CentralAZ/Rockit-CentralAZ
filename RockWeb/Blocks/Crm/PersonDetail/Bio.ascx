﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Bio.ascx.cs" Inherits="RockWeb.Blocks.Crm.PersonDetail.Bio" %>

<script>
    $(function () {
        $(".photo a").fluidbox();
        $('span.js-email-status').tooltip({ html: true, container: 'body', delay: { show: 100, hide: 100 } });
    });
</script>

<div class="rollover-container">
    <div class="actions rollover-item">
        <asp:LinkButton ID="lbEditPerson" runat="server" CssClass="edit btn btn-action btn-xs" OnClick="lbEditPerson_Click"><i class="fa fa-pencil"></i> Edit Individual</asp:LinkButton>
    </div>

    <div class="row">
	    <div class="col-sm-6">
		    <h1 class="title name"><asp:Literal ID="lName" runat="server" /></h1>
	    </div>
        <div class="col-sm-6 labels">

            <Rock:PersonProfileBadgeList id="blStatus" runat="server" />

            <ul id="ulActions" runat="server" class="nav pull-right">
                <li class="dropdown">
                    <a class="persondetails-actions dropdown-toggle" data-toggle="dropdown" href="#" tabindex="0">
                        <i class="fa fa-cog"></i>
                        <span>Actions</span>
                        <b class="caret"></b>
                    </a>
                    <ul class="dropdown-menu">
                        <asp:Literal ID="lActions" runat="server" />
                    </ul>
                </li>
            </ul>

        </div>
    </div> 

    <div class="row">
	    <div class="col-sm-4">
            <div class="photo">
                <asp:Literal ID="lImage" runat="server" />
                <asp:Panel ID="pnlFollow" runat="server" CssClass="following-status"><i class="fa fa-star"></i></asp:Panel>
            </div>
            <ul class="social-icons list-unstyled margin-t-sm">
                <asp:Repeater ID="rptSocial" runat="server">
                    <ItemTemplate>
                        <li class='icon icon-<%# Eval("name").ToString().ToLower() %>'><a href='<%# Eval("url") %>' target="_blank"><i class ='<%# Eval("icon") %>'></i></a></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>



        <div class="col-sm-8">
            <div class="row">
                <div class="col-md-4">
                    <div class="summary">
                        <Rock:TagList ID="taglPersonTags" runat="server" CssClass="clearfix" />
                        <div class="demographics">
                            <asp:Literal ID="lAge" runat="server" />
                            <asp:Literal ID="lGender" runat="server" /><br />
                            <asp:Literal ID="lMaritalStatus" runat="server" /> 
                            <asp:Literal ID="lAnniversary" runat="server" /><br />
                            <asp:Literal ID="lGraduation" runat="server" />
                            <asp:Literal ID="lGrade" runat="server" />
                        </div>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="personcontact">

                        <ul class="list-unstyled phonenumbers">
                        <asp:Repeater ID="rptPhones" runat="server">
                            <ItemTemplate>
                                <li data-value="<%# Eval("Number") %>"><%# (bool)Eval("IsUnlisted") ? "Unlisted" : FormatPhoneNumber( Eval("CountryCode"), Eval("Number") ) %> <small><%# Eval("NumberTypeValue.Name") %></small></li>
                            </ItemTemplate>
                        </asp:Repeater>
                        </ul>

                        <div class="email">
                            <asp:Literal ID="lEmail" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        
    </div>
</div>


