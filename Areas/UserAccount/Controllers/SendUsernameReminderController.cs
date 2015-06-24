using WebHost.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using WebHost.MR;

namespace WebHost.Areas.UserAccount.Controllers
{
    public class SendUsernameReminderController : Controller
    {
        UserAccountService<CustomUser> userAccountService;

        public SendUsernameReminderController(UserAccountService<CustomUser> userAccountService)
        {
            this.userAccountService = userAccountService;
        }

        public ActionResult Index()
        {            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SendUsernameReminderInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {                   
                    this.userAccountService.SendUsernameReminder(model.Email);
                    ViewData["Email"] = model.Email;
                    return View("Success");
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
