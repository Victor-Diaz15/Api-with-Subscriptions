namespace WebApiAutores.Middlewares
{

    //Clase de extension que expone el middleware de loguear las respuestas http
    public static class LoguerResponseHTTPMiddlewareExtensions
    {
        //metodo que retorna dicho middleware
        public static IApplicationBuilder UseLoguerResponseHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguerResponseHTTPMiddleware>();
        }
    }


    public class LoguerResponseHTTPMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LoguerResponseHTTPMiddleware> logger;

        public LoguerResponseHTTPMiddleware(RequestDelegate next,
            ILogger<LoguerResponseHTTPMiddleware> logger
            )
        {
            this.next = next;
            this.logger = logger;
        }


        //Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                //guardando la respuesta de mi api

                var originalBoddyResponse = context.Response.Body;
                context.Response.Body = ms;

                //aqui permito que la peticion continua a los demas middlewares
                await next(context);

                //aqui copio o leo la respuesta que dara mi api a esa peticion.
                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(originalBoddyResponse);
                context.Response.Body = originalBoddyResponse;

                //aqui estoy haciendo el log de dicha respuesta
                logger.LogInformation(response);
            }
        }


    }
}
