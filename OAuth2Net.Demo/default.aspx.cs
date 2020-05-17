using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OAuth2Net.Demo
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PersonId.Text = (string)Session["PersonId"];
            PersonName.Text = (string)Session["PersonName"];
            PersonEmail.Text = (string)Session["PersonEmail"];
            PersonPicture.ImageUrl = (string)Session["PersonPicture"];
        }



        


        protected void LinkedInLoginBtn_Click(object sender, EventArgs e)
        {
            var linkedIn = new OAuth2LinkedIn(
                "86lgvuhdop1tce",
                "KVVScEdl1fbpTVyW",
                "http://localhost:6625/auth/linkedin/default.aspx");

            Response.Redirect(linkedIn.GetAuthorizationUrl("/"));
        }


        protected void GithubLoginBtn_Click(object sender, EventArgs e)
        {
            var gitHub = new OAuth2GitHub(
                "Iv1.6d67c0533ba8029c",
                "5e413805b8cb0d1fee5b07fd330f94e205939060",
                "http://localhost:6625/auth/github/default.aspx");

            Response.Redirect(gitHub.GetAuthorizationUrl("/"));
        }

        protected void FacebookLoginBtn_Click(object sender, EventArgs e)
        {
            var facebook = new OAuth2Facebook(
                "2825308727576238",
                "ec0ef89c230451f91a510e00f217c1a2",
                "http://localhost:6625/auth/facebook/default.aspx");

            Response.Redirect(facebook.GetAuthorizationUrl("/"));
        }

        protected void GoogleLoginBtn_Click(object sender, EventArgs e)
        {
            var google = new OAuth2Google(
                "13833862291-comaorjrjsp4vl85am44qkt3b7vqlip2.apps.googleusercontent.com",
                "wnWzkiapl7aLY7bKMiRjTend",
                "http://localhost:6625/auth/google/default.aspx");

            Response.Redirect(google.GetAuthorizationUrl("/"));
        }
    }
}