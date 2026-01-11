using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Auth_WebAPI.Helpers

{
    public static class PasswordHasher
    {
        public static string Password { get; private set; }

        public static string Hash(string password)
        {
            var hasher = new PasswordHasher<object>();
            string hash = hasher.HashPassword(null, Password);
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
        public static bool Verify(string password, string hash)
        {
            return Hash(password) == hash;
        }
    }
}
