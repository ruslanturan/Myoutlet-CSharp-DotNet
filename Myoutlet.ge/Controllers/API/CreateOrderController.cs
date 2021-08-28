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
    [RoutePrefix("api/createorder")]
    public class CreateOrderController : ApiController
    {
        MyoutletEntities db = new MyoutletEntities();
        public int Get(string fullname, string email, string phone, string town, string address, int? price, string? desc)
        {
            order order = new order();
            order.Fullname = fullname;
            order.Mail = email;
            order.Mobile = phone;
            if (town == "0")
            {
                order.City = "Tbilisi";
            }
            order.Address = address;
            order.Price = price;
            order.Description = desc;
            order.Price = price;
            order.Description = desc;
            order.status = false;
            db.orders.Add(order);
            db.SaveChanges();
            return order.Id;
        }
    }
}

