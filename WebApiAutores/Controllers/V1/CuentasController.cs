using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;
using WebApiAutores.Entiities;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<Usuario> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<Usuario> signInManager;
        private readonly HashService hashService;
        private readonly LlaveApiService llaveApiService;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<Usuario> userManager,
            IConfiguration configuration,
            SignInManager<Usuario> signInManager,
            //Aqui injectaremos el data protector provider
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService,
            LlaveApiService llaveApiService
            )
        {
            this.signInManager = signInManager;
            this.hashService = hashService;
            this.llaveApiService = llaveApiService;
            this.userManager = userManager;
            this.configuration = configuration;

            //Creando el protector
            dataProtector = dataProtectionProvider.CreateProtector("valor-Unico_e_MentirA-E");
        }

        //[HttpGet("hash/{textoPlano}")]
        //public ActionResult RealizarHash(string textoPlano)
        //{
        //    var resultado1 = hashService.Hash(textoPlano);
        //    var resultado2 = hashService.Hash(textoPlano);

        //    return Ok(new
        //    {
        //        textoPlano,
        //        Hash1 = resultado1,
        //        Hash2 = resultado2,
        //    });
        //}

        //[HttpGet("Encriptar")]
        //public ActionResult Encriptar()
        //{
        //    var textPlano = "Victor Diaz";
        //    var textoCifrado = dataProtector.Protect(textPlano);
        //    var textoDesencriptado = dataProtector.Unprotect(textoCifrado);

        //    return Ok(new
        //    {
        //        textPlano,
        //        textoCifrado,
        //        textoDesencriptado
        //    });
        //}

        [HttpPost("registrar", Name = "registrarUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new Usuario { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);


            if (resultado.Succeeded)
            {
                await llaveApiService.CreateLlave(usuario.Id, TipoLlave.Gratuita);
                return Ok(await GenerarToken(credencialesUsuario, usuario.Id));
            }
            else
            {
                return BadRequest(resultado.Errors);
            }

        }

        [HttpPost("login", Name = "loginUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (!resultado.Succeeded) return BadRequest("Login incorrecto");

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            return await GenerarToken(credencialesUsuario, usuario.Id);
        }

        [HttpGet("RenovarToken", Name = "renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> RenovarToken()
        {
            var emailClaim = HttpContext.User.Claims
                .Where(claim => claim.Type == "Email").FirstOrDefault();
            var email = emailClaim.Value;

            var idClaim = HttpContext.User.Claims.Where(claim => claim.Type == "Id").FirstOrDefault();
            var usuarioId = idClaim.Value;

            var credendialesUsuario = new CredencialesUsuario()
            {
                Email = email
            };

            return Ok(await GenerarToken(credendialesUsuario, usuarioId));
        }

        //este enpoint se encargara de editar un usuario y volverlo adminsitrador
        [HttpPost("HacerAdmin", Name = "hacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDto dto)
        {

            var usuario = await userManager.FindByEmailAsync(dto.Email);

            //creandole un clain al usuario en la base de datos
            await userManager.AddClaimAsync(usuario, new Claim("EsAdmin", "1"));

            return NoContent();
        }

        //este enpoint se encargara de editar un usuario y eliminarlo de adminsitrador
        [HttpPost("RemoverAdmin", Name = "removerAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDto dto)
        {

            var usuario = await userManager.FindByEmailAsync(dto.Email);

            //creandole un clain al usuario en la base de datos
            await userManager.RemoveClaimAsync(usuario, new Claim("EsAdmin", "1"));

            return NoContent();
        }


        private async Task<RespuestaAutenticacion> GenerarToken(CredencialesUsuario credencialesUsuario,
            string usuarioId)
        {
            var claims = new List<Claim>()
            {
                new Claim("Email", credencialesUsuario.Email),
                new Claim("Id", usuarioId)
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var apiKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["firmToken"]));
            var creeds = new SigningCredentials(apiKey, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creeds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion,
            };


        }


    }
}
