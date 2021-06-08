using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Myoutlet.ge.Models;
using Myoutlet.ge.ViewModels;
using static System.Net.WebRequestMethods;

namespace Myoutlet.ge.Areas.Main.Controllers
{
    [LoginAuthentication]
    public class AdminController : Controller
    {
        MyoutletEntities db = new MyoutletEntities();
        // GET: Main/Admin
        public ActionResult Partners()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Partners = db.Partners.ToList();
            return View(vm);
        }
        public ActionResult Advertisements()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Products = db.Products.ToList();
            return View(vm);
        }
        public ActionResult Register_New_Partner()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register_New_Partner(Partner partner)
        {
            if (db.Partners.FirstOrDefault(x => x.CompanyId == partner.CompanyId) != null)
            {
                ModelState.AddModelError("Error", "ეს ID უკვე არსებობს.");
                return View();
            }
            if (db.Partners.FirstOrDefault(x => x.CompanyName == partner.CompanyName) != null)
            {
                ModelState.AddModelError("Error", "ეს კომპანია უკვე არსებოს.");
                return View();
            }
            if (db.Partners.FirstOrDefault(x => x.Email == partner.Email) != null)
            {
                ModelState.AddModelError("Error", "ეს მეილი უკვე არსებობს.");
                return View();
            }
            string HashedPass = System.Web.Helpers.Crypto.HashPassword(partner.Pass);
            partner.Pass = HashedPass;
            List<decimal> uniqueNumberList = new List<decimal>();
            foreach (var num in db.Partners.ToList())
            {
                uniqueNumberList.Add(num.UniqueNum);
            }
            Random rnd = new Random();
            decimal UniqueNumber;
            do
            {
                UniqueNumber = rnd.Next(100000000, 999999999);
            } while (uniqueNumberList.Contains(UniqueNumber));
            partner.UniqueNum = UniqueNumber;
            partner.Status = true;
            partner.CompanyLogo = "logo.png";
            db.Partners.Add(partner);
            db.SaveChanges();
            return RedirectToAction("partners","admin");
        }
        public ActionResult ChangeAddStatus(int? id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Product pt = db.Products.FirstOrDefault(m => m.Id == id);
            if(pt.Status == null || pt.Status == false)
            {
                pt.Status = true;
                pt.added_date = DateTime.Today;
            }
            else
            {
                pt.Status = false;
                List<RecentyAdded> ra = db.RecentyAddeds.ToList();
                foreach(var r in ra)
                {
                    if(r.ProductId == pt.Id)
                    {
                        string mainConn5 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                        SqlConnection sqlConn5 = new SqlConnection(mainConn5);
                        string sqlQuery5 = " DELETE FROM [dbo].[RecentyAdded] WHERE ProductId=" + pt.Id;
                        SqlCommand sqlComm5 = new SqlCommand(sqlQuery5, sqlConn5);
                        sqlConn5.Open();
                        sqlComm5.ExecuteNonQuery();
                        sqlConn5.Close();
                    }
                }
                List<HotDiscount> hd = db.HotDiscounts.ToList();
                foreach (var h in hd)
                {
                    if (h.ProductId == pt.Id)
                    {
                        string mainConn5 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                        SqlConnection sqlConn5 = new SqlConnection(mainConn5);
                        string sqlQuery5 = " DELETE FROM [dbo].[HotDiscounts] WHERE ProductId=" + pt.Id;
                        SqlCommand sqlComm5 = new SqlCommand(sqlQuery5, sqlConn5);
                        sqlConn5.Open();
                        sqlComm5.ExecuteNonQuery();
                        sqlConn5.Close();
                    }
                }
                List<WeeklyDiscount> wd = db.WeeklyDiscounts.ToList();
                foreach (var w in wd)
                {
                    if (w.ProductId == pt.Id)
                    {
                        string mainConn5 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                        SqlConnection sqlConn5 = new SqlConnection(mainConn5);
                        string sqlQuery5 = " DELETE FROM [dbo].[WeeklyDiscounts] WHERE ProductId=" + pt.Id;
                        SqlCommand sqlComm5 = new SqlCommand(sqlQuery5, sqlConn5);
                        sqlConn5.Open();
                        sqlComm5.ExecuteNonQuery();
                        sqlConn5.Close();
                    }
                }
            }
            db.Entry(pt).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("advertisements","admin");
        }
        public ActionResult ChangePartnerStatus(int? id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Partner pt = db.Partners.FirstOrDefault(m => m.Id == id);
            if (pt.Status == false)
            {
                pt.Status = true;
            }
            else
            {
                pt.Status = false;
                List<ValuablePartner> vp = db.ValuablePartners.ToList();
                foreach (var v in vp)
                {
                    if (v.Partner == pt.Id)
                    {
                        string mainConn5 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                        SqlConnection sqlConn5 = new SqlConnection(mainConn5);
                        string sqlQuery5 = " DELETE FROM [dbo].[ValuablePartners] WHERE Partner=" + pt.Id;
                        SqlCommand sqlComm5 = new SqlCommand(sqlQuery5, sqlConn5);
                        sqlConn5.Open();
                        sqlComm5.ExecuteNonQuery();
                        sqlConn5.Close();
                    }
                }
            }
            db.Entry(pt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("partners","admin");
        }
        public ActionResult RemovePartner(int? id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Partner pt = db.Partners.FirstOrDefault(m => m.Id == id);
            List<Product> products = db.Products.Where(m => m.PartnerId == id).ToList();
            //removing descriptions
            foreach(var product in products)
            {
                string mainConn = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                SqlConnection sqlConn = new SqlConnection(mainConn);
                string sqlQuery = " DELETE FROM [dbo].[Descriptions] WHERE ProductId=" + product.Id;
                SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
                sqlConn.Open();
                sqlComm.ExecuteNonQuery();
                sqlConn.Close();
                List<int> callRequests = db.callRequests.Where(x => x.ProductId == product.Id).Select(x => x.ProductId).ToList();
                foreach(var cr in callRequests)
                {
                    string mainConn77 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                    SqlConnection sqlConn77 = new SqlConnection(mainConn77);
                    string sqlQuery77 = " DELETE FROM [dbo].[callRequests] WHERE ProductId=" + cr;
                    SqlCommand sqlComm77 = new SqlCommand(sqlQuery77, sqlConn77);
                    sqlConn77.Open();
                    sqlComm77.ExecuteNonQuery();
                    sqlConn77.Close();
                }
                //removing image files
                List<string> imgNames = db.Galleries.Where(x => x.ProductUniqueNum == product.ProductUniqueNum).Select(m => m.Link).ToList();
                foreach (var img in imgNames)
                {
                    string myPath = Server.MapPath("//Images/Product-images/" + product.ProductUniqueNum + "/" + img);
                    System.IO.File.Delete(myPath);
                }
                //removing images
                string mainConn2 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                SqlConnection sqlConn2 = new SqlConnection(mainConn2);
                string sqlQuery2 = " DELETE FROM [dbo].[Galleries] WHERE ProductUniqueNum=" + product.ProductUniqueNum;
                SqlCommand sqlComm2 = new SqlCommand(sqlQuery2, sqlConn2);
                sqlConn2.Open();
                sqlComm2.ExecuteNonQuery();
                sqlConn2.Close();
                string mainConn6 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                SqlConnection sqlConn6 = new SqlConnection(mainConn6);
                string sqlQuery6 = " DELETE FROM [dbo].[Comments] WHERE ProductId=" + product.Id;
                SqlCommand sqlComm6 = new SqlCommand(sqlQuery6, sqlConn6);
                sqlConn6.Open();
                sqlComm6.ExecuteNonQuery();
                sqlConn6.Close();
                string mainConn7 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                SqlConnection sqlConn7 = new SqlConnection(mainConn7);
                string sqlQuery7 = " DELETE FROM [dbo].[RecentyAdded] WHERE ProductId=" + product.Id;
                SqlCommand sqlComm7 = new SqlCommand(sqlQuery7, sqlConn7);
                sqlConn7.Open();
                sqlComm7.ExecuteNonQuery();
                sqlConn7.Close();
                string mainConn8 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                SqlConnection sqlConn8 = new SqlConnection(mainConn8);
                string sqlQuery8 = " DELETE FROM [dbo].[WeeklyDiscounts] WHERE ProductId=" + product.Id;
                SqlCommand sqlComm8 = new SqlCommand(sqlQuery8, sqlConn8);
                sqlConn8.Open();
                sqlComm8.ExecuteNonQuery();
                sqlConn8.Close();
                string mainConn9 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                SqlConnection sqlConn9 = new SqlConnection(mainConn9);
                string sqlQuery9 = " DELETE FROM [dbo].[HotDiscounts] WHERE ProductId=" + product.Id;
                SqlCommand sqlComm9 = new SqlCommand(sqlQuery9, sqlConn9);
                sqlConn9.Open();
                sqlComm9.ExecuteNonQuery();
                sqlConn9.Close();
                //removing product
                string mainConn3 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                SqlConnection sqlConn3 = new SqlConnection(mainConn3);
                string sqlQuery3 = " DELETE FROM [dbo].[Products] WHERE Id=" + product.Id;
                SqlCommand sqlComm3 = new SqlCommand(sqlQuery3, sqlConn3);
                sqlConn3.Open();
                sqlComm3.ExecuteNonQuery();
                sqlConn3.Close();
            }
            //removing product
            string mainConn5 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn5 = new SqlConnection(mainConn5);
            string sqlQuery5 = " DELETE FROM [dbo].[ValuablePartners] WHERE Partner=" + pt.Id;
            SqlCommand sqlComm5 = new SqlCommand(sqlQuery5, sqlConn5);
            sqlConn5.Open();
            sqlComm5.ExecuteNonQuery();
            sqlConn5.Close();
            string mainConn4 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn4 = new SqlConnection(mainConn4);
            string sqlQuery4 = " DELETE FROM [dbo].[Partners] WHERE Id=" + pt.Id;
            SqlCommand sqlComm4 = new SqlCommand(sqlQuery4, sqlConn4);
            sqlConn4.Open();
            sqlComm4.ExecuteNonQuery();
            sqlConn4.Close();
            if (pt.CompanyLogo != "logo.png")
            {
                string myPath = Server.MapPath("//Images/Partner-logos/" + pt.CompanyLogo);
                System.IO.File.Delete(myPath);
            }
            return RedirectToAction("partners","admin");
        }
        public ActionResult AboutUs()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            About about = db.Abouts.FirstOrDefault();
            return View(about);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AboutUs(About about)
        {
            About oldabout = db.Abouts.FirstOrDefault();
            if(oldabout != null)
            {
                oldabout.Id = about.Id;
                oldabout.LittleInfo = about.LittleInfo;
                oldabout.Mail = about.Mail;
                oldabout.MobilePhone = about.MobilePhone;
                oldabout.Text = about.Text;
            }
            else
            {
                db.Abouts.Add(about);
            }
            db.SaveChanges();
            return RedirectToAction("Partners");
        }
        public ActionResult FacebookLink()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            FbLink fb = db.FbLinks.FirstOrDefault();
            return View(fb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FacebookLink(FbLink fb)
        {
            FbLink oldFb = db.FbLinks.FirstOrDefault();
            if (oldFb != null)
            {
                oldFb.Id = fb.Id;
                oldFb.Link = fb.Link;
            }
            else
            {
                db.FbLinks.Add(fb);
            }
            db.SaveChanges();
            return RedirectToAction("Partners");
        }
        public ActionResult InstaLink()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            InstaLink insta = db.InstaLinks.FirstOrDefault();
            return View(insta);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InstaLink(InstaLink insta)
        {
            InstaLink oldInsta = db.InstaLinks.FirstOrDefault();
            if (oldInsta != null)
            {
                oldInsta.Id = insta.Id;
                oldInsta.Link = insta.Link;
            }
            else
            {
                db.InstaLinks.Add(insta);
            }
            db.SaveChanges();
            return RedirectToAction("Partners");
        }
        public ActionResult YoutubeChannel()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            YoutubeLink youtube = db.YoutubeLinks.FirstOrDefault();
            return View(youtube);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YoutubeChannel(YoutubeLink youtube)
        {
            YoutubeLink oldYoutube = db.YoutubeLinks.FirstOrDefault();
            if (oldYoutube != null)
            {
                oldYoutube.Id = youtube.Id;
                oldYoutube.Link = youtube.Link;
            }
            else
            {
                db.YoutubeLinks.Add(youtube);
            }
            db.SaveChanges();
            return RedirectToAction("Partners");
        }
        public ActionResult Adds()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Adds = db.Adds.ToList();
            return View(vm);
        }
        public ActionResult NewAdd()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            List<Add> adds = db.Adds.ToList();
            if(adds.Count > 1)
            {
                ViewBag.Text = "თქვენ შეგიძლიათ მხოლოდ 2 რეკლამის დამატება";
                return View("error");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewAdd(Add add, HttpPostedFileBase image)
        {
            if (image != null)
            {
                Random rnd = new Random();
                add.ImgName = rnd.Next(0, 999) + "-" + image.FileName;
            }
            db.Adds.Add(add);
            db.SaveChanges();
            if (image != null && image.ContentLength > 0)
            {
                string fileName = Path.GetFileName(add.ImgName);
                string imgPath = Path.Combine(Server.MapPath("~/Images/Adds/"), add.ImgName);
                image.SaveAs(imgPath);
            }
            return RedirectToAction("adds");
        }
        public ActionResult RemoveAdd(int? id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Add add = db.Adds.FirstOrDefault(m => m.Id == id);
            string mainConn = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(mainConn);
            string sqlQuery = " DELETE FROM [dbo].[Adds] WHERE Id=" + id;
            SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
            sqlConn.Open();
            sqlComm.ExecuteNonQuery();
            sqlConn.Close();
            string myPath = Server.MapPath("//Images/Adds/" + add.ImgName);
            System.IO.File.Delete(myPath);
            return RedirectToAction("adds");
        }
        public ActionResult Campaigns()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "You can't access to this page.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Campaigns = db.Campaigns.ToList();
            return View(vm);
        }
        public ActionResult NewCampaign()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            List<Campaign> campaigns = db.Campaigns.ToList();
            if (campaigns.Count > 3)
            {
                ViewBag.Text = "თქვენ შეგიძლიათ მხოლოდ 4 კომპანიის დამატება";
                return View("error");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCampaign(Campaign campaign, HttpPostedFileBase image)
        {
            if (image != null)
            {
                Random rnd = new Random();
                campaign.Link = rnd.Next(0, 999) + "-" + image.FileName;
            }
            db.Campaigns.Add(campaign);
            db.SaveChanges();
            if (image != null && image.ContentLength > 0)
            {
                string fileName = Path.GetFileName(campaign.Link);
                string imgPath = Path.Combine(Server.MapPath("~/Images/Campaigns/"), campaign.Link);
                image.SaveAs(imgPath);
            }
            return RedirectToAction("campaigns");
        }
        public ActionResult RemoveCampaign(int? id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Campaign campaign = db.Campaigns.FirstOrDefault(m => m.Id == id);
            string mainConn = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(mainConn);
            string sqlQuery = " DELETE FROM [dbo].[Campaigns] WHERE Id=" + id;
            SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
            sqlConn.Open();
            sqlComm.ExecuteNonQuery();
            sqlConn.Close();
            string myPath = Server.MapPath("//Images/Campaigns/" + campaign.Link);
            System.IO.File.Delete(myPath);
            return RedirectToAction("campaigns");
        }
        public ActionResult HotDiscounts()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Products = db.Products.Where(x => x.Status == true).ToList();
            List<int> dList = new List<int>();
            foreach(var d in db.HotDiscounts)
            {
                dList.Add(d.ProductId);
            }
            vm.DiscountsList = dList;
            return View(vm);
        }
        public ActionResult AddHotDiscount(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            HotDiscount disc = new HotDiscount();
            disc.ProductId = id;
            db.HotDiscounts.Add(disc);
            db.SaveChanges();
            return RedirectToAction("HotDiscounts");
        }
        public ActionResult RemoveHotDiscount(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            string mainConn = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(mainConn);
            string sqlQuery = " DELETE FROM [dbo].[HotDiscounts] WHERE ProductId=" + id;
            SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
            sqlConn.Open();
            sqlComm.ExecuteNonQuery();
            sqlConn.Close();
            return RedirectToAction("HotDiscounts");
        }
        public ActionResult WeeklyDiscounts()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Products = db.Products.Where(x => x.Status == true).ToList();
            List<int> dList = new List<int>();
            foreach (var d in db.WeeklyDiscounts)
            {
                dList.Add(d.ProductId);
            }
            vm.DiscountsList = dList;
            return View(vm);
        }
        public ActionResult AddWeeklyDiscount(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            WeeklyDiscount disc = new WeeklyDiscount();
            disc.ProductId = id;
            db.WeeklyDiscounts.Add(disc);
            db.SaveChanges();
            return RedirectToAction("WeeklyDiscounts");
        }
        public ActionResult RemoveWeeklyDiscount(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            string mainConn = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(mainConn);
            string sqlQuery = " DELETE FROM [dbo].[WeeklyDiscounts] WHERE ProductId=" + id;
            SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
            sqlConn.Open();
            sqlComm.ExecuteNonQuery();
            sqlConn.Close();
            return RedirectToAction("WeeklyDiscounts");
        }
        public ActionResult ValuablePartners()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Partners = db.Partners.Where(x => x.Status == true).ToList();
            List<int> dList = new List<int>();
            foreach (var d in db.ValuablePartners)
            {
                dList.Add(d.Partner);
            }
            vm.DiscountsList = dList;
            return View(vm);
        }
        public ActionResult AddValuablePartner(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ValuablePartner disc = new ValuablePartner();
            disc.Partner = id;
            db.ValuablePartners.Add(disc);
            db.SaveChanges();
            return RedirectToAction("ValuablePartners");
        }
        public ActionResult RemoveValuablePartner(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            string mainConn = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(mainConn);
            string sqlQuery = " DELETE FROM [dbo].[ValuablePartners] WHERE Partner=" + id;
            SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
            sqlConn.Open();
            sqlComm.ExecuteNonQuery();
            sqlConn.Close();
            return RedirectToAction("ValuablePartners");
        }
        public ActionResult Categories()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            return View(vm);
        }
        public ActionResult AddCategory()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(Category ct)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            List<string> ctList = db.Categories.Select(x => x.Name.ToUpper()).ToList();
            if (ctList.Contains(ct.Name.ToUpper()))
            {
                ModelState.AddModelError("Error", "ეს კატეგორია უკვე არსებობს.");
                return View();
            }
            db.Categories.Add(ct);
            db.SaveChanges();
            return RedirectToAction("categories");
        }
        public ActionResult RemoveCategory(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            List<Kind> kinds = new List<Kind>();
            kinds = db.Kinds.Where(x => x.CategoryId == id).ToList();
            if(kinds.Count > 0)
            {
                ViewBag.Text = "ამ კატეგორიის წაშლით თქვენ წაშლით ყველა მის ქვეკატეგორიას.";
                return View("error");
            }
            string mainConn2 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn2 = new SqlConnection(mainConn2);
            string sqlQuery2 = " DELETE FROM [dbo].[Categories] WHERE Id=" + id;
            SqlCommand sqlComm2 = new SqlCommand(sqlQuery2, sqlConn2);
            sqlConn2.Open();
            sqlComm2.ExecuteNonQuery();
            sqlConn2.Close();
            return RedirectToAction("Categories");
        }
        public ActionResult Kinds()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Kinds = db.Kinds.ToList();
            return View(vm);
        }
        public ActionResult AddKind()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKind(Kind kind, HttpPostedFileBase image)
        {
            List<string> kndList = db.Kinds.Where(x => x.CategoryId == kind.CategoryId).Select(x => x.Name.ToUpper()).ToList();
            if (kndList.Contains(kind.Name.ToUpper()))
            {
                ModelState.AddModelError("Error", "ქვეკატეგორია უკვე არსებობს.");
                ViewModel vm = new ViewModel();
                vm.Categories = db.Categories.ToList();
                return View(vm);
            }
            if(image == null)
            {
                ModelState.AddModelError("Error", "სურათის ატვირთვა.");
                ViewModel vm = new ViewModel();
                vm.Categories = db.Categories.ToList();
                return View(vm);
            }
            if (image != null)
            {
                Random rnd = new Random();
                kind.image = rnd.Next(0, 999) + "-" + image.FileName;
            }
            db.Kinds.Add(kind);
            db.SaveChanges();
            if (image != null && image.ContentLength > 0)
            {
                string fileName = Path.GetFileName(kind.image);
                string imgPath = Path.Combine(Server.MapPath("~/Images/Kinds/"), kind.image);
                image.SaveAs(imgPath);
            }
            return RedirectToAction("Kinds");
        }
        public ActionResult RemoveKind(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            List<Product> products = new List<Product>();
            products = db.Products.Where(x => x.KindId == id).ToList();
            if (products.Count > 0)
            {
                ViewBag.Text = "ამ მნიშვნელობის წაშლით თქვენ წაშლით ყველა პროდუქტს იგივე მნიშვნელობით.";
                return View("error");
            }
            string mainConn2 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn2 = new SqlConnection(mainConn2);
            string sqlQuery2 = " DELETE FROM [dbo].[Kinds] WHERE Id=" + id;
            SqlCommand sqlComm2 = new SqlCommand(sqlQuery2, sqlConn2);
            sqlConn2.Open();
            sqlComm2.ExecuteNonQuery();
            sqlConn2.Close();
            return RedirectToAction("Kinds");
        }
        public ActionResult Comments()
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Comments = db.Comments.Where(x => x.Status == false).ToList();
            return View(vm);
        }
        public ActionResult ConfirmComment(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            Comment cmnt = db.Comments.FirstOrDefault(x => x.Id == id);
            cmnt.Status = true;
            db.SaveChanges();
            return RedirectToAction("Comments");
        }
        public ActionResult RemoveComment(int id)
        {
            string session = (string)HttpContext.Session["Logged"];
            if (session != "admin")
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            string mainConn2 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn2 = new SqlConnection(mainConn2);
            string sqlQuery2 = " DELETE FROM [dbo].[Comments] WHERE Id=" + id;
            SqlCommand sqlComm2 = new SqlCommand(sqlQuery2, sqlConn2);
            sqlConn2.Open();
            sqlComm2.ExecuteNonQuery();
            sqlConn2.Close();
            return RedirectToAction("Comments");
        }
    }
}