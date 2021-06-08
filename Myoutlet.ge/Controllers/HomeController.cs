using Myoutlet.ge.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Myoutlet.ge.Models;
using System.Globalization;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Myoutlet.ge.Controllers
{
    public class HomeController : Controller
    {
        MyoutletEntities db = new MyoutletEntities();
        public ActionResult Index()
        {
            ViewModel vm = new ViewModel();
            List<Product> productslist = db.Products.Where(x => x.Status == true).ToList();
            int num = productslist.Count();
            if (num <= 20)
            {
                vm.Products = productslist;
            }
            else
            {
                vm.Products = productslist.GetRange(num - 21, 20);
            }
            List<Partner> valuablePartners = new List<Partner>();
            vm.Campaigns = db.Campaigns.ToList();
            vm.Categories = db.Categories.ToList();
            vm.Adds = db.Adds.ToList();
            vm.Partners = db.Partners.Where(x => x.Status == true).ToList();
            var hotList = db.HotDiscounts.ToList();
            productslist.Clear();
            for (var i = 0; i < hotList.Count; i++)
            {
                int id = hotList[i].ProductId;
                Product pt = db.Products.FirstOrDefault(x => x.Id == id);
                if (pt != null)
                {
                    productslist.Add(pt);
                }
            }
            vm.HotDiscounts = productslist;
            var weeklyList = db.WeeklyDiscounts.ToList();
            productslist.Clear();
            for (var i = 0; i < weeklyList.Count; i++)
            {
                int id = weeklyList[i].ProductId;
                Product pt = db.Products.FirstOrDefault(x => x.Id == id);
                if (pt != null)
                {
                    productslist.Add(pt);
                }
            }
            vm.WeeklyDiscounts = productslist;
            var valuablePartnersList = db.ValuablePartners.ToList();
            for (var i = 0; i < valuablePartnersList.Count; i++)
            {
                int id = valuablePartnersList[i].Partner;
                Partner pt = db.Partners.FirstOrDefault(x => x.Id == id);
                if (pt != null)
                {
                    valuablePartners.Add(pt);
                }
            }
            vm.ValuablePartners = valuablePartners;
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult Partner(int? id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Partner pt = db.Partners.FirstOrDefault(x => x.Id == id && x.Status == true);
            if (pt == null)
            {
                ViewBag.Text = "პარტნიორი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            vm.Adds = db.Adds.ToList();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Partners = db.Partners.Where(x => x.Status == true).ToList();
            vm.Products = pt.Products.Where(x => x.Status == true).ToList();
            vm.info = db.Abouts.FirstOrDefault();
            vm.Partner = pt;
            return View(vm);
        }
        public ActionResult Product(int? id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            Product pt = db.Products.FirstOrDefault(x => x.Id == id && x.Status == true);
            if (pt == null)
            {
                ViewBag.Text = "პროდუქტი არ არის ხელმისაწვდომი.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Adds = db.Adds.ToList();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Partners = db.Partners.Where(x => x.Status == true).ToList();
            vm.Galleries = db.Galleries.Where(x => x.ProductUniqueNum == pt.ProductUniqueNum).ToList();
            vm.Partner = db.Partners.FirstOrDefault(x => x.Id == pt.Partner.Id && x.Status == true);
            vm.info = db.Abouts.FirstOrDefault();
            vm.Descriptions = db.Descriptions.Where(x => x.ProductId == pt.Id).ToList();
            vm.Product = pt;
            var similarsIndexes = db.Products.Where(x => x.KindId == pt.KindId && x.Status == true && x.Id != pt.Id).Select(x => x.Id).ToList();
            if (similarsIndexes.Count > 3)
            {
                var similarList = new List<Product>();
                do
                {
                    Random rnd = new Random();
                    int rndNum = 0;
                    rndNum = rnd.Next(0, similarsIndexes.Count);
                    var index = similarsIndexes[rndNum];
                    Product spt = db.Products.FirstOrDefault(x => x.Id == index);
                    if (!similarList.Contains(spt) && spt != null)
                    {
                        similarList.Add(spt);
                    }
                } while (similarList.Count < 3);
                vm.SimilarProducts = similarList;
            }
            vm.Comments = db.Comments.Where(x => x.Status == true && x.ProductId == id).ToList();
            pt.Views++;
            db.SaveChanges();
            return View(vm);
        }
        public ActionResult ProductbyKind(int? id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            List<Product> pt = db.Products.Where(x => x.KindId == id && x.Status == true).ToList();
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            vm.Adds = db.Adds.ToList();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Partners = db.Partners.Where(x => x.Status == true).ToList();
            vm.Products = pt;
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult KindbyCategory(int? id)
        {
            if (id == null)
            {
                ViewBag.Text = "ეს გვერდი მოითხოვს ID.";
                return View("error");
            }
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            vm.Adds = db.Adds.ToList();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Partners = db.Partners.Where(x => x.Status == true).ToList();
            vm.Category = db.Categories.FirstOrDefault(x => x.Id == id);
            if (vm.Category == null)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ კატეგორიის პოვნა.";
                return View("error");
            }
            vm.Kinds = db.Kinds.Where(x => x.CategoryId == id).ToList();
            if (vm.Kinds == null)
            {
                ViewBag.Text = "ჩვენ არ შეგვიძლია ამ ქვეკატეგორიების პოვნა.";
                return View("error");
            }
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult AboutUs()
        {
            ViewModel vm = new ViewModel();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Adds = db.Adds.ToList();
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult AboutPayment()
        {
            ViewModel vm = new ViewModel();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Adds = db.Adds.ToList();
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult Delivery()
        {
            ViewModel vm = new ViewModel();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Adds = db.Adds.ToList();
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult Partners()
        {
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            vm.Adds = db.Adds.ToList();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Partners = db.Partners.Where(x => x.Status == true).ToList();
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult Post(int id, int? commentId, string name, string mail, string message)
        {
            Comment cmnt = new Comment();
            cmnt.ProductId = id;
            cmnt.Name = name;
            cmnt.Email = mail;
            cmnt.Text = message;
            cmnt.PostDate = DateTime.Now;
            cmnt.Status = false;
            if (commentId != null)
            {
                cmnt.CommentId = commentId;
            }
            db.Comments.Add(cmnt);
            db.SaveChanges();
            return RedirectToAction("Product/" + id, "home");
        }
        public ActionResult Filter(int? categoryId, int? KindId, int? PartnerId, int minCost, int maxCost)
        {
            List<Product> pt = db.Products.Where(x => x.SaledCost > minCost && x.SaledCost < maxCost && x.Status == true).ToList();
            if (categoryId != null)
            {
                pt = pt.Where(x => x.CategoryId == categoryId).ToList();
            }
            if (KindId != null)
            {
                pt = pt.Where(x => x.KindId == KindId).ToList();
            }
            if (PartnerId != null)
            {
                pt = pt.Where(x => x.PartnerId == PartnerId).ToList();
            }
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            vm.Adds = db.Adds.ToList();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Partners = db.Partners.Where(x => x.Status == true).ToList();
            vm.Products = pt;
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult Search(string search)
        {
            List<Product> pt = db.Products.Where(x => x.Name.IndexOf(search) > -1 && x.Status == true).ToList();
            ViewModel vm = new ViewModel();
            vm.Categories = db.Categories.ToList();
            vm.Adds = db.Adds.ToList();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Partners = db.Partners.Where(x => x.Status == true).ToList();
            vm.Products = pt;
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult Basket(string productsList)
        {
            ViewModel vm = new ViewModel();
            List<Product> productsVm = new List<Product>();
            if (productsList != null)
            {
                if (productsList.ToString().IndexOf(",") > -1)
                {
                    String[] productsAray = productsList.ToString().Split(',');
                    foreach (var product in productsAray)
                    {
                        if (product.Length > 0)
                        {
                            var id = Convert.ToInt32(product);
                            Product pt = db.Products.FirstOrDefault(x => x.Id == id && x.Status == true);
                            if (pt != null)
                            {
                                productsVm.Add(pt);
                            }
                        }

                    }
                }
                else
                {
                    var id = Convert.ToInt32(productsList);
                    Product pt = db.Products.FirstOrDefault(x => x.Id == id && x.Status == true);
                    if (pt != null)
                    {
                        productsVm.Add(pt);
                    }
                }
            }
            vm.Products = productsVm;
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.info = db.Abouts.FirstOrDefault();
            return View(vm);
        }
        public ActionResult AfterPayment(string encoded)
        {
            ViewModel vm = new ViewModel();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.info = db.Abouts.FirstOrDefault();
            string products = encoded.Split('-')[0];
            var productsArr = products.Split(',');
            List<Product> productsList = new List<Product>();
            foreach (var p in productsArr)
            {
                if (p != "") {
                    int index = Convert.ToInt32(p);
                    productsList.Add(db.Products.FirstOrDefault(x => x.Id == index));
                }
            }
            vm.Products = productsList;
            string detailsWithTotal = encoded.Split('-')[1];
            string details = detailsWithTotal.Split('!')[0];
            var detailsArr = details.Split('_');
            List<string> detailsList = new List<string>();
            foreach (var d in detailsArr)
            {
                if (d != "")
                {
                    detailsList.Add(d);
                }
            }
            vm.Details = detailsList;
            vm.Total = detailsWithTotal.Split('!')[1];
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AfterPayment(string fullname, string email, string phone, string town,string address, int? price, string? desc)
        {
            order order = new order();
            order.Fullname = fullname;
            order.Mail = email;
            order.Mobile = phone;
            if(town == "0")
            {
                order.City = "Tbilisi";
            }
            order.Address = address;
            order.Price = price;
            order.Description = desc;
            order.Price = price;
            order.status = false;
            order.Description = desc;
            db.orders.Add(order);
            db.SaveChanges();
            return RedirectToAction("createorder/" + order.Id, "unipay");
        }
    }
}