using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ContosoAdsWebAPI
{
    using ContosoAds.Logger;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new TraceAPIFilter(new AILogger()));
            config.Filters.Add(new ExceptionFilter(new AILogger()));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
