using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace OAuth2Net
{
    public class OAuth2LinkedIn : OAuth2App
    {        
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string PersonEmail { get; set; }
        public string PersonPhotoUrl { get; set; }

        public OAuth2LinkedIn(
            string client_id,
            string client_secret,
            string redirect_uri,
            Action<OAuth2LinkedIn> success,
            Action<OAuth2LinkedIn> failure,
            string scope = "r_liteprofile r_emailaddress")
            : base( 
                "https://www.linkedin.com/oauth/v2/authorization",
                "https://www.linkedin.com/oauth/v2/accessToken",
                client_id,
                redirect_uri,
                scope,
                success_api => 
                {
                    var linkedInApi = success_api as OAuth2LinkedIn;
                    var profileJson = JObject.Parse(linkedInApi.GetProfileJson());
                    var emailJson = JObject.Parse(linkedInApi.GetEmailJson());

                    linkedInApi.PersonId = profileJson["id"].Value<string>();

                    linkedInApi.PersonName =
                        profileJson["firstName"]?["localized"].First.First.Value<string>()
                        + " " + profileJson["lastName"]?["localized"].First.First.Value<string>();

                    linkedInApi.PersonEmail =
                        emailJson["elements"].First["handle~"]?["emailAddress"].Value<string>();

                    linkedInApi.PersonPhotoUrl =
                        profileJson["profilePicture"]?["displayImage~"]?["elements"]
                        .Select(el =>
                            new
                            {
                                width = el["data"]?["com.linkedin.digitalmedia.mediaartifact.StillImage"]?["storageSize"]?["width"].Value<int>(),
                                url = el["identifiers"].First["identifier"].Value<string>()
                            })
                            .OrderByDescending(el => el.width)
                            .Select(el => el.url)
                            .FirstOrDefault();

                    success(linkedInApi);
                },
                failure_api => failure((OAuth2LinkedIn)failure_api))
        {
            AuthorizationParams["client_secret"] = client_secret;
        }


        public WebClient NewClient(string agent = null)
        {
            var cli = new WebClient();

            cli.Headers["Authorization"] = "Bearer " + AccessToken;
            cli.Headers["User-Agent"] = agent ?? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";
            cli.Encoding = Encoding.UTF8;

            return cli;
        }


        public string GetProfileJson()
        {
            return NewClient().DownloadString(
                "https://api.linkedin.com/v2/me" +
                "?projection=(id,firstName,lastName,profilePicture(displayImage~:playableStreams))");
        }


        public string GetEmailJson()
        {
            return NewClient().DownloadString(
                "https://api.linkedin.com/v2/emailAddress" +
                "?q=members&projection=(elements*(handle~))");
        }
    }
}
