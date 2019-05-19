using NUnit.Framework;
using OAuth2.GitHubApi;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Tests
{
    public class GithubTests
    {
        HttpListener listener;
        OAuth2GitHubApi api;
        string result;


        [SetUp]
        public void Setup()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:6625/auth/github/");

            api = new OAuth2GitHubApi(
                "5e52550211ef44b46330",
                "a892638b5995ece53b16256e9182364fff549c28",
                "http://localhost:6625/auth/github/default.aspx",
                success: api => result = "success",
                failure: api => result = "failure");
        }


        [Test]
        public void Test1()
        {
            var authUrl = api.GetAuthorizationUrl();

            var cli = new WebClient();

            cli.Encoding = Encoding.UTF8;

            listener.Start();

            var context = listener.GetContext();


            OAuth2.Api.OAuth2Api.Callback(
                context.Request.QueryString["code"],
                context.Request.QueryString["error"],
                context.Request.QueryString["state"],
                context.Request.QueryString["error_description"],
                context.Request.QueryString["error_uri"]);




        }
    }
}