using API.Models;
using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class InsertCompanyTransportationController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(
          string Lang
        , string UserName
        , int CompanyID
        , decimal? Factor
        , string Title
        , int TransportationCalculate_BasicData
        , decimal? TransportationPrice
        )
        {
            try
            {
                DataAccess.CompanyTransportation model = new DataAccess.CompanyTransportation();
                model.CompanyID = CompanyID;
                model.Factor = Factor;
                model.IsFree = db.BasicDatas.Where(a => a.GroupType == "TransportationCalculate" && a.CodeINT == TransportationCalculate_BasicData).Select(a => a.Show).FirstOrDefault();
                model.LogUser = Settings.SetNull(UserName);
                model.Title = Settings.SetNull(Title);
                model.TransportationCalculate_BasicData = TransportationCalculate_BasicData;
                model.TransportationPrice = TransportationPrice;
                model.CreateDate = DateTime.Now;
                db.CompanyTransportations.Add(model);
                var dd = db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertCompanyTransportation :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertCompanyTransportation InnerException :" + ex.InnerException.Message);
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
