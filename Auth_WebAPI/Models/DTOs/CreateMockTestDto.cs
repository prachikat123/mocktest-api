namespace Auth_WebAPI.Models.DTOs
{
    public class CreateMockTestDto
    {
        public string TestName { get; set; }
        public int SubjectId { get; set; }
        public int LevelId { get; set; }
        public int TotalQuestions { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
