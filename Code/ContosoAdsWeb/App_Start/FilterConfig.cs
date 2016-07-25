using System.Web;
using System.Web.Mvc;

namespace ContosoAdsWeb
{
    using ContosoAds.Logger;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new WebCorrelationFilter());
            filters.Add(new TraceFilter(new AILogger()));
            filters.Add(new ExceptionFilter(new AILogger()));
        }
    }
}
