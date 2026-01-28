using Microsoft.EntityFrameworkCore;
using Auth_WebAPI.Data;
using Auth_WebAPI.Models;
using Auth_WebAPI.Models.DTOs;

namespace Auth_WebAPI.Services
{
    public class ResultService : IResultService
    {
        private readonly ApplicationDbContext _context;

        public ResultService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TestResultDto> GetResultByAttemptId(int userId, int attemptId)
        {
            var attempt =await _context.TestAttempts
                .Include(a => a.MockTest)
                 .FirstOrDefaultAsync(a => a.AttemptId == attemptId 
                                        && a.UserId == userId);

            if(attempt == null)
            {
                return null;
            }

            var answers =await _context.UserAnswers
                .Where(a =>a.AttemptId == attemptId)
                .ToListAsync();

            int totalQuestions = answers.Count;
            int correct = answers.Count(a => a.IsCorrect == true);
            int wrong = answers.Count(a => a.IsCorrect == false);
            int attempted = answers.Count(a => a.SelectedAnswer != null);

            var result = new TestResultDto
            {
                AttemptId = attempt.AttemptId,
                TestName = attempt.MockTest.TestName,
                TotalQuestions = totalQuestions,
                Attempted = attempted,
                Correct = correct,
                Wrong = wrong,
                Score = attempt.Score ?? 0,
                Status = attempt.Status,
                StartTime = attempt.StartTime,
                EndTime = attempt.EndTime,
                QuestionResults = answers.Select(a => new QuestionResultDto
                {
                    QuestionId = a.QuestionId,
                    SelectedAnswer = a.SelectedAnswer,
                    IsCorrect = a.IsCorrect ?? false
                }).ToList()
            };
            return result;
        }
    }
}
