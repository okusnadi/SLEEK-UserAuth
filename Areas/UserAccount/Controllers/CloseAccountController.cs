using BrockAllen.MembershipReboot;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebHost.MR;

namespace WebHost.Areas.UserAccount.Controllers
{
    [Authorize]
    public class CloseAccountController : Controller
    {
        UserAccountService<CustomUser> userAccountService;
        public CloseAccountController(UserAccountService<CustomUser> userAccountService)
        {
            this.userAccountService = userAccountService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string button)
        {
            if (button == "yes")
            {
                try
                {
                    this.userAccountService.DeleteAccount(User.GetUserID());
                    return RedirectToAction("Index", "Logout");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View();
        }

    }
}
