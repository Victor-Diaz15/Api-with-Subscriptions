using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filters
{
    public class MyFilterOfAction : IActionFilter
    {
        private readonly ILogger<MyFilterOfAction> logger;

        public MyFilterOfAction(ILogger<MyFilterOfAction> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Antes de ejecutar la accion");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Despues de ejecutar la accion");

        }

    }
}
