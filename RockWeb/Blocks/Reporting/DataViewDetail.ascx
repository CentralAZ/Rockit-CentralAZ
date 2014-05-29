<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DataViewDetail.ascx.cs" Inherits="RockWeb.Blocks.Reporting.DataViewDetail" %>
<asp:UpdatePanel ID="upDataView" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlDetails" runat="server" Visible="false">
            <asp:HiddenField ID="hfDataViewId" runat="server" />

            <div id="pnlEditDetails" runat="server">

                <div class="banner">
                    <h1>
                        <asp:Literal ID="lActionTitle" runat="server" />
                    </h1>
                </div>

                <asp:ValidationSummary ID="vsDetails" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />

                <fieldset>

                    <div class="row">
                        <div class="col-md-6">
                            <Rock:DataTextBox ID="tbName" runat="server" SourceTypeName="Rock.Model.DataView, Rock" PropertyName="Name" CssClass="" />
                            <Rock:DataTextBox ID="tbDescription" runat="server" SourceTypeName="Rock.Model.DataView, Rock" PropertyName="Description" TextMode="MultiLine" Rows="4" />
                            <Rock:DataDropDownList ID="ddlEntityType" runat="server" SourceTypeName="Rock.Model.DataView, Rock" PropertyName="EntityTypeId"
                                Label="Applies To" DataTextField="FriendlyName" DataValueField="Id" AutoPostBack="true" OnSelectedIndexChanged="ddlEntityType_SelectedIndexChanged" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockDropDownList ID="ddlTransform" runat="server" Label="Post-filter Transformation" />
                            <Rock:CategoryPicker ID="cpCategory" runat="server" EntityTypeName="Rock.Model.DataView" Label="Category" Required="true" />
                        </div>
                    </div>
                </fieldset>

                <asp:PlaceHolder ID="phFilters" runat="server"></asp:PlaceHolder>

                <div class="actions">
                    <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-sm btn-primary" OnClick="btnSave_Click" />
                    <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-link btn-sm" CausesValidation="false" OnClick="btnCancel_Click" />
                    <asp:LinkButton ID="btnPreview" runat="server" Text="Preview" CssClass="btn btn-action btn-sm pull-right" CausesValidation="false" OnClick="btnPreview_Click" />
                </div>

            </div>

            <div id="pnlViewDetails" runat="server">

                <div class="banner">
                    <h1>
                        <asp:Literal ID="lReadOnlyTitle" runat="server" />
                    </h1>
                </div>

                <fieldset>

                    <p class="description">
                        <asp:Literal ID="lDescription" runat="server"></asp:Literal>
                    </p>

                    <div class="row">
                        <div class="col-md-6">
                            <asp:Literal ID="lblMainDetails" runat="server" />
                        </div>
                        <div class="col-md-6">
                            <asp:Literal ID="lFilters" runat="server" />
                        </div>
                    </div>

                    <Rock:NotificationBox ID="nbEditModeMessage" runat="server" NotificationBoxType="Info" />

                    <div class="actions">
                        <asp:LinkButton ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-primary btn-sm" OnClick="btnEdit_Click" />
                        <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-link btn-sm" OnClick="btnDelete_Click" />
                        <Rock:SecurityButton ID="btnSecurity" runat="server" class="btn btn-sm btn-action pull-right" />
                        <Rock:ModalAlert ID="mdDeleteWarning" runat="server" />
                    </div>

                </fieldset>

                <h4>Results</h4>

                <Rock:Grid ID="gReport" runat="server" AllowSorting="true" EmptyDataText="No Results" />

            </div>

            <Rock:ModalDialog ID="modalPreview" runat="server" Title="Preview (top 15 rows )" ValidationGroup="Preview">
                <Content>
                    <div class="modal-body">
                        <div id="preview-modal-scroll-container" class="scroll-container scroll-container-vertical">
                            <div class="scrollbar">
                                <div class="track">
                                    <div class="thumb">
                                        <div class="end"></div>
                                    </div>
                                </div>
                            </div>
                            <div class="viewport">
                                <div class="overview">
                                    <Rock:Grid ID="gPreview" runat="server" AllowSorting="true" EmptyDataText="No Results" ShowActionRow="false" DisplayType="Light" />
                                </div>
                            </div>
                        </div>
                </Content>
            </Rock:ModalDialog>

        </asp:Panel>
        <script type="text/javascript">
            Sys.Application.add_load(function () {
                $('#preview-modal-scroll-container').mouseenter(function () {
                    $('#preview-modal-scroll-container').tinyscrollbar({ size: 150, sizethumb: 20 });

                    $('.scroll-container').on('mouseenter', function () {
                        $('.scroll-container').tinyscrollbar_update('relative');
                    });
                });
            });
        </script>
    </ContentTemplate>
</asp:UpdatePanel>
