using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Security;
using TwilioSharp.Request;
using Twilio;
using SeniorDesign.Models;

namespace SeniorDesign.Repository
{
    public class SmsMessageRepository : ISmsMessageRepository, IDisposable
    {
        private SdContext context;

        public SmsMessageRepository(SdContext context)
        {
            this.context = context;
        }
        public IEnumerable<SmsMessage> GetSmsMessages()
        {
            return context.SmsMessages;
        }
        public IEnumerable<SmsMessage> GetSmsMessagesByUserName(string user)
        {
            return context.SmsMessages
                .Where(m => m.username == user);
        }
        public IEnumerable<SmsMessage> GetSmsMessagesByPhone(string phone)
        {
            var l = context.SmsMessages
                .Where(m => m.To == phone || m.From == phone);
            return l;
        }
        public bool IsFirstMessage(string phone)
        {
            var l = context.SmsMessages
                .Where(m => m.From == phone)
                .Count();
            if (l == 0) return true;
            return false;
        }
        public SmsMessage GetSmsMessageByID(int id)
        {
            return context.SmsMessages
                .Find(id);
        }
        public SmsMessage GetLastMessageSentToPhone(string phone)
        {
            var r = context.SmsMessages
                .Where(t => t.To == phone)
                .OrderByDescending(t => t.timestamp);
            if (r.Count() > 0) return r.First();
            return null;            
        }
        public void InsertSmsMessage(SmsMessage SmsMessage)
        {            
            context.SmsMessages
                .Add(SmsMessage);
        }
        public void LogMessage(String To, String From, String Body, String SmsSid, String Status = "Unknown")
        {
            SmsMessage inc = new SmsMessage();
            inc.To = To;
            inc.From = From;
            inc.Body = Body;
            inc.status = Status;
            inc.timestamp = DateTime.Now.ToUniversalTime().Subtract(
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset);
            MDTProfile p = new MDTProfile().GetProfileByPhone(From);
            inc.username = p.UserName;
            inc.responseRequested = false;
            InsertSmsMessage(inc);
        }
        // Direct logging if SmsMessage is validated
        public int LogMessage(SmsMessage SmsMsg)
        {
            try 
            {
                InsertSmsMessage(SmsMsg);
                return 0;
            }
            catch 
            {
                return 1;
            }
        }
        public void InsertTextRequest(TextRequest request)
        {
            SmsMessage sms = new SmsMessage();
            sms.To = request.To;
            sms.From = request.From;
            sms.timestamp = DateTime.Now.ToUniversalTime().Subtract(
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset);
            sms.Body = request.Body;
            sms.status = "recieved";
            //MDTRequest.MembershipUser = Membership.GetUser("Administrator");
            InsertSmsMessage(sms);
        }
        public void DeleteSmsMessage(int SmsMessageID)
        {
            SmsMessage SmsMessage = context.SmsMessages.Find(SmsMessageID);
            context.SmsMessages.Remove(SmsMessage);
        }
        public void UpdateSmsMessage(SmsMessage SmsMessage)
        {
            context.Entry(SmsMessage).State = EntityState.Modified;
        }
        public SmsMessage ExpectingReponse(String phone)
        {   
            SmsMessage s = GetLastMessageSentToPhone(phone);
            if (s != null && s.responseRequested)
            {
                s.responseRequested = false;
                return s;
            }
            return null;
        }
        public int GetNumMessages(string phone)
        {
            return context.SmsMessages
                .Where(s => s.From == phone || s.To == phone)
                .Count();
        }
        public void Save()
        {
            context.SaveChanges();
        }
        public void SendMessage(SmsMessage msg)
        {
            if (true) //(Membership.GetUser(msg.username).IsApproved)
            {
                try
                {
                    // instantiate a new Twilio Rest Client
                    var client = new TwilioRestClient(
                        System.Configuration.ConfigurationManager.AppSettings["TW_SID_TOKEN"] as string,
                        System.Configuration.ConfigurationManager.AppSettings["TW_AUTH_TOKEN"] as string);

                    msg.From = System.Configuration.ConfigurationManager.AppSettings["TW_PHONE_NUMBER"] as string;
                    // Send a new outgoing SMS by POSTing to the SMS resource */
                    client.SendSmsMessage(
                        msg.From,
                        msg.To,
                        msg.Body
                    );
                    msg.timestamp = DateTime.Now.ToUniversalTime().Subtract(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset);
                    msg.status = "sent";
                    if (msg.username == null) msg.username = new MDTProfile().GetProfileByPhone(msg.To).UserName;

                    InsertSmsMessage(msg);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    msg.status = e.Message;
                }
            }
            else
            {
                throw new Exception("User is not approved yet. Try confirming your registration");
            }
        }        
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}