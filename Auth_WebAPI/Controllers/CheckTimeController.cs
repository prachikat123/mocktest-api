using Auth_WebAPI.Data;
using Auth_WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Auth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckTimeController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public CheckTimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("remaining-time/{attemptId}")]
        [Authorize]
        public async Task<IActionResult> GetRemainingTime([FromRoute] int attemptId)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.PrimarySid);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim missing.");
            }

            //var checkTime= _context.TestAttempts
            //    .Where(t => t.AttemptId == attemptId)
            //    .Select(t =>  t.StartTime);

            var starttime = await (
                from a in _context.TestAttempts
                join t in _context.MockTests
                on a.TestId equals t.TestId
                where a.AttemptId == attemptId
                select new MockTestTimeResponseDto
                {
                    AttemptId = a.AttemptId,
                    TestId = t.TestId,
                    StartTime = (DateTime)a.StartTime,
                    DurationInMinutes = t.DurationInMinutes,
                   
                }
                ).FirstOrDefaultAsync();

            if (starttime == null)
                return NotFound();


            Console.WriteLine("remaining-time works");
            return Ok(starttime);
        }
    }
}
