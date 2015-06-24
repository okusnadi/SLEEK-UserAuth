using WebHost.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using WebHost.MR;

namespace WebHost.Areas.UserAccount.Controllers
{
    [Authorize]
    public class ChangeEmailController : Controller
    {
        UserAccountService<CustomUser> userAccountService;
        AuthenticationService<CustomUser> authSvc;

        public ChangeEmailController(AuthenticationService<CustomUser> authSvc)
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
        public ActionResult Index(ChangeEmailRequestInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //this.userAccountService.ChangeEmailRequest(User.GetUserID(), model.NewEmail);

                    var _claimsID = new System.Security.Claims.ClaimsIdentity(User.Identity);
                    if (!_claimsID.HasClaim("sub"))
                    {
                        return new HttpUnauthorizedResult();
                    }
                    this.userAccountService.ChangeEmailRequest(System.Guid.Parse(_claimsID.Claims.GetValue("sub")), model.NewEmail);

                    if (userAccountService.Configuration.RequireAccountVerification)
                    {                       
                        return View("ChangeRequestSuccess", (object)model.NewEmail);
                    }
                    else
                    {
                        return View("Success");
                    }
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View("Index", model);
        }

        [AllowAnonymous]
        public ActionResult Confirm(string id)
        {
            var account = this.userAccountService.GetByVerificationKey(id);
            if (account.HasPassword())
            {                
                var vm = new ChangeEmailFromKeyInputModel();
                vm.Key = id;
                return View("Confirm", vm);
            }
            else
            {
                try
                {
                    userAccountService.VerifyEmailFromKey(id, out account);
                    // since we've changed the email, we need to re-issue the cookie that
                    // contains the claims.
                    authSvc.SignIn(account);
                    return RedirectToAction("Success");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                return View("Confirm", null);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(ChangeEmailFromKeyInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //BrockAllen.MembershipReboot.UserAccount account;
                    CustomUser account;
                    var kk = "FDD848F09A522C94502316625E0149C8B6E61FD1E4E9904EF1B85AE8552736E2"; //
                    this.userAccountService.VerifyEmailFromKey(model.Key, model.Password, out account);
                    
                    // since we've changed the email, we need to re-issue the cookie that
                    // contains the claims.
                    authSvc.SignIn(account);
                    return RedirectToAction("Success");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            return View("Confirm", model);
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}
