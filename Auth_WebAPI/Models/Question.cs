namespace Auth_WebAPI.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int SubjectId { get; set; }
        public int LevelId { get; set; }
        public string QuestionText { get; set; }
        public List<string> CorrectAnswer { get; set; }

        public int Marks { get; set; }

        public string TypeName { get; set; } 

        public string AnswerSet { get; set; }

        
    }
}
