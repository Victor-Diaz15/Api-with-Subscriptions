using Microsoft.AspNetCore.Mvc;

namespace WebApiAutores.Controllers
{
    public abstract class CustomBaseController : ControllerBase
    {
        protected string GetUsuarioId()
        {
            var idClaim = HttpContext.User.Claims.Where(x => x.Type == "Id").FirstOrDefault();
            var usuarioId = idClaim.Value;
            return usuarioId;
        }
    }
}
