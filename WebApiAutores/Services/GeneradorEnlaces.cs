using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Runtime.InteropServices;
using WebApiAutores.DTOs;

namespace WebApiAutores.Services
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor, 
            IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        public async Task GenerarEnlaces(AutorDto dto)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirUrlHelper();

            dto.Enlances.Add(new DataHATEOAS(
                enlace: Url.Link("obtenerAutor", new { Id = dto.Id }),
                descripcion: "self",
                metodo: "GET"
                ));

            if (esAdmin)
            {
                dto.Enlances.Add(new DataHATEOAS(
                    enlace: Url.Link("actualizarAutor", new { Id = dto.Id }),
                    descripcion: "autor-actualizar",
                    metodo: "PUT"
                    ));
                dto.Enlances.Add(new DataHATEOAS(
                    enlace: Url.Link("borrarAutor", new { Id = dto.Id }),
                    descripcion: "autor-eliminar",
                    metodo: "DELETE"
                    ));
            }

        }

        private async Task<bool> EsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;

            var esAdmin = await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");
            return esAdmin.Succeeded;
        }

        //haciendo factoria para poder utilizar Url
        private IUrlHelper ConstruirUrlHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }

    }
}
