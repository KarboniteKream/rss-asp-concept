using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Services;

using MySql.Data.MySqlClient;

namespace RSS
{
    public partial class Help : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["userID"] == null)
            {
                Response.Redirect("/");
            }

            Session["location"] = "help";
            Session["feedID"] = -6;

            MySqlConnection connection = new MySqlConnection("server=3020f0c4-873a-49b6-b007-a3ff00933a9e.mysql.sequelizer.com;database=db3020f0c4873a49b6b007a3ff00933a9e;userid=evvbdlzgyodaumqz;password=iGSHBCF2WwpzrmRXjhhCbUTiDfjnk3c3MvECQzWQt8pTnD7VZsZNwi3wVHevstQ3");
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

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(article_id) AS unread FROM Unread WHERE user_id = @userID";
            command.Parameters.AddWithValue("@userID", Session["userID"]);
            MySqlDataReader result = command.ExecuteReader();
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

            subscriptions.Controls.Add(noFolder);
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

            subscriptions.Controls.Add(folders);
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
    }
}