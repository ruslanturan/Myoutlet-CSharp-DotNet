using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Myoutlet.ge.Models;

namespace Myoutlet.ge.Areas.Main.Controllers
{
    public class AjaxController : Controller
    {
        MyoutletEntities db = new MyoutletEntities();
        // GET: Main/Ajax
        public ActionResult GetLogo(int Id)
        {
            return Json(db.Partners.Where(x => x.Id == Id)
                .Select(m => new
                {
                    m.CompanyLogo
                }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStatus(int Id)
        {
            return Json(db.Partners.Where(x => x.Id == Id)
                .Select(m => new
                {
                    m.Status
                }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetKinds(int Id)
        {
            return Json(db.Kinds.Where(x => x.CategoryId == Id)
                .Select(m => new
                {
                    m.Id,
                    m.Name
                }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPartners(int Id)
        {
            var pt = db.Products.Where(x => x.KindId == Id);
            var pts = new List<Partner>();
            foreach(var p in pt)
            {
                if (!pts.Contains(db.Partners.FirstOrDefault(x => x.Id == p.PartnerId)))
                {
                pts.Add(db.Partners.FirstOrDefault(x => x.Id == p.PartnerId));
                }
            }
            return Json(pts.Select(m => new
                {
                    m.Id,
                    m.CompanyName
                }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUniqueNums()
        {
            return Json(db.Products.Select(m => new
                {
                    m.Id,
                    m.ProductUniqueNum
                }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UploadImages(decimal ? id)
        {
            if(id != null)
            {
                // Checking no of files injected in Request object  
                if (Request.Files.Count > 0)
                {
                    try
                    {
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string fname;
                            string link;
                            // Checking for Internet Explorer  
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = id + testfiles[testfiles.Length - 1];
                                link = fname;
                            }
                            else
                            {
                                fname = id + file.FileName;
                                link = fname;
                            }
                            string subPath = "~/Images/Product-Images/" + id;
                            bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));
                            if (!exists)
                                Directory.CreateDirectory(Server.MapPath(subPath));
                            // Get the complete folder path and store the file inside it.  
                            fname = Path.Combine(Server.MapPath(subPath), fname);
                            file.SaveAs(fname);
                            Gallery img = new Gallery();
                            img.Link = link;
                            img.ProductUniqueNum = id;
                            db.Galleries.Add(img);
                            db.SaveChanges();
                        }
                    }
                    catch
                    {
                    }
                }
            }           
            return Json("Successful");
        }
        public ActionResult RemoveImage(int id, string name)
        {
            string myPath = Server.MapPath("//Images/Product-images/" + id + "/" + name);
            System.IO.File.Delete(myPath);
            int imgId = db.Galleries.FirstOrDefault(x => x.Link == name).Id;
            string mainConn = ConfigurationManager.ConnectionStrings["MyoutletEntitiesCodeFirst"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(mainConn);
            string sqlQuery = " DELETE FROM [dbo].[Galleries] WHERE id=" + imgId;
            SqlCommand sqlComm = new SqlCommand(sqlQuery, sqlConn);
            sqlConn.Open();
            sqlComm.ExecuteNonQuery();
            sqlConn.Close();
            Product pt = db.Products.FirstOrDefault(x => x.ProductUniqueNum == id);
            pt.Photo = db.Galleries.FirstOrDefault(x => x.ProductUniqueNum == pt.ProductUniqueNum).Link;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddCallRequest(int id, int phone, int productId)
        {
            callRequest req = new callRequest();
            req.ProductId = id;
            req.PhoneNum = phone;
            req.PartnerId = productId;
            req.Status = false;
            db.callRequests.Add(req);
            db.SaveChanges();
            return Json("Successful");
        }
        public ActionResult GetSearchResult(string val)
        {
            var pt = db.Products.Where(x => x.Name.Contains(val) && x.Status == true);
            var pts = new List<Product>();
            foreach (var p in pt)
            {
                    pts.Add(p);                
            }
            return Json(pts.Select(m => new
            {
                m.Id,
                m.Name
            }), JsonRequestBehavior.AllowGet);
        }
    }
}