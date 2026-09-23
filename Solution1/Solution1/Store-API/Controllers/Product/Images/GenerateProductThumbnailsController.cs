using API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    /// <summary>
    /// ساخت دسته‌ای تصاویر کوچک برای فایل‌های موجود در پوشه Product
    /// GET /api/GenerateProductThumbnails/Get?CompanyID=1&amp;force=false
    /// </summary>
    public class GenerateProductThumbnailsController : ApiController
    {
        [HttpGet]
        public object Get(int? CompanyID = null, bool force = false)
        {
            var result = new ThumbBatchResult();
            try
            {
                string root = ProductImageThumbHelper.GetDocumentRoot();
                if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
                {
                    result.Success = false;
                    result.Message = "DocumentUrl not found";
                    return result;
                }

                string productRoot = Path.Combine(root.TrimEnd('\\', '/'), "Product");
                if (!Directory.Exists(productRoot))
                {
                    result.Success = false;
                    result.Message = "Product folder not found: " + productRoot;
                    return result;
                }

                IEnumerable<string> companyDirs;
                if (CompanyID.HasValue && CompanyID.Value > 0)
                {
                    string companyPath = Path.Combine(productRoot, CompanyID.Value.ToString());
                    if (!Directory.Exists(companyPath))
                    {
                        result.Success = false;
                        result.Message = "Company folder not found: " + companyPath;
                        return result;
                    }
                    companyDirs = new[] { companyPath };
                }
                else
                {
                    companyDirs = Directory.GetDirectories(productRoot);
                }

                foreach (string companyDir in companyDirs)
                {
                    ProcessDirectory(companyDir, force, result);
                }

                result.Success = true;
                result.Message = string.Format(
                    "Processed={0}, Created={1}, Skipped={2}, Failed={3}",
                    result.Processed, result.Created, result.Skipped, result.Failed);
                return result;
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.WriteErrorLog("GenerateProductThumbnails: " + ex.Message);
                result.Success = false;
                result.Message = ex.Message;
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        private void ProcessDirectory(string directory, bool force, ThumbBatchResult result)
        {
            // فقط فایل‌های سطح همین پوشه (نه داخل small و نه زیرپوشه‌های دیگر غیر از خود شرکت)
            string[] files;
            try
            {
                files = Directory.GetFiles(directory);
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Errors.Add(directory + " : " + ex.Message);
                return;
            }

            foreach (string file in files)
            {
                if (!ProductImageThumbHelper.IsImageFile(file))
                {
                    continue;
                }

                // فایل‌های قدیمی با پسوند _s را نادیده بگیر
                string name = Path.GetFileNameWithoutExtension(file);
                if (name.EndsWith("_s", StringComparison.OrdinalIgnoreCase))
                {
                    result.Skipped++;
                    continue;
                }

                result.Processed++;
                try
                {
                    bool created = ProductImageThumbHelper.CreateSmallFromFile(file, force);
                    if (created)
                    {
                        result.Created++;
                    }
                    else
                    {
                        result.Skipped++;
                    }
                }
                catch (Exception ex)
                {
                    result.Failed++;
                    result.Errors.Add(Path.GetFileName(file) + " : " + ex.Message);
                    try
                    {
                        Log log = new Log();
                        log.WriteErrorLog("GenerateProductThumbnails file " + file + " : " + ex.Message);
                    }
                    catch { }
                }
            }
        }
    }

    public class ThumbBatchResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Processed { get; set; }
        public int Created { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }
        public List<string> Errors { get; set; }

        public ThumbBatchResult()
        {
            Errors = new List<string>();
            Message = "";
        }
    }
}
