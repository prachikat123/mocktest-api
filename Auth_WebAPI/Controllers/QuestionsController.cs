using Auth_WebAPI.Data;
using Auth_WebAPI.Models.DTOs;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace Auth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public QuestionsController(ApplicationDbContext context) 
        {
            _context = context;
        }

        [HttpPost("get-questions")]
        public IActionResult GetQuesitons([FromBody] GetQuestionsRequestDto request)
        {
            var questions = _context.Questions
                .Where(q => q.SubjectId == request.SubjectId
                        && q.LevelId == request.LevelId)
                .ToList();
        

            return Ok(questions);
        }
    }
}
