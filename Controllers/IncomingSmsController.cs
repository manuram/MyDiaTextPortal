using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using SeniorDesign.Models;
using SeniorDesign.Repository;
using TwilioSharp.MVC3.Controllers;

namespace SeniorDesign.Controllers
{
    [HandleError]
    public class IncomingSmsController : TwiMLController
    {
        #region Repos & Private members
        private ISmsMessageRepository SmsMessageRepository;
        private IResponseGeneratorRepository ResponseGeneratorRepository;
        private IRatingRepository RatingRepository;
        private static SmsMessage incomingSms;
        private static SmsMessage responseSms;
        private static MDTProfile SenderProfile;

        public IncomingSmsController()
        {
            this.SmsMessageRepository = new SmsMessageRepository(new SdContext());
            this.ResponseGeneratorRepository = new ResponseGeneratorRepository(new SdContext());
            this.RatingRepository = new RatingRepository(new SdContext());
            
        }

        public IncomingSmsController(ISmsMessageRepository SmsMessageRepository, IResponseGeneratorRepository ResponseGeneratorRepository)
        {
            this.SmsMessageRepository = SmsMessageRepository;
            this.ResponseGeneratorRepository = ResponseGeneratorRepository;
            this.RatingRepository = RatingRepository;
        }
        #endregion
        [HttpPost,ValidateRequest("df2bfbcd29b1e814483dcafee6da3406")]
        public ActionResult IncomingSmsHandler()
        {
        #region Populate and log incoming message
            SenderProfile = new MDTProfile().GetProfileByPhone(incomingSms.From);

            incomingSms.username = (SenderProfile != null) ? SenderProfile.UserName : "";
            responseSms.username = incomingSms.username;

            if (SmsMessageRepository.LogMessage(incomingSms) != 0)
            {
                GeneralError("Logging incoming message");
            }

            //if (SenderProfile != null ? !Membership.GetUser(incomingSms.username).IsApproved : false) return null;

        #endregion
        #region Choose response to render
            /* Choose Actions:
             * **Possible Actions:**
             * Progress rating
             * Commands:
             *  register
             *  count
             *  profile
             *  demo
             *  [unknown]
            */
            //try {
                if (new Regex(@"demo", RegexOptions.IgnoreCase).IsMatch(incomingSms.Body))
                {
                    // send random message from pool for demo day
                    Random r = new Random();
                    responseSms.Body = ResponseGeneratorRepository
                        .GenerateResponseMessageFromCategoryCode(Convert.ToUInt16(r.Next(14)), "Unknown");
                }
                    else if (new Regex(@"register", RegexOptions.IgnoreCase).IsMatch(incomingSms.Body))
                    {
                        //SmsMessageRepository.IsFirstMessage(incomingSms.From);
                        AcctConfirm();
                    }
                        else if (new Regex(@"count", RegexOptions.IgnoreCase).IsMatch(incomingSms.Body))
                        {
                            Count();
                        }
                            else if (new Regex(@"profile", RegexOptions.IgnoreCase).IsMatch(incomingSms.Body))
                            {
                                GetProfileInfo(SenderProfile);
                            }
                                else if (SenderProfile.ExpectingRating)
                                {
                                    Regex r = new Regex(@"\b([0-9]|10)\b");
                                    int rating = Convert.ToInt32((r.Match(incomingSms.Body)).Value.Trim());
                                    LogRating(rating);
                                }
                                    else
                                    {
                                        responseSms.Body = "MyDiaText couldn't understand you. If you have a medical emergency, please contact your provider or call 911";
                                    }
            //}
            //catch (Exception e)
            //{
            //    responseSms.Body = ("Error producing response: " + e.Message).Substring(0, 160);
            //}

    #endregion
        #region Save entities and serve TwiML response
            if (responseSms.Body == null) GeneralError("MyDiaText did not produce a response");
            SmsMessageRepository.LogMessage(responseSms);
            SmsMessageRepository.Save();
            if(SenderProfile != null) SenderProfile.Save();
            return TwiML(response => response.Sms(responseSms.Body));
        #endregion
        }

        [HttpPost, ValidateRequest("df2bfbcd29b1e814483dcafee6da3406")]
        public ActionResult FallbackHandler()
        {
            string errorCode = Request["ErrorCode"];
            // Timeout error code
            //if (errorCode == "11200") RedirectToAction("IncomingSmsHandler");
            //else
            //{
            //    // Log error;
            //}
            return TwiML(r => r.Sms(string.Format("Your message could not be processed. Please try again later. {0}", errorCode)));
        }
    #region Response Methods
        private void GeneralError(string e = "Other")
        {
            responseSms.Body = "Error: " + e;
        }
        private void LogRating(int rating)
        {
            if (rating == 0)
            {
                responseSms.Body = "Your rating was not understood, please reply again using a number as the first character";
            }
            else
            {
                // Code to log points for responding
                if(RatingRepository.LogRating(incomingSms.username, rating))
                {
                    responseSms.Body = (rating < 7) ? "Keep working on your goal!" : "Great job, keep it up!";
                    SenderProfile.ExpectingRating = false;
                }
                else
                {
                    responseSms.Body = "Error logging rating";
                }
            }
        }
        private void AcctConfirm()
        {
            SenderProfile.isPhoneConfirmed = true;
            responseSms.Body = string.Format("Congratulations {0}, you are now registered for MyDiaText!",SenderProfile.UserName);
        }
        private void Count() // This is done
        {
            responseSms.Body = string.Format("You have exchanged {0} messages with MyDiaText.", SmsMessageRepository.GetNumMessages(incomingSms.From));
        }
        private void GetProfileInfo(MDTProfile profile)
        {
            if (profile != null)
            {
                responseSms.Body = string.Format("You are confirmed as {0} with the goal \"{1}\". You have exchanged {3} messages with MyDiaText",
                    SenderProfile.UserName, SenderProfile.GoalCode, SmsMessageRepository.GetNumMessages(incomingSms.From));
            }
            else
            {
                responseSms.Body = "MyDiaText couldn't recognize you. Are you registered?";
            }
        }
    #endregion
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            incomingSms = new SmsMessage();
            // For Fiddler Debugging //
            //incomingSms.Body = HttpContext.Request.Headers["Body"];
            //incomingSms.From = HttpContext.Request.Headers["From"].TrimStart('+', '1');
            //incomingSms.To = HttpContext.Request.Headers["To"].TrimStart('+', '1');
            //incomingSms.SmsSid = HttpContext.Request.Headers["SmsSid"];
            // // // // // // // //
            incomingSms.To = Request["To"].TrimStart('+', '1');
            incomingSms.From = Request["From"].TrimStart('+', '1');
            incomingSms.Body = Request["Body"];
            incomingSms.SmsSid = Request["SmsSid"];            
            incomingSms.status = "received";
            incomingSms.timestamp = DateTime.Now.ToUniversalTime().Subtract(
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset);

            responseSms = new SmsMessage(
                incomingSms.To,
                incomingSms.From,
                null,
                null);
            responseSms.status = "sent";
            responseSms.timestamp = DateTime.Now.ToUniversalTime().Subtract(
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset);

            base.OnActionExecuting(filterContext);
        }                    
    }
}
