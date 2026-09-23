using API.Models;
using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class InsertProductController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();

        [HttpPost]

        public string Post([FromBody] ProductObject prod)
        {
            try
            {
                string UpdateType = "Insert";
                DataAccess.Product model = new DataAccess.Product();
                if (prod.PostData.product.ID != null && prod.PostData.product.ID != 0)
                {
                    model = db.Products.Where(a => a.ID == prod.PostData.product.ID).FirstOrDefault();
                    UpdateType = "Update";
                }
                model.Acive = prod.PostData.product.Acive;
                model.AvailableCount = prod.PostData.product.AvailableCount;
                model.BarCode = Settings.SetNull(prod.PostData.product.BarCode);
                model.CompanyID = prod.PostData.product.CompanyID;
                model.CreaeDate = DateTime.Now;
                model.Currency = Settings.SetNull(prod.PostData.product.Currency);
                model.FromDateSpecialSales = Settings.SetNull(prod.PostData.product.FromDateSpecialSales);
                model.FromDateSpecialSales_Mila = pc.ShamsiToMiladi(Settings.SetNull(prod.PostData.product.FromDateSpecialSales));
                model.ImageUrl = Settings.SetNull(prod.PostData.product.ImageUrl);
                model.IsAvailable = prod.PostData.product.IsAvailable;
                model.IsSpecialSales = prod.PostData.product.IsSpecialSales;
                model.LogUser = Settings.SetNull(prod.PostData.product.LogUser);
                model.Name = Settings.SetNull(prod.PostData.product.Name);
                model.Negative = 0;
                model.OffPercent = prod.PostData.product.OffPercent;
                model.ParentID = null;
                model.Positive = 0;
                model.PriceBuy = prod.PostData.product.PriceBuy;
                model.PriceSales = prod.PostData.product.PriceSales;
                model.ProductGroupID = prod.PostData.product.ProductGroupID;
                model.SalesCount = prod.PostData.product.SalesCount;
                model.ToDateSpecialSales = Settings.SetNull(prod.PostData.product.ToDateSpecialSales);
                model.ToDateSpecialSales_Mila = pc.ShamsiToMiladi(Settings.SetNull(prod.PostData.product.ToDateSpecialSales));
                model.IsViewInstagram = model.ImageUrl.SetNull() != null ? prod.PostData.product.IsViewInstagram : false;
                model.IsViewTelegram = model.ImageUrl.SetNull() != null ? prod.PostData.product.IsViewTelegram : false;

                model.InstagramTag = prod.PostData.product.InstagramTag;
                model.IsOffPercent = prod.PostData.product.IsOffPercent;
                model.TransportationID = prod.PostData.product.TransportationID == -1 ? null : prod.PostData.product.TransportationID;

                if (model.IsAvailable == true && model.AvailableCount == null)
                {
                    model.AvailableCount = 1;
                }

                if (model.AvailableCount != prod.PostData.product.AvailableCount)
                {
                    model.UpdateDateAvailableCount = DateTime.Now;
                }

                if (UpdateType != "Insert")
                {
                    model.IsSendInstagram = false;
                    model.IsSendTelegram = false;
                }
                if (UpdateType == "Insert")
                {
                    db.Products.Add(model);
                }


                db.SaveChanges();
                var Details = db.ProductDetails.Where(a => a.ProductID == model.ID).ToList();
                db.ProductDetails.RemoveRange(Details);
                var Image = db.ProductImages.Where(a => a.ProductID == model.ID && a.IsDefault == true).ToList();
                db.ProductImages.RemoveRange(Image);
                var des = db.ProductDescriptions.Where(a => a.ProductID == model.ID).ToList();
                db.ProductDescriptions.RemoveRange(des);
                model.SumerySpecification = "";
                foreach (var item in prod.PostData.detail)
                {
                    NewProductSpecifications(prod, model, item);

                }
                if (Settings.SetNull(model.ImageUrl) != null)
                {
                    DataAccess.ProductImage image = new DataAccess.ProductImage();
                    image.Description = model.Name;
                    image.Image_Url = Settings.SetNull(model.ImageUrl);
                    image.IsDefault = true;
                    image.ProductID = model.ID;
                    db.ProductImages.Add(image);
                }
                if (Settings.SetNull(prod.PostData.product.Description) != null)
                {
                    DataAccess.ProductDescription desc = new DataAccess.ProductDescription();
                    desc.Description = prod.PostData.product.Description;
                    desc.Lang = prod.PostData.product.Lang;
                    desc.ProductID = model.ID;
                    desc.IsShow = true;
                    db.ProductDescriptions.Add(desc);
                }
                //-------------اگر مقدار بارکد داده نشده باشد--------------
                if (model.BarCode == null)
                {
                    model.BarCode = model.ID.ToString();
                }
                var dd = db.SaveChanges();
                if (model.IsViewInstagram == true && model.ImageUrl.SetNull() != null)
                {


                    Settings.ResizeImageInstagram(model.ImageUrl, 400, 400, model.ID.ToString());

                }
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertProduct :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertProduct InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
        }

        private void NewProductSpecifications(ProductObject prod, DataAccess.Product model, detail item)
        {
            if (item.PeropertyItems.Count() > 0)
            {
                foreach (var dt in item.PeropertyItems)
                {
                    DataAccess.ProductDetail po = new ProductDetail();
                    po.ProductID = model.ID;

                    DataAccess.ProductSpecification ba = new DataAccess.ProductSpecification();
                    //اگر این مقدار وجود ندارد ایجاد شود
                    if (db.ProductSpecifications.Where(a => a.Title == dt.Text && a.GroupTypeName == item.ParamSearch).Count() == 0)
                    {


                        ba.GroupType = Convert.ToInt32(model.ProductGroupID);
                        ba.GroupTypeName_En = item.ParamSearch;
                        ba.GroupTypeName = item.ParamSearch;
                        ba.Title = dt.Text;

                        ba.CompanyID = prod.PostData.product.CompanyID;
                        db.ProductSpecifications.Add(ba);
                        db.SaveChanges();
                        po.ProductSpecificationID = ba.ID;
                    }
                    else
                    {
                        po.ProductSpecificationID = db.ProductSpecifications.Where(a => a.Title == dt.Text && a.GroupTypeName == item.ParamSearch).Select(a => a.ID).FirstOrDefault();
                    }



                    db.ProductDetails.Add(po);
                }
                item.Specification = String.Join(",", item.PeropertyItems.Select(a => a.Text));
                model.SumerySpecification = model.SumerySpecification + "\r\n" + item.ParamSearch + " : " + item.Specification;
            }
        }



    }
}
