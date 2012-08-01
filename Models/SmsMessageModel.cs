using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Twilio;
using System.Web.Security;

namespace SeniorDesign.Models
{
    public class SmsMessage
    {
        public SmsMessage()
        {
        }
        public SmsMessage(string From, string To, string Body, string SmsSid)
        {
            this.From = From;
            this.To = To;
            this.Body = Body;
            this.SmsSid = SmsSid;
            this.responseRequested = false;
            this.timestamp = DateTime.Now.ToUniversalTime().Subtract(
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset);
        }

        [Key]
        public int smsId { get; set; }
        public string SmsSid { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Timestamp")]
        //[Required]
        public DateTime timestamp { get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "The {0} must be at least {2} numbers long.", MinimumLength = 7)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "To")]
        public string To { get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "The {0} must be at least {2} numbers long.", MinimumLength = 7)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "From")]
        public string From { get; set; }

        //[Required]
        //[DataType(DataType.Text)]
        [Display(Name = "Message")]
        public string Body { get; set; }
        public bool responseRequested { get; set; }
        public string username { get; set; }
    }
}