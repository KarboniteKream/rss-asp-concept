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
                        <div class="header-secondary-30">
                            <h2>Account</h2>
                        </div>
                        <h2>Settings</h2>
                    </div>
                    <div id="reader">
                        <div id="settings-content" class="home-left">
						    <div>
							    <div id="general-settings">
								    <div>
									    <fieldset>
										    <legend>Home page</legend>
										    <input id="featured-home" type="checkbox" checked />
										    <label for="featured-home">Show featured articles</label>
										    <br />
										    <input id="xkcd-time" type="checkbox" checked />
										    <label for="xkcd-time">Show XKCD #1190</label>
									    </fieldset>
									    <fieldset>
										    <legend>Appearance</legend>
										    <label for="theme">Theme</label>
										    <select id="theme">
											    <option value="Dark">Dark</option>
											    <option value="Light" selected="selected">Light</option>
										    </select>
									    </fieldset>
								    </div>
							    </div>
							    <div id="user-interface">
								    <div>
									    <fieldset>
										    <legend>Article order</legend>
										    <input id="order-normal" type="radio" checked="checked" />
										    <label for="order-normal">Normal (newer articles first)</label>
										    <br />
										    <input id="order-reverse" type="radio" />
										    <label for="order-reverse">Reverse (older articles first)</label>
										    <br />
									    </fieldset>
									    <fieldset>
										    <legend>Reading preferences</legend>
										    <input id="mark-read" type="checkbox" checked />
										    <label for="mark-read">Mark articles as read when scrolling</label>
									    </fieldset>
								    </div>
							    </div>
						    </div>
						    <div>
							    <span class="button-primary block settings-button">Save</span>
                                <a id="manage-subscriptions" class="button-secondary settings-button" href="Subscriptions.aspx">Manage subscriptions</a>
						    </div>
					    </div>
                        <div id="account-content" class="home-right">
                            <div id="change-email">
						        <fieldset>
								    <legend>Change e-mail</legend>
								    <asp:TextBox id="newEmail" TextMode="Email" placeholder="new e-mail" runat="server" />
								    <br />
                                    <asp:TextBox id="confirmEmail" CssClass="confirm-email" TextMode="Email" placeholder="confirm e-mail" runat="server" />
								    <br />
                                    <asp:CustomValidator CssClass="form-error-new" runat="server" ErrorMessage="E-mails do not match." OnServerValidate="ValidateEmail" />
								    <button type="submit">Change</button>
							    </fieldset>
                            </div>
						    <div id="change-password">
                                <fieldset>
								    <legend>Change password</legend>
								    <asp:TextBox id="currentPassword" TextMode="Password" placeholder="current password" runat="server" />
								    <br />
								    <asp:TextBox id="newPassword" TextMode="Password" placeholder="new password" runat="server" />
								    <br />
                                    <asp:TextBox id="confirmPassword" CssClass="confirm-password" TextMode="Password" placeholder="confirm password" runat="server" />
								    <br />
                                    <asp:CustomValidator CssClass="form-error-new" ErrorMessage="Passwords do not match." OnServerValidate="ValidatePassword" runat="server" />
								    <button type="submit">Change</button>
                                </fieldset>
						    </div>
						    <span id="delete-account-button" class="button-red block open-popup" target-popup="#delete-account">Delete my account</span>
					    </div>
				    </div>
			    </div>
		    </div>
            <div id="delete-account" class="popup">
			    <div class="header">
				    <span class="button-secondary" onclick="hideOverlay()">&times;</span>
				    <h3>Delete my account</h3>
			    </div>
			    <div id="delete-account-form">
				    <fieldset>
					    <asp:TextBox id="deletePassword" TextMode="Password" placeholder="password" runat="server" />
					    <br />
					    <asp:TextBox id="confirmDeletePassword" TextMode="Password" placeholder="confirm password" runat="server" />
					    <br />
                        <asp:CustomValidator CssClass="form-error" ErrorMessage="Passwords do not match." OnServerValidate="ValidateDeleteAccount" runat="server" />
					    <button type="submit" class="button-red">Delete</button>
					    <label>This cannot be undone!</label>
				    </fieldset>
			    </div>
		    </div>
            <div id="overlay"></div>
        </form>
    </body>
</html>
