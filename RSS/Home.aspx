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
			<h1><a href="/home.xhtml">kream.io</a><span class="version"> rss</span></h1>
			<nav>
				<ul>
					<li><a href="/home.xhtml" class="active">Home</a></li>
					<li><a href="/settings.xhtml">Settings</a></li>
					<li><a href="/help.xhtml">Help</a></li>
					<li><a href="/">Sign out</a></li>
				</ul>
			</nav>
		</header>
        <div id="main">
			<div id="sidebar">
				<span id="new-subscription" class="button-primary block">New subscription</span>
				<div id="add-subscription">
					<form action="javascript:;" method="post">
						<fieldset>
							<button type="submit" tabindex="2">Add</button>
							<input type="url" placeholder="subscription URL" tabindex="1"/>
						</fieldset>
					</form>
				</div>
				<div id="sidebar-content">
					<div id="menu">
						<ul>
							<li><a id="home" onclick="PageMethods.changeFeed('home', '-1');location.reload();">Home</a></li>
                            <li><a id="unread" onclick="PageMethods.changeFeed('unread', '-2');location.reload();">Unread</a></li>
                            <li><a id="liked" onclick="PageMethods.changeFeed('liked', '-3');location.reload();">Liked</a></li>
                            <li><a id="all" onclick="PageMethods.changeFeed('all', '-4');location.reload();">All articles</a></li>
						</ul>
					</div>
                    <asp:Panel id="subscriptions" runat="server"/>
				</div>
			</div>
			<div id="content">
				<div class="header">
					<span class="button-primary">Refresh</span>
					<span class="button-secondary">Settings</span>
					<span class="button-secondary open-popup" target-popup="#unsubscribe">Unsubscribe</span>
					<h2 id="feed-name"><asp:label id="feedName" runat="server" /></h2>
				</div>
                <form runat="server">
                    <asp:ScriptManager ID="scriptManager" runat="server" EnablePageMethods="true" />
                    <asp:Panel id="reader" runat="server" />
                </form>
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
					<span id="form-question">Are you sure you want to unsubscribe from FEED NAME?</span>
					UNSUBSCRIBE BUTTON
					<button id="unsubscribe-button" type="button" class="button-secondary" onclick="hideOverlay()" tabindex="1">No</button>
				</fieldset>
			</form>
		</div>
    </body>
</html>
