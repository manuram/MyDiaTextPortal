using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwilioSharp.Request;
using System.Web.Security;
using SeniorDesign.Models;

namespace SeniorDesign.Repository
{
    public interface ISmsMessageRepository : IDisposable
    {
        // Get
        IEnumerable<SmsMessage> GetSmsMessages();
        IEnumerable<SmsMessage> GetSmsMessagesByUserName(string user);
        IEnumerable<SmsMessage> GetSmsMessagesByPhone(string phone);
        SmsMessage GetSmsMessageByID(int SmsMessageId);
        SmsMessage GetLastMessageSentToPhone(string phone);

        // Insert
        
        void InsertSmsMessage(SmsMessage SmsMessage);
        void InsertTextRequest(TextRequest request);
        void DeleteSmsMessage(int SmsMessageID);
        void UpdateSmsMessage(SmsMessage SmsMessage);
        void SendMessage(SmsMessage msg);
        void LogMessage(String To, String From, String Body, String SmsSid, String Status);
        int LogMessage(SmsMessage SmsMsg);
        SmsMessage ExpectingReponse(String phone);
        int GetNumMessages(string phone);

        // Other
        void Save();
        bool IsFirstMessage(string phone);             
    }
}