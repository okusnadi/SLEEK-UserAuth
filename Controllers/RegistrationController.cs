using BrockAllen.MembershipReboot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityServer.Core;
using WebHost.MR;
using WebHost.ViewModels;

namespace WebHost.Controllers
{
    public class RegistrationController : Controller
    {
        UserAccountService<CustomUser> userAccountService;

        public RegistrationController(UserAccountService<CustomUser> userAccountService)
        {
            this.userAccountService = userAccountService;
        }

        //
        // GET: /Registration/
        public ActionResult Index(string signin)
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string signin, RegisterUser model)
        {
            if (ModelState.IsValid)
            {
                //UserAccountService<CustomUser> dd = new UserAccountService<CustomUser>();
                //dd.CreateAccount("", model.Username, "password", model.Email, dateCreated: DateTime.Now, account: model);

                //new CustomUserRepository().
                //new CustomUserAccountService().CreateUserAccount();

                using (var db = new CustomDatabase("MembershipReboot"))
                {
                    var svc = userAccountService; //GetUserAccountService(db);
                    //svc.Configuration.EmailIsUsername = true;
                    var account = svc.GetByUsername(model.UserName);
                    if (account == null)
                    {
                        account = svc.CreateAccount(model.UserName, model.Password, model.UserName);
                        
                        svc.AddClaim(account.ID, "name", model.LastName + " " + model.FirstName);

                        account.FirstName = model.FirstName;
                        account.LastName = model.LastName;
                        //account.Age = model.Age;
                        account.Organisation = model.Organisation;
                        account.Title = model.Title;
                        
                        svc.Update(account);
                    }
                }

                //new CustomUserAccountService().CreateAccount("uname", "password", "em@ail.com", dateCreated: DateTime.Now);
                
                return Redirect("~/" + Constants.RoutePaths.Login + "?signin=" + signin);
            }
            return View();
        }


        static UserAccountService<CustomUser> GetUserAccountService(CustomDatabase db)
        {
            var repo = new CustomUserRepository(db);
            UserAccountService<CustomUser> svc = new UserAccountService<CustomUser>(repo);
            return svc;
        }

        //GET: /Registration/SignOut
        public ActionResult SignOut()
        {
            this.Request.GetOwinContext().Authentication.SignOut();
            return this.Redirect("https://localhost:44333/connect/endsession");
        }
	}
}