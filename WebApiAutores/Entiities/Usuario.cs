using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entiities
{
    public class Usuario : IdentityUser
    {
        public bool MalaPaga { get; set; }
    }
}
