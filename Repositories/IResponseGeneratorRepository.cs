using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwilioSharp.Request;
using System.Web.Security;
using SeniorDesign.Models;

namespace SeniorDesign.Repository
{
    public interface IResponseGeneratorRepository : IDisposable
    {
        // Get
        IEnumerable<ResponseMessage> GetAllResponseMessages();
        IEnumerable<ResponseMessage> GetAllResponseMessagesByCategoryCode(UInt16 code);
        ResponseMessage GetResponseMessageByID(int id);
        string GenerateResponseMessageFromCategoryCode(UInt16 cat, string user);

        // Insert

        void InsertResponseMessage(ResponseMessage newMsg);
        void DeleteResponseMessage(int msgID);
        void UpdateResponseMessage(ResponseMessage RMsg);

        // Other
        void Save();          
    }
}