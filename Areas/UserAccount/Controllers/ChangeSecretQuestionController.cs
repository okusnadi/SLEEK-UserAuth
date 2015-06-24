using WebHost.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System;
using BrockAllen.MembershipReboot;
using WebHost.MR;

namespace WebHost.Areas.UserAccount.Controllers
{
    [Authorize]
    public class ChangeSecretQuestionController : Controller
    {
        UserAccountService<CustomUser> userAccountService;
        public ChangeSecretQuestionController(UserAccountService<CustomUser> userAccountService)
        {
            this.userAccountService = userAccountService;
        }
        
        public ActionResult Index()
        {
            //var account = this.userAccountService.GetByID(User.GetUserID());
            var _claimsID = new System.Security.Claims.ClaimsIdentity(User.Identity);
            if (!_claimsID.HasClaim("sub"))
            {
                return new HttpUnauthorizedResult();
            }
            var account = this.userAccountService.GetByID(System.Guid.Parse(_claimsID.Claims.GetValue("sub")));
            var vm = new PasswordResetSecretsViewModel
            {
                Secrets = account.PasswordResetSecrets.ToArray()
            };
            return View("Index", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(Guid id)
        {
            //this.userAccountService.RemovePasswordResetSecret(User.GetUserID(), id);
            var _claimsID = new System.Security.Claims.ClaimsIdentity(User.Identity);
            if (!_claimsID.HasClaim("sub"))
            {
                return new HttpUnauthorizedResult();
            }
            this.userAccountService.RemovePasswordResetSecret(System.Guid.Parse(_claimsID.Claims.GetValue("sub")), id);
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddSecretQuestionInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //this.userAccountService.AddPasswordResetSecret(User.GetUserID(), model.Question, model.Answer);
                    var _claimsID = new System.Security.Claims.ClaimsIdentity(User.Identity);
                    if (!_claimsID.HasClaim("sub"))
                    {
                        return new HttpUnauthorizedResult();
                    }
                    this.userAccountService.AddPasswordResetSecret(System.Guid.Parse(_claimsID.Claims.GetValue("sub")), model.Question, model.Answer);
                    
                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View("Add", model);
        }
    }
}
