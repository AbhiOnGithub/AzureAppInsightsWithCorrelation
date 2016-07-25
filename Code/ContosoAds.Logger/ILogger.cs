using System;
using System.Collections.Generic;

namespace ContosoAds.Logger
{
    using Microsoft.ApplicationInsights.DataContracts;

    public interface ILogger
    {
            #region Trace

            void TrackTrace(string message);

            void TrackTrace(string message, IDictionary<string, string> properties);

            void TrackTrace(string message, SeverityLevel severityLevel);

            void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string> properties);

            #endregion

            #region Event

            void TrackEvent(string eventName);

            void TrackEvent(string eventName, IDictionary<string, string> properties);

            void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);

            #endregion

            #region PageView

            void TrackPageView(string pageName);

            void TrackPageView(string pageName, string url = null, IDictionary<string, string> properties = null);

            #endregion

            #region Exception

            void TrackException(Exception exception);

            #endregion

            #region Request

            void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode,
                bool success, string url = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);

            #endregion

            #region Metric

            void TrackMetric(string name, double value, IDictionary<string, string> properties = null);

            #endregion
    }
}
