using Newtonsoft.Json;

namespace GuardianGate.Models;

public class UserAuthModel
{
    
    public string? Email { get; set; }
    public string? Password { get; set; }
    public Role? Role { get; set; } = Models.Role.Guest;
}

//Enum Role
public enum Role
{
    Admin,
    User,
    Guest
}