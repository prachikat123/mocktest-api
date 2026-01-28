using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth_WebAPI.Models
{
    public class TestAttempt
    {
        [Key]
        public int AttemptId { get; set; }
      
        public int? UserId { get; set; }
        public int? TestId { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int? Score { get; set; }
        public string Status { get; set; }

        [ForeignKey("TestId")]
        public MockTest MockTest { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
