using Auth_WebAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Auth_WebAPI.Models.controller
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly IConfiguration _config;

    public UserController(IConfiguration config)
    {
      this._config=config;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
      List<Users> u = new List<Users>();
      using SqlConnection con =
        new SqlConnection(_config.GetConnectionString("cs"));
      SqlCommand cmd = new SqlCommand("Select * from Users", con);
      con.Open();
      SqlDataReader dr= cmd.ExecuteReader();
      while (dr.Read())
      {
        u.Add(new Users
        {
          Id = Convert.ToInt32(dr["Id"]),
          Name = dr["Name"].ToString(),
          Email = dr["Email"].ToString(),
          Role = dr["Role"].ToString(),
          IsActive = (bool)dr["IsActive"]
        });

      }
      con.Close();
      return Ok(u);

    }
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
      Users u = null;
      using SqlConnection con =
                new SqlConnection(_config.GetConnectionString("cs"));
      SqlCommand cmd = new SqlCommand("select * from Users where Id=@id", con);

      cmd.Parameters.AddWithValue("@id", id);
      con.Open();
      SqlDataReader dr = cmd.ExecuteReader();
      while (dr.Read())
      {
        u = new Users
        {
          Id = Convert.ToInt32(dr["Id"]),
          Name = dr["Name"].ToString(),
          Email = dr["Email"].ToString(),
          Role = dr["Role"].ToString(),
          IsActive = (bool)dr["IsActive"]
        };
      }
      con.Close();
      return u == null ? NotFound() : Ok(u);
    }

    [HttpPost("register")]
    public IActionResult Create([FromBody] Users user)
    {
      try
      {
                if (string.IsNullOrEmpty(user.Password))
                    return BadRequest("Password is required");

                var hasher = new PasswordHasher<Users>();
                string hashedPassword = hasher.HashPassword(user, user.Password);

                using SqlConnection con =
        new SqlConnection(_config.GetConnectionString("cs"));
        SqlCommand cmd = new SqlCommand("insert into Users(name,email,passwordHash,role,isActive) values(@n,@e,@p,@r,@a)", con);
        cmd.Parameters.AddWithValue("@n", user.Name);
        cmd.Parameters.AddWithValue("@e", user.Email);
        cmd.Parameters.AddWithValue("@p", hashedPassword);
        cmd.Parameters.AddWithValue("@r", "User");
        cmd.Parameters.AddWithValue("@a", true);
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();


        return Ok("User Added Successfully");
      }
      catch (Exception ex)
      {
        return StatusCode(500, ex.Message);

      }
    }

    [HttpPut("{id}")]

    public IActionResult Update(int id , Users u)
    {
      using SqlConnection con =
                new SqlConnection(_config.GetConnectionString("cs"));
      SqlCommand cmd = new SqlCommand
                ("update Users set Name=@n,Email=@e,Role=@r,IsActive=@a, PasswordHash=@p where Id=@id", con);
      cmd.Parameters.AddWithValue("@id", id);
      cmd.Parameters.AddWithValue("@n", u.Name);
      cmd.Parameters.AddWithValue("@e", u.Email);
      cmd.Parameters.AddWithValue("@r", u.Role);
      cmd.Parameters.AddWithValue("@a", u.IsActive);
      cmd.Parameters.AddWithValue("@p", PasswordHasher.Hash(u.Password));

      con.Open();
      int rows = cmd.ExecuteNonQuery();
      con.Close();

      return rows > 0 ? Ok("User Updated") : NotFound();
    }

    [HttpDelete("{id}")]

    public IActionResult Delete(int id)
    {
      using SqlConnection con =
                new SqlConnection(_config.GetConnectionString("cs"));
      SqlCommand cmd =
               new SqlCommand("DELETE FROM Users WHERE Id=@id", con);
      cmd.Parameters.AddWithValue("@id", id);
      con.Open();
      int rows = cmd.ExecuteNonQuery();
      con.Close();

      return rows > 0 ? Ok("User Deleted") : NotFound();
    }
  }
}
