using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights;

namespace ContosoAds.Logger
{
    public class AILogger : ILogger
    {
        /// <summary>
        /// Actual Application Insights object which logs data
        /// </summary>
        private TelemetryClient client;
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public AILogger()
        {
            this.client = new TelemetryClient();
        }

        #region Trace

        public void TrackTrace(string message)
        {
            client.TrackTrace(message);
        }

        public void TrackTrace(string message, IDictionary<string, string> properties)
        {
            client.TrackTrace(message, properties);
        }

        public void TrackTrace(string message, SeverityLevel level)
        {
            client.TrackTrace(message, level);
        }

        public void TrackTrace(string message, SeverityLevel level, IDictionary<string, string> properties)
        {
            client.TrackTrace(message, level, properties);
        }

        #endregion

        #region Event

        public void TrackEvent(string eventName)
        {
            client.TrackEvent(new EventTelemetry(eventName));
        }

        public void TrackEvent(string eventName, IDictionary<string, string> properties)
        {
            var evt = new EventTelemetry(eventName);
            foreach (var prop in properties)
                evt.Properties.Add(prop);

            client.TrackEvent(evt);
        }

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            this.client.TrackEvent(eventName, properties, metrics);
        }

        #endregion

        #region PageView

        public void TrackPageView(string pageName)
        {
            client.TrackPageView(pageName);
        }

        public void TrackPageView(string pageName, string url = null, IDictionary<string, string> properties = null)
        {
            var pageView = new PageViewTelemetry(pageName);

            if (url != null)
                pageView.Url = new Uri(url);

            if (properties != null)
                foreach (var prop in properties)
                    pageView.Properties.Add(prop);

            client.TrackPageView(pageName);
        }

        #endregion

        #region Exception

        public void TrackException(Exception exception)
        {
            client.TrackException(exception);
        }

        #endregion

        #region Request

        public void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode,
            bool success, string url = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            var request = new RequestTelemetry(name, startTime, duration, responseCode, success);

            if (url != null)
                request.Url = new Uri(url);

            if (properties != null)
                foreach (var prop in properties)
                    request.Properties.Add(prop);

            if (metrics != null)
                foreach (var prop in metrics)
                    request.Metrics.Add(prop);

            client.TrackRequest(request);

        }

        #endregion

        #region Metric

        public void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            client.TrackMetric(name, value, properties);
        }

        #endregion
    }
}
