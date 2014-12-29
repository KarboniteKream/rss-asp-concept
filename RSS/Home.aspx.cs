﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Services;

using MySql.Data.MySqlClient;

namespace RSS
{
    public partial class Home : System.Web.UI.Page
    {
        private MySqlConnection connection = null;
        private MySqlCommand command = null;
        private MySqlDataReader result = null;

        private string connectionString = "server=3020f0c4-873a-49b6-b007-a3ff00933a9e.mysql.sequelizer.com;database=db3020f0c4873a49b6b007a3ff00933a9e;userid=evvbdlzgyodaumqz;password=iGSHBCF2WwpzrmRXjhhCbUTiDfjnk3c3MvECQzWQt8pTnD7VZsZNwi3wVHevstQ3";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["userID"] == null)
            {
                // Response.Redirect("/");
            }

            connection = new MySqlConnection(connectionString);
            connection.Open();

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
                link.Attributes["onclick"] = "PageMethods.changeFeed(\"feed\", \"" + link.ID + "\");location.reload();";
                link.InnerHtml = result.GetString("name");
                feed.Controls.Add(link);

                if(result.IsDBNull(3) == false)
                {
                    HtmlGenericControl unread = new HtmlGenericControl("span");
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
                link.Attributes["onclick"] = "PageMethods.changeFeed(\"feed\", \"" + link.ID + "\");location.reload();";
                link.InnerHtml = result.GetString("name");
                feed.Controls.Add(link);

                if(result.IsDBNull(3) == false)
                {
                    HtmlGenericControl unread = new HtmlGenericControl("span");
                    unread.Attributes["class"] = "badge";
                    unread.InnerHtml = " " + result.GetString("unread");
                    feed.Controls.Add(unread);
                }

                feeds.Controls.Add(feed);

                folders.Controls.Add(folder);
            }

            subscriptions.Controls.Add(folders);
            result.Close();

            if(Session["location"].Equals("home") == true)
            {
                feedName.Text = "Home";
            }
            else if(Session["location"].Equals("unread") == true)
            {
                feedName.Text = "Unread";
            }
            else if(Session["location"].Equals("liked") == true)
            {
                feedName.Text = "Liked";
            }
            else if(Session["location"].Equals("all") == true)
            {
                feedName.Text = "All articles";
            }
            else
            {
                command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM Feeds WHERE id = @feedID";
                command.Parameters.AddWithValue("@feedID", Session["feedID"]);

                result = command.ExecuteReader();
                result.Read();
                feedName.Text = result.GetString("name");
                result.Close();
            }

            command = connection.CreateCommand();

            if(Session["location"].Equals("unread") == true)
            {
                command.CommandText = "SELECT a.id, a.title, a.url, a.author, a.date, a.content FROM Unread l JOIN Articles a ON l.article_id = a.id WHERE user_id = @userID ORDER BY a.date DESC";
                command.Parameters.AddWithValue("@userID", Session["userID"]);
            }
            else if(Session["location"].Equals("liked") == true)
            {
                command.CommandText = "SELECT a.id, a.title, a.url, a.author, a.date, a.content FROM Liked l JOIN Articles a ON l.article_id = a.id WHERE user_id = @userID";
                command.Parameters.AddWithValue("@userID", Session["userID"]);
            }
            else if(Session["location"].Equals("all") == true)
            {
                command.CommandText = "SELECT a.id, a.title, a.url, a.author, a.date, a.content, l.user_id AS liked, u.user_id AS unread FROM Subscriptions s JOIN Feeds f ON s.feed_id = f.id JOIN Articles a ON f.id = a.feed_id LEFT JOIN Liked l ON a.id = l.article_id LEFT JOIN Unread u ON a.id = u.article_id WHERE s.user_id = @userID ORDER BY a.date DESC LIMIT 100";
                command.Parameters.AddWithValue("@userID", Session["userID"]);
            }
            else
            {
                command.CommandText = "SELECT a.id, a.title, a.url, a.author, a.date, a.content FROM Users u JOIN Unread ur ON u.id = ur.user_id JOIN Articles a ON ur.article_id = a.id JOIN Feeds f ON a.feed_id = f.id WHERE u.id = @userID AND f.id = @feedID ORDER BY a.date DESC";
                command.Parameters.AddWithValue("@userID", Session["userID"]);
                command.Parameters.AddWithValue("@feedID", Session["feedID"]);
            }

            result = command.ExecuteReader();

            while(result.Read() == true)
            {
                HtmlGenericControl article = new HtmlGenericControl("article");

                if(Session["location"].Equals("unread") == true || Session["location"].Equals("feed") == true)
                {
                    article.Attributes["class"] = "unread";
                }
                else if(Session["location"].Equals("liked") == true)
                {
                    article.Attributes["class"] = "liked";
                }
                else if(Session["location"].Equals("all") == true)
                {
                    if(result.IsDBNull(6) == false)
                    {
                        article.Attributes["class"] = "liked";
                    }
                    else if(result.IsDBNull(7) == false)
                    {
                        article.Attributes["class"] = "unread";
                    }
                }

                HtmlGenericControl date = new HtmlGenericControl("div");
                date.Attributes["class"] = "date";
                date.InnerHtml = result.GetString("date").Substring(0, 10);
                article.Controls.Add(date);

                HtmlGenericControl title = new HtmlGenericControl("h2");
                HtmlGenericControl titleLink = new HtmlGenericControl("a");
                titleLink.Attributes["href"] = result.GetString("url");
                titleLink.InnerHtml = result.GetString("title");
                title.Controls.Add(titleLink);
                article.Controls.Add(title);

                if(result.IsDBNull(3) == false)
                {
                    HtmlGenericControl author = new HtmlGenericControl("div");
                    author.Attributes["class"] = "author";
                    author.InnerHtml = "by <b>" + result.GetString("author") + "</b>";
                    article.Controls.Add(author);
                }

                HtmlGenericControl content = new HtmlGenericControl("div");
                content.Attributes["class"] = "content";
                content.InnerHtml = "<p>" + result.GetString("content") + "</p>";
                article.Controls.Add(content);

                HtmlGenericControl actionBar = new HtmlGenericControl("div");
                actionBar.Attributes["class"] = "action-bar";

                HtmlAnchor likeButton = new HtmlAnchor();
                if(article.Attributes["class"] == "liked")
                {
                    likeButton.Attributes["onclick"] = "PageMethods.like(\"liked\", \"" + result.GetString("id") + "\")";
                    likeButton.InnerHtml = "Unlike";
                }
                else
                {
                    likeButton.Attributes["onclick"] = "PageMethods.like(\"unliked\", \"" + result.GetString("id") + "\")";
                    likeButton.InnerHtml = "Like";
                }
                actionBar.Controls.Add(likeButton);

                HtmlAnchor unreadButton = new HtmlAnchor();
                if(article.Attributes["class"] == "unread")
                {
                    unreadButton.Attributes["onclick"] = "PageMethods.markAsRead(\"unread\", \"" + result.GetString("id") + "\")";
                    unreadButton.InnerHtml = "Mark as read";
                }
                else
                {
                    unreadButton.Attributes["onclick"] = "PageMethods.markAsRead(\"read\", \"" + result.GetString("id") + "\")";
                    unreadButton.InnerHtml = "Mark as unread";
                }
                actionBar.Controls.Add(unreadButton);

                article.Controls.Add(actionBar);

                reader.Controls.Add(article);
            }

            result.Close();

            /*command = connection.CreateCommand();
            command.CommandText = "SELECT id, real_name, email, cookie FROM Users";
            //command.Parameters.AddWithValue("@id", Session["userID"]);

            DataTable dataTable = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(dataTable);
            
            readerView.DataSource = dataTable;
            readerView.DataBind();*/

            connection.Close();
        }

        [WebMethod]
        public static void markAsRead(string status, string id)
        {
            if(status == "unread")
            {
                System.Diagnostics.Debug.WriteLine("mark as read: " + id);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("mark as unread: " + id);
            }
        }

        [WebMethod]
        public static void like(string status, string id)
        {
            if(status != "liked")
            {
                System.Diagnostics.Debug.WriteLine("like: " + id);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("unlike: " + id);
            }
        }

        [WebMethod]
        public static void changeFeed(string location, string id)
        {
            HttpContext.Current.Session["location"] = location;
            HttpContext.Current.Session["feedID"] = Convert.ToInt32(id);
        }
    }
}