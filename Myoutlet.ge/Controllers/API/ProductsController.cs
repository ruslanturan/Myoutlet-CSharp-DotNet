using Myoutlet.ge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Myoutlet.ge.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
    MyoutletEntities db = new MyoutletEntities();
        public IQueryable<Object> GetByKind(int id)
        {
            return db.Products.Where(x => x.Status == true && x.KindId == id && x.productCount > 0).Select(x => new {
                id = x.Id,
                sale = x.SaleInPercent,
                saledCost = x.SaledCost,
                productName = x.Name,
                productUniqueNumber = x.ProductUniqueNum,
                photo = x.Photo
            });
        }
    }
}
