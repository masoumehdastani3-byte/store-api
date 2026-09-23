using API.Models;
using DataAccess;
using System.Linq;
using System.Web.Mvc;

namespace Api.Controllers.Product
{
    public class ProductListElmazController : Controller
    {
        // GET: ProductListElmaz
        StoreEntities db = new StoreEntities();
        public ActionResult Index()
        {
            var comp = db.Companies.Where(a => a.ID == 1).FirstOrDefault();
            var companyID = comp.ID;
            ViewBag.Title = comp.Name;
            int? ProductGroupID = null;
            int? ProductID = null;

            var result = db.sp_Product_Select_Elmaz("fa", Settings.GetIP(), ProductGroupID, ProductID, companyID).ToList();
            return View(result);
        }
    }
}