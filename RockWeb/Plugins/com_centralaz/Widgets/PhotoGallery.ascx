<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PhotoGallery.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Widgets.PhotoGallery" %>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <div id="divPhotoDropzone" class="dropzone" runat="server">
            <div class="fallback">
                <input name="file" type="file" multiple />
            </div>
        </div>
        <Rock:BootstrapButton ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-primary" />
        <script>
            Sys.Application.add_load(function () {

                $("#<%=divPhotoDropzone.ClientID%>").dropzone({
                    url: "/file/post",
                    // Prevents Dropzone from uploading dropped files immediately
                    autoProcessQueue: false,
                    maxFiles: 5,
                    init: function () {
                        var submitButton = document.getElementById("#<%=divPhotoDropzone.ClientID%>");
                        myDropzone = this;
                        submitButton.addEventListener("click", function () {
                            myDropzone.processQueue();  // Tell Dropzone to process all queued files.
                        });
                        // to handle the added file event
                        this.on("addedfile", function (file) {
                            var removeButton = Dropzone.createElement("<button> Remove file </button>");
                            // Capture the Dropzone instance as closure.
                            var _this = this;

                            // Listen to the click event
                            removeButton.addEventListener("click", function (e) {
                                // Make sure the button click doesn't submit the form:
                                e.preventDefault();
                                e.stopPropagation();

                                // Remove the file preview.
                                _this.removeFile(file);
                                // If you want to the delete the file on the server as well,
                                // you can do the AJAX request here.
                            });
                            file.previewElement.appendChild(removeButton);
                        });
                        this.on("maxfilesexceeded", function (file) {
                            this.removeFile(file);
                        });
                    }
                });

                $("#<%=divPhotoDropzone.ClientID%>").sortable({
                    items: '.dz-preview',
                    cursor: 'move',
                    opacity: 0.5,
                    containment: '#image-dropzone',
                    distance: 20,
                    tolerance: 'pointer'
                });
            });
        </script>
    </ContentTemplate>
</asp:UpdatePanel>
