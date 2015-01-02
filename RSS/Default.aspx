<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RSS.Default" %>

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
    <body id="index">
        <header>
			<h1><a href="/">kream.io</a><span class="version"> rss</span></h1>
			<nav>
				<ul>
					<li><span class="button-secondary open-popup" target-popup="#register">Register</span></li>
					<li><span class="button-primary open-popup" target-popup="#sign-in">Sign in</span></li>
				</ul>
			</nav>
		</header>
        <main>
            <form runat="server">
                <asp:Button ID="qwe" runat="server" OnClick="qwe_Click" />
                <div id="banner">the next-generation <span class="rss">RSS</span> reader</div>
		        <div id="landing">
			        <div>
				        <div class="header">
					        <span id="fullscreen" class="button-primary">v</span>
					        <h2>Featured articles</h2>
				        </div>
					    <asp:Panel id="featured" runat="server" />
			        </div>
		        </div>
            </form>
		</main>
        <div id="overlay" onclick="hideOverlay()"></div>
        <div id="sign-in" class="popup">
			<div class="header">
				<span class="button-secondary" onclick="hideOverlay()">×</span>
				<h3>Sign in</h3>
			</div>
            <form>

            </form>
		</div>
        <div id="register" class="popup">
			<div class="header">
				<span class="button-secondary" onclick="hideOverlay()">×</span>
				<h3>Register</h3>
			</div>
			<form>

			</form>
		</div>
    </body>
</html>
