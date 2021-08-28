using Myoutlet.ge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Myoutlet.ge.Controllers
{
    [RoutePrefix("api/barcode")]
    public class BarcodeController : ApiController
    {
        MyoutletEntities db = new MyoutletEntities();
        public IEnumerable<Object> Get(int id)
        {
            List<int?> product_ids = db.Barcodes.Where(x => x.barcodeId == id).Select(x => x.productId).ToList();
            List<Product> products = new List<Product>();
            foreach (var pt_id in product_ids)
            {
                Product pt = db.Products.FirstOrDefault(x => x.Id == pt_id && x.Status == true);
                if(pt != null)
                {
                    products.Add(pt);
                }
            }
            return products.Select(x => new {
                id = x.Id,
                sale = x.SaleInPercent,
                saledCost = x.SaledCost,
                productName = x.Name,
                count = x.productCount,
                productUniqueNumber = x.ProductUniqueNum,
                photo = x.Photo
            });
        }

    }
}
