using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;

namespace ToFacebook1
{
    public static class FacebookClass
    {


        public static void loginToFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {

                client_id = "1734189983463496",

                redirect_uri = "http://localhost:65241/home/publish",

                response_type = "code",

                scope = "email,user_likes,publish_actions,manage_pages, publish_pages" // Add other permissions as needed

            });
            HttpContext.Current.Response.Redirect(loginUrl.AbsoluteUri);  // User not connected, ask them to sign in again
        }

        public static void getAccessToken()
        {
            if (HttpContext.Current.Request.QueryString["code"] != null)
            {
                string accessCode = HttpContext.Current.Request.QueryString["code"].ToString();

                var fb = new FacebookClient();

                // throws OAuthException 
                dynamic result = fb.Post("oauth/access_token", new
                {

                    client_id = "1734189983463496",

                    client_secret = "e5ad58f505c92f68e7538ad5f10796f7",

                    redirect_uri = "http://localhost:65241/home/publish",

                    code = accessCode

                });
                
                var accessToken = result.access_token;
                HttpContext.Current.Session["AccessToken"] = result.access_token;
                postToFacebook(HttpContext.Current.Session["AccessToken"]);


            }
            else if (HttpContext.Current.Request.QueryString["error"] != null)
            {
                // Notify the user as you like
                string error = HttpContext.Current.Request.QueryString["error"];
                string errorResponse = HttpContext.Current.Request.QueryString["error_reason"];
                string errorDescription = HttpContext.Current.Request.QueryString["error_description"];

                
            }
        }

        public static void postToFacebook(dynamic accessToken)
        {

            var fb = new FacebookClient();
            // update the facebook client with the access token 
            fb.AccessToken = accessToken;

            string pageAccessToken = "";
            JsonObject jsonResponse = fb.Get("me/accounts") as JsonObject;
            foreach (var account in (JsonArray)jsonResponse["data"])
            {
                string accountName = (string)(((JsonObject)account)["name"]);

                if (accountName == "Datasmörj")
                {
                    pageAccessToken = (string)(((JsonObject)account)["access_token"]);
                    break;
                }
            }
            var client = new FacebookClient(pageAccessToken);
            Dictionary<string, object> fbParams = new Dictionary<string, object>();
            fbParams["message"] = "Test message" + new Random().Next(int.MinValue, int.MaxValue).ToString();
            var publishedResponse = client.Post("/datasmorj/feed", fbParams);
            

        }


    }
}