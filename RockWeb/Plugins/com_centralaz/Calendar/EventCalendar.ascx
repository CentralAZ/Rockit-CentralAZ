<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EventCalendar.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Calendar.EventCalendar" %>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <div class="col-md-3">
            <asp:Panel ID="pnlControl" runat="server" CssClass="panel panel-block">

                <div class="panel-body">

                    <div class="alert alert-info">
                        <h2>Event Filters</h2>
                        <Rock:DateRangePicker ID="drpDateRange" runat="server" Label="<h3>Date Range</h3>" Visible="false" />

                        <Rock:RockTextBox ID="tbKeyword" runat="server" CssClass="input-width-md" Label="<h3>Keywords</h3>" />

                        <Rock:RockRadioButtonList ID="rblView" runat="server" Label="<h3>View</h3>">
                            <asp:ListItem Value="Calendar" Text="Calendar" Selected="True" />
                            <asp:ListItem Value="List" Text="List" />
                        </Rock:RockRadioButtonList>

                        <Rock:RockCheckBoxList ID="cblCampus" runat="server" Label="<h3>Campus</h3>" />
                        <Rock:NumberBox ID=" numbNumberOfItems" runat="server" Label="<h3>Number of Events</h3>" Visible="false" />
                    </div>

                </div>

            </asp:Panel>
        </div>

        <div class="col-md-9">
            <asp:Panel ID="pnlCalendar" runat="server" CssClass="panel panel-block">

                <div class="panel-body">

                    <div class="alert alert-info">
                        <h4>Stark Template Block</h4>
                        <p>This block serves as a starting point for creating new blocks. After copy/pasting it and renaming the resulting file be sure to make the following changes:</p>

                        <strong>Changes to the Codebehind (ascx.cs) File</strong>
                        <ul>
                            <li>Update the namespace to match your directory</li>
                            <li>Update the class name</li>
                            <li>Fill in the DisplayName, Category and Description attributes</li>
                        </ul>

                        <strong>Changes to the Usercontrol (.ascx) File</strong>
                        <ul>
                            <li>Update the Inherhits to match the namespace and class file</li>
                            <li>Remove this text... unless you really like it...</li>
                        </ul>
                    </div>

                </div>

            </asp:Panel>

            <asp:Panel ID="pnlList" runat="server" CssClass="panel panel-block" Visible="false">

                <div class="panel-body">

                    <div class="alert alert-info">
                        <h4>Stark Template Block</h4>
                        <p>This block serves as a starting point for creating new blocks. After copy/pasting it and renaming the resulting file be sure to make the following changes:</p>

                        <strong>Changes to the Codebehind (ascx.cs) File</strong>
                        <ul>
                            <li>Update the namespace to match your directory</li>
                            <li>Update the class name</li>
                            <li>Fill in the DisplayName, Category and Description attributes</li>
                        </ul>

                        <strong>Changes to the Usercontrol (.ascx) File</strong>
                        <ul>
                            <li>Update the Inherhits to match the namespace and class file</li>
                            <li>Remove this text... unless you really like it...</li>
                        </ul>
                    </div>

                </div>

            </asp:Panel>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
