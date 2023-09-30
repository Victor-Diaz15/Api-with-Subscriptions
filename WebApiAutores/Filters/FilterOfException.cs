using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filters
{
    public class FilterOfException : ExceptionFilterAttribute
    {
        private readonly ILogger<FilterOfException> logger;

        public FilterOfException(ILogger<FilterOfException> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {

            logger.LogError(context.Exception, context.Exception.Message);

            base.OnException(context);
        }

    }
}
