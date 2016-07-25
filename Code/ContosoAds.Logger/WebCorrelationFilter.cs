using System;
using System.Web.Mvc;
using Microsoft.ApplicationInsights.DataContracts;

namespace ContosoAds.Logger
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class WebCorrelationFilter : FilterAttribute, IActionFilter
    {
        /// <summary>
        /// The request telemetry key
        /// </summary>
        private const string RequestTelemetryKey = "Microsoft.ApplicationInsights.RequestTelemetry";

        /// <summary>
        /// Called before an action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Items.Contains(RequestTelemetryKey))
            {
                var requestTelemetry = filterContext.HttpContext.Items[RequestTelemetryKey] as RequestTelemetry;
                if (requestTelemetry == null)
                {
                    return;
                }
                CorrelationManager.SetOperationId(requestTelemetry.Id);
            }
        }

        /// <summary>
        /// Called after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //// Method intentionally left empty.
        }
    }
}
