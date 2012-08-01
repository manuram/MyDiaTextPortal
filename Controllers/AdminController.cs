using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Telerik.Web.Mvc;
using SeniorDesign.Models;
using SeniorDesign.Repository;

namespace SeniorDesign.Controllers
{   
    [HandleError,Authorize(Users = "Administrator")]
    public class AdminController : Controller
    {
        private ISmsMessageRepository SmsMessageRepository;
        private IResponseGeneratorRepository ResponseGeneratorRepository;

        public AdminController()
        {
            this.SmsMessageRepository = new SmsMessageRepository(new SdContext());
            this.ResponseGeneratorRepository = new ResponseGeneratorRepository(new SdContext());
        }

        public AdminController(ISmsMessageRepository SmsMessageRepository, IResponseGeneratorRepository ResponseGeneratorRepository)
        {
            this.SmsMessageRepository = SmsMessageRepository;
            this.ResponseGeneratorRepository = ResponseGeneratorRepository;
        }

        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Admin/History/

        public ActionResult History(string username)
        {
            ViewBag.username = username;
            return View();
        }

        [GridAction]
        public ActionResult _AjaxBinding(string username)
        {
            IEnumerable<SmsMessage> model = null;
            if (username == null)
            {
                ViewBag.Title = "History for all users";
                model = SmsMessageRepository.GetSmsMessages();
            }
            else
            {
                ViewBag.Title = string.Format("History for user \"{0}\".", username);
                model = SmsMessageRepository.GetSmsMessagesByUserName(username);
            }
            return View(new GridModel<SmsMessage>(model));
        }
        
        // GET: /Admin/Users

        public ActionResult Users()
        {
            var users = Membership.GetAllUsers();
            return View(users);
        }

        //
        // GET: /Admin/DeleteUser

        public ActionResult DeleteUser()
        {
            ViewBag.username = Request["username"];
            return View();
        }

        //
        // POST: /Admin/DeleteUser

        [HttpPost]
        public ActionResult DeleteUser(string username, string confirm)
        {
            if (confirm != "PERMANENTLY DELETE THIS USER") return View("Index");
            if (Membership.DeleteUser(username))
            {
                TempData["ErrorMessage"] = string.Format("User \"{0}\" was deleted!", username);
            }
            else
            {
                TempData["ErrorMessage"] = "User could not be deleted";
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Admin/DisableUser

        public ActionResult DisableUser()
        {
            ViewBag.username = Request["username"];
            return View();
        } 

        //
        // POST: /Admin/DisableUser
        [HttpPost]
        public ActionResult DisableUser(string username, string confirm)
        {
            if (confirm != "DISABLE THIS USER") return View("Index");
            MembershipUser m = Membership.GetUser(username);
            if (m.IsApproved)
            {
                m.IsApproved = false;
                TempData["ErrorMessage"] = string.Format("User \"{0}\" was disabled", username);
            }
            else
            {
                m.IsApproved = true;
                TempData["ErrorMessage"] = string.Format("User \"{0}\" was enabled", username);
            }
            Membership.UpdateUser(m);
            return RedirectToAction("Users");
        } 

        // 
        // GET: /Admin/Profile/username

        public ActionResult Profile(string username)
        {
            MDTProfile p = new MDTProfile().GetProfile(username);
            ViewBag.Phone = p.Phone;
            ViewBag.FirstName = p.FirstName;
            ViewBag.LastName = p.LastName;
            ViewBag.LastProfileChange = p.LastActivityDate;
            ViewBag.Username = p.UserName;
            ViewBag.ReferralCode = p.referralCode;
            ViewBag.Email = Membership.GetUser().Email;
            return View();
        }

        //
        // GET: /Admin/Send

        public ActionResult Send()
        {
            return View();
        }
        //
        // POST: /Admin/Send
        [HttpPost]
        public ActionResult Send(SmsMessage m)
        {
            SmsMessageRepository.SendMessage(m);
            TempData["ErrorMessage"] = "Sent message to " + m.To.ToString();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SendForm(string to, string message)
        {

            SmsMessage s = new SmsMessage(null, to, message, null);
            SmsMessageRepository.SendMessage(s);
            MDTProfile p = new MDTProfile().GetProfileByPhone(to);
            TempData["ErrorMessage"] = "Sent message to " + p.UserName;
            return RedirectToAction("Profile", new { username = p.UserName });
        }
        //
        // POST: /Admin/Create
    }
}
