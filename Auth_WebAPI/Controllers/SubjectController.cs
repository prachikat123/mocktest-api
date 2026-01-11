using Auth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Auth_WebAPI.Controllers
{
    [Route("api/subject")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SubjectController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("add")]
        public IActionResult AddSubject(Subject s)
        {
            using SqlConnection con =
                new SqlConnection(_config.GetConnectionString("cs"));
            string query = @"INSERT INTO SUBJECTS VALUES(@n,@d,1)";

            SqlCommand cmd=new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@n", s.SubjectName);
            cmd.Parameters.AddWithValue("@d", s.Description);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return Ok("Subject Added Successfully");
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public IActionResult GetSubjects()
        {
            List<Subject> list = new();
            using SqlConnection con =
                new SqlConnection(_config.GetConnectionString("cs"));

            string query = @"SELECT * FROM Subjects WHERE IsActive=1";
            SqlCommand cmd=new SqlCommand(query, con);

            con.Open();
            SqlDataReader dr= cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new Subject
                {
                    SubjectId = (int)dr["SubjectId"],
                    SubjectName = dr["SubjectName"].ToString(),
                    Description = dr["Description"].ToString(),
                    IsActive = (bool)dr["IsActive"]
                });
            }
            con.Close() ;

            return Ok(list);
        }

    }
}
