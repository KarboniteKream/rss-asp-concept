<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="RSS.Home" %>

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
				    <li><a href="/Home.aspx" class="active">Home</a></li>
				    <li><a href="/Settings.aspx">Settings</a></li>
				    <li><a href="/Help.aspx">Help</a></li>
				    <li><a onclick="PageMethods.signOut();window.location='/';" runat="server">Sign out</a></li>
			    </ul>
			</nav>
		</header>
        <div id="main">
            <div id="sidebar">
				<span id="new-subscription" class="button-primary block">New subscription</span>
                <form runat="server">
                    <asp:ScriptManager ID="scriptManager" runat="server" EnablePageMethods="true" />
				    <div id="add-subscription">
					    <fieldset>
							<asp:Button ID="testButton" runat="server" Text="Add" OnClick="addSubscription" tabindex="2" />
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
                                <asp:AsyncPostBackTrigger ControlID="testButton" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
				    </div>
                </form>
			</div>
			<div id="content">
                <asp:Panel id="header" CssClass="header" runat="server" />
                <asp:Panel id="reader" runat="server" />
			</div>
		</div>
		<div id="overlay" onclick="hideOverlay()"></div>
		<div id="unsubscribe" class="popup">
			<div class="header">
				<span class="button-secondary" onclick="hideOverlay()">×</span>
				<h3>Unsubscribe</h3>
			</div>
			<form method="post">
				<fieldset>
					<span id="form-question">Are you sure you want to unsubscribe from <asp:Label id="feedNameUnsubscribe" runat="server" />?</span>
					<button type="submit" onclick="PageMethods.unsubscribe()">Yes</button>
					<button type="button" class="button-secondary" onclick="hideOverlay()" tabindex="1">No</button>
				</fieldset>
			</form>
		</div>
    </body>
</html>
