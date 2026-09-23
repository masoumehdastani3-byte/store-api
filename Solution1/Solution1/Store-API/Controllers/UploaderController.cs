using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using API.Models;


namespace API.Controllers
{

    public class UploaderController : ApiController
    {
        [HttpPost]
        public string Post()
        {
            string uploadPathRoot = System.Configuration.ConfigurationManager.AppSettings["DocumentUrl"];
            var httpRequest = System.Web.HttpContext.Current.Request;
            string FolderName = httpRequest.Form["FolderName"];
            string path = httpRequest.Form["path"];
            string Height = httpRequest.Form["imageHeight"];
            string Width = httpRequest.Form["imageWidth"];

            string uploadPath = uploadPathRoot + FolderName;
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            if (httpRequest.Files == null || httpRequest.Files.Count == 0)
            {
                return "Error";
            }

            var savedPaths = new List<string>();

            // پشتیبانی از یک یا چند فایل در یک درخواست
            for (int i = 0; i < httpRequest.Files.Count; i++)
            {
                HttpPostedFile postedFile = httpRequest.Files[i];
                if (postedFile == null || postedFile.ContentLength <= 0)
                {
                    continue;
                }

                string relative = SaveOneFile(postedFile, uploadPath, path, Height, Width);
                if (relative == "Error")
                {
                    // اگر تنها فایل خطا بدهد Error برمی‌گردد؛ در حالت چندفایلی بقیه ادامه می‌یابند
                    if (httpRequest.Files.Count == 1)
                    {
                        return "Error";
                    }
                    continue;
                }
                if (!string.IsNullOrEmpty(relative))
                {
                    savedPaths.Add(relative);
                }
            }

            if (savedPaths.Count == 0)
            {
                return "Error";
            }

            // سازگاری با کلاینت‌های قدیمی: یک فایل → یک مسیر
            // چند فایل → مسیرها با | جدا می‌شوند
            return string.Join("|", savedPaths);
        }

        private string SaveOneFile(HttpPostedFile postedFile, string uploadPath, string path, string Height, string Width)
        {
            string FileName = null;
            string ext = Path.GetExtension(postedFile.FileName) ?? "";

            if (!ext.ToLower().Contains("mp4"))
            {
                Image imgFile = Image.FromStream(postedFile.InputStream);
                Bitmap img = new Bitmap(imgFile);

                if (!ext.ToLower().Contains("png"))
                {
                    imgFile = GetCompressedBitmap(img, 2048);
                }
                var imageHeight = img.Height;
                var imageWidth = img.Width;

                if ((imageHeight.ToString() == Height && imageWidth.ToString() == Width) || (Height == null))
                {
                    FileName = BuildUniqueFileName(postedFile.FileName);
                    var filePath = Path.Combine(uploadPath.TrimEnd('\\', '/'), FileName);

                    string watermark = "dastani carpet";
                    // بزرگ‌تر تا روی فرش‌های پرگل دیده شود؛ وسط تا با بریدن لبه حذف نشود
                    float fontSize = Math.Max(32f, Math.Min(96f, imageWidth * 0.075f));
                    Font font = new Font("Arial", fontSize, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel);
                    int opacity = 220;
                    SolidBrush brush = new SolidBrush(Color.FromArgb(opacity, Color.Gold));
                    // حاشیه تیره برای خوانایی روی زمینه شلوغ
                    SolidBrush outlineBrush = new SolidBrush(Color.FromArgb(160, Color.Black));

                    Graphics graphics = Graphics.FromImage(img);
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    // مورب در مرکز تصویر — سخت‌تر برای بریدن و استفاده دوباره
                    graphics.TranslateTransform(imageWidth / 2f, imageHeight / 2f);
                    graphics.RotateTransform(-30f);
                    PointF center = new PointF(0f, 0f);
                    float outlineOffset = Math.Max(1.5f, fontSize * 0.035f);
                    for (float ox = -outlineOffset; ox <= outlineOffset; ox += outlineOffset)
                    {
                        for (float oy = -outlineOffset; oy <= outlineOffset; oy += outlineOffset)
                        {
                            if (ox == 0f && oy == 0f)
                            {
                                continue;
                            }
                            graphics.DrawString(watermark, font, outlineBrush, new PointF(ox, oy), sf);
                        }
                    }
                    graphics.DrawString(watermark, font, brush, center, sf);
                    graphics.ResetTransform();

                    graphics.Dispose();
                    brush.Dispose();
                    outlineBrush.Dispose();
                    font.Dispose();
                    sf.Dispose();

                    string saveExt = Path.GetExtension(FileName).ToLowerInvariant();
                    if (saveExt == ".png")
                    {
                        img.Save(filePath, ImageFormat.Png);
                    }
                    else
                    {
                        using (Bitmap jpegReady = ToJpegCompatible(img))
                        {
                            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                            ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                            EncoderParameters parameters = new EncoderParameters(1);
                            parameters.Param[0] = qualityParam;
                            jpegReady.Save(filePath, jpegCodec, parameters);
                        }
                    }

                    try
                    {
                        ProductImageThumbHelper.SaveSmallFromBitmap(img, uploadPath, FileName);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Models.Log log = new Models.Log();
                            log.WriteErrorLog(" SaveSmallImage :" + ex.Message);
                        }
                        catch { }
                    }

                    img.Dispose();
                    imgFile.Dispose();
                }
                else
                {
                    img.Dispose();
                    imgFile.Dispose();
                    return "Error";
                }
            }
            else
            {
                FileName = BuildUniqueFileName(postedFile.FileName);
                var filePath = Path.Combine(uploadPath.TrimEnd('\\', '/'), FileName);
                postedFile.SaveAs(filePath);
            }

            string prefix = path ?? "";
            if (!prefix.EndsWith("/") && !prefix.EndsWith("\\"))
            {
                prefix = prefix + "/";
            }
            return prefix.Replace('\\', '/') + FileName;
        }

        private static string BuildUniqueFileName(string originalName)
        {
            string baseName = Path.GetFileNameWithoutExtension(originalName) ?? "file";
            baseName = new string(baseName.Take(10).ToArray()).Replace(" ", "-");
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = "file";
            }
            string ext = Path.GetExtension(originalName) ?? ".jpg";
            // Guid جلوی تداخل نام در آپلود همزمان چند فایل را می‌گیرد
            return baseName + DateTime.Now.ToString("yyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N").Substring(0, 8) + ext;
        }

        private static Bitmap ToJpegCompatible(Bitmap source)
        {
            Bitmap result = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.White);
                g.DrawImage(source, 0, 0, source.Width, source.Height);
            }
            return result;
        }

        private Image GetCompressedBitmap(Bitmap bmp, long quality)
        {
            using (var mss = new MemoryStream())
            {
                using (Bitmap jpegReady = ToJpegCompatible(bmp))
                {
                    EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    ImageCodecInfo imageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                    EncoderParameters parameters = new EncoderParameters(1);
                    parameters.Param[0] = qualityParam;
                    jpegReady.Save(mss, imageCodec, parameters);
                }
                return Image.FromStream(new MemoryStream(mss.ToArray()));
            }
        }


    }
}
