namespace Auth_WebAPI.Models.DTOs
{
    public class QuestionResponseDto
    {
        public int QuestionId { get; set; }
        public int SubjectId { get; set; }
        public int LevelId { get; set; }
        public string QuestionText { get; set; }
        public string TypeName { get; set; }
        public int Marks { get; set; }
        public List<AnswerOptionDto> AnswerSet { get; set; }
    }
}
