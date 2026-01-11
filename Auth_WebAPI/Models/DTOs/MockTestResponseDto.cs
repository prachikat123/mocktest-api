namespace Auth_WebAPI.Models.DTOs
{
    public class MockTestResponseDto
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectDescription { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public int TotalMarks { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalQuestions { get; set; }
    }

}
