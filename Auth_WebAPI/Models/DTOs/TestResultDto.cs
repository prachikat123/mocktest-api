namespace Auth_WebAPI.Models.DTOs
{
    public class TestResultDto
    {
        public int AttemptId { get; set; }
        public string TestName { get; set; }

        public int TotalQuestions { get; set; }
        public int Attempted { get; set; }
        public int Correct { get; set; }
        public int Wrong { get; set; }

        public int Score { get; set; }
        public string Status { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public List<QuestionResultDto> QuestionResults { get; set; } = new();
    }
}
