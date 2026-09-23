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
    public class UpdateProductGroupController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpPost]
        public string Post([FromBody] Rootobject Model)

        {
            try
            {
                DataAccess.ProductGroup model = db.ProductGroups.Where(a => a.ID == Model._params.ID).FirstOrDefault();
                model.LogUser = Settings.SetNull(Model._params.LogUser);
                model.ParentID = Model._params.ParentID;
                model.Title = Settings.SetNull(Model._params.Title);
                model.UpdateDate = DateTime.Now;

                var dd = db.SaveChanges();
                if (Settings.SetNull(Model._params.Specification) != null)
                {
                    model.IsLastChid = true;
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Details>), new XmlRootAttribute("ProductGroupDetails"));
                    StringReader stringReader = new StringReader(Model._params.Specification);
                    List<Details> Detail = (List<Details>)serializer.Deserialize(stringReader);
                    string Type = "Insert";
                    foreach (var item in Detail)

                    {
                        var bb = db.ProductGroupDetails.Where(a => a.ID == item.ID).FirstOrDefault();


                        if (bb == null)
                        {

                            bb = new ProductGroupDetail();
                            Type = "Insert";
                        }
                        else
                        {
                            db.ProductSpecifications.Where(a => a.GroupType == model.ID && a.GroupTypeName == bb.GroupTypeName).ToList().ForEach(a => { a.GroupTypeName = item.GroupTypeName; a.GroupTypeName_En = item.GroupTypeName; });
                            Type = "Update";
                        }
                        bb.GroupTypeName = item.GroupTypeName;
                        bb.IsShowSearch = item.IsShowSearch;
                        bb.ProductGroupID = model.ID;
                        bb.Sort = bb.Sort = Convert.ToInt32(Settings.SetNull(item.Sort));
                        if (Type == "Insert")
                        {
                            db.ProductGroupDetails.Add(bb);
                        }


                    }

                    foreach (var item in db.ProductGroupDetails.Where(a => a.ProductGroupID == model.ID).ToList())
                    {
                        var cc = Detail.Where(a => a.ID == item.ID).ToList();
                        if (cc.Count() == 0)
                        {
                            db.ProductGroupDetails.Remove(item);
                        }
                    }
                    if (Detail.Count() > 0)
                    {
                        model.IsLastChid = true;
                    }
                    else
                    {
                        model.IsLastChid = false;
                    }
                    db.SaveChanges();
                }

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

        public class Rootobject
        {
            public Params _params { get; set; }
        }

        public class Params
        {
            public int ID { get; set; }
            public string Lang { get; set; }
            public string Title { get; set; }
            public int? ParentID { get; set; }
            public string Specification { get; set; }
            public string LogUser { get; set; }
        }


    }
}
