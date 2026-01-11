using Auth_WebAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace Auth_WebAPI.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;
        public JwtHelper(IConfiguration config)
        {
            this._config=config;
        }
        public string GenerateToken(Users u)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,u.Email),
                new Claim(ClaimTypes.Name,u.Name),
                new Claim(ClaimTypes.Role,u.Role),
                new Claim(ClaimTypes.PrimarySid, u.Id.ToString())
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(this._config["Jwt:Key"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: this._config["Jwt:Issuer"],
                audience: this._config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(this._config["Jwt:ExpireMinutes"])
                ),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
            
    }
}
