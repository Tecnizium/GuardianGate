using System.Security.Claims;
using GuardianGate.DTOs;
using GuardianGate.Models;
using GuardianGate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuardianGate.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<UserController> _logger;
    //private readonly IMongoDbService _mongoDbService;
    private readonly ICosmosDbService _cosmosDbService;
    private readonly IAuthService _authService;
    
    public UserController(IConfiguration config, ILogger<UserController> logger, ICosmosDbService cosmosDbService, IAuthService authService)
    {
        _config = config;
        _logger = logger;
        //_mongoDbService = mongoDbService;
        _cosmosDbService = cosmosDbService;
        _authService = authService;
    }
  
    //Sign up
    [HttpPost("signup", Name = "SignUp")]
    [Authorize(Policy = "BasicPolicy")]
    public async Task<IActionResult> SignUp([FromBody] UserModel user)
    {
        try
        {
            //Create user
            await _cosmosDbService.CreateUser(user);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    //Sign in
    [HttpPost("signin", Name = "SignIn")]
    [Authorize(Policy = "BasicPolicy")]
    public async Task<IActionResult> SignIn([FromBody] UserAuthUsernameAndPasswordDto userAuth)
    {
        try
        {
            //Get user
            var dbUser = await _cosmosDbService.GetUser(userAuth);
            
            //Validate password
            if (!BCrypt.Net.BCrypt.Verify(userAuth.Password, dbUser!.UserAuth!.Password))
            {
                throw new Exception("Invalid password!");
            }
            //Generate JWT token
            var token = _authService.GenerateJwtToken(userAuth.Username!, dbUser.Id!);
            return Ok(new {token});
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //Get user
    [HttpGet("getuser", Name = "GetUser")]
    [Authorize(Policy = "JwtPolicy")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            //Get user
            var dbUser = await _cosmosDbService.GetUser(new UserAuthUsernameAndPasswordDto()
            {
                Username = User.FindFirst(ClaimTypes.Name)?.Value
            });
            UserDto userDto = new()
                {
                    Id = dbUser!.Id,
                    Username = dbUser.Username,
                    UserAuth = new()
                    {
                        Email = dbUser.UserAuth!.Email,
                        Role = dbUser.UserAuth!.Role
                    },
                    UserDetails = dbUser.UserDetails
                };
                return Ok(userDto);
            
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    //Update user
    [HttpPut("updateuser", Name = "UpdateUser")]
    [Authorize(Policy = "JwtPolicy")]
    public async Task<IActionResult> UpdateUser([FromBody] UserModel user)
    {
        try
        {
            //Update user
            
            await _cosmosDbService.UpdateUser(user);
            //await _mongoDbService.UpdateUser(user);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    //Delete user
    [HttpDelete("deleteuser/{username}", Name = "DeleteUser")]
    [Authorize(Policy = "JwtPolicy")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        try
        {
            //Delete user
            await _cosmosDbService.DeleteUser(username);
            //await _mongoDbService.DeleteUser(username);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }   
    
}