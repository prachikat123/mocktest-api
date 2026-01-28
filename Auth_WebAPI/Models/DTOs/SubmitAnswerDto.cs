using System.ComponentModel.DataAnnotations;

namespace Auth_WebAPI.Models.DTOs
{
    public class SubmitAnswerDto
    {
        
        public int QuestionId { get; set; }
        public required List<string> SelectedAnswer { get; set; }
    }
}
