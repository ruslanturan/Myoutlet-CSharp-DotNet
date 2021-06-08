using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Myoutlet.ge.Models;
using Myoutlet.ge.ViewModels;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Myoutlet.ge.Areas.Main.Controllers
{
   // [LoginAuthentication]
    public class PartnerController : Controller
    {
        MyoutletEntities db = new MyoutletEntities();
        // GET: Partner
        public ActionResult Dashboard(int? id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Partner = db.Partners.FirstOrDefault(x => x.Id == id);
            if (vm.Partner == null)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ პარტნიორის პოვნა.";
                return View("error");
            }
            string session = (string)HttpContext.Session["Logged"];
            if (session != vm.Partner.CompanyName)
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            vm.Products = db.Products.Where(x => x.PartnerId == vm.Partner.Id).ToList();
            return View(vm);
        }
        public ActionResult AddProduct(int ? id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Partner pt = db.Partners.FirstOrDefault(x => x.Id == id);
            if (pt == null)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ პარტნიორის პოვნა.";
                return View("error");
            }
            string session = (string)HttpContext.Session["Logged"];
            if (session != pt.CompanyName)
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            if (pt.Status == false)
            {
                ViewBag.Text = "თქვენი სტატუსი შეჩერებულია, თქვენ არ შეგიძლიათ ახალი პროდუქტის დამატება.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            vm.Partner = pt;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddProduct(int id, Product product, string description)
        {
            product.Views = 0;
            product.PartnerId = id;
            product.ShoppingLink = "gjhj";
            product.Photo = db.Galleries.FirstOrDefault(x => x.ProductUniqueNum == product.ProductUniqueNum).Link.ToString();
            if(product.SaledCost > 0)
            {
                product.SaleInPercent = (decimal)(((product.Cost - product.SaledCost) / product.Cost)*100);
            }
            else
            {
                product.SaledCost = Convert.ToInt32(product.Cost);
            }
            db.Products.Add(product);   
            db.SaveChanges();
            Description dc = new Description();
            dc.Heading = description;
            dc.ProductId = product.Id;
            db.Descriptions.Add(dc);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return RedirectToAction("dashboard/" + product.PartnerId,"partner");
        }
        public ActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            vm.Product = db.Products.FirstOrDefault(x => x.Id == id);
            if (vm.Product == null)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ პროდუქტის პოვნა.";
                return View("error");
            }
            vm.Kinds = db.Kinds.Where(x => x.CategoryId == vm.Product.CategoryId).ToList();
            vm.Partner = db.Partners.FirstOrDefault(x => x.Id == vm.Product.PartnerId);
            string session = (string)HttpContext.Session["Logged"];
            if (session != vm.Partner.CompanyName)
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            vm.Galleries = db.Galleries.Where(x => x.ProductUniqueNum == vm.Product.ProductUniqueNum).ToList();
            vm.Descriptions = db.Descriptions.Where(x => x.ProductId == vm.Product.Id).ToList();
            vm.description = db.Descriptions.FirstOrDefault(x => x.ProductId == vm.Product.Id);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditProduct(Product product, string description)
        {
            product.Photo = db.Galleries.FirstOrDefault(x => x.ProductUniqueNum == product.ProductUniqueNum).Link;
            if (product.SaledCost > 0)
            {
                product.SaleInPercent = (decimal)(((product.Cost - product.SaledCost) / product.Cost) * 100);
            }
            else
            {
                product.SaledCost = Convert.ToInt32(product.Cost);
            }
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            Description dc = db.Descriptions.FirstOrDefault(x => x.ProductId == product.Id);
            if(dc != null)
            {
                dc.Heading = description;
            }
            else
            {
                Description d = new Description();
                d.Heading = description;
                d.ProductId = product.Id;
                db.Descriptions.Add(d);
            }
            db.SaveChanges();
            List<HotDiscount> hd = db.HotDiscounts.ToList();
            foreach (var h in hd)
            {
                if (h.ProductId == product.Id)
                {
                    string mainConn5 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                    SqlConnection sqlConn5 = new SqlConnection(mainConn5);
                    string sqlQuery5 = " DELETE FROM [dbo].[HotDiscounts] WHERE ProductId=" + product.Id;
                    SqlCommand sqlComm5 = new SqlCommand(sqlQuery5, sqlConn5);
                    sqlConn5.Open();
                    sqlComm5.ExecuteNonQuery();
                    sqlConn5.Close();
                }
            }
            List<WeeklyDiscount> wd = db.WeeklyDiscounts.ToList();
            foreach (var w in wd)
            {
                if (w.ProductId == product.Id)
                {
                    string mainConn5 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
                    SqlConnection sqlConn5 = new SqlConnection(mainConn5);
                    string sqlQuery5 = " DELETE FROM [dbo].[WeeklyDiscounts] WHERE ProductId=" + product.Id;
                    SqlCommand sqlComm5 = new SqlCommand(sqlQuery5, sqlConn5);
                    sqlConn5.Open();
                    sqlComm5.ExecuteNonQuery();
                    sqlConn5.Close();
                }
            }
            return RedirectToAction("dashboard/" + product.PartnerId,"partner");
        }
        public ActionResult RemoveProduct(int? id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Product pt = db.Products.FirstOrDefault(x => x.Id == id);
            if (pt == null)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ პროდუქტის პოვნა.";
                return View("error");
            }
            int ptId = pt.PartnerId;
            string session = (string)HttpContext.Session["Logged"];
            if (session != pt.Partner.CompanyName)
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            //removing descriptions
            string mainConn = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(mainConn);
            string sqlQuery = " DELETE FROM [dbo].[Descriptions] WHERE ProductId=" + pt.Id;
            SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
            sqlConn.Open();
            sqlComm.ExecuteNonQuery();
            sqlConn.Close();
            //removing image files
            List<string> imgNames = db.Galleries.Where(x => x.ProductUniqueNum == pt.ProductUniqueNum).Select(m => m.Link).ToList();
            foreach(var img in imgNames)
            {
                string myPath = Server.MapPath("//Images/Product-images/" + pt.ProductUniqueNum + "/" + img);
                System.IO.File.Delete(myPath);
            }
            //removing images
            string mainConn2 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn2 = new SqlConnection(mainConn2);
            string sqlQuery2 = " DELETE FROM [dbo].[Galleries] WHERE ProductUniqueNum=" + pt.ProductUniqueNum;
            SqlCommand sqlComm2 = new SqlCommand(sqlQuery2, sqlConn2);
            sqlConn2.Open();
            sqlComm2.ExecuteNonQuery();
            sqlConn2.Close();
            //removing comments
            string mainConn4 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn4 = new SqlConnection(mainConn4);
            string sqlQuery4 = " DELETE FROM [dbo].[Comments] WHERE ProductId=" + pt.Id;
            SqlCommand sqlComm4 = new SqlCommand(sqlQuery4, sqlConn4);
            sqlConn4.Open();
            sqlComm4.ExecuteNonQuery();
            sqlConn4.Close();
            string mainConn5 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn5 = new SqlConnection(mainConn5);
            string sqlQuery5 = " DELETE FROM [dbo].[WeeklyDiscounts] WHERE ProductId=" + pt.Id;
            SqlCommand sqlComm5 = new SqlCommand(sqlQuery5, sqlConn5);
            sqlConn5.Open();
            sqlComm5.ExecuteNonQuery();
            sqlConn5.Close();
            string mainConn6 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn6 = new SqlConnection(mainConn6);
            string sqlQuery6 = " DELETE FROM [dbo].[HotDiscounts] WHERE ProductId=" + pt.Id;
            SqlCommand sqlComm6 = new SqlCommand(sqlQuery6, sqlConn6);
            sqlConn6.Open();
            sqlComm6.ExecuteNonQuery();
            sqlConn6.Close();
            //removing product
            string mainConn3 = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn3 = new SqlConnection(mainConn3);
            string sqlQuery3 = " DELETE FROM [dbo].[Products] WHERE Id=" + pt.Id;
            SqlCommand sqlComm3 = new SqlCommand(sqlQuery3, sqlConn3);
            sqlConn3.Open();
            sqlComm3.ExecuteNonQuery();
            sqlConn3.Close();
            return RedirectToAction("dashboard/" + ptId,"partner");
        }
        public ActionResult PasswordChange(int?id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს პარტნიორის საიდუმლო ნომერს.";
                return View("error");
            }
            Partner pt = db.Partners.FirstOrDefault(x => x.UniqueNum == id);
            if (pt == null)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ პარტნიორის პოვნა.";
                return View("error");
            }
            string session = (string)HttpContext.Session["Logged"];
            if (session != pt.CompanyName)
            {
                ViewBag.Text = "ეს გვერდი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            return View(pt);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordChange(int id, string newPass)
        {
            Partner pt = db.Partners.FirstOrDefault(x => x.UniqueNum == id);
            if (pt == null)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ პარტნიორის პოვნა.";
                return View("error");
            }
            pt.Pass = System.Web.Helpers.Crypto.HashPassword(newPass);
            db.Entry(pt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("dashboard/" + pt.Id,"partner");
        }
        public ActionResult Settings(int ? id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Partner pt = db.Partners.FirstOrDefault(x => x.Id == id);
            if(pt == null)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ პარტნიორის პოვნა.";
                return View("error");
            }
            string session = (string)HttpContext.Session["Logged"];
            if (session != pt.CompanyName)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ პარტნიორის პოვნა.";
                return View("error");
            }
            return View(pt);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings (Partner partner, HttpPostedFileBase CompanyLogo)
        {
            if(CompanyLogo != null)
            {
                partner.CompanyLogo = partner.CompanyId + "-" + CompanyLogo.FileName;
            }
            db.Entry(partner).State = EntityState.Modified;
            db.SaveChanges();
            if (CompanyLogo != null && CompanyLogo.ContentLength > 0)
            {
                List<string> LogosOnTable = db.Partners.Select(m => m.CompanyLogo).ToList();
                DirectoryInfo di = new DirectoryInfo(Server.MapPath("/Images/Partner-logos"));
                var LogoFiles = di.GetFiles();
                //removing old logo
                foreach(var logoFile in LogoFiles)
                {
                    if (!LogosOnTable.Contains(logoFile.Name))
                    {
                        if(logoFile.Name != "logo.png")
                        {
                            string myPath = Server.MapPath("//Images/Partner-logos/" + logoFile.Name);
                            System.IO.File.Delete(myPath);
                        }
                    }
                }
                string fileName = Path.GetFileName(CompanyLogo.FileName);
                string imgPath = Path.Combine(Server.MapPath("~/Images/Partner-Logos/"), partner.CompanyId + "-" + fileName);
                CompanyLogo.SaveAs(imgPath);
                partner.CompanyLogo = partner.UniqueNum + fileName;
            }
            return RedirectToAction("dashboard/" + partner.Id,"partner");
        }
        public ActionResult CallList(int id)
        {
            ViewModel vm = new ViewModel();
            vm.Partner = db.Partners.FirstOrDefault(x => x.Id == id);
            vm.Requests = db.callRequests.Where(x => x.PartnerId == id && x.Status == false).ToList();
            return View(vm);
        }
        public ActionResult Called(int id)
        {
            callRequest req = db.callRequests.FirstOrDefault(x => x.Id == id);
            req.Status = true;
            db.SaveChanges();
            return RedirectToAction("CallList/" + req.PartnerId);
        }
    }
}
