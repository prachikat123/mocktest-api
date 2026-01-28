using System.ComponentModel.DataAnnotations.Schema;

namespace Auth_WebAPI.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }

        public List<string>? SelectedAnswer { get; set; }
        public bool? IsCorrect { get; set; }

        [ForeignKey("AttemptId")]
        public TestAttempt TestAttempt { get; set; }
    }
}
