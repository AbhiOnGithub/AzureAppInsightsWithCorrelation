using System;
using System.Threading.Tasks;

namespace ContosoAds.BLL
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using System.Configuration;
    using System.IO;
    using System.Web;

    using Newtonsoft.Json;
    using ContosoAdsCommon;
    using Logger;

    using Microsoft.ApplicationInsights.Extensibility;

    public class BlobManager
    {
        private CloudQueue thumbnailRequestQueue;
        private static CloudBlobContainer imagesBlobContainer;

        private ILogger logger = new AILogger();

        public void InitializeStorage()
        {
            this.logger.TrackTrace($"BLL : BlobManager->InitializeStorage Started");

            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

            // Get context object for working with blobs, and 
            // set a default retry policy appropriate for a web user interface.
            var blobClient = storageAccount.CreateCloudBlobClient();
            //blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the blob container.
            imagesBlobContainer = blobClient.GetContainerReference("images");

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            //queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            thumbnailRequestQueue = queueClient.GetQueueReference("thumbnailrequest");

            this.logger.TrackTrace("BLL : BlobManager->InitializeStorage Finished");
        }

        public void InitializeTelemetry()
        {
            //You can also read it from Web.Config
            TelemetryConfiguration.Active.InstrumentationKey = "8780ef94-54e1-4cc9-8ee3-e70b31a08a88";
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new CorrelatingTelemetryInitializer());
        }

        public async Task<CloudBlockBlob> UploadAndSaveBlobAsync(HttpPostedFileBase imageFile)
        {
            this.logger.TrackTrace("BLL : BlobManager->UploadAndSaveBlobAsync Started");

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            // Retrieve reference to a blob. 
            CloudBlockBlob imageBlob = imagesBlobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            using (var fileStream = imageFile.InputStream)
            {
                await imageBlob.UploadFromStreamAsync(fileStream);
            }

            this.logger.TrackTrace("BLL : BlobManager->UploadAndSaveBlobAsync Finished");
            return imageBlob;
            
        }

        public async Task AddMessageToQueueAsync(BlobInformation blobInfo)
        {
            this.logger.TrackTrace("BLL : BlobManager->AddMessageToQueueAsync Started");

            blobInfo.OperationId = Logger.CorrelationManager.GetOperationId();

            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(blobInfo));
            await thumbnailRequestQueue.AddMessageAsync(queueMessage);

            this.logger.TrackTrace("BLL : BlobManager->AddMessageToQueueAsync Finished");
        }

        public async Task DeleteAdBlobAsync(Uri blobUri)
        {
            this.logger.TrackTrace("BLL : BlobManager->DeleteAdBlobAsync Started");

            string blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            this.logger.TrackTrace($"BLL: Blobmanager->Deleting image blob {blobName}");

            try
            {
                CloudBlockBlob blobToDelete = imagesBlobContainer.GetBlockBlobReference(blobName);
                await blobToDelete.DeleteAsync();
            }
            catch (Exception ex)
            {
                this.logger.TrackTrace("BLL : BlobManager->DeleteAdBlobAsync Blob to Delete not found");
                this.logger.TrackException(ex);
            }

            this.logger.TrackTrace("BLL : BlobManager->DeleteAdBlobAsync Finished");
        }
    }
}
