using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace API.Models
{
    /// <summary>
    /// ساخت و مسیر تصویر کوچک در پوشه small کنار تصویر اصلی.
    /// مثال: /Product/1/a.jpg → /Product/1/small/a.jpg
    /// </summary>
    public static class ProductImageThumbHelper
    {
        public const int SmallImageMaxSize = 400;
        public const long SmallImageQuality = 60;
        public const string SmallFolderName = "small";

        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };

        public static string GetDocumentRoot()
        {
            return System.Configuration.ConfigurationManager.AppSettings["DocumentUrl"];
        }

        /// <summary>
        /// مسیر نسبی نسخه کوچک از روی مسیر نسبی تصویر بزرگ
        /// </summary>
        public static string GetSmallRelativeUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return relativeUrl;
            }

            relativeUrl = relativeUrl.Replace('\\', '/').Trim();
            if (relativeUrl.IndexOf("/" + SmallFolderName + "/", StringComparison.OrdinalIgnoreCase) >= 0
                || relativeUrl.StartsWith(SmallFolderName + "/", StringComparison.OrdinalIgnoreCase))
            {
                return relativeUrl;
            }

            int lastSlash = relativeUrl.LastIndexOf('/');
            if (lastSlash < 0)
            {
                return SmallFolderName + "/" + relativeUrl;
            }

            return relativeUrl.Substring(0, lastSlash + 1) + SmallFolderName + "/" + relativeUrl.Substring(lastSlash + 1);
        }

        public static string ToPhysicalPath(string relativeUrl)
        {
            string root = GetDocumentRoot() ?? "";
            string normalized = (relativeUrl ?? "").Replace('/', '\\').TrimStart('\\');
            return Path.Combine(root.TrimEnd('\\', '/'), normalized);
        }

        public static bool IsImageFile(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
            {
                return false;
            }
            return ImageExtensions.Contains(ext.ToLowerInvariant());
        }

        public static bool IsSmallFolderPath(string path)
        {
            string n = (path ?? "").Replace('\\', '/');
            return n.IndexOf("/" + SmallFolderName + "/", StringComparison.OrdinalIgnoreCase) >= 0
                || n.EndsWith("/" + SmallFolderName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// از Bitmap در حافظه، فایل کوچک را در پوشه small ذخیره می‌کند.
        /// </summary>
        public static void SaveSmallFromBitmap(Bitmap source, string largePhysicalDirectory, string fileName)
        {
            if (source == null || string.IsNullOrEmpty(largePhysicalDirectory) || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string smallDir = Path.Combine(largePhysicalDirectory.TrimEnd('\\', '/'), SmallFolderName);
            if (!Directory.Exists(smallDir))
            {
                Directory.CreateDirectory(smallDir);
            }

            string smallPath = Path.Combine(smallDir, fileName);
            SaveResized(source, smallPath, Path.GetExtension(fileName));
        }

        /// <summary>
        /// از فایل بزرگ روی دیسک، نسخه کوچک می‌سازد. اگر force=false و فایل کوچک موجود باشد، رد می‌شود.
        /// </summary>
        public static bool CreateSmallFromFile(string largePhysicalPath, bool force = false)
        {
            if (string.IsNullOrEmpty(largePhysicalPath) || !File.Exists(largePhysicalPath) || !IsImageFile(largePhysicalPath))
            {
                return false;
            }

            string dir = Path.GetDirectoryName(largePhysicalPath);
            string fileName = Path.GetFileName(largePhysicalPath);
            string smallDir = Path.Combine(dir, SmallFolderName);
            string smallPath = Path.Combine(smallDir, fileName);

            if (!force && File.Exists(smallPath))
            {
                return false;
            }

            if (!Directory.Exists(smallDir))
            {
                Directory.CreateDirectory(smallDir);
            }

            using (var source = new Bitmap(largePhysicalPath))
            {
                SaveResized(source, smallPath, Path.GetExtension(fileName));
            }
            return true;
        }

        private static void SaveResized(Bitmap source, string smallPath, string ext)
        {
            ext = (ext ?? "").ToLowerInvariant();
            int maxSide = Math.Max(source.Width, source.Height);
            int w = source.Width;
            int h = source.Height;
            if (maxSide > SmallImageMaxSize)
            {
                float scale = (float)SmallImageMaxSize / maxSide;
                w = Math.Max(1, (int)(source.Width * scale));
                h = Math.Max(1, (int)(source.Height * scale));
            }

            if (ext == ".png")
            {
                using (Bitmap toSave = new Bitmap(w, h, PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(toSave))
                    {
                        g.Clear(Color.Transparent);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.DrawImage(source, 0, 0, w, h);
                    }
                    toSave.Save(smallPath, ImageFormat.Png);
                }
            }
            else
            {
                using (Bitmap toSave = new Bitmap(w, h, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(toSave))
                    {
                        g.Clear(Color.White);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.DrawImage(source, 0, 0, w, h);
                    }
                    EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, SmallImageQuality);
                    ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                    EncoderParameters parameters = new EncoderParameters(1);
                    parameters.Param[0] = qualityParam;
                    toSave.Save(smallPath, jpegCodec, parameters);
                }
            }
        }
    }
}
