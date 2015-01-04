<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="RSS.Help" %>

<!DOCTYPE html>

<html>
    <head runat="server">
        <title>kream</title>
		<meta charset="UTF-8" />
		<link href="/resources/kream.png" rel="icon" type="image/png" />
		<link href="/style.css" rel="stylesheet" type="text/css" />
		<script src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
		<script src="/scripts/html.sortable.min.js" type="text/javascript"></script>
		<script src="/script.js" type="text/javascript"></script>
    </head>
    <body>
        <header>
			<h1><a href="/home.aspx">kream.io</a><span class="version"> rss</span></h1>
			<nav>
                <ul>
				    <li><a href="/Home.aspx">Home</a></li>
				    <li><a href="/Settings.aspx">Settings</a></li>
				    <li><a href="/Help.aspx" class="active">Help</a></li>
				    <li><a onclick="PageMethods.signOut();window.location='/';" runat="server">Sign out</a></li>
			    </ul>
			</nav>
		</header>
        <form runat="server">
            <asp:ScriptManager ID="scriptManager" runat="server" EnablePageMethods="true" />
            <div id="main">
			    <div id="sidebar">
				    <span id="new-subscription" class="button-primary block">New subscription</span>
				    <div id="add-subscription">
					    <fieldset>
						    <asp:Button ID="addSubscriptionButton" runat="server" Text="Add" OnClick="addSubscription" tabindex="2" />
						    <input id="subscriptionURL" placeholder="subscription URL" tabindex="1" runat="server" />
					    </fieldset>
				    </div>
				    <div id="sidebar-content">
					    <div id="menu">
						    <ul>
							    <asp:Panel id="menuItems" runat="server" />
						    </ul>
					    </div>
                        <asp:UpdatePanel id="subscriptions" runat="server">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="addSubscriptionButton" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
				    </div>
			    </div>
			    <div id="content">
                    <div class="header">
                        <h2>Help</h2>
                    </div>
				    <div id="reader">
                        <div id="banner">work in progress</div>
				    </div>
			    </div>
		    </div>
        </form>
    </body>
</html>
