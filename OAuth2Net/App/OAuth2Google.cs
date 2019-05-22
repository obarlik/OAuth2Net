using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace OAuth2Net
{
    public class OAuth2Google : OAuth2App
    {
        public OAuth2Google(
                string client_id,
                string client_secret,
                string redirect_uri,
                Action<OAuth2Google> success,
                Action<OAuth2Google> failure,
                string scope = "openid profile email") 
            : base(
            openIdDiscoveryUrl: "https://accounts.google.com/.well-known/openid-configuration",
            client_id,
            client_secret,
            redirect_uri,
            scope,
            success: api => {
                var google = api as OAuth2Google;
                
                using (var cli = google.NewAuthorizedClient("application/json"))
                {
                    var json = cli.DownloadString(google.UserInfoEndpoint);
                    var data = JObject.Parse(json);

                    google.PersonId = data["sub"].Value<string>();
                    google.PersonName = data["name"].Value<string>();
                    google.PersonEmail = data["email"].Value<string>();
                    google.PersonPhotoUrl = data["picture"].Value<string>();
                    google.PersonProfileUrl = data["profile"].Value<string>();
                    google.PersonLocale = data["locale"].Value<string>();
                }

                success?.Invoke(google);
            })
        {
        }

    }
}
