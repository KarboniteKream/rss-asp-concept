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
    public partial class Default : System.Web.UI.Page
    {
        private static MySqlConnection connection = null;
        private static MySqlCommand command = null;
        private static MySqlDataReader result = null;

        private string connectionString = "server=3020f0c4-873a-49b6-b007-a3ff00933a9e.mysql.sequelizer.com;database=db3020f0c4873a49b6b007a3ff00933a9e;userid=evvbdlzgyodaumqz;password=iGSHBCF2WwpzrmRXjhhCbUTiDfjnk3c3MvECQzWQt8pTnD7VZsZNwi3wVHevstQ3";

        protected void Page_Load(object sender, EventArgs e)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();

            loadFeatured();

            connection.Close();
        }

        private void loadFeatured()
        {
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

                featured.Controls.Add(article);
            }

            result.Close();
        }

        protected void ValidateSignIn(object sender, ServerValidateEventArgs args)
        {
            if(email.Text.Equals("") == true || password.Text.Equals("") == true)
            {
                args.IsValid = true;
                return;
            }

            connection.Open();

            command = connection.CreateCommand();
            command.CommandText = "SELECT id FROM Users WHERE email = @email AND password = SHA1(@password)";
            command.Parameters.AddWithValue("@email", email.Text);
            command.Parameters.AddWithValue("@password", password.Text);
            result = command.ExecuteReader();

            if(result.HasRows == false)
            {
                result.Close();
                connection.Close();

                ((CustomValidator)sender).ErrorMessage = "Incorrect e-mail/password.";
                args.IsValid = false;
                return;
            }

            result.Read();
            Session["userID"] = result.GetInt32("id");
            Session["location"] = "home";
            Session["feedID"] = -1;

            result.Close();
            connection.Close();

            args.IsValid = true;
            Response.Redirect("/Home.aspx");
        }

        protected void ValidateRegister(object sender, ServerValidateEventArgs args)
        {
            if(realName.Text.Equals("") == true || newEmail.Text.Equals("") == true || newPassword1.Text.Equals("") == true || newPassword2.Text.Equals("") == true)
            {
                ((CustomValidator)sender).ErrorMessage = "";
                args.IsValid = false;
                return;
            }

            if(newPassword1.Text.Equals(newPassword2.Text) == false)
            {
                ((CustomValidator)sender).ErrorMessage = "Passwords do not match.";
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
            command.CommandText = "INSERT INTO Users (real_name, email, password) VALUES (@realName, @email, SHA1(@password))";
            command.Parameters.AddWithValue("@realName", realName.Text);
            command.Parameters.AddWithValue("@email", newEmail.Text);
            command.Parameters.AddWithValue("@password", newPassword1.Text);
            command.ExecuteNonQuery();

            Session["userID"] = command.LastInsertedId;
            Session["location"] = "home";
            Session["feedID"] = -1;

            connection.Close();

            args.IsValid = true;
            Response.Redirect("/Home.aspx");
        }
    }
}