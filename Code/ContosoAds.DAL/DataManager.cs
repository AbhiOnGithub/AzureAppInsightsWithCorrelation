using ContosoAdsCommon;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using ContosoAds.Logger;
using System;

namespace ContosoAds.DAL
{
    
    public class DataManager : IDisposable
    {
        private ContosoAdsContext db;

        private ILogger logger;

        public DataManager()
        {
            db = new ContosoAdsContext();
            logger = new AILogger();
        }

        public async Task<List<Ad>> GetAdsList(int? category)
        {
            this.logger.TrackTrace($"DAL :  DataManager->GetAdsList Started for category " +category);

            var adsList = db.Ads.AsQueryable();
            if (category != null)
            {
                adsList = adsList.Where(a => a.Category == (Category)category);
            }

            this.logger.TrackTrace($"DAL :  DataManager->GetAdsList Completed for category {category} with items {adsList.Count()}");
            return await adsList.ToListAsync();
        }

        public async Task<Ad> GetAdAsync(int? id)
        {
            this.logger.TrackTrace($"DAL :  DataManager->GetAd Started for id - {id}");
            try
            {
                return await db.Ads.FindAsync(id);
            }
            finally
            {
                this.logger.TrackTrace($"DAL :  DataManager->GetAd Finished for id - {id}");
            }
            
        }

        public async Task AddAd(Ad ad)
        {
            this.logger.TrackTrace($"DAL : DataManager->AddAd Started for Ad - {ad.Title}");
            db.Ads.Add(ad);
            db.SaveChanges();
            this.logger.TrackTrace($"DAL : DataManager->AddAd Finished for Ad - {ad.Title}");
        }

        public async Task<Ad> FindAdAsync(int? id)
        {
            this.logger.TrackTrace($"DAL : DataManager->FindAdAsync Started for id - {id}");
            try
            {
                return await db.Ads.FindAsync(id);
            }
            finally
            {
                this.logger.TrackTrace($"DAL : DataManager->FindAdAsync Finished for id - {id}");
            }
        }

        public async Task UpdateAdAsync(Ad ad)
        {
            this.logger.TrackTrace($"DAL : DataManager->UpdateAdAsync Started for id - {ad.Title}");
            db.Entry(ad).State = EntityState.Modified;
             db.SaveChanges();
            this.logger.TrackTrace($"DAL : DataManager->UpdateAdAsync Finished for id - {ad.Title}");
        }

        public async Task DeleteAdAsync(Ad ad)
        {
            this.logger.TrackTrace($"DAL : DataManager->DeleteAdAsync Started for id - {ad.Title}");
            db.Ads.Remove(ad);
            db.SaveChanges();
            this.logger.TrackTrace($"DAL : DataManager->DeleteAdAsync Finished for id - {ad.Title}");
        }

        public void Dispose()
        {
            this.db.Dispose();
            this.logger = null;
        }
    }
}
