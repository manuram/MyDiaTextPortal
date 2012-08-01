using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using SeniorDesign.Models;

namespace SeniorDesign.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to MyDiaText";
            if(User.Identity.IsAuthenticated)
            {
                ViewBag.Message += ", " + User.Identity.Name + "!";
            }
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Bug()
        {
            return View();
        }
        public ActionResult Demo()
        {
            try
            {
                MDTProfile p = MDTProfile.CurrentUser;
                p.Save();
                ViewBag.userphone = p.Phone;
                ViewBag.GoalCode = p.GoalCode;
            }
            catch { }
            return View();
        }
        [Authorize]
        public ActionResult Quizzes(int quizId = 0)
        {            
            if (quizId == 0)
            {
                MDTProfile p = MDTProfile.CurrentUser;
                return View(p);
            }
            ViewBag.Title = string.Concat("Quiz #", quizId.ToString());
            string page = string.Concat("Quiz", quizId.ToString());
            return View(page);
        }
        [Authorize, HttpPost]
        public ActionResult Quizzes(string[] answers)
        {
            bool correct = false;
            
            if (Request.Params["quiznumber"] == "1")
            {
                List<String> ans = new List<String>();
                ans.Add("false");
                ans.Add("false");
                ans.Add("true");
                ans.Add("false");
                ans.Add("true");
                ans.Add("false");
                ans.Add("false");
                ans.Add("true");
                ans.Add("false");
                ans.Add("false");
                ans.Add("false");
                ans.Add("false");
                ans.Add("true");
                ans.Add("false");
                ans.Add("false");
                ans.Add("false");
                ans.Add("true");
                ans.Add("false");
                ans.Add("false");
                ans.Add("false");

                List<bool> result = new List<bool>();
                for (int i = 1; i < 21; i++)
                {
                    string q = string.Format("q{0}", i);
                    result.Add((Request.Params[q]) == ans[i - 1]);
                    //str = string.Concat(str, string.Format("{0}", Request.Params[string.Format("q{0}", i)]));
                }
                //int hash = str.GetHashCode();
                //if (hash == -1552431627)
                //{
                //    correct = true;
                //}
            }
            if (Request.Params["quiznumber"] == "2")
            {
                string str = "";
                for (int i = 1; i < 6; i++)
                {
                    str = string.Concat(str, string.Format("{0}", Request.Params[string.Format("q{0}", i)]));
                }
                int hash = str.GetHashCode();
                if (hash == -442727613)
                {
                    correct = true;
                }
            }
            if (Request.Params["quiznumber"] == "3")
            {
                string str = "";
                for (int i = 1; i < 10; i++)
                {
                    str = string.Concat(str, string.Format("{0}", Request.Params[string.Format("q{0}", i)]));
                }
                int hash = str.GetHashCode();
                if (hash == 710712443)
                {
                    correct = true;
                }
            }

            if (correct) return RedirectToAction("QuizSuccess");
            else
            {
                TempData["ErrorMessage"] = string.Format("Some answers were incorrect, try again.");
                return RedirectToAction("Quizzes", new { quizId = 0 });
            }
        }
        public ActionResult QuizSuccess(){          
            return View("QuizSuccess");
        }
        public ActionResult Links()
        {
            ViewBag.Title = "Helpful Links";
            return View();
        }
    }
}
