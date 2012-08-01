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
    public class ResponseGeneratorRepository : IResponseGeneratorRepository, IDisposable
    {
        private SdContext context;

        public ResponseGeneratorRepository(SdContext context)
        {
            this.context = context;
        }

        public IEnumerable<ResponseMessage> GetAllResponseMessages()
        {
            return context.ResponseMessages.ToList();
        }

        public IEnumerable<ResponseMessage> GetAllResponseMessagesByCategoryCode(UInt16 code)
        {
            return context.ResponseMessages
                .Where(c => c.categoryId == code)
                .ToList();
        }

        public ResponseMessage GetResponseMessageByID(int id)
        {
            return context.ResponseMessages
                .Find(id);
        }

        public string GenerateResponseMessageFromCategoryCode(UInt16 cat, string user)
        {            
            return context.ResponseMessages
                .Where(m => m.categoryId == cat)
                //Also implement user checking for duplicate messages
                .OrderBy(t => System.Guid.NewGuid())
                .First().message.ToString();
        }

        public void InsertResponseMessage(ResponseMessage newMsg)
        {
            context.ResponseMessages
                .Add(newMsg);
        }

        public void DeleteResponseMessage(int msgID)
        {
            ResponseMessage RMsb = context.ResponseMessages.Find(msgID);
            context.ResponseMessages.Remove(RMsb);
        }

        public void UpdateResponseMessage(ResponseMessage RMsg)
        {
            context.Entry(RMsg).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
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