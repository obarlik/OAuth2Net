using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace OAuth2Net.Demo
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }


        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var id = Context?.User?.Identity as FormsIdentity;

            if (id == null || string.IsNullOrWhiteSpace(id.Ticket.UserData))
                return;

            var claims = JArray.Parse(id.Ticket.UserData);

            foreach(var claim in claims)
            {
                id.AddClaim(new Claim(claim["Type"].Value<string>(), claim["Value"].Value<string>()));
            }

        }
    }
}