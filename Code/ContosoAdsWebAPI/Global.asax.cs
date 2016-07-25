using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace ContosoAdsWebAPI
{
    using ContosoAds.Logger;
    using Microsoft.ApplicationInsights.Extensibility;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            InitializeTelemtry();
        }

        private void InitializeTelemtry()
        {
            //You can also read it from Web.Config
            TelemetryConfiguration.Active.InstrumentationKey = "8780ef94-54e1-4cc9-8ee3-e70b31a08a88";
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new CorrelatingTelemetryInitializer());
        }

    }
}
