using NUnit.Framework;
using OAuth2.LinkedInApi;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Tests
{
    public class LinkedinTests
    {
        HttpListener listener;
        OAuth2LinkedInApi api;
        string result;


        [SetUp]
        public void Setup()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:6625/auth/linkedin/");

            api = new OAuth2LinkedInApi(
                "86qi1hyfpuxefy",
                "LwaFNhDSzTYwHSKA",
                "http://localhost:6625/auth/linkedin/default.aspx",
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