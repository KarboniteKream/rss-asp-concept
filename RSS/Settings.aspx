<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="RSS.Settings" %>
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
				    <li><a href="/Settings.aspx" class="active">Settings</a></li>
				    <li><a href="/Help.aspx">Help</a></li>
				    <li><a onclick="PageMethods.signOut();window.location='/';" runat="server">Sign out</a></li>
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
							<asp:Panel id="menuItems" runat="server" />
						</ul>
					</div>
                    <asp:Panel id="subscriptions" runat="server" />
				</div>
			</div>
			<div id="content">
                <div class="header">
                    <div class="header-secondary-30">
                        <h2>Account</h2>
                    </div>
                    <h2>Settings</h2>
                </div>
				<div id="reader">
                    <form runat="server">
                        <asp:ScriptManager ID="scriptManager" runat="server" EnablePageMethods="true" />
                        <div id="banner">work in progress</div>
                    </form>
				</div>
			</div>
		</div>
    </body>
</html>
