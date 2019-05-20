﻿using Newtonsoft.Json.Linq;
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
            success: api => {
                var facebook = api as OAuth2Facebook;

                var cli = facebook.NewClient();
                var json = cli.DownloadString("https://graph.facebook.com/v3.3/me?fields=id,name,picture,email");
                var data = JObject.Parse(json);

                facebook.PersonId = data["id"].Value<string>();
                facebook.PersonName = data["name"].Value<string>();
                facebook.PersonPhotoUrl = data["picture"].Value<string>();
                facebook.PersonEmail = data["email"].Value<string>();

                success?.Invoke(facebook);
            })
        {
        }


        public WebClient NewClient(string agent = null)
        {
            var cli = new WebClient();

            cli.Headers["Accept"] = "application/json";
            cli.Headers["User-Agent"] = agent ?? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
            cli.Headers["Authorization"] = $"{AccessTokenType} {AccessToken}";
            cli.Encoding = Encoding.UTF8;

            return cli;
        }
    }
}