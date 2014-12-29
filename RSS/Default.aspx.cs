using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RSS
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void qwe_Click(object sender, EventArgs e)
        {
            Session["userID"] = 1;
            Session["location"] = "feed";
            Session["feedID"] = 25;
            Response.Redirect("Home.aspx");
        }
    }
}