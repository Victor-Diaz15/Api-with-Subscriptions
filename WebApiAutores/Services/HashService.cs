using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiAutores.DTOs;

namespace WebApiAutores.Services
{
    public class HashService
    {
        public ResultadoHash Hash(string textPlano)
        {
            var salt = new byte[16];

            using(var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Hash(textPlano, salt);
        }

        public ResultadoHash Hash(string textPlano, byte[] salt)
        {
            var llaveDerivada = KeyDerivation.Pbkdf2(password: textPlano,
                salt: salt, prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000, numBytesRequested: 32);

            var hash = Convert.ToBase64String(llaveDerivada);

            return new ResultadoHash()
            {
                Hash = hash,
                Salt = salt
            };

        }
    }
}
