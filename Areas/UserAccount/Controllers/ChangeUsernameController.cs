using WebHost.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using WebHost.MR;

namespace WebHost.Areas.UserAccount.Controllers
{
    [Authorize]
    public class ChangeUsernameController : Controller
    {
        UserAccountService<CustomUser> userAccountService;
        AuthenticationService<CustomUser> authSvc;

        public ChangeUsernameController(AuthenticationService<CustomUser> authSvc)
        {
            this.userAccountService = authSvc.UserAccountService;
            this.authSvc = authSvc;
        }

        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ChangeUsernameInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //this.userAccountService.ChangeUsername(User.GetUserID(), model.NewUsername);                    
                    var _claimsID = new System.Security.Claims.ClaimsIdentity(User.Identity);
                    if (!_claimsID.HasClaim("sub"))
                    {
                        return new HttpUnauthorizedResult();
                    }
                    this.userAccountService.ChangeUsername(System.Guid.Parse(_claimsID.Claims.GetValue("sub")), model.NewUsername);

                    this.authSvc.SignIn(User.GetUserID());
                    return RedirectToAction("Success");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View("Index", model);
        }

        public ActionResult Success()
        {
            return View("Success", (object)User.Identity.Name);
        }
    }
}
