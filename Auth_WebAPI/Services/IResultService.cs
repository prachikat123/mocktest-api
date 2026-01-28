using Auth_WebAPI.Models.DTOs;

namespace Auth_WebAPI.Services
{
    public interface IResultService
    {
        Task<TestResultDto> GetResultByAttemptId(int userId, int attemptId);
    }
}
