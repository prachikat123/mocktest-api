using System.Text.Json.Serialization;

namespace Auth_WebAPI.Models
{
  public class Users
  {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Password { get; set; }
    public string? Role { get; set; }
    public bool IsActive { get; set; }
  }
}
