
namespace ContosoAds.Logger
{
    using System.Web.Mvc;

    public class TraceFilter : ActionFilterAttribute
    {
        /// <summary>
        /// ILogger object for logging
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceFilter"/> class.
        /// </summary>
        /// <param name="logger">implementation of ILogger</param>
        public TraceFilter(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// This will execute right after Action Method of controller starts executing
        /// </summary>
        /// <param name="filterContext">context of action method</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string originController = filterContext.RouteData.Values["controller"].ToString();
            string originAction = filterContext.RouteData.Values["action"].ToString();

            this.logger.TrackTrace($"Web - Method Started {originController}Controller.{originAction}");
        }

        /// <summary>
        /// This will execute right before Action Method of controller ends executing
        /// </summary>
        /// <param name="filterContext">context of action method</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string originController = filterContext.RouteData.Values["controller"].ToString();
            string originAction = filterContext.RouteData.Values["action"].ToString();

            this.logger.TrackTrace($"Web - Method Ended {originController}Controller.{originAction}");
        }
    }
}
