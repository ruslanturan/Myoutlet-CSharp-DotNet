using Myoutlet.ge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Myoutlet.ge.Controllers
{
    [RoutePrefix("api/kinds")]
    public class KindsController : ApiController
    {
        MyoutletEntities db = new MyoutletEntities();
        public IQueryable<Object> Get(int id)
        {
            return db.Kinds.Where(x => x.CategoryId == id && x.Products.Count > 0).Select(x => new { x.Id, x.Name,x.image });
        }
    }
}
