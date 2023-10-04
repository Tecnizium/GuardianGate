using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GuardianGate.Services;

public class AuthService : IAuthService{

    
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _config;
    
    public AuthService(ILogger<AuthService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }
    
    //Bearer Token
    public string GenerateJwtToken(string username, string id)
    {
        _logger.LogInformation("Generating JWT token for user {Username}", username);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ??  _config["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] 
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Sid, id)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public string? ValidateJwtToken(string token)
    {
        _logger.LogInformation("Validating JWT token");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ??  _config["Jwt:Key"]!);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);
        var jwtToken = (JwtSecurityToken)validatedToken;
        return jwtToken.Claims.First(x => x.Type == "unique_name").Value;
    }
    
}