using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace Api.Controllers.PageMenu
{
    public class SelectMenuPageActiveController : ApiController
    {
        StoreEntities db = new StoreEntities();
        public List<Menu> Get()
        {
            int? companyID = null;


            string WebSite = Settings.WebsiteName();
            if (companyID == null)
            {
                companyID = db.Companies.Where(a => a.WebsiteUrl == WebSite).Select(a => a.ID).FirstOrDefault();
            }
            List<Menu> menu = new List<Menu>();
            var list = db.PageMenus.Where(a => a.CompanyID == companyID && a.Active == true).ToList();
            var sub = db.PageGenerators.Where(a => a.CompanyID == companyID && a.Active == true).ToList().OrderBy(a => a.Sort);
            foreach (var item in list)
            {

                Menu me = new Menu();
                var gg = sub.Where(a => a.PageSystemCode == item.SystemCode).ToList();
                me.MenuTitle = item.MenuTitle;
                me.SystemCode = item.SystemCode;
                List<SubMenu> subm = new List<SubMenu>();
                gg.ForEach(a =>
                {
                    SubMenu m = new SubMenu();

                    m.PageTitle = a.PageTitle;
                    m.ID = a.ID;
                    subm.Add(m);

                });
                me.subMenus = subm;
                menu.Add(me);
            }
            return menu;
        }
        public class Menu
        {
            public string MenuTitle { get; set; }
            public int? SystemCode { get; set; }
            public List<SubMenu> subMenus { get; set; }

        }
        public class SubMenu
        {
            public string PageTitle { get; set; }
            public int ID { get; set; }

        }

    }
}