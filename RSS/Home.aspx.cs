using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Xml;

using MySql.Data.MySqlClient;

namespace RSS
{
    public partial class Home : System.Web.UI.Page
    {
        private static MySqlConnection connection = null;
        private static MySqlCommand command = null;
        private static MySqlDataReader result = null;

        private string unreadCount = "";
        private string connectionString = "server=3020f0c4-873a-49b6-b007-a3ff00933a9e.mysql.sequelizer.com;database=db3020f0c4873a49b6b007a3ff00933a9e;userid=evvbdlzgyodaumqz;password=iGSHBCF2WwpzrmRXjhhCbUTiDfjnk3c3MvECQzWQt8pTnD7VZsZNwi3wVHevstQ3";

        private UpdatePanel weather = null;
        private TextBox location = null;
        private Button refreshWeather = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["userID"] == null)
            {
                Response.Redirect("/");
            }

            connection = new MySqlConnection(connectionString);
            connection.Open();

            loadSidebar();

            if(Session["Location"].Equals("home") == false)
            {
                HtmlGenericControl button = new HtmlGenericControl("span");
                button.Attributes["class"] = "button-primary";
                button.InnerText = "Refresh";
                header.Controls.Add(button);

                button = new HtmlGenericControl("span");
                button.Attributes["class"] = "button-secondary";
                button.InnerText = "Settings";
                header.Controls.Add(button);

                button = new HtmlGenericControl("span");
                button.Attributes["class"] = "button-secondary open-popup";
                button.Attributes["target-popup"] = "#unsubscribe";
                button.InnerText = "Unsubscribe";
                header.Controls.Add(button);
            }
            else
            {
                HtmlGenericControl headerSecondary = new HtmlGenericControl("div");
                headerSecondary.Attributes["class"] = "header-secondary";

                HtmlGenericControl refresh = new HtmlGenericControl("span");
                refresh.Attributes["class"] = "button-primary";
                refresh.InnerText = "Refresh";
                headerSecondary.Controls.Add(refresh);

                HtmlGenericControl title = new HtmlGenericControl("h2");
                title.InnerText = "Featured articles";
                headerSecondary.Controls.Add(title);

                HtmlGenericControl settings = new HtmlGenericControl("span");
                settings.Attributes["class"] = "button-secondary";
                settings.InnerText = "Settings";

                header.Controls.Add(headerSecondary);
                header.Controls.Add(settings);
            }

            HtmlGenericControl feedName = new HtmlGenericControl("h2");

            if(Session["location"].Equals("home") == true)
            {
                feedName.InnerText = "Home";
            }
            else if(Session["location"].Equals("unread") == true)
            {
                feedName.InnerText = "Unread";
            }
            else if(Session["location"].Equals("liked") == true)
            {
                feedName.InnerText = "Liked";
            }
            else if(Session["location"].Equals("all") == true)
            {
                feedName.InnerText = "All articles";
            }
            else
            {
                command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM Feeds WHERE id = @feedID";
                command.Parameters.AddWithValue("@feedID", Session["feedID"]);

                result = command.ExecuteReader();
                result.Read();

                feedName.InnerText = result.GetString("name");
                feedNameUnsubscribe.Text = result.GetString("name");

                result.Close();
            }

            header.Controls.Add(feedName);

            if(Session["Location"].Equals("home") == false)
            {
                loadArticles();
            }
            else
            {
                loadFeatured();
            }

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

        private void loadSidebar()
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            HtmlGenericControl a = new HtmlGenericControl("a");
            a.ID = "home";
            a.Attributes["onclick"] = "PageMethods.changeFeed(\"" + a.ID + "\", \"-1\");location.reload();";
            a.Attributes["class"] = Session["location"].Equals(a.ID) == true ? "active" : "";
            a.InnerHtml = "Home";
            li.Controls.Add(a);
            menuItems.Controls.Add(li);

            li = new HtmlGenericControl("li");
            a = new HtmlGenericControl("a");
            a.ID = "unread";
            a.Attributes["onclick"] = "PageMethods.changeFeed(\"" + a.ID + "\", \"-2\");location.reload();";
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
            unreadCount = result.GetString("unread");
            result.Close();
            li.Controls.Add(unread);

            menuItems.Controls.Add(li);

            li = new HtmlGenericControl("li");
            a = new HtmlGenericControl("a");
            a.ID = "liked";
            a.Attributes["onclick"] = "PageMethods.changeFeed(\"" + a.ID + "\", \"-3\");location.reload();";
            a.Attributes["class"] = Session["location"].Equals(a.ID) == true ? "active" : "";
            a.InnerHtml = "Liked";
            li.Controls.Add(a);
            menuItems.Controls.Add(li);

            li = new HtmlGenericControl("li");
            a = new HtmlGenericControl("a");
            a.ID = "all";
            a.Attributes["onclick"] = "PageMethods.changeFeed(\"" + a.ID + "\", \"-4\");location.reload();";
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
                link.Attributes["onclick"] = "PageMethods.changeFeed(\"feed\", \"" + link.ID + "\");location.reload();";
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
                link.Attributes["onclick"] = "PageMethods.changeFeed(\"feed\", \"" + link.ID + "\");location.reload();";
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
        }

        private void loadArticles()
        {
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
                    likeButton.Attributes["onclick"] = "PageMethods.like(\"" + result.GetString("id") + "\")";
                    likeButton.InnerHtml = "Unlike";
                }
                else
                {
                    likeButton.Attributes["onclick"] = "PageMethods.like(\"" + result.GetString("id") + "\")";
                    likeButton.InnerHtml = "Like";
                }
                actionBar.Controls.Add(likeButton);

                HtmlAnchor unreadButton = new HtmlAnchor();
                if(article.Attributes["class"] == "unread")
                {
                    unreadButton.Attributes["onclick"] = "PageMethods.markAsRead(\"" + result.GetString("id") + "\")";
                    unreadButton.InnerHtml = "Mark as read";
                }
                else
                {
                    unreadButton.Attributes["onclick"] = "PageMethods.markAsRead(\"" + result.GetString("id") + "\")";
                    unreadButton.InnerHtml = "Mark as unread";
                }
                actionBar.Controls.Add(unreadButton);

                article.Controls.Add(actionBar);

                reader.Controls.Add(article);
            }

            result.Close();
        }

        private void loadFeatured()
        {
            HtmlGenericControl homeLeft = new HtmlGenericControl("div");
            homeLeft.Attributes["class"] = "home-left";

            HtmlGenericControl unreadNotice = new HtmlGenericControl("h2");
            unreadNotice.Attributes["class"] = "notice";
            unreadNotice.InnerText = "You have " + unreadCount + " unread articles.";
            homeLeft.Controls.Add(unreadNotice);
            
            HtmlGenericControl widgets = new HtmlGenericControl("div");
            widgets.ID = "widgets";

            refreshWeather = new Button();
            refreshWeather.ID = "refreshWeather";
            refreshWeather.Attributes["class"] = "button-secondary";
            refreshWeather.Text = "Refresh";
            refreshWeather.Click += loadWeather;

            weather = new UpdatePanel();
            weather.ID = "weather";
            AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
            trigger.ControlID = "refreshWeather";
            trigger.EventName = "Click";
            weather.Triggers.Add(trigger);
            
            location = new TextBox();
            location.Attributes["class"] = "city";
            location.Text = "Ljubljana, Slovenia";

            loadWeather(null, null);
            widgets.Controls.Add(weather);

            HtmlImage xkcd = new HtmlImage();
            xkcd.Src = "http://imgs.xkcd.com/comics/time.png";
            xkcd.Attributes["class"] = "full-widget";
            xkcd.Attributes["title"] = "The end.";
            xkcd.Alt = "Current time is unknown.";
            widgets.Controls.Add(xkcd);

            homeLeft.Controls.Add(widgets);

            reader.Controls.Add(homeLeft);

            HtmlGenericControl homeRight = new HtmlGenericControl("div");
            homeRight.Attributes["class"] = "home-right";

            command = connection.CreateCommand();
            command.CommandText = "SELECT a.id, a.title, a.url, a.author, a.date, a.content FROM Liked l JOIN Articles a ON l.article_id = a.id GROUP BY l.article_id ORDER BY COUNT(l.article_id) DESC";
            result = command.ExecuteReader();

            while(result.Read() == true)
            {
                HtmlGenericControl article = new HtmlGenericControl("article");

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
                likeButton.Attributes["onclick"] = "PageMethods.like(\"" + result.GetString("id") + "\")";
                likeButton.InnerHtml = "Like";

                actionBar.Controls.Add(likeButton);
                article.Controls.Add(actionBar);

                homeRight.Controls.Add(article);
            }

            result.Close();

            reader.Controls.Add(homeRight);
        }

        [WebMethod]
        public static void markAsRead(string id)
        {
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Unread WHERE user_id = @userID AND article_id = @articleID";
            command.Parameters.AddWithValue("@userID", HttpContext.Current.Session["userID"]);
            command.Parameters.AddWithValue("@articleID", id);

            result = command.ExecuteReader();
            bool unread = result.HasRows;
            result.Close();

            if(unread == true)
            {
                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Unread WHERE user_id = @userID AND article_id = @articleID";
                command.Parameters.AddWithValue("@userID", HttpContext.Current.Session["userID"]);
                command.Parameters.AddWithValue("@articleID", id);

                command.ExecuteNonQuery();
            }
            else
            {
                command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Unread VALUES (@userID, @articleID)";
                command.Parameters.AddWithValue("@userID", HttpContext.Current.Session["userID"]);
                command.Parameters.AddWithValue("@articleID", id);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        [WebMethod]
        public static void like(string id)
        {
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Liked WHERE user_id = @userID AND article_id = @articleID";
            command.Parameters.AddWithValue("@userID", HttpContext.Current.Session["userID"]);
            command.Parameters.AddWithValue("@articleID", id);

            result = command.ExecuteReader();
            bool liked = result.HasRows;
            result.Close();

            if(liked == true)
            {
                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Liked WHERE user_id = @userID AND article_id = @articleID";
                command.Parameters.AddWithValue("@userID", HttpContext.Current.Session["userID"]);
                command.Parameters.AddWithValue("@articleID", id);

                command.ExecuteNonQuery();
            }
            else
            {
                command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Liked VALUES (@userID, @articleID)";
                command.Parameters.AddWithValue("@userID", HttpContext.Current.Session["userID"]);
                command.Parameters.AddWithValue("@articleID", id);

                command.ExecuteNonQuery();

                command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Unread WHERE user_id = @userID AND article_id = @articleID";
                command.Parameters.AddWithValue("@userID", HttpContext.Current.Session["userID"]);
                command.Parameters.AddWithValue("@articleID", id);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        [WebMethod]
        public static void changeFeed(string location, string id)
        {
            HttpContext.Current.Session["location"] = location;
            HttpContext.Current.Session["feedID"] = Convert.ToInt32(id);
        }

        [WebMethod]
        public static void unsubscribe()
        {
            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Subscriptions WHERE user_id = @userID AND feed_id = @feedID";
            command.Parameters.AddWithValue("@userID", HttpContext.Current.Session["userID"]);
            command.Parameters.AddWithValue("@feedID", HttpContext.Current.Session["feedID"]);

            command.ExecuteNonQuery();
            connection.Close();

            HttpContext.Current.Session["location"] = "home";
            HttpContext.Current.Session["feedID"] = "-1";
        }

        [WebMethod]
        public static void signOut()
        {
            HttpContext.Current.Session["userID"] = null;
            HttpContext.Current.Session["location"] = null;
            HttpContext.Current.Session["feedID"] = null;
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

        protected void loadWeather(object sender, EventArgs e)
        {
            weather.ContentTemplateContainer.Controls.Clear();
            weather.ContentTemplateContainer.Controls.Add(refreshWeather);

            XmlReader reader = XmlReader.Create("http://www.myweather2.com/developer/forecast.ashx?uac=cOuhT9cxw6&output=xml&temp_unit=c&ws_unit=kph&query=46.056947,14.505751");
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            XmlNode current = doc.SelectSingleNode("//weather/curren_weather");

            HtmlGenericControl title = new HtmlGenericControl("span");
            title.Attributes["class"] = "weather-title";
            title.InnerHtml = "Weather ";
            weather.ContentTemplateContainer.Controls.Add(title);
            weather.ContentTemplateContainer.Controls.Add(location);

            HtmlGenericControl lineBreak = new HtmlGenericControl("span");
            lineBreak.Attributes["style"] = "display: block;";
            weather.ContentTemplateContainer.Controls.Add(lineBreak);

            HtmlImage currImage = new HtmlImage();
            currImage.Src = "/resources/weather/" + current["weather_code"].InnerText + ".gif";
            weather.ContentTemplateContainer.Controls.Add(currImage);

            HtmlGenericControl currTemp = new HtmlGenericControl("span");
            currTemp.Attributes["class"] = "temperature";
            currTemp.InnerHtml = current["temp"].InnerText + " °" + current["temp_unit"].InnerText.ToUpper();
            weather.ContentTemplateContainer.Controls.Add(currTemp);

            reader.Close();
        }
    }
}