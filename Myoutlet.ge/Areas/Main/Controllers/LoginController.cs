using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Myoutlet.ge.Models;

namespace Myoutlet.ge.Areas.Main.Controllers
{
    public class LoginController : Controller
    {
        MyoutletEntities db = new MyoutletEntities();
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string pass, string logged)
        {
            if (logged == "Admin")
            {
                Sub_admin admin = db.Sub_admin.FirstOrDefault(m => m.username == email);
                if (admin != null)
                {
                    if (System.Web.Helpers.Crypto.VerifyHashedPassword(admin.pass, pass))
                    {
                        Session["Logged"] = "admin";
                        return RedirectToAction("partners", "admin");
                    }
                    else
                    {
                        ModelState.AddModelError("LoginError", "პაროლი არასწორია.");
                        return View();
                    }

                }
                else
                {
                    ModelState.AddModelError("LoginError", "მეილი ჯერ არ არის დარეგისტრირებული.");
                    return View();
                }
            }
            if (logged == "Partner")
            {
                Partner pt = db.Partners.FirstOrDefault(x => x.Email == email);
                if (pt != null)
                {
                    if (System.Web.Helpers.Crypto.VerifyHashedPassword(pt.Pass, pass))
                    {
                        Session["Logged"] = pt.CompanyName;
                        return RedirectToAction("dashboard/" + pt.Id, "partner");
                    }
                    else
                    {
                        ModelState.AddModelError("LoginError", "პაროლი არასწორია.");
                        return View();
                    }

                }
                else
                {
                    ModelState.AddModelError("LoginError", "მეილი ჯერ არ არის დარეგისტრირებული.");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("LoginError", "მეილი ჯერ არ არის დარეგისტრირებული.");
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("login","login");
        }
        public ActionResult Forgot_pass()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Forgot_pass(int companyId ,string email)
        {
            Partner pt = db.Partners.FirstOrDefault(x => x.CompanyId == companyId);
            if(pt == null)
            {
                ModelState.AddModelError("Error", "არ არის პარტნიორი ამ კომპანიის ID-ით.");
                return View();
            }
            if(pt.Email != email)
            {
                ModelState.AddModelError("Error", "შეყვანილი მეილი არასწორია.");
                return View();
            }
            Session["Logged"] = pt.CompanyName;
            return RedirectToAction("PasswordChange/" + pt.UniqueNum, "Partner");
        }
    }
}