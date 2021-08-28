using Myoutlet.ge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Myoutlet.ge.Controllers
{
    [RoutePrefix("api/categories")]
    public class CategoriesController : ApiController
    {
        MyoutletEntities db = new MyoutletEntities();
        public IQueryable<Object> Get()
        {
            return db.Categories.Where(x => x.Products.Count > 0).Select(x => new { x.Id, x.Name });
        }
    }
}
