using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;

namespace ToFacebook1.Controllers
{
    public class HomeController : Controller
    {
        
        public string Access_Token { get; set; }

        public ActionResult Start() {
            // Check if already Signed In
            if (Session["AccessToken"] != null)
            {
                
                Response.Redirect("http://localhost:65241/home/index");
            }
           
            else
            {
              
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {

                client_id = "1734189983463496",

                redirect_uri = "http://localhost:65241/home/index",

                response_type = "code",

                scope = "email,user_likes,publish_actions,manage_pages, publish_pages" // Add other permissions as needed

            });
            Response.Redirect(loginUrl.AbsoluteUri);  // User not connected, ask them to sign in again
            ViewBag.Text = "Not Signed In";
            }
            return View();
        }
        
        public ActionResult Index()
        {
            // Check if already Signed In
            if (Session["AccessToken"] != null)
            {
              postToFacebook(Session["AccessToken"]);
            }
           

            else if (Request.QueryString["code"] != null)
            {
                string accessCode = Request.QueryString["code"].ToString();

                var fb = new FacebookClient();

                // throws OAuthException 
                dynamic result = fb.Post("oauth/access_token", new
                {

                    client_id = "1734189983463496",

                    client_secret = "e5ad58f505c92f68e7538ad5f10796f7",

                    redirect_uri = "http://localhost:65241/home/index",

                    code = accessCode

                });

                var accessToken = result.access_token;
                Session["AccessToken"] = result.access_token;
                postToFacebook(Session["AccessToken"]);


            }

            else if (Request.QueryString["error"] != null)
            {
                // Notify the user as you like
                string error = Request.QueryString["error"];
                string errorResponse = Request.QueryString["error_reason"];
                string errorDescription = Request.QueryString["error_description"];

                ViewBag.Text = errorDescription;
            }

            else
            {
                // User not connected, ask them to sign in again
                ViewBag.Text = "Not Signed In";
            }
            
            return View();
        }
        public void postToFacebook(dynamic accessToken) {

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
            // Calling Graph API for user info
            //dynamic me = fb.Get("me?fields=friends,name,email");
            //string id = me.id; // You can store it in the database
            //string name = me.name;
            //string email = me.email;
            //ViewBag.id = id;
            //ViewBag.name = name;
            ViewBag.email = "Sucess";

        }
    }

    
}