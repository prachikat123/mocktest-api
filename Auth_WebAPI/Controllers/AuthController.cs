using Auth_WebAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth_WebAPI.Models.controller
{
  
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly IConfiguration _config;
        JwtHelper _jwtHelper;



        public AuthController(JwtHelper jwtHelper, IConfiguration config)
            {
                _jwtHelper = jwtHelper;
                 _config = config;
            }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(Login log)
    {
            using SqlConnection con =
            new SqlConnection(_config.GetConnectionString("cs"));

            string sql = @"SELECT * 
                   FROM Users 
                   WHERE Email = @e AND IsActive = 1";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@e", SqlDbType.NVarChar, 100).Value = log.Email;

            con.Open();
            Users u = null;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                u = new Users
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    Name = dr["Name"].ToString(),
                    Email = dr["Email"].ToString(),
                    Password = dr["PasswordHash"].ToString(),
                    Role = dr["Role"].ToString(),
                    IsActive = (bool)dr["IsActive"]
                };
            }
            con.Close();

            if (u == null)
                return Unauthorized("Invalid email");

            var hasher = new PasswordHasher<Users>();

            var result = hasher.VerifyHashedPassword(
                u,
                u.Password,
                log.Password
            );

            

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid password");


            var token = _jwtHelper.GenerateToken(u);
            return Ok(new 
            { 
                token=token,
                name=u.Name
            });
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
      return Ok("This is Protected API");
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Get token from Authorization header
        var authHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return BadRequest("Invalid token");

        var token = authHeader.Replace("Bearer ", "");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expiry = jwtToken.ValidTo;

        using SqlConnection con =
            new SqlConnection(_config.GetConnectionString("cs"));

        string sql = @"INSERT INTO BlacklistedTokens (Token, ExpiryDate)
                VALUES (@t, @e)";

        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@t", token);
        cmd.Parameters.AddWithValue("@e", expiry);

        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();

        return Ok(new { message = "Logout successful" });
        }

   }
}
