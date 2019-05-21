﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OAuth2Net.Demo
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        void LoginUser(
            string id,
            string name,
            string email,
            string pictureUrl,
            params string[] roles)
        {
            var claims = new List<Claim>();

            claims.AddRange(
                new[] {
                    new Claim(ClaimTypes.Sid, id),
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email),
                    new Claim("ProfileImageUrl", pictureUrl)
                });

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var ticket = new FormsAuthenticationTicket(
               1,
               email,
               DateTime.Now,
               DateTime.Now + FormsAuthentication.Timeout,
               true,
               Newtonsoft.Json.JsonConvert.SerializeObject(claims.ToArray()));

            HttpContext.Current.Response.Cookies.Add(
                new HttpCookie(
                    FormsAuthentication.FormsCookieName,
                    FormsAuthentication.Encrypt(ticket)));
        }


        protected void LinkedInLoginBtn_Click(object sender, EventArgs e)
        {
            var linkedIn = new OAuth2LinkedIn(
                "86lgvuhdop1tce",
                "KVVScEdl1fbpTVyW",
                "http://localhost:6625/auth/linkedin/default.aspx",
                success: api =>
                {
                    LoginUser(
                        "LinkedIn-" + api.PersonId,
                        api.PersonName,
                        api.PersonEmail,
                        api.PersonPhotoUrl,
                        "User");

                    HttpContext.Current.Response.Redirect(api.ReturnUrl);
                },
                failure: api =>
                {
                    HttpContext.Current.Response.Redirect(api.ReturnUrl);
                });

            Response.Redirect(linkedIn.GetAuthorizationUrl("/"));
        }



        protected void GithubLoginBtn_Click(object sender, EventArgs e)
        {
            var gitHub = new OAuth2GitHub(
                "Iv1.6d67c0533ba8029c",
                "5e413805b8cb0d1fee5b07fd330f94e205939060",
                "http://localhost:6625/auth/github/default.aspx",
                success: api =>
                {
                    LoginUser(
                        "GitHub-" + api.PersonId,
                        api.PersonName,
                        api.PersonEmail,
                        api.PersonPhotoUrl,
                        "User");

                    HttpContext.Current.Response.Redirect(api.ReturnUrl);
                },
                failure: api =>
                {
                    HttpContext.Current.Response.Redirect(api.ReturnUrl);
                });

            Response.Redirect(gitHub.GetAuthorizationUrl("/"));
        }

        protected void FacebookLoginBtn_Click(object sender, EventArgs e)
        {
            var facebook = new OAuth2Facebook(
                "2293070497624844",
                "c530b7a2614a93b328f0c170c6fcd001",
                "http://localhost:6625/auth/facebook/default.aspx",
                success: api =>
                {
                    LoginUser(
                        "Facebook-" + api.PersonId,
                        api.PersonName,
                        api.PersonEmail,
                        api.PersonPhotoUrl,
                        "User");

                    HttpContext.Current.Response.Redirect(api.ReturnUrl);
                },
                failure: api =>
                {
                    HttpContext.Current.Response.Redirect(api.ReturnUrl);
                });

            Response.Redirect(facebook.GetAuthorizationUrl("/"));
        }
    }
}