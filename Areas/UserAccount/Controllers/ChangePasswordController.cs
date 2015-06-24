using WebHost.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using WebHost.MR;

namespace WebHost.Areas.UserAccount.Controllers
{
    //[Authorize]
    public class ChangePasswordController : Controller
    {
        UserAccountService<CustomUser> userAccountService;
        public ChangePasswordController(UserAccountService<CustomUser> userAccountService)
        {
            this.userAccountService = userAccountService;
        }
        
        public ActionResult Index()
        {            
            //if (!User.HasUserID())
            //{
            //    return new HttpUnauthorizedResult();
            //}
            //var acct = this.userAccountService.GetByID(User.GetUserID());
            
            var _claimsID = new System.Security.Claims.ClaimsIdentity(User.Identity);
            if (!_claimsID.HasClaim("sub"))
            {
                return new HttpUnauthorizedResult();
            }
            //var acct = this.userAccountService.GetByID(System.Guid.Parse(Thinktecture.IdentityServer.Core.Extensions.PrincipalExtensions.GetSubjectId(this.User)));
            var acct = this.userAccountService.GetByID(System.Guid.Parse(_claimsID.Claims.GetValue("sub")));
            
            if (acct.HasPassword())
            {
                return View(new ChangePasswordInputModel());
            }
            else
            {
                return View("SendPasswordReset");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ChangePasswordInputModel model)
        {
            //if (!User.HasUserID())
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var _claimsID = new System.Security.Claims.ClaimsIdentity(User.Identity);
            if (!_claimsID.HasClaim("sub"))
            {
                return new HttpUnauthorizedResult();
            }
            var acctID = System.Guid.Parse(_claimsID.Claims.GetValue("sub"));


            if (ModelState.IsValid)
            {
                try
                {
                    //this.userAccountService.ChangePassword(User.GetUserID(), model.OldPassword, model.NewPassword);
                    this.userAccountService.ChangePassword(acctID, model.OldPassword, model.NewPassword);
                    return View("Success");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendPasswordReset()
        {
            //if (!User.HasUserID())
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var _claimsID = new System.Security.Claims.ClaimsIdentity(User.Identity);
            if (!_claimsID.HasClaim("sub"))
            {
                return new HttpUnauthorizedResult();
            }            

            try
            {
                //var acct = this.userAccountService.GetByID(User.GetUserID());
                var acct = this.userAccountService.GetByID(System.Guid.Parse(_claimsID.Claims.GetValue("sub")));
                this.userAccountService.ResetPassword(acct.Tenant, acct.Email);
                return View("Sent");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("SendPasswordReset");
        }
    }
}
