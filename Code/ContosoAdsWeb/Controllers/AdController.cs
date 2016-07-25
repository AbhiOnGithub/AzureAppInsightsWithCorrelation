using System;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoAdsCommon;
using Microsoft.WindowsAzure.Storage.Blob;
using ContosoAds.BLL;
using ContosoAds.DAL;

namespace ContosoAdsWeb.Controllers
{
    public class AdController : Controller
    {
        private readonly DataManager dbManager;
        private readonly BlobManager blobManager;
        public AdController()
        {
            this.dbManager = new DataManager();
            this.blobManager = new BlobManager();
            this.blobManager.InitializeStorage();
        }

        // GET: Ad
        public async Task<ActionResult> Index(int? category)
        {
            return View(await this.dbManager.GetAdsList(category));
        }

        // GET: Ad/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await this.dbManager.GetAdAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // GET: Ad/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Title,Price,Description,Category,Phone")] Ad ad,
            HttpPostedFileBase imageFile)
        {
            CloudBlockBlob imageBlob = null;
            // A production app would implement more robust input validation.
            // For example, validate that the image file size is not too large.
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    imageBlob = await blobManager.UploadAndSaveBlobAsync(imageFile);
                    ad.ImageURL = imageBlob.Uri.ToString();
                }
                ad.PostedDate = DateTime.Now;
                await this.dbManager.AddAd(ad);

                if (imageBlob != null)
                {
                    BlobInformation blobInfo = new BlobInformation() { AdId = ad.AdId, BlobUri = new Uri(ad.ImageURL) };
                    await this.blobManager.AddMessageToQueueAsync(blobInfo);
                }
                return RedirectToAction("Index");
            }

            return View(ad);
        }

        // GET: Ad/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await this.dbManager.FindAdAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: Ad/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "AdId,Title,Price,Description,ImageURL,ThumbnailURL,PostedDate,Category,Phone")] Ad ad,
            HttpPostedFileBase imageFile)
        {
            CloudBlockBlob imageBlob = null;
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    // User is changing the image -- delete the existing
                    // image blobs and then upload and save a new one.
                    await DeleteAdBlobsAsync(ad);
                    imageBlob = await blobManager.UploadAndSaveBlobAsync(imageFile);
                    ad.ImageURL = imageBlob.Uri.ToString();
                }

                await this.dbManager.UpdateAdAsync(ad);

                if (imageBlob != null)
                {
                    BlobInformation blobInfo = new BlobInformation() { AdId = ad.AdId, BlobUri = new Uri(ad.ImageURL) };
                    await this.blobManager.AddMessageToQueueAsync(blobInfo);
                }
                return RedirectToAction("Index");
            }
            return View(ad);
        }

        // GET: Ad/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = await dbManager.FindAdAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // POST: Ad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var ad = await this.dbManager.FindAdAsync(id);
            await DeleteAdBlobsAsync(ad);

            this.dbManager.DeleteAdAsync(ad);
            return RedirectToAction("Index");
        }

        private async Task DeleteAdBlobsAsync(Ad ad)
        {
            if (!string.IsNullOrWhiteSpace(ad.ImageURL))
            {
                Uri blobUri = new Uri(ad.ImageURL);
                await blobManager.DeleteAdBlobAsync(blobUri);
            }
            if (!string.IsNullOrWhiteSpace(ad.ThumbnailURL))
            {
                Uri blobUri = new Uri(ad.ThumbnailURL);
                await blobManager.DeleteAdBlobAsync(blobUri);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.dbManager != null)
                this.dbManager.Dispose();
        }
    }
}
