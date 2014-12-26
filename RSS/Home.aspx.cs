using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using MySql.Data.MySqlClient;

namespace RSS
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["UserID"] == null)
            {
                // Response.Redirect("/");
            }

            Session["UserID"] = 1;
            Session["Location"] = "feed";
            Session["FeedID"] = 1;

            MySqlConnection connection = new MySqlConnection("server=kream.io;database=rss;userid=root;password=gasilec");
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM Feeds WHERE id = @id";
            command.Parameters.AddWithValue("@id", Session["FeedID"]);

            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            feedName.Text = reader.GetString("name");
            reader.Close();

            command = connection.CreateCommand();
            command.CommandText = "SELECT f.id, f.name, f.icon, u.unread FROM Subscriptions s JOIN Feeds f ON s.feed_id = f.id LEFT JOIN (SELECT a.feed_id, COUNT(a.feed_id) AS unread FROM Unread JOIN Articles a ON article_id = a.id WHERE user_id = @user_id GROUP BY feed_id) AS u ON f.id = u.feed_id WHERE s.user_id = @user_id AND s.folder IS NULL ORDER BY f.name";
            command.Parameters.AddWithValue("@user_id", Session["UserID"]);
            reader = command.ExecuteReader();

            HtmlGenericControl noFolder = new HtmlGenericControl("ul");
            noFolder.Attributes["class"] = "connected sortable";

            while(reader.Read() == true)
            {
                HtmlGenericControl feed = new HtmlGenericControl("li");
                HtmlGenericControl link = new HtmlGenericControl("a");
                link.ID = reader.GetString("id");

                if(link.ID.Equals(Session["FeedID"].ToString()) == true)
                {
                    link.Attributes["class"] = "active";
                }

                link.Attributes["style"] = "background-image: url(data:image/png;base64," + Convert.ToBase64String((byte[])reader.GetValue(2)) + ");";
                link.Attributes["href"] = "#";
                link.InnerHtml = reader.GetString("name");

                if(reader.IsDBNull(3) == false)
                {
                    link.InnerHtml += " <span class=\"badge\">" + reader.GetString("unread") + "</span>";
                }

                feed.Controls.Add(link);
                noFolder.Controls.Add(feed);
            }

            HtmlGenericControl empty = new HtmlGenericControl("li");
            empty.Attributes["class"] = "empty-li";
            noFolder.Controls.Add(empty);

            subscriptions.Controls.Add(noFolder);
            reader.Close();

            command = connection.CreateCommand();
            command.CommandText = "SELECT f.id, f.name, f.icon, u.unread, s.folder FROM Subscriptions s JOIN Feeds f ON s.feed_id = f.id LEFT JOIN (SELECT a.feed_id, COUNT(a.feed_id) AS unread FROM Unread JOIN Articles a ON article_id = a.id WHERE user_id = @user_id GROUP BY feed_id) AS u ON f.id = u.feed_id WHERE s.user_id = @user_id AND s.folder IS NOT NULL ORDER BY s.folder, f.name";
            command.Parameters.AddWithValue("@user_id", Session["UserID"]);
            reader = command.ExecuteReader();

            HtmlGenericControl folders = new HtmlGenericControl("ul");
            HtmlGenericControl folder = null, input, label, feeds = null;
            string previousFolder = null;

            while(reader.Read() == true)
            {
                string currentFolder = reader.GetString("folder");

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
                link.ID = reader.GetString("id");

                if(link.ID.Equals(Session["FeedID"].ToString()) == true)
                {
                    link.Attributes["class"] = "active";
                }

                link.Attributes["style"] = "background-image: url(data:image/png;base64," + Convert.ToBase64String((byte[])reader.GetValue(2)) + ");";
                link.Attributes["href"] = "#";
                link.InnerHtml = reader.GetString("name");

                if (reader.IsDBNull(3) == false)
                {
                    link.InnerHtml += " <span class=\"badge\">" + reader.GetString("unread") + "</span>";
                }

                feed.Controls.Add(link);
                feeds.Controls.Add(feed);

                folders.Controls.Add(folder);
            }

            subscriptions.Controls.Add(folders);
            reader.Close();

            command = connection.CreateCommand();
            command.CommandText = "SELECT id, real_name, email, cookie FROM Users";
            //command.Parameters.AddWithValue("@id", Session["UserID"]);

            DataTable dataTable = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(dataTable);
            
            readerView.DataSource = dataTable;
            readerView.DataBind();

            connection.Close();
        }
    }
}