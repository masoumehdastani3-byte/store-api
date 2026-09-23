using API.Models;
using DataAccess;
using System;
using System.Web.Http;

namespace API.Controllers
{
    public class InsertProductGroupDetailController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(
          string BasicDataGrouptype
        , bool? IsShowSearch
        , int? ProductGroupID
        )
        {
            try
            {
                DataAccess.ProductGroupDetail model = new DataAccess.ProductGroupDetail();
                model.GroupTypeName = Settings.SetNull(BasicDataGrouptype);
                model.IsShowSearch = IsShowSearch;
                model.ProductGroupID = ProductGroupID;
                db.ProductGroupDetails.Add(model);
                var dd = db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertProductGroupDetail :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertProductGroupDetail InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
        }
    }
}
