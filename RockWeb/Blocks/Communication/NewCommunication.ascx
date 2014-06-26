﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewCommunication.ascx.cs" Inherits="RockWeb.Blocks.Communication.NewCommunication" %>

<asp:UpdatePanel ID="upPanel" runat="server">
    <ContentTemplate>
 
        <div class="banner">
            <h1><asp:Literal ID="lTitle" runat="server"></asp:Literal></h1>
            <Rock:HighlightLabel ID="hlStatus" runat="server" />
        </div>

        <asp:Panel ID="pnlEdit" runat="server">

            <asp:HiddenField ID="hfCommunicationId" runat="server" />
            <asp:HiddenField ID="hfChannelId" runat="server" />

            <asp:ValidationSummary ID="ValidationSummary" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />

            <div id="divChannels" runat="server" class="nav navbar nav-pagelist">
                <ul class="nav nav-pills">
                    <asp:Repeater ID="rptChannels" runat="server">
                        <ItemTemplate>
                            <li class='<%# (int)Eval("Key") == ChannelEntityTypeId ? "active" : "" %>'>
                                <asp:LinkButton ID="lbChannel" runat="server" Text='<%# Eval("Value") %>' CommandArgument='<%# Eval("Key") %>' OnClick="lbChannel_Click" CausesValidation="false">
                                </asp:LinkButton>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        
            <Rock:NotificationBox ID="nbInvalidTransport" runat="server" NotificationBoxType="Warning" Dismissable="true" Title="Warning" Visible="false" />

            <div class="row">
                <div class="col-md-12">
                    <div class="pull-right">
                        <Rock:RockCheckBox ID="cbBulk" runat="server" Text="Bulk Communication" CssClass="js-bulk-option"
                          Help="Select this option if you are sending this email to a group of people.  This will include the option for recipients to unsubscribe and will not send the email to any recipients that have already asked to be unsubscribed." />
                    </div>
                </div>
            </div>

            <div class="panel panel-widget recipients">
                <div class="panel-heading clearfix">
                    <div class="control-label pull-left">
                        To: <asp:Literal ID="lNumRecipients" runat="server" />
                    </div> 
                    
                    <div class="pull-right">
                        <Rock:PersonPicker ID="ppAddPerson" runat="server" PersonName="Add Person" OnSelectPerson="ppAddPerson_SelectPerson" />
                    </div>

                    <asp:CustomValidator ID="valRecipients" runat="server" OnServerValidate="valRecipients_ServerValidate" Display="None" ErrorMessage="At least one recipient is required." />
                
                 </div>   
                
                 <div class="panel-body">

                        <ul class="recipients list-unstyled">
                            <asp:Repeater ID="rptRecipients" runat="server" OnItemCommand="rptRecipients_ItemCommand" OnItemDataBound="rptRecipients_ItemDataBound">
                                <ItemTemplate>
                                    <li class='recipient <%# Eval("Status").ToString().ToLower() %>'><asp:Literal id="lRecipientName" runat="server"></asp:Literal> <asp:LinkButton ID="lbRemoveRecipient" runat="server" CommandArgument='<%# Eval("PersonId") %>' CausesValidation="false"><i class="fa fa-times"></i></asp:LinkButton></li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>

                        <div class="pull-right">
                            <asp:LinkButton ID="lbShowAllRecipients" runat="server" CssClass="btn btn-action btn-xs" Text="Show All" OnClick="lbShowAllRecipients_Click" CausesValidation="false"/>
                            <asp:LinkButton ID="lbRemoveAllRecipients" runat="server" Text="Remove All Pending Recipients" CssClass="remove-all-recipients btn btn-action btn-xs" OnClick="lbRemoveAllRecipients_Click" CausesValidation="false"/>
                        </div>

                </div>
            </div>

            <Rock:RockDropDownList ID="ddlTemplate" runat="server" Label="Template" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplate_SelectedIndexChanged" />

            <asp:PlaceHolder ID="phContent" runat="server" />

            <Rock:DateTimePicker ID="dtpFutureSend" runat="server" Label="Delay Send Until" SourceTypeName="Rock.Model.Communication" PropertyName="FutureSendDateTime" />

            <div class="actions">
                <asp:LinkButton ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                <asp:LinkButton ID="btnSave" runat="server" Text="Save as Draft" CssClass="btn btn-link" OnClick="btnSave_Click" />
            </div>

        </asp:Panel>

        <asp:Panel ID="pnlResult" runat="server" Visible="false">
            <Rock:NotificationBox ID="nbResult" runat="server" NotificationBoxType="Success" />
            <br />
            <asp:HyperLink ID="hlViewCommunication" runat="server" Text="View Communication" />
        </asp:Panel>

        <script type="text/javascript">
            Sys.Application.add_load(function () {

                // Set all recipients tooltip
                $('.recipient span').tooltip();

                // Set the display of any recipients that have preference of NoBulkEmail based on if this is a bulk communication
                $('.js-bulk-option').click(function () {
                    if ($(this).is(':checked')) {
                        $('.js-no-bulk-email').addClass('text-danger');
                    } else {
                        $('.js-no-bulk-email').removeClass('text-danger');
                    }
                });
            })
        </script>


    </ContentTemplate>
</asp:UpdatePanel>


