using System.ComponentModel.DataAnnotations;

namespace Auth_WebAPI.Models
{
    public class MockTest
    {
        [Key]
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int LevelId { get; set; }
        public Level Level { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsActive { get; set; }
    }
}
