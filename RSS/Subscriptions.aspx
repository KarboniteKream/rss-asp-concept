<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Subscriptions.aspx.cs" Inherits="RSS.Subscriptions" %>
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
                        <h2>Manage subscriptions</h2>
                    </div>
                    <div id="reader">
                        <asp:GridView ID="subscriptionsGV" BorderStyle="None" GridLines="None" DataKeyNames="id" OnRowEditing="subscriptionsGV_RowEditing" OnRowCancelingEdit="subscriptionsGV_RowCancelingEdit" OnRowUpdating="subscriptionsGV_RowUpdating" runat="server" AutoGenerateColumns="False">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="select" CssClass="table-checkbox" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderStyle-CssClass="table-header" ItemStyle-CssClass="data-name" DataField="name" HeaderText="Feed name" />
                                <asp:BoundField HeaderStyle-CssClass="table-header" ItemStyle-CssClass="data-folder" DataField="folder" HeaderText="Folder" />
                                <asp:CommandField ItemStyle-CssClass="edit-column" ShowEditButton="True" ButtonType="Image" EditImageUrl="/resources/edit.png" CancelImageUrl="/resources/cancel.png" UpdateImageUrl="/resources/update.png" />
                            </Columns>
                        </asp:GridView>
                        <asp:Button CssClass="button-primary subscription-button" OnClick="saveSubscriptions" runat="server" Text="Save" />
                        <span class="button-secondary subscription-button open-popup" target-popup="#unsubscribe">Unsubscribe</span>
				    </div>
			    </div>
		    </div>
            <div id="overlay"></div>
            <div id="unsubscribe" class="popup">
			    <div class="header">
				    <span class="button-secondary" onclick="hideOverlay()">×</span>
				    <h3>Unsubscribe</h3>
			    </div>
			    <div>
				    <fieldset>
					    <span id="form-question">Are you sure you want to unsubscribe from selected feeds?</span>
					    <asp:Button CssClass="button-primary button-margin" runat="server" Text="Yes" OnClick="unsubscribe" tabindex="2" />
					    <button type="button" class="button-secondary" onclick="hideOverlay()" tabindex="1">No</button>
				    </fieldset>
			    </div>
		    </div>
        </form>
    </body>
</html>
