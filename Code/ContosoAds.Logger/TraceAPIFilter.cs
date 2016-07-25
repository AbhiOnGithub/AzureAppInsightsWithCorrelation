using System.Linq;

namespace ContosoAds.Logger
{
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public class TraceAPIFilter : ActionFilterAttribute
    {
        private const string RequestTelemetryKey = "OperationId";

        /// <summary>
        /// logger object for Logging
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceAPIFilter"/> class.
        /// </summary>
        /// <param name="logger">Implementation of ILogger</param>
        public TraceAPIFilter(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// method On action executing
        /// </summary>
        /// <param name="actionContext">action context</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Contains(RequestTelemetryKey))
            {
                CorrelationManager.SetOperationId(actionContext.Request.Headers.GetValues("OperationId").ToList()[0]);
                this.logger.TrackTrace($"WebAPI got Request {actionContext.Request.RequestUri}");
            }
        }

        /// <summary>
        /// method on action excecuted
        /// </summary>
        /// <param name="actionExecutedContext">action context</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            this.logger.TrackTrace($"WebAPI served Request {actionExecutedContext.Request.RequestUri}");
        }
    }
}
