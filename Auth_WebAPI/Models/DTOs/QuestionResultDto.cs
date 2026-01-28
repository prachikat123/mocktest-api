namespace Auth_WebAPI.Models.DTOs
{
    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public List<string> SelectedAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
