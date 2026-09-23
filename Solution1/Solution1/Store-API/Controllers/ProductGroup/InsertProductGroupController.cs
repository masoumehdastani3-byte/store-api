using API.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Xml.Serialization;
namespace API.Controllers
{
    public class InsertProductGroupController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(string Lang
        , string LogUser
        , int? ParentID
        , string Title
        , string Specification
        )
        {
            try
            {
                DataAccess.ProductGroup model = new DataAccess.ProductGroup();
                model.LogUser = Settings.SetNull(LogUser);
                model.ParentID = ParentID;
                model.Title = Settings.SetNull(Title);
                model.CreateDate = DateTime.Now;
                db.ProductGroups.Add(model);
                var dd = db.SaveChanges();
                if (Settings.SetNull(Specification) != null)
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(List<Details>), new XmlRootAttribute("ProductGroupDetails"));
                    StringReader stringReader = new StringReader(Specification);
                    List<Details> Detail = (List<Details>)serializer.Deserialize(stringReader);

                    foreach (var item in Detail)
                    {
                        var bb = new ProductGroupDetail();

                        bb.GroupTypeName = item.GroupTypeName;
                        bb.IsShowSearch = item.IsShowSearch;
                        bb.ProductGroupID = model.ID;
                        bb.Sort = Convert.ToInt32(Settings.SetNull(item.Sort));
                        db.ProductGroupDetails.Add(bb);

                    }
                    if (Detail.Count() > 0)
                    {
                        model.IsLastChid = true;
                    }
                    else
                    {
                        model.IsLastChid = false;
                    }

                }

                db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertProductGroup :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertProductGroup InnerException :" + ex.InnerException.Message);
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
