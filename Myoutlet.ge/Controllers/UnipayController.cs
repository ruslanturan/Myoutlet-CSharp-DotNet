using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using System;
using Myoutlet.ge.Models;
using System.Linq;
using System.Data.Entity;
using Myoutlet.ge.ViewModels;

namespace Myoutlet.ge.Controllers
{
    public class UnipayController : Controller
    {
        MyoutletEntities db = new MyoutletEntities();
        // GET: Unipay   
        [HttpGet]
        public ActionResult Callback()
        {
            ViewModel vm = new ViewModel();
            vm.FbLink = db.FbLinks.FirstOrDefault();
            vm.YoutubeLink = db.YoutubeLinks.FirstOrDefault();
            vm.InstaLink = db.InstaLinks.FirstOrDefault();
            vm.Adds = db.Adds.ToList();
            vm.info = db.Abouts.FirstOrDefault();
            string session = (string)HttpContext.Session["order"];
            if(session != null)
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://www.unipay.com/checkout?id=" + session);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();
                order o = db.orders.FirstOrDefault(x => x.orderId == session);
                if (result.IndexOf("icon_confirmed") > -1)
                {
                    o.status = true;
                    db.Entry(o).State = EntityState.Modified;
                    db.SaveChanges();
                    string[] products = o.Description.Split('/');
                    for(var i = 1; i < products.Length - 1; i++)
                    {
                        int lastIndex = products[i].Split('X')[0].LastIndexOf("-");
                        string name = products[i].Split('X')[0].Substring(1,lastIndex - 2);
                        var count = Convert.ToInt32(products[i].Split('X')[0].Substring(lastIndex + 2));
                        Product pt = db.Products.FirstOrDefault(x => x.Name == name);
                        pt.productCount = pt.productCount - count;
                        db.Entry(pt).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    sr.Close();
                    myResponse.Close();
                    return View("SuccessPayment", vm);
                }
                else
                {
                    o.status = false;
                    db.Entry(o).State = EntityState.Modified;
                    db.SaveChanges();
                    sr.Close();
                    myResponse.Close();
                    return View("FailedPayment", vm);
                }
            }
            return View("FailedPayment",vm);
        }

        public ActionResult Connection()
        {
            int portno = 18443;
            IPAddress ipAddress = Dns.GetHostEntry("ecommerce.ufc.ge").AddressList[0];
            try
            {
                TcpListener tcpListener = new TcpListener(ipAddress, portno);
                tcpListener.Start();
                ViewBag.Text = "salam";
            }
            catch (SocketException ex)
            {
                ViewBag.Text = ex.Message;
            }
            return View();
        }

        public ActionResult CreateOrder(int Id)
        {
            order order = db.orders.FirstOrDefault(x => x.Id == Id);
            string merchantID = "7599603";
            string password = "";
            string hashed = "";
            // back link
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes("https://myoutlet.ge/Unipay/callback");
            string backLink = Convert.ToBase64String(toEncodeAsBytes);
            // logo
            byte[] toEncodeAsBytes2 = ASCIIEncoding.ASCII.GetBytes("https://myoutlet.ge/Images/logo.png");
            string logo = Convert.ToBase64String(toEncodeAsBytes2);
            // hash
            string hash = ""+ Id + "|" + order.Price + "|GEL|Buy from Myoutlet|" + order.Description + "|" + backLink + "|" + logo + "|Myoutlet.ge|GE";
            using (MD5 md5Hash = MD5.Create())
            {
                hashed = GetMd5Hash(md5Hash, hash);

            }
            string json = new JavaScriptSerializer().Serialize(new
            {
                MerchantID = "7599603",
                MerchantUser = "000003",
                MerchantOrderID = Id,
                OrderPrice = order.Price,
                OrderCurrency = "GEL",
                OrderName = "Buy from Myoutlet",
                OrderDescription = order.Description,
                BackLink = backLink,
                Mlogo = logo,
                Mslogan = "Myoutlet.ge",
                Language = "GE",
                Hash = hashed
            });
            string passwordStr = "" + Id + "|" + order.Price + "|Buy from Myoutlet";
            using (MD5 md5Hash = MD5.Create())
            {
                password = GetMd5Hash(md5Hash, passwordStr);
            }
            CredentialCache credentialCache = new CredentialCache
            {
                {
                    new Uri("https://api.unipay.com/checkout/createorder"), "Basic",
                    new NetworkCredential(merchantID, password)
                }
            };
            var request = (HttpWebRequest)WebRequest.Create("https://api.unipay.com/checkout/createorder");
            request.Credentials = credentialCache;
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }
            var response = request.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                string result = sr.ReadToEnd();
                string code = result.Split(',')[0];
                string orderHashId = result.Split(',')[3];
                order.orderHashId = orderHashId.Replace("\"UnipayOrderHashID\":\"", "").Replace("\"}}","");
                if (code == "{\"Errorcode\":0")
                {
                    string check = result.Split(',')[2];
                    string checkout = check.Replace(@"\", @"").Replace("\"Data\":{\"Checkout\":\"","").Replace("\"","").Replace("https://","");
                    string orderId = checkout.Replace("www.unipay.com/ka/checkout?id=", "");
                    order.orderId = orderId;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["order"] = orderId;
                    return Redirect("//" + checkout);
                }
                else
                {
                    ViewBag.Text = "შეცდომა";
                }
            } 
            return View();
        }
        string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

    }
}