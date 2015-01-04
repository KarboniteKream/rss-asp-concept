using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Xml;

using MySql.Data.MySqlClient;

namespace RSS
{
    public partial class Settings : System.Web.UI.Page
    {
        private MySqlConnection connection = null;
        private MySqlCommand command = null;
        private MySqlDataReader result = null;

        private string connectionString = "server=3020f0c4-873a-49b6-b007-a3ff00933a9e.mysql.sequelizer.com;database=db3020f0c4873a49b6b007a3ff00933a9e;userid=evvbdlzgyodaumqz;password=iGSHBCF2WwpzrmRXjhhCbUTiDfjnk3c3MvECQzWQt8pTnD7VZsZNwi3wVHevstQ3";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["userID"] == null)
            {
                Response.Redirect("/");
            }

            Session["location"] = "settings";
            Session["feedID"] = -5;

            connection = new MySqlConnection(connectionString);
            loadSidebar();
        }

        private void loadSidebar()
        {
            connection.Open();

            HtmlGenericControl li = new HtmlGenericControl("li");
            HtmlGenericControl a = new HtmlGenericControl("a");
            a.ID = "home";
            a.Attributes["onclick"] = "PageMethods.changeFeed(\"" + a.ID + "\", \"-1\");window.location='Home.aspx';";
            a.Attributes["class"] = Session["location"].Equals(a.ID) == true ? "active" : "";
            a.InnerHtml = "Home";
            li.Controls.Add(a);
            menuItems.Controls.Add(li);

            li = new HtmlGenericControl("li");
            a = new HtmlGenericControl("a");
            a.ID = "unread";
            a.Attributes["onclick"] = "PageMethods.changeFeed(\"" + a.ID + "\", \"-2\");window.location='Home.aspx';";
            a.Attributes["class"] = Session["location"].Equals(a.ID) == true ? "active" : "";
            a.InnerHtml = "Unread";
            li.Controls.Add(a);

            HtmlGenericControl unread = new HtmlGenericControl("span");
            unread.Attributes["class"] = "badge";

            command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(article_id) AS unread FROM Unread WHERE user_id = @userID";
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            result = command.ExecuteReader();
            result.Read();
            unread.InnerText = " " + result.GetString("unread");
            result.Close();
            li.Controls.Add(unread);

            menuItems.Controls.Add(li);

            li = new HtmlGenericControl("li");
            a = new HtmlGenericControl("a");
            a.ID = "liked";
            a.Attributes["onclick"] = "PageMethods.changeFeed(\"" + a.ID + "\", \"-3\");window.location='Home.aspx';";
            a.Attributes["class"] = Session["location"].Equals(a.ID) == true ? "active" : "";
            a.InnerHtml = "Liked";
            li.Controls.Add(a);
            menuItems.Controls.Add(li);

            li = new HtmlGenericControl("li");
            a = new HtmlGenericControl("a");
            a.ID = "all";
            a.Attributes["onclick"] = "PageMethods.changeFeed(\"" + a.ID + "\", \"-4\");window.location='Home.aspx';";
            a.Attributes["class"] = Session["location"].Equals(a.ID) == true ? "active" : "";
            a.InnerHtml = "All articles";
            li.Controls.Add(a);
            menuItems.Controls.Add(li);

            command = connection.CreateCommand();
            command.CommandText = "SELECT f.id, f.name, f.icon, u.unread FROM Subscriptions s JOIN Feeds f ON s.feed_id = f.id LEFT JOIN (SELECT a.feed_id, COUNT(a.feed_id) AS unread FROM Unread JOIN Articles a ON article_id = a.id WHERE user_id = @userID GROUP BY feed_id) AS u ON f.id = u.feed_id WHERE s.user_id = @userID AND s.folder IS NULL ORDER BY f.name";
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            result = command.ExecuteReader();

            HtmlGenericControl noFolder = new HtmlGenericControl("ul");
            noFolder.Attributes["class"] = "connected sortable";

            while(result.Read() == true)
            {
                HtmlGenericControl feed = new HtmlGenericControl("li");
                HtmlGenericControl link = new HtmlGenericControl("a");
                link.ID = result.GetString("id");

                if(link.ID.Equals(Session["feedID"].ToString()) == true)
                {
                    link.Attributes["class"] = "active";
                }

                link.Attributes["style"] = "background-image: url(data:image/png;base64," + Convert.ToBase64String((byte[])result.GetValue(2)) + ");";
                link.Attributes["onclick"] = "PageMethods.changeFeed(\"feed\", \"" + link.ID + "\");window.location='Home.aspx';";
                link.InnerHtml = result.GetString("name");
                feed.Controls.Add(link);

                if(result.IsDBNull(3) == false)
                {
                    unread = new HtmlGenericControl("span");
                    unread.Attributes["class"] = "badge";
                    unread.InnerHtml = " " + result.GetString("unread");
                    feed.Controls.Add(unread);
                }

                noFolder.Controls.Add(feed);
            }

            HtmlGenericControl empty = new HtmlGenericControl("li");
            empty.Attributes["class"] = "empty-li";
            noFolder.Controls.Add(empty);

            subscriptions.ContentTemplateContainer.Controls.Add(noFolder);
            result.Close();

            command = connection.CreateCommand();
            command.CommandText = "SELECT f.id, f.name, f.icon, u.unread, s.folder FROM Subscriptions s JOIN Feeds f ON s.feed_id = f.id LEFT JOIN (SELECT a.feed_id, COUNT(a.feed_id) AS unread FROM Unread JOIN Articles a ON article_id = a.id WHERE user_id = @userID GROUP BY feed_id) AS u ON f.id = u.feed_id WHERE s.user_id = @userID AND s.folder IS NOT NULL ORDER BY s.folder, f.name";
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            result = command.ExecuteReader();

            HtmlGenericControl folders = new HtmlGenericControl("ul");
            HtmlGenericControl folder = null, input, label, feeds = null;
            string previousFolder = null;

            while(result.Read() == true)
            {
                string currentFolder = result.GetString("folder");

                if(currentFolder != previousFolder)
                {
                    folder = new HtmlGenericControl("li");
                    folder.Attributes["class"] = "folder";

                    input = new HtmlGenericControl("input");
                    input.ID = currentFolder;
                    input.Attributes["type"] = "checkbox";
                    folder.Controls.Add(input);

                    label = new HtmlGenericControl("label");
                    label.Attributes["for"] = currentFolder;
                    label.InnerText = currentFolder;
                    folder.Controls.Add(label);

                    feeds = new HtmlGenericControl("ul");
                    feeds.Attributes["class"] = "connected sortable";
                    folder.Controls.Add(feeds);

                    empty = new HtmlGenericControl("li");
                    empty.Attributes["class"] = "empty-li";
                    feeds.Controls.Add(empty);

                    previousFolder = currentFolder;
                }

                HtmlGenericControl feed = new HtmlGenericControl("li");
                HtmlGenericControl link = new HtmlGenericControl("a");
                link.ID = result.GetString("id");

                if(link.ID.Equals(Session["feedID"].ToString()) == true)
                {
                    link.Attributes["class"] = "active";
                }

                link.Attributes["style"] = "background-image: url(data:image/png;base64," + Convert.ToBase64String((byte[])result.GetValue(2)) + ");";
                link.Attributes["onclick"] = "PageMethods.changeFeed(\"feed\", \"" + link.ID + "\");window.location='Home.aspx';";
                link.InnerHtml = result.GetString("name");
                feed.Controls.Add(link);

                if(result.IsDBNull(3) == false)
                {
                    unread = new HtmlGenericControl("span");
                    unread.Attributes["class"] = "badge";
                    unread.InnerHtml = " " + result.GetString("unread");
                    feed.Controls.Add(unread);
                }

                feeds.Controls.Add(feed);

                folders.Controls.Add(folder);
            }

            subscriptions.ContentTemplateContainer.Controls.Add(folders);
            result.Close();

            connection.Close();
        }

        [WebMethod]
        public static void changeFeed(string location, string id)
        {
            HttpContext.Current.Session["location"] = location;
            HttpContext.Current.Session["feedID"] = Convert.ToInt32(id);
        }

        [WebMethod]
        public static void signOut()
        {
            HttpContext.Current.Session["userID"] = null;
            HttpContext.Current.Session["location"] = null;
            HttpContext.Current.Session["feedID"] = null;
        }

        protected void ValidateEmail(object sender, ServerValidateEventArgs args)
        {
            if(newEmail.Text.Equals("") == true && confirmEmail.Text.Equals("") == true)
            {
                args.IsValid = true;
                return;
            }

            if(newEmail.Text.Equals(confirmEmail.Text) == false)
            {
                ((CustomValidator)sender).ErrorMessage = "E-mails do not match.";
                args.IsValid = false;
                return;
            }

            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE email = @email";
            command.Parameters.AddWithValue("@email", newEmail.Text);

            result = command.ExecuteReader();
            bool exists = result.HasRows;
            result.Close();

            if(exists == true)
            {
                connection.Close();

                ((CustomValidator)sender).ErrorMessage = "This e-mail is already in use.";
                args.IsValid = false;
                return;
            }

            command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET email = @email WHERE id = @userID";
            command.Parameters.AddWithValue("@email", newEmail.Text);
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            command.ExecuteNonQuery();

            connection.Close();
            args.IsValid = true;
        }

        protected void ValidatePassword(object sender, ServerValidateEventArgs args)
        {
            if(currentPassword.Text.Equals("") == true && newPassword.Text.Equals("") == true && confirmPassword.Text.Equals("") == true || newPassword.Text.Equals("") == true && confirmPassword.Text.Equals("") == true)
            {
                args.IsValid = true;
                return;
            }

            if(newPassword.Text.Equals(confirmPassword.Text) == false)
            {
                ((CustomValidator)sender).ErrorMessage = "Passwords do not match.";
                args.IsValid = false;
                return;
            }

            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE id = @userID AND password = SHA1(@password)";
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            command.Parameters.AddWithValue("@password", currentPassword.Text);

            result = command.ExecuteReader();
            bool correct = result.HasRows;
            result.Close();

            if(correct == false)
            {
                connection.Close();

                ((CustomValidator)sender).ErrorMessage = "The current password is incorrect.";
                args.IsValid = false;
                return;
            }

            command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET password = SHA1(@password) WHERE id = @userID";
            command.Parameters.AddWithValue("@password", newPassword.Text);
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            command.ExecuteNonQuery();

            connection.Close();
            args.IsValid = true;
        }

        protected void ValidateDeleteAccount(object sender, ServerValidateEventArgs args)
        {
            if(deletePassword.Text.Equals("") == true && confirmDeletePassword.Text.Equals("") == true)
            {
                args.IsValid = true;
                return;
            }

            if(deletePassword.Text.Equals(confirmDeletePassword.Text) == false)
            {
                ((CustomValidator)sender).ErrorMessage = "Passwords do not match.";
                args.IsValid = false;
                return;
            }

            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE id = @userID AND password = SHA1(@password)";
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            command.Parameters.AddWithValue("@password", deletePassword.Text);

            result = command.ExecuteReader();
            bool correct = result.HasRows;
            result.Close();

            if(correct == false)
            {
                connection.Close();

                ((CustomValidator)sender).ErrorMessage = "The current password is incorrect.";
                args.IsValid = false;
                return;
            }

            command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE id = @userID";
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            command.ExecuteNonQuery();

            connection.Close();
            args.IsValid = true;

            signOut();
            Response.Redirect("/");
        }

        protected void addSubscription(object sender, EventArgs e)
        {
            connection.Open();

            string url = subscriptionURL.Value;

            if(url.StartsWith("http://") == false)
            {
                url = String.Concat("http://", url);
            }

            XmlReader reader = XmlReader.Create(url);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            XmlNode name = doc.SelectSingleNode("//channel/title");
            byte[] icon = new WebClient().DownloadData("http://www.google.com/s2/favicons?domain=" + url);

            command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Feeds (name, icon) VALUES (@name, @icon)";
            command.Parameters.AddWithValue("@name", name.InnerText);
            command.Parameters.AddWithValue("@icon", icon);
            command.ExecuteNonQuery();

            long feedID = command.LastInsertedId;

            command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Subscriptions (user_id, feed_id) VALUES (@userID, @feedID)";
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            command.Parameters.AddWithValue("@feedID", feedID);
            command.ExecuteNonQuery();

            XmlNodeList articles = doc.SelectNodes("//item");
            command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Articles (feed_id, title, url, author, date, content) VALUES (@feedID, @title, @url, @author, @date, @content)";
            command.Parameters.AddWithValue("@feedID", feedID);
            command.Parameters.Add(new MySqlParameter("@title", MySqlDbType.VarChar));
            command.Parameters.Add(new MySqlParameter("@url", MySqlDbType.VarChar));
            command.Parameters.Add(new MySqlParameter("@author", MySqlDbType.VarChar));
            command.Parameters.Add(new MySqlParameter("@date", MySqlDbType.DateTime));
            command.Parameters.Add(new MySqlParameter("@content", MySqlDbType.MediumText));
            command.Prepare();

            MySqlCommand unreadCommand = connection.CreateCommand();
            unreadCommand.CommandText = "INSERT INTO Unread VALUES (@userID, @articleID)";
            unreadCommand.Parameters.AddWithValue("@userID", Session["userID"]);
            unreadCommand.Parameters.Add(new MySqlParameter("@articleID", MySqlDbType.UInt32, 10));

            foreach(XmlNode article in articles)
            {
                command.Parameters["@title"].Value = article["title"].InnerText;
                command.Parameters["@url"].Value = article["link"].InnerText;
                command.Parameters["@author"].Value = null;
                command.Parameters["@date"].Value = DateTime.Now;

                DateTime date;

                if(DateTime.TryParseExact(article["pubDate"].InnerText, "ddd, dd MMM yyyy HH:mm:ss 'EST'", null, System.Globalization.DateTimeStyles.None, out date) == true)
                {
                    command.Parameters["@date"].Value = date;
                }
                else
                {
                    command.Parameters["@date"].Value = DateTime.Now;
                }

                command.Parameters["@content"].Value = article["description"].InnerText;

                foreach(XmlNode node in article.ChildNodes)
                {
                    if(node.Name.Equals("author") == true)
                    {
                        command.Parameters["@author"].Value = node.InnerText;
                    }
                }

                command.ExecuteNonQuery();

                unreadCommand.Parameters["@articleID"].Value = command.LastInsertedId;
                unreadCommand.ExecuteNonQuery();
            }

            reader.Close();

            subscriptions.ContentTemplateContainer.Controls.Clear();
            loadSidebar();

            subscriptionURL.Value = "";

            connection.Close();
        }
    }
}