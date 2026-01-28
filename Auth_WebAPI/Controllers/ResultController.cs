using Auth_WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auth_WebAPI.Controllers
{
    [Route("api/results")]
    [Authorize]
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly IResultService _resultService;

        public ResultController(IResultService resultService)
        {
            _resultService = resultService;
        }

        [HttpGet("{attemptId}")]
        public async Task<IActionResult> GetResult(int attemptId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.PrimarySid);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim missing.");
            }
            int userId = int.Parse(userIdClaim.Value);
            var result = await _resultService.GetResultByAttemptId(userId, attemptId);

            if (result == null)
            {
                return NotFound("Result Not Found");
            }
            return Ok(result);
        }

    }
}
