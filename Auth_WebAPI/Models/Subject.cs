namespace Auth_WebAPI.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public ICollection<MockTest> MockTests { get; set; }

    }
}
