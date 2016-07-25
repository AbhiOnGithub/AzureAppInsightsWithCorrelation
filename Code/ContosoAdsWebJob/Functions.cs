using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using ContosoAdsCommon;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ContosoAdsWebJob
{
    using ContosoAds.Logger;

    using Microsoft.ApplicationInsights.Extensibility;

    public class Functions
    {
        private static ILogger logger = new AILogger();
        
        public static void GenerateThumbnail(
        [QueueTrigger("thumbnailrequest")] BlobInformation blobInfo,
        [Blob("images/{BlobName}", FileAccess.Read)] Stream input,
        [Blob("images/{BlobNameWithoutExtension}_thumbnail.jpg")] CloudBlockBlob outputBlob)
        {
            CorrelationManager.SetOperationId(blobInfo.OperationId);

            logger.TrackTrace("WebJob -> GenerateThumbnail Started");

            using (Stream output = outputBlob.OpenWrite())
            {
                ConvertImageToThumbnailJPG(input, output);
                outputBlob.Properties.ContentType = "image/jpeg";
            }

            // Entity Framework context class is not thread-safe, so it must
            // be instantiated and disposed within the function.
            using (ContosoAdsContext db = new ContosoAdsContext())
            {
                var id = blobInfo.AdId;
                Ad ad = db.Ads.Find(id);
                if (ad == null)
                {
                    logger.TrackException(new ContosoBusinessException($"No Ad found for id {id}"));
                    return;
                }
                ad.ThumbnailURL = outputBlob.Uri.ToString();
                db.SaveChanges();
            }

            logger.TrackTrace("WebJob -> GenerateThumbnail Finished");
        }

        public static void ConvertImageToThumbnailJPG(Stream input, Stream output)
        {
            logger.TrackTrace("WebJob -> ConvertImageToThumbnailJPG Started");

            if (input == null || output == null) return;

            int thumbnailsize = 80;
            int width;
            int height;
            var originalImage = new Bitmap(input);

            if (originalImage.Width > originalImage.Height)
            {
                width = thumbnailsize;
                height = thumbnailsize * originalImage.Height / originalImage.Width;
            }
            else
            {
                height = thumbnailsize;
                width = thumbnailsize * originalImage.Width / originalImage.Height;
            }

            Bitmap thumbnailImage = null;
            try
            {
                thumbnailImage = new Bitmap(width, height);

                using (Graphics graphics = Graphics.FromImage(thumbnailImage))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(originalImage, 0, 0, width, height);
                }

                thumbnailImage.Save(output, ImageFormat.Jpeg);
            }
            finally
            {
                if (thumbnailImage != null)
                {
                    thumbnailImage.Dispose();
                }
            }

            logger.TrackTrace("WebJob -> ConvertImageToThumbnailJPG Finished");
        }
    }
}
