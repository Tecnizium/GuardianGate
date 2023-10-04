using GuardianGate.Models;

namespace GuardianGate.DTOs;

public class UserDto
{
    //UserDto
    public string? Id { get; set; }
    public string? Username { get; set; }
    public UserAuthDto? UserAuth { get; set; }
    public UserDetailsModel? UserDetails { get; set; }
    
}

public class UserAuthDto
{
    //UserAuthDto
    public string? Email { get; set; }
    public Role? Role { get; set; }
}

//DTO by signIn
public class UserAuthUsernameAndPasswordDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}