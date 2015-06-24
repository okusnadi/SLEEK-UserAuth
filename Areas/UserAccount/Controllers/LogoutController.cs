using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebHost.Areas.UserAccount.Controllers
{
    public class LogoutController : Controller
    {
        //
        // GET: /UserAccount/Logout/
        public ActionResult Index()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");  //View();
        }
	}
}