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
    public class SuccessOrderController : ApiController
    {
        MyoutletEntities db = new MyoutletEntities();

        public string Get(int id)
        {
            order order = db.orders.FirstOrDefault(x => x.Id == id);
            order.status = true;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            return ("ok");
        }
    }
}
