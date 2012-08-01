using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using SeniorDesign.Models;   

namespace SeniorDesign.Models
{
    public class MDTProfile : ProfileBase
    {
        static public MDTProfile CurrentUser
        {
            get { return (Create(Membership.GetUser().UserName) as MDTProfile); }
        }

        public string ConfirmPhoneNumber(string number)
        {
            foreach (MembershipUser usr in Membership.GetAllUsers())
            {
                MDTProfile p = new MDTProfile().GetProfile(usr.UserName);
                if (p.isPhoneConfirmed == false)
                {
                    if (p.Phone == number)
                    {
                        p.isPhoneConfirmed = true;
                        p.Save();
                        return p.UserName;
                    }
                }
            }
            return "";
        }

        public MDTProfile GetProfileByPhone(string phone)
        {
            phone = phone.Replace("+", "");
            foreach (MembershipUser u in Membership.GetAllUsers())
            {
                MDTProfile p = new MDTProfile().GetProfile(u.UserName);
                if (p.Phone == phone)
                {
                    return p;
                }                
            }
            return null;
        }

        public void UpdateProfile(RegisterModel m)
        {
            this.FirstName = m.FirstName;
            this.LastName = m.LastName;
            this.Phone = m.phone;
            this.GoalCode = m.GoalCode;
            //this.referralCode = m.referralCode;
            //this.Quiz1Complete = false;
            //this.Quiz2Complete = false;
            //this.Quiz3Complete = false;
            //this.Quiz4Complete = false;
            //this.Quiz5Complete = false;
            //this.Quiz6Complete = false;
            //this.Quiz7Complete = false;
            this.Save();
        }

        //public virtual SelectList SelectGoalCodes()
        //{
            
        //    var l = new List<SelectListItem>();
        //    l.Add(new SelectListItem { Text = "Read food labels", Value = "1" });
        //    l.Add(new SelectListItem { Text = "Eat fruits/veggies", Value = "2" });
        //    l.Add(new SelectListItem { Text = "Portion control", Value = "3" });
        //    l.Add(new SelectListItem { Text = "Be active", Value = "4" });
        //    l.Add(new SelectListItem { Text = "Less computer/TV", Value = "5" });
        //    l.Add(new SelectListItem { Text = "Log blood sugars", Value = "6" });
        //    l.Add(new SelectListItem { Text = "Check ketones", Value = "7" });
        //    l.Add(new SelectListItem { Text = "Insulin injections", Value = "8" });
        //    l.Add(new SelectListItem { Text = "Rotate injections", Value = "9" });
        //    l.Add(new SelectListItem { Text = "Brush teeth", Value = "10" });
        //    l.Add(new SelectListItem { Text = "Watch low blood sugar, bring snacks", Value = "11" });
        //    l.Add(new SelectListItem { Text = "Medical bracelet", Value = "12" });
        //    l.Add(new SelectListItem { Text = "Tell friends", Value = "13" });
        //    l.Add(new SelectListItem { Text = "Remain calm when reviewing blood sugar", Value = "14" });
        //    return new SelectList(l, "GoalCode", "Goal Title");
        //}

        public virtual string FirstName
        {
            get { return (string)this.GetPropertyValue("FirstNAme"); }
            set { this.SetPropertyValue("FirstName", value); }
        }
        public virtual string LastName
        {
            get { return (string)this.GetPropertyValue("LastName"); }
            set { this.SetPropertyValue("LastName", value); }
        }
        public virtual bool isPhoneConfirmed
        {
            get { return (bool)this.GetPropertyValue("isPhoneConfirmed"); }
            set { this.SetPropertyValue("isPhoneConfirmed", value); }
        }
        public virtual bool ExpectingRating
        {
            get { return (bool)this.GetPropertyValue("ExpectingRating"); }
            set { this.SetPropertyValue("ExpectingRating", value); }
        }
        //public virtual bool isPhoneConfirmed
        //{
        //    get { return (bool)this.GetPropertyValue("isPhoneConfirmed"); }
        //    set { this.SetPropertyValue("isPhoneConfirmed", value); }
        //} 
        public virtual string Phone
        {
            get { return ((string)(this.GetPropertyValue("Phone"))); }
            set { this.SetPropertyValue("Phone", value); }
        }
        public virtual string referralCode
        {
            get { return ((string)(this.GetPropertyValue("referralCode"))); }
            set { this.SetPropertyValue("referralCode", value); }
        }
        public virtual int GoalCode
        {
            get { return ((int)(this.GetPropertyValue("GoalCode"))); }
            set { this.SetPropertyValue("GoalCode", value); }
        }
        //public virtual bool Quiz1Complete
        //{
        //    get { return (bool)this.GetPropertyValue("quiz1"); }
        //    set { this.SetPropertyValue("quiz1", value); }
        //}
        //public virtual bool Quiz2Complete
        //{
        //    get { return (bool)this.GetPropertyValue("quiz2"); }
        //    set { this.SetPropertyValue("quiz2", value); }
        //}
        //public virtual bool Quiz3Complete
        //{
        //    get { return (bool)this.GetPropertyValue("quiz3"); }
        //    set { this.SetPropertyValue("quiz3", value); }
        //}
        //public virtual bool Quiz4Complete
        //{
        //    get { return (bool)this.GetPropertyValue("quiz4"); }
        //    set { this.SetPropertyValue("quiz4", value); }
        //}
        //public virtual bool Quiz5Complete
        //{
        //    get { return (bool)this.GetPropertyValue("quiz5"); }
        //    set { this.SetPropertyValue("quiz5", value); }
        //}
        //public virtual bool Quiz6Complete
        //{
        //    get { return (bool)this.GetPropertyValue("quiz6"); }
        //    set { this.SetPropertyValue("quiz6", value); }
        //}
        //public virtual bool Quiz7Complete
        //{
        //    get { return (bool)this.GetPropertyValue("quiz7"); }
        //    set { this.SetPropertyValue("quiz7", value); }
        //}
        public virtual MDTProfile GetProfile(string username)
        {
            return Create(username) as MDTProfile;
        }
    }
}
