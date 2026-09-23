using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace Api.Controllers
{
    public class SelectProductGroupDetailGroupByController : ApiController
    {
        StoreEntities db = new StoreEntities();
        API.Models.PersianCulture pc = new API.Models.PersianCulture();
        [HttpGet]
        public List<sp_ProductGroupDetail_GroupBy_Select_Result> Get(string UserName, string Lang)
        {
            var result = db.sp_ProductGroupDetail_GroupBy_Select(UserName, Lang).ToList();
            return result;
        }
    }
}
