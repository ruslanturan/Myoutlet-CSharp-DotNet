using Myoutlet.ge.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Myoutlet.ge.Controllers
{
    public class DecreaseProductCountController : ApiController
    {
        MyoutletEntities db = new MyoutletEntities();

        public string Get(int id, int count)
        {
            Product pt = db.Products.FirstOrDefault(x => x.Id == id);
            pt.productCount = pt.productCount - count;
            db.Entry(pt).State = EntityState.Modified;
            db.SaveChanges();
            return ("ok");
        }
    }
}
