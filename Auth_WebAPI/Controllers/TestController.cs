using Auth_WebAPI.Data;
using Auth_WebAPI.Models;
using Auth_WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth_WebAPI.Controllers
{
    [Route("api/test/")]
    [Authorize]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitTest([FromBody] SubmitTestDto dto)
        {
            (bool flowControl, IActionResult value) = isValidRequest(dto);
            if (!flowControl)
            {
                return value;
            }

            var attempt = await _context.TestAttempts
                .FirstOrDefaultAsync(a => a.AttemptId == dto.AttemptId);

            if (attempt == null)
            {
                return NotFound("Attempt not found");
            }

            var existingAnswers = await _context.UserAnswers
                .Where(a => a.AttemptId == dto.AttemptId)
                .ToListAsync();

            if (existingAnswers.Any())
            {
                _context.UserAnswers.RemoveRange(existingAnswers);
            }

            var attemptedAnswers = dto.Answers
                .Where(a => a.SelectedAnswer != null && a.SelectedAnswer.Count > 0)
                .ToList();
            int score = 0;
            foreach (var ans in attemptedAnswers)
            {
                var question = await _context.Questions
                    .FirstOrDefaultAsync(q => q.QuestionId == ans.QuestionId);

                if (question == null)
                {
                    return BadRequest($"Question not found:{ans.QuestionId}");
                }


                //Normalize correct answers
                var correctAnswers = question.CorrectAnswer?
                    .Select(a => a.Trim().ToLower())
                    .ToList() ?? new List<string>();

                // Normalize selected answers
                var selectedAnswers = ans.SelectedAnswer?
                    .Select(a => a.Trim().ToLower())
                    .ToList() ?? new List<string>();



                if (!selectedAnswers.Any())
                {
                    _context.UserAnswers.Add(new UserAnswer
                    {
                        AttemptId = dto.AttemptId,
                        QuestionId = ans.QuestionId,
                        SelectedAnswer = null,
                        IsCorrect = false
                    });
                    continue;
                }

                bool isCorrect = IsAnswerCorrect(correctAnswers, selectedAnswers);


                if (isCorrect)
                {
                    score += question.Marks;
                }
                _context.UserAnswers.Add(new UserAnswer
                {
                    AttemptId = dto.AttemptId,
                    QuestionId = ans.QuestionId,
                    SelectedAnswer = selectedAnswers,

                    IsCorrect = isCorrect
                });

            }
            //attempt update
            attempt.Score = score;
            attempt.Status = "Completed";
            attempt.EndTime = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, ex.ToString());
            }
            return Ok(new { attemptId = attempt.AttemptId });

        }

        private (bool flowControl, IActionResult value) isValidRequest(SubmitTestDto dto)
        {
            if (dto == null)
            {
                return (flowControl: false, value: BadRequest("Invalid or empty request body"));
            }

            if (!ModelState.IsValid)
            {
                return (flowControl: false, value: BadRequest(ModelState)); //   This catches malformed or missing data early
            }

            if (dto.Answers == null || !dto.Answers.Any())
                return (flowControl: false, value: BadRequest("Answers are missing"));
            return (flowControl: true, value: null);
        }

        private bool IsAnswerCorrect(
            List<string> correctAnswers,
            List<string> selectedAnswers)
        {
            if (correctAnswers == null || selectedAnswers == null)
                return false;

            if (!selectedAnswers.Any())
                return false;

            if (correctAnswers.Count != selectedAnswers.Count)
                return false;

            return correctAnswers
                .Select(a => a.Trim().ToLower())
                .OrderBy(a => a)
                .SequenceEqual(
                    selectedAnswers
                        .Select(a => a.Trim().ToLower())
                        .OrderBy(a => a)
                );

        }
    }
}
