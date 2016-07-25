using System.Collections.Generic;
using System.Web.Http;
using ContosoAds.DAL;
using System.Threading.Tasks;
using System.Web.Http.Description;

using ContosoAdsCommon;

namespace ContosoAdsWebAPI.Controllers
{
    public class AdController : ApiController
    {
        private DataManager db;

        public AdController()
        {
            this.db = new DataManager();
        }


        // GET api/Ad
        public async Task<IEnumerable<Ad>> Get()
        {
            return await this.db.GetAdsList(null);
        }

        // GET api/Ad/5
        public async Task<IHttpActionResult> Get(int id)
        {
            var ad = await this.db.GetAdAsync(id);
            if (ad == null)
                return NotFound();

            return Ok(ad);
        }

        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> Post(Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AddAd(ad);
            return this.Ok(ad);
        }

        [ResponseType(typeof(Ad))]
        public async Task<IHttpActionResult> Put(Ad ad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await db.UpdateAdAsync(ad);
            return this.Ok(ad);
        }
        
        //DELETE api/Ad/5
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var ad = await db.GetAdAsync(id);
                if (ad == null) return BadRequest(ModelState);
                else
                {
                    await this.db.DeleteAdAsync(ad);
                    return this.Ok();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}