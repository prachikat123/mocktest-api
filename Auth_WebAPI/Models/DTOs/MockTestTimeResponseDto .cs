namespace Auth_WebAPI.Models.DTOs
{
    public class MockTestTimeResponseDto
    {
        public int AttemptId {  get; set; }
        public int TestId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationInMinutes {  get; set; }
    }
}
