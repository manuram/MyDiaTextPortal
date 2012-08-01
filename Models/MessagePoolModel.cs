using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SeniorDesign.Models
{
    public class ResponseMessage
    {
        [Key]
        public int messageId { get; set; }

        [Display(Name = "Message body")]
        [Required,DataType(DataType.Text),StringLength(160)]
        public string message { get; set; }

        [Display(Name = "Cat. Code")]
        public int categoryId { get; set; }
        /* Categories:
         *  1	Read food labels
            2	Eat fruits/veggies
            3	Portion control
            4	Be active
            5	Less computer/TV
            6	Log blood sugars
            7	Check ketones
            8	Insulin injections
            9	Rotate injections
            10	Brush teeth
            11	Watch low blood sugar, bring snacks
            12	Medical bracelet
            13	Tell friends
            14	Remain calm when reviewing blood sugar
         */

        public bool expectsResponse { get; set; }
    }
}