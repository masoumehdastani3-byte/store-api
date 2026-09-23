using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace API.Controllers
{
    public class SelectProductDetail_SpecificationController : ApiController
    {
        StoreEntities db = new StoreEntities();

        [HttpGet]

        public List<sp_ProductDetail_Specification_Select_Result> Get(string Lang, string UserName, int ProductID)
        {

            var list = db.sp_ProductDetail_Specification_Select(Lang, UserName, ProductID).ToList();
            return list;
        }



    }
}
