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
        public ActionResult Publish()
        {
            if (Request.QueryString["publish"] == "1")
            {
                if (Session["AccessToken"] != null)
                {
                    FacebookClass.postToFacebook(Session["AccessToken"]);
                    Session["Facebook"] = "√";
                }
                else
                {
                    FacebookClass.loginToFacebook();
                }
            }

            else if (Request.QueryString["code"] != null)
            {
                FacebookClass.getAccessToken();
                Session["Facebook"] = "√";
            }
            else if (Request.QueryString["error"] != null)
            {
                // Notify the user as you like
                string error = Request.QueryString["error"];
                string errorResponse = Request.QueryString["error_reason"];
                string errorDescription = Request.QueryString["error_description"];
                ViewBag.Text = errorDescription;
            }
            
            Session["LinkedIn"] = "√";
            
            ViewBag.Facebook = Session["Facebook"];
            ViewBag.LinkedIn = Session["LinkedIn"];
            return View();
        }

       
        public ActionResult Logout()
        {
            Session.Remove("AccessToken");
            ViewBag.message = "App logged out of facebook";
            return View();
        }
       

       
    }


}