<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountabilityGroupDetail.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Accountability.AccountabilityGroupDetail" %>

<script type="text/javascript">
    function clearActiveDialog() {
        $('#<%=hfActiveDialog.ClientID %>').val('');
    }
</script>

<asp:UpdatePanel ID="upnlGroupList" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlDetails" runat="server">
            <asp:HiddenField ID="hfGroupId" runat="server" />

            <div class="panel panel-block">

                <div class="panel-heading clearfix">
                    <h1 class="panel-title pull-left">
                        <asp:Literal ID="lGroupIconHtml" runat="server" />
                        <asp:Literal ID="lReadOnlyTitle" runat="server" />
                    </h1>

                    <div class="panel-labels">
                        <Rock:HighlightLabel ID="hlInactive" runat="server" LabelType="Danger" Text="Inactive" />
                        <Rock:HighlightLabel ID="hlType" runat="server" LabelType="Type" />
                        <Rock:HighlightLabel ID="hlCampus" runat="server" LabelType="Campus" />
                    </div>
                </div>

                <div class="panel-body">
                    <Rock:NotificationBox ID="nbEditModeMessage" runat="server" NotificationBoxType="Info" />
                    <Rock:NotificationBox ID="nbRoleLimitWarning" runat="server" NotificationBoxType="Warning" Heading="Role Limit Warning" />
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />

                    <div id="pnlEditDetails" runat="server">

                        <div class="row">
                            <div class="col-md-6">
                                <Rock:DataTextBox ID="tbName" runat="server" SourceTypeName="Rock.Model.Group, Rock" PropertyName="Name" />
                            </div>
                            <div class="col-md-6">
                                <Rock:RockCheckBox ID="cbIsActive" runat="server" Text="Active" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <Rock:DataTextBox ID="tbDescription" runat="server" SourceTypeName="Rock.Model.Group, Rock" PropertyName="Description" TextMode="MultiLine" Rows="4" />
                            </div>
                        </div>

                        <asp:PlaceHolder ID="phGroupAttributes" runat="server" EnableViewState="false"></asp:PlaceHolder>




                        <div class="actions">
                            <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                            <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" OnClick="btnCancel_Click" />
                        </div>

                    </div>

                    <fieldset id="fieldsetViewDetails" runat="server">

                        <p class="description">
                            <asp:Literal ID="lGroupDescription" runat="server"></asp:Literal>
                        </p>

                        <div class="row">
                            <div class="col-md-6">
                                <asp:Literal ID="lblMainDetails" runat="server" />
                                <asp:PlaceHolder ID="phAttributes" runat="server"></asp:PlaceHolder>
                            </div>
                            <div class="col-md-6 location-maps">
                                <asp:PlaceHolder ID="phMaps" runat="server" />
                            </div>
                        </div>


                        <div class="actions">
                            <asp:LinkButton ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click" CausesValidation="false" />
                            <Rock:ModalAlert ID="mdDeleteWarning" runat="server" />
                            <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-link" OnClick="btnDelete_Click" CausesValidation="false" />
                            <span class="pull-right">
                                <asp:HyperLink ID="hlMap" runat="server" CssClass="btn btn-sm btn-default" ToolTip="Interactive Map"><i class="fa fa-map-marker"></i></asp:HyperLink>
                                <Rock:SecurityButton ID="btnSecurity" runat="server" class="btn btn-sm btn-security" Title="Secure Group" />
                            </span>
                        </div>

                    </fieldset>
                </div>
            </div>

        </asp:Panel>

        <asp:HiddenField ID="hfActiveDialog" runat="server" />



    </ContentTemplate>
</asp:UpdatePanel>
