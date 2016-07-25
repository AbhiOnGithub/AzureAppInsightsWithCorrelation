using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Http.Filters;
using ContosoAdsCommon;

namespace ContosoAds.Logger
{
    public class ExceptionFilter : ExceptionFilterAttribute, System.Web.Mvc.IExceptionFilter
    {
        /// <summary>
        /// logger object for logging
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionFilter"/> class.
        /// </summary>
        /// <param name="logger">actual implementation of ILogger</param>
        public ExceptionFilter(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Method executes when exception occurs
        /// </summary>
        /// <param name="filterContext">context of action method</param>
        public void OnException(ExceptionContext filterContext)
        {
            this.logger.TrackException(filterContext.Exception);
        }

        /// <summary>
        /// Exception filter.
        /// </summary>
        /// <param name="actionExecutedContext">Action executes context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns void.</returns>
        public override async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var requestInfo = $"Method:{actionExecutedContext.Request.Method},Uri:{actionExecutedContext.Request.RequestUri}";
            var requestMessage = await actionExecutedContext.Request.Content.ReadAsStringAsync();

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            else if (actionExecutedContext.Exception is ContosoBusinessException) //Your Business Exception
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new System.Net.Http.StringContent(actionExecutedContext.Exception.Message)
                };
                this.logger.TrackException(actionExecutedContext.Exception);
            }
            else if (actionExecutedContext.Exception is Exception) // Generic Exception
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                {
                    Content = new System.Net.Http.StringContent(actionExecutedContext.Exception.Message),
                };
                this.logger.TrackException(actionExecutedContext.Exception);
            }
        }
    }
}
