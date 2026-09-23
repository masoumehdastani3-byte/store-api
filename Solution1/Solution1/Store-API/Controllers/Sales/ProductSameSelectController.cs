using Api.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers.Product
{

    public class ProductSameSelectController : ApiController
    {
        DataAccess.StoreEntities db = new StoreEntities();
        public List<ProductSelect> Get(string lang, string loguser, int productID)
        {
            ProductResult model = new ProductResult();
            string stringParamGroupName = "";
            var fiter = db.sp_ProductSameSearch(productID).ToList();
            foreach (var item in fiter)
            {
                stringParamGroupName = stringParamGroupName + item.Title + ",";
            }
            var productGroupId = db.Products.Where(a => a.ID == productID).Select(a => a.ProductGroupID).FirstOrDefault();
            List<ProductSelect> pro;
            pro = model.SelectProduct(lang, loguser, null, false, false, productGroupId, null, null, null, stringParamGroupName, null, "PriceSales", 1);
            return pro;

        }

    }
}