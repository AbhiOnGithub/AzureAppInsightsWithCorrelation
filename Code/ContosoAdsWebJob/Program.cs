using ContosoAds.Logger;
using Microsoft.Azure.WebJobs;

namespace ContosoAdsWebJob
{
    using Microsoft.ApplicationInsights.Extensibility;

    class Program
    {
        private static ILogger logger = new AILogger();
        static void Main(string[] args)
        {
            
            InitializeTelemtry();

            logger.TrackTrace("Web Job Started");

            JobHost host = new JobHost();
            host.RunAndBlock();
        }

        private static void InitializeTelemtry()
        {
            //You can also read it from Web.Config
            TelemetryConfiguration.Active.InstrumentationKey = "8780ef94-54e1-4cc9-8ee3-e70b31a08a88";
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new CorrelatingTelemetryInitializer());
        }
    }
}
