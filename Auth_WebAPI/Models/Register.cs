namespace Auth_WebAPI.Models
{
    public class Register
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string Password { get; set; }
    }
}
