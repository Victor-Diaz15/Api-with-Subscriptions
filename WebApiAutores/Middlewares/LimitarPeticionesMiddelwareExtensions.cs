using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;

namespace WebApiAutores.Middlewares
{
    public static class LimitarPeticionesMiddelwareExtensions
    {
        public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitarPeticionMiddleware>();
        }
    }

    public class LimitarPeticionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public LimitarPeticionMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
        {
            var limitarPeticionesConfiguration = new LimitarPeticionConfiguration();
            configuration.GetRequiredSection("LimitarPeticiones").Bind(limitarPeticionesConfiguration);

            //Esto es para las rutas que no requieren api key para realizar peticiones.
            var path = httpContext.Request.Path.ToString();
            var noNecesitaApiKey = limitarPeticionesConfiguration.ListaBlancaRutas.Any(x => path.Contains(x));

            if(noNecesitaApiKey)
            {
                await next(httpContext);
                return;
            }

            var llavesStringValues = httpContext.Request.Headers["X-Api-Key"];

            if(llavesStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Debe prover la llave en la cabecera X-Api-Key.");
                return;
            }

            if(llavesStringValues.Count > 1)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Solo puede hacer una llave presente.");
                return;
            }

            var llave = llavesStringValues[0];

            var llaveDb = await context.LlavesApi
                .Include(x => x.RestriccionesDominio)
                .Include(x => x.RestriccionesIP)
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Llave == llave);

            if(llaveDb == null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Esa llave no existe");
                return;
            }

            if (!llaveDb.Activa)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Esa llave esta inactiva");
                return;
            }

            if(llaveDb.TipoLlave == TipoLlave.Gratuita)
            {
                var hoy = DateTime.Today;
                var manana = hoy.AddDays(1);

                var limitePeticion = await context.Peticiones.CountAsync(x => x.LlaveId == llaveDb.Id &&
                    x.FechaPeticion >= hoy && x.FechaPeticion < manana);

                if(limitePeticion >= limitarPeticionesConfiguration.CantidadPeticionGratuitaPorDia)
                {
                    httpContext.Response.StatusCode = 429;
                    await httpContext.Response.WriteAsync("Ha excedido el limite de peticiones por dia." +
                        " Si desea realiazar mas peticiones, " +
                        "actualize su suscripcion a una cuenta profesional.");
                    return;
                }
            }
            else if (llaveDb.Usuario.MalaPaga)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Eres un cliente mala paga.");
                return;
            }

            var superaRestricciones = SuperaTodasLasRestricciones(llaveDb, httpContext);

            if (!superaRestricciones)
            {
                httpContext.Response.StatusCode = 443;
                return;
            }

            //si paso todas las validaciones, guardaremos la peticion
            var peticion = new Peticion() { LlaveId = llaveDb.Id, FechaPeticion = DateTime.UtcNow };
            context.Peticiones.Add(peticion);
            await context.SaveChangesAsync();



            await next(httpContext);

        }

        private bool SuperaTodasLasRestricciones(LlaveApi llave, HttpContext context)
        {
            var hayRestricciones = llave.RestriccionesDominio.Any() || llave.RestriccionesIP.Any();

            if (!hayRestricciones) return true;

            //Logica para tratar las restricciones de los dominios
            var superaRestriccionesDeDomino = SuperaRestriccionesDeDominio(llave.RestriccionesDominio, context);

            //Logica para tratar las restricciones de las IP
            var superaRestriccionesIP = SuperaRestriccionesDeIP(llave.RestriccionesIP, context);

            return superaRestriccionesDeDomino || superaRestriccionesIP;
        }

        private bool SuperaRestriccionesDeIP(List<RestriccionIP> restriccionIPs, HttpContext context) 
        {
            if (restriccionIPs == null || restriccionIPs.Count == 0) return false;

            var IP = context.Connection.RemoteIpAddress.ToString();

            if(IP == string.Empty) return false;

            var superaRestriccion = restriccionIPs.Any(x => x.IP == IP);

            return superaRestriccion;
        }

        private bool SuperaRestriccionesDeDominio(List<RestriccionDominio> restriccionDominios, HttpContext context)
        {
            if(restriccionDominios == null || restriccionDominios.Count == 0) return false;

            var referer = context.Request.Headers["Referer"].ToString();

            if (referer == string.Empty) return false;

            Uri myUri = new(referer);

            string host = myUri.Host;

            var superaRestriccion = restriccionDominios.Any(x => x.Dominio == host);

            return superaRestriccion;
        }
    }
}
