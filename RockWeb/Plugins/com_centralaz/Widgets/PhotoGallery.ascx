<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PhotoGallery.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Widgets.PhotoGallery" %>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <div id="divPhotoDropzone" class="dropzone" runat="server">
            <div class="fallback">
                <input name="file" type="file" multiple />
            </div>
        </div>
        <asp:ListView runat="server" ID="lvImages" OnItemCommand="lvImages_ItemCommand" OnItemDataBound="lvImages_ItemDataBound">
            <ItemTemplate>
                <asp:ImageButton ID="imbItem" runat="server"
                    CommandArgument="<%# Container.DataItem %>"
                    ImageUrl="<%# Container.DataItem %>" Width="320" Height="240"
                    OnCommand="imbItem_Command" />
                <asp:LinkButton ID="lbDelete" runat="server" CommandName="Remove"
                    CommandArgument="<%# Container.DataItem %>" Text="Delete" Visible="false" />
            </ItemTemplate>
            <InsertItemTemplate>
                <p>
                    <asp:Label Text="Please upload an image" runat="server" ID="lblImageUpload" />
                    <asp:FileUpload runat="server" ID="fupImage" />
                    <asp:Button ID="btnUpload" Text="Upload" runat="server" />
                </p>
                <p>
                    <asp:Label Text="" runat="server" ID="lblImageUploadStatus" />
                </p>
            </InsertItemTemplate>
        </asp:ListView>
        <script>
            Sys.Application.add_load(function () {
                $("#<%=divPhotoDropzone.ClientID%>").dropzone({
                    url: "../FileUploader.ashx?rootFolder=" + encodeURIComponent('<%= Rock.Security.Encryption.EncryptString(GetAttributeValue( "ImageFolderPath" )) %>'),

                });
            });
        </script>
    </ContentTemplate>
</asp:UpdatePanel>
