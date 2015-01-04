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
		</main>
        <div id="overlay" onclick="hideOverlay()"></div>
        <form runat="server">
            <div id="sign-in" class="popup">
			    <div class="header">
				    <span class="button-secondary" onclick="hideOverlay()">×</span>
				    <h3>Sign in</h3>
			    </div>
                <fieldset>
                    <asp:TextBox id="email" placeholder="e-mail" TextMode="Email" tabindex="1" runat="server" />
                    <br />
                    <asp:TextBox id="password" placeholder="password" TextMode="Password" tabindex="2" runat="server" />
                    <br />
                    <asp:CustomValidator CssClass="form-error" ErrorMessage="Incorrect e-mail/password." OnServerValidate="ValidateSignIn" runat="server" />
                    <button type="submit" tabindex="4">Sign in</button>
                    <label for="rememberMe">
                        <asp:CheckBox id="rememberMe" tabindex="3"  runat="server"/>
                        Remember me
                    </label>
                </fieldset>
		    </div>
            <div id="register" class="popup">
			    <div class="header">
				    <span class="button-secondary" onclick="hideOverlay()">×</span>
				    <h3>Register</h3>
			    </div>
			    <fieldset>
                    <asp:TextBox id="realName" placeholder="real name" runat="server" />
                    <br />
                    <asp:TextBox id="newEmail" placeholder="e-mail" TextMode="Email" runat="server" />
                    <br />
                    <br />
                    <asp:TextBox id="newPassword1" placeholder="password" TextMode="Password" runat="server" />
                    <br />
                    <asp:TextBox id="newPassword2" CssClass="confirm-password" placeholder="confirm password" TextMode="Password" runat="server"></asp:TextBox>
                    <br />
                    <asp:CustomValidator CssClass="form-error" ErrorMessage="This e-mail is already registered." OnServerValidate="ValidateRegister" runat="server" />
                    <button type="submit">Register</button>
			    </fieldset>
		    </div>
        </form>
    </body>
</html>
