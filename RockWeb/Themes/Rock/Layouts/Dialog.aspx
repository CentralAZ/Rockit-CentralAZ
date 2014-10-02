﻿<%@ Page Language="C#" AutoEventWireup="true" Inherits="Rock.Web.UI.DialogPage" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="Rock" %>

<!DOCTYPE html>

<script runat="server">
    
    /// <summary>
    /// Handles the Click event of the btnSave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void btnSave_Click( object sender, EventArgs e )
    {
        base.FireSave( sender, e );
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit( EventArgs e )
    {
        base.OnInit( e );

        lTitle.Text = Request.QueryString["t"] ?? "Title";

        btnSave.Text = Request.QueryString["pb"] ?? "Save";
        btnSave.Visible = btnSave.Text.Trim() != string.Empty;

        btnCancel.Text = Request.QueryString["sb"] ?? "Cancel";
        btnCancel.Visible = btnCancel.Text.Trim() != string.Empty;
        if ( !btnSave.Visible )
        {
            btnCancel.AddCssClass( "btn-primary" );
        }
    }    
    
</script>

<html class="no-js">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=10" />
    <title></title>

    <script src="<%# ResolveRockUrl("~/Scripts/modernizr.js", true) %>" ></script>
    <script src="<%# ResolveRockUrl("~/Scripts/jquery-1.10.2.min.js", true) %>"></script>

    <link rel="stylesheet" href="<%# ResolveRockUrl("~~/Styles/bootstrap.css", true) %>"/>
    <link rel="stylesheet" href="<%# ResolveRockUrl("~~/Styles/theme.css", true) %>" />
    <link rel="stylesheet" href="<%# ResolveRockUrl("~/Styles/developer.css", true) %>" />

    <style>
        html, body {
            height: 100%;
            min-height: 100%;
            width: 100%;
            min-width: 100%;
            margin: 0 0 0 0;
            padding: 0 0 0 0;
            vertical-align: top;
        }
    </style>

</head>

<body id="dialog" class="rock-modal">
    <form id="form1" runat="server">
        <ajaxToolkit:ToolkitScriptManager ID="sManager" runat="server" />
        <asp:UpdatePanel ID="updatePanelDialog" runat="server">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <a id="closeLink" href="#" class="close" onclick="window.parent.Rock.controls.modal.close();">&times;</a>
                        <h3 class="modal-title"><asp:Literal ID="lTitle" runat="server"></asp:Literal></h3>
                        <% if (!String.IsNullOrWhiteSpace(SubTitle)) { %>
                        <small><%= SubTitle %></small>
                        <% } %>
                    </div>

                    <div class="modal-body">
                        <div id="modal-scroll-container" class="scroll-container scroll-container-vertical">
                            <div class="scrollbar">
                                <div class="track">
                                    <div class="thumb">
                                        <div class="end"></div>
                                    </div>
                                </div>
                            </div>
                            <div class="viewport">
                                <div class="overview">
                                    <Rock:Zone Name="Main" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="modal-footer">
                        <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-link" OnClientClick="window.parent.Rock.controls.modal.close();" CausesValidation="false" />
                        <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click " />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>

    <script type="text/javascript">
        Sys.Application.add_load(function () {

            // use setTimeout so that tinyscrollbar will get initialized after renders
            setTimeout(function () {
                $('#modal-scroll-container').tinyscrollbar({ size: 150, sizethumb: 20 });
            }, 0);
            
        });
    </script>
</body>
</html>
