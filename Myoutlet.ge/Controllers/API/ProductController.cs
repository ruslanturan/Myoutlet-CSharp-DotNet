using Myoutlet.ge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Myoutlet.ge.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        MyoutletEntities db = new MyoutletEntities();
        public IQueryable<Object> Get(int id)
        {
            return db.Products.Where(x => x.Id == id && x.Status == true).Select(x => new {
                id = x.Id,
                cost = x.Cost,
                sale = x.SaleInPercent,
                saledCost = x.SaledCost,
                productName = x.Name,
                kindName = x.Kind.Name,
                categoryName = x.Category.Name,
                partner = x.Partner.CompanyName,
                uniqueNum = x.ProductUniqueNum,
                count = x.productCount,
                comments = x.Comments.Where(z => z.ProductId == id && z.Status == true).Select(j => new
                {
                    j.Name,
                    j.Email,
                    j.Text,
                    j.CommentId,
                    j.PostDate
                }),
                descriptions = x.Descriptions.Where(z => z.ProductId == id).Select(j => j.Heading),
                galleries = db.Galleries.Where(z => z.ProductUniqueNum == x.ProductUniqueNum).Select(j => j.Link)

            });
        }
    }
}
