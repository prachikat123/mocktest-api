namespace Auth_WebAPI.Models.DTOs
{
    public class SubmitTestDto
    {
        public int AttemptId { get; set; }
        public List<SubmitAnswerDto> Answers { get; set; } = new();
    }
}
