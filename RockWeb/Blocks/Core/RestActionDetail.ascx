﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RestActionDetail.ascx.cs" Inherits="RockWeb.Blocks.Administration.RestActionDetail" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <h4>
            <asp:Literal ID="ltlRestActionName" runat="server" /></h4>
        <Rock:HiddenFieldWithClass ID="hfUrl" runat="server" CssClass="rest-url" />
        <Rock:HelpBlock runat="server" ID="hbUrlPreview" />
        <Rock:RockTextBox ID="tbPayload" runat="server" Label="Payload" TextMode="MultiLine" Rows="10" />
        <Rock:KeyValueList ID="lstParameterValues" runat="server" Label="Parameter Values" />
        <a id="btnPOST" class="btn btn-action" runat="server" href="javascript:doPost()">POST</a>
        <a id="btnDELETE" class="btn btn-action" runat="server" href="javascript:doDelete()">DELETE</a>
        <a id="btnPUT" class="btn btn-action" runat="server" href="javascript:doPut()">PUT</a>
        <a id="btnGET" class="btn btn-action" runat="server" href="javascript:doGet()">GET</a>

        <h4>Result</h4>
        <pre id="result-data">
        </pre>

        <script>
            function doPost() {
                $.ajax({
                    url: getRestUrl(),
                    type: 'POST',
                    contentType: 'application/json',
                    data: getPayload()
                }).done(handleDone).fail(handleFail);
            }

            function doDelete() {
                $.ajax({
                    url: getRestUrl(),
                    type: 'DELETE'
                }).done(handleDone).fail(handleFail);
            }

            function doPut() {
                $.ajax({
                    url: getRestUrl(),
                    type: 'PUT',
                    contentType: 'application/json',
                    data: getPayload()
                }).done(handleDone).fail(handleFail);
            }

            function doGet() {
                var restUrl = getRestUrl();
                $.ajax({
                    url: restUrl,
                    type: 'GET'
                }).done(handleDone).fail(handleFail);
            }

            function getRestUrl() {
                var restUrl = $('.rest-url').val();
                var $keys = $('.key-value-rows .key-value-key');
                $.each($keys, function (keyIndex) {
                    var key = $keys[keyIndex];
                    var $value = $(key).siblings('.key-value-value').first();
                    restUrl = restUrl.replace('{' + $(key).val() + '}', $value.val());
                });
                return restUrl;
            }

            function handleFail(a, b, c, d) {
                debugger
                $('#result-data').html('FAIL:' + a.status + '<br/>' + a.statusText + '<br/><br/>' + a.responseText);
            }

            function handleDone(resultData) {
                $('#result-data').html(JSON.stringify(resultData, null, 2));
            }

            function getPayload() {
                return $('#<%=tbPayload.ClientID %>').val();
                //return JSON.parse($('#<%=tbPayload.ClientID %>').val());
            }

        </script>

    </ContentTemplate>
</asp:UpdatePanel>
