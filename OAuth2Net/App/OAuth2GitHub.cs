using System;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OAuth2Net
{
    public class OAuth2GitHub : OAuth2App
    {
        public OAuth2GitHub(
            string client_id,
            string client_secret,
            string redirect_uri, 
            Action<OAuth2GitHub> success, 
            Action<OAuth2GitHub> failure = null) 
            : base(
                "https://github.com/login/oauth/authorize",
                "https://github.com/login/oauth/access_token", 
                client_id, 
                client_secret,
                redirect_uri, 
                null,
                success_api =>
                {
                    var gitHubApi = success_api as OAuth2GitHub;

                    using (var cli = gitHubApi.NewAuthorizedClient("application/vnd.github.machine-man-preview+json"))
                    {
                        var jsonText = cli.DownloadString("https://api.github.com/user");
                        var json = JObject.Parse(jsonText);

                        gitHubApi.PersonId = json["id"].Value<string>();
                        gitHubApi.PersonName = json["name"].Value<string>() ?? json["login"].Value<string>();
                        gitHubApi.PersonEmail = json["email"].Value<string>();
                        gitHubApi.PersonPhotoUrl = json["avatar_url"].Value<string>();
                        gitHubApi.PersonProfileUrl = json["html_url"].Value<string>();
                        gitHubApi.PersonLocation = json["location"].Value<string>();
                        gitHubApi.PersonInfo = json["bio"].Value<string>();
                    }

                    if (string.IsNullOrWhiteSpace(gitHubApi.PersonEmail))
                        try
                        {
                            using (var cli = gitHubApi.NewAuthorizedClient("application/vnd.github.machine-man-preview+json"))
                            {
                                var text = cli.DownloadString("https://api.github.com/user/emails");
                                var mails = JArray.Parse(text);

                                gitHubApi.PersonEmail =
                                    mails.Where(m => m["primary"].Value<bool>())
                                    .Select(m => m["email"].Value<string>())
                                    .FirstOrDefault();
                            }
                        }
                        catch
                        {
                            // tried
                        }
                                       

                    success((OAuth2GitHub)success_api);
                },
                failure_api => failure((OAuth2GitHub)failure_api))
        {
        }
    }
}
