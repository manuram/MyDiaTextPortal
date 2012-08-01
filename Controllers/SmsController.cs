using System.Web.Mvc;
using SeniorDesign.Models;
using SeniorDesign.Repository;
using Telerik.Web.Mvc;
using System.Collections.Generic;
using TwilioSharp.MVC3.Controllers;

namespace SeniorDesign.Controllers
{
    [HandleError]
    public class SmsController : TwiMLController
    {
        private ISmsMessageRepository SmsMessageRepository;
        private IResponseGeneratorRepository ResponseGeneratorRepository;

        public SmsController()
        {
            this.SmsMessageRepository = new SmsMessageRepository(new SdContext());
            this.ResponseGeneratorRepository = new ResponseGeneratorRepository(new SdContext());
        }

        public SmsController(ISmsMessageRepository SmsMessageRepository, IResponseGeneratorRepository ResponseGeneratorRepository)
        {
            this.SmsMessageRepository = SmsMessageRepository;
            this.ResponseGeneratorRepository = ResponseGeneratorRepository;
        }
        
        //
        // GET: /Sms/History (shows all Body)
            
        [Authorize]
        public ViewResult History() 
        {
            ViewBag.username = User.Identity.Name;
            return View();
        }
        public ActionResult test()
        {
            TempData["ErrorMessage"] = ResponseGeneratorRepository.GenerateResponseMessageFromCategoryCode(2, "");
            return View("Index");
        }
        //
        // GET: /Sms

        public ActionResult Index(SmsMessage model)
        {
            return View(model.status);
        }

        //
        // GET: /Sms/AddMessage

        [Authorize]
        public ActionResult AddMessage()
        {
            return View();
        }

        //
        // GET: /Sms/Send

        [Authorize]
        public ActionResult Send()
        {
            return View();
        } 

        // POST: /Sms/Send
        // Adda Body to the database

        [HttpPost, Authorize]
        public ActionResult AddMessage(ResponseMessage model)
        {
            try
            {
                return RedirectToAction("AddMessage");
            }
            catch
            {
                return View();
            }
        }

        //
        // POST: /Sms/Send

        [HttpPost, Authorize]
        public ActionResult Send(SmsMessage model)
        {
            //model.SendMessage();

            return RedirectToAction("Index");
        }

        //[Authorize(Users = "Administrator")]
        [Authorize]
        public ActionResult GenerateSMS(ushort code, string phoneTo = "3017683441")
        {
            if (phoneTo == null)
            {
                TempData["ErrorMessage"] = "No phone number supplied";
                return RedirectToAction("Index");
            }

            SmsMessage sms = new SmsMessage();
            MDTProfile p = new MDTProfile().GetProfileByPhone(phoneTo);
            // Get Body from database
            string thisIsTheRealAnswer = ResponseGeneratorRepository.GenerateResponseMessageFromCategoryCode(code, "Administrator");
            sms.Body = thisIsTheRealAnswer;
            sms.username = p.UserName;
            sms.To = phoneTo;
            sms.responseRequested = false;
            // Get ready to receive response:
                        
            SmsMessageRepository.SendMessage(sms);
            SmsMessageRepository.Save();
            return RedirectToAction("Demo","Home");
        }

        [Authorize]
        public ActionResult GenerateQuestionSMS(ushort code, string phoneTo = "3017683441")
        {
            if (phoneTo == null)
            {
                TempData["ErrorMessage"] = "No phone number supplied";
                return RedirectToAction("Demo", "Home");
            }

            SmsMessage sms = new SmsMessage();
            MDTProfile p = new MDTProfile().GetProfileByPhone(phoneTo);

            //string thisIsTheRealAnswer = ResponseGeneratorRepository.GenerateResponseMessageFromCategoryCode(code, "Administrator");
            // Get Body from database
            sms.Body = "Please rate your progress this week by responding with a number 1-10";
            sms.username = p.UserName;
            sms.To = phoneTo;
            sms.responseRequested = true;
            p.ExpectingRating = true;
            p.Save();

            SmsMessageRepository.SendMessage(sms);
            SmsMessageRepository.Save();
            return RedirectToAction("Demo", "Home");
        }

        [GridAction]
        public ActionResult _AjaxBinding(string username)
        {
            IEnumerable<SmsMessage> model = null;
            if (username == null)
            {
                return null;
            }
            else
            {
                ViewBag.Title = string.Format("History for user \"{0}\".", username);
                model = SmsMessageRepository.GetSmsMessagesByUserName(username);
            }
            return View(new GridModel<SmsMessage>(model));
        }

        public ActionResult RegisterConfirm(RegisterModel model)
        {
            SmsMessage sms = new SmsMessage();
            //MDTRequest.From = System.Configuration.ConfigurationManager.AppSettings["TW_PHONE_NUMBER"] as string;
            sms.To = model.phone;
            sms.Body = string.Format("Hi {0}. Please reply with, \"Register\" to finish registering for MyDiaText.", model.UserName); //.Substring(0, 160);            
            SmsMessageRepository.SendMessage(sms);
            SmsMessageRepository.Save();
            return RedirectToAction("Demo", "Home");
        }

        [HttpPost]
        public ActionResult ConfirmNumber(string To, string From, string SmsSid)
        {
            //var doc = new XDocument();
            //var response = new XElement("Response");
            string answer = "";
            MDTProfile p = new MDTProfile().GetProfileByPhone(From);

            if(p != null) 
            {
                //check for right conf code?
                p.isPhoneConfirmed = true;
                answer = string.Format("Thanks {0}, you are now registered for MyDiaText!", p.UserName);
                //response.Add(new XElement("Sms", "Thanks for confirming your number!"));
                //doc.Add(response);
            }
            else {
                answer = "I'm sorry, your phone number was not recognized by MyDiaText, please register again.";
            }
            return TwiML(Response => Response.Sms(answer));
        }
    }
}
