<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TwitterBlock.ascx.cs" Inherits="RockWeb.Plugins.com_centralaz.Widgets.TwitterBlock" %>
<script src="http://twitter.com/javascripts/blogger.js" type="text/javascript"></script>
<script src='<%= String.Format("https://api.twitter.com/1/statuses/user_timeline/{0}.json?callback=twitterCallback2&count={1}",GetAttributeValue("TwitterUsername"), GetAttributeValue("NumberOfTweets") )%>' type="text/javascript"></script>
<style>
    #twitter {
        -webkit-border-radius: 5px;
        -moz-border-radius: 5px;
        border-radius: 5px;
        width: 430px;
        border: 1px solid #c9c9c9;
    }

    #twitter_t {
        width: 405px;
        height: 44px;
        background: #c68aeb url("~/Plugins/com_centralaz/Baptism/Assets/Icons/CentralChristianChurchArizona_165_90.png");
        color: #fff;
        text-shadow: .5px .5px #333;
        font-size: 18px;
        font-family: Candara;
        padding-top: 20px;
        padding-left: 25px;
    }

    #twitter_m {
        width: 400px;
        padding: 0 15px;
        background: #f7effc;
    }

    #twitter_container {
        min-height: 45px;
        height: auto !important;
        height: 40px;
        padding-bottom: 5px;
        padding-top: 5px;
    }

    #twitter_update_list {
        width: 413px;
        padding: 0;
        overflow: hidden;
        font-family: Georgia;
        font-size: 14px;
        font-style: italic;
        color: #31353d;
        line-height: 16px;
        font-weight: bold;
        margin-left: -13px;
    }

        #twitter_update_list li {
            width: 400px;
            list-style: none;
            padding: 15px;
            border-bottom: dotted 1px #ccc;
        }

            #twitter_update_list li a {
                color: #631891;
                text-decoration: none;
            }

                #twitter_update_list li a:hover {
                    color: #31353d;
                }

    #twitter_b {
        width: 430px;
        height: 29px;
        background: #c68aeb url("~/Plugins/com_centralaz/Baptism/Assets/Icons/CentralChristianChurchArizona_165_90.png");
    }
</style>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <div id="twitter">
            <div id="twitter_t"></div>
            <div id="twitter_m">
                <div id="twitter_container">
                    <ul id="twitter_update_list"></ul>
                </div>
            </div>
            <div id="twitter_b"></div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

