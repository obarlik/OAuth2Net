using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace OAuth2Net
{
    public class OAuth2Facebook : OAuth2App
    {
        public string PersonId { get; protected set; }
        public string PersonName { get; protected set; }
        public string PersonEmail { get; protected set; }
        public string PersonPhotoUrl { get; protected set; }


        public OAuth2Facebook(
                string client_id,
                string client_secret,
                string redirect_uri,
                Action<OAuth2Facebook> success,
                Action<OAuth2Facebook> failure,
                string scope = "public_profile,email") : base(
            "https://www.facebook.com/v3.3/dialog/oauth",
            "https://graph.facebook.com/v3.3/oauth/access_token",
            client_id,
            client_secret,
            redirect_uri,
            scope,
            success: api =>
            {
                var facebook = api as OAuth2Facebook;

                using (var cli = facebook.NewAuthorizedClient("application/json"))
                {
                    var json = cli.DownloadString($"https://graph.facebook.com/v3.3/me?access_token={facebook.AccessToken}&fields=id,name,email");
                    var data = JObject.Parse(json);

                    facebook.PersonId = data["id"].Value<string>();
                    facebook.PersonName = data["name"].Value<string>();
                    facebook.PersonEmail = data["email"].Value<string>();
                }

                using (var cli = facebook.NewAuthorizedClient("application/json"))
                {
                    var json = cli.DownloadString($"https://graph.facebook.com/{facebook.PersonId}/picture?type=large&redirect=false");
                    var data = JObject.Parse(json);

                    facebook.PersonPhotoUrl = data["data"]?["url"].Value<string>();
                }

                success?.Invoke(facebook);
            })
        {
        }
    }
}
