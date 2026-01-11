using Auth_WebAPI.Data;
using Auth_WebAPI.Models;
using Auth_WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Auth_WebAPI.Controllers
{
    [Route("api/mocktest")]
    [ApiController]
    public class MockTestController : ControllerBase
    {
        private readonly IConfiguration _config;

        private readonly ApplicationDbContext _context;

        public MockTestController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public IActionResult CreateTest(CreateMockTestDto dto)
        {
            using SqlConnection con =
            new SqlConnection(_config.GetConnectionString("cs"));

            string sql = @"INSERT INTO MockTests
                      (TestName, SubjectId, LevelId,
                       TotalQuestions, TotalMarks,
                       DurationInMinutes, IsActive)
                      VALUES
                      (@n, @s, @l, @tq,
                       @tm, @d, 1)";

            int totalMarks = dto.TotalQuestions * 1;

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@n", dto.TestName);
            cmd.Parameters.AddWithValue("@s", dto.SubjectId);
            cmd.Parameters.AddWithValue("@l", dto.LevelId);
            cmd.Parameters.AddWithValue("@tq", dto.TotalQuestions);
            cmd.Parameters.AddWithValue("@tm", totalMarks);
            cmd.Parameters.AddWithValue("@d", dto.DurationInMinutes);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return Ok("Mock Test Created Successfully");
        }

        [Authorize]
        [HttpGet("active")]
        public IActionResult GetActiveTests()
        {

            var tests = _context.MockTests
                .Where(t => t.IsActive)
                .Select(t => new MockTestResponseDto
                {
                    TestId = t.TestId,
                    TestName = t.TestName,
                    SubjectId = t.Subject.SubjectId,
                    SubjectName = t.Subject.SubjectName,
                    SubjectDescription = t.Subject.Description,
                    LevelId = t.Level.LevelId,
                    LevelName = t.Level.LevelName,
                    TotalMarks = t.TotalMarks,
                    TotalQuestions = t.TotalQuestions,
                    DurationInMinutes = t.DurationInMinutes
                })
                .ToList();

            return Ok(tests);
        }

        [Authorize]
        [HttpGet("start/{testId}")]
        public IActionResult StartTest([FromRoute] string testId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.PrimarySid);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim missing.");
            }
            int userId = int.Parse(userIdClaim.Value);

            using SqlConnection con =
            new SqlConnection(_config.GetConnectionString("cs"));

            string sql = @"INSERT INTO TestAttempts
                   (UserId, TestId, StartTime, Status)
                   VALUES (@u, @t, GETDATE(), 'Started')";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@t", testId);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return Ok(new { message = "Test Started" });

        }

    }
}
