namespace GuardianGate.Services;

public interface IAuthService
{
    //Bearer Token
    string GenerateJwtToken(string username, string id);
    string? ValidateJwtToken(string token);
}