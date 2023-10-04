using GuardianGate.DTOs;
using GuardianGate.Models;
using Microsoft.Azure.Cosmos;

namespace GuardianGate.Services;

public class CosmosDbService : ICosmosDbService
{
    private readonly ILogger<CosmosDbService> _logger;
    private readonly Container _users;
    
    public CosmosDbService(IConfiguration config, ILogger<CosmosDbService> logger )
    {
        //Config CosmoDb
        CosmosClient client = new CosmosClient(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? config["CosmosDb:ConnectionString"]!);
        Database database = client.GetDatabase(config["CosmosDb:DatabaseName"]);
        _users = database.GetContainer(config["CosmosDb:UsersContainerName"]);
        _logger = logger;
    }
    
    //CRUD User
    public async Task<bool> CreateUser(UserModel user)
    {
        //Check if user exists
        var userQueryList = _users
            .GetItemLinqQueryable<UserModel>(true)
            .Where(u => u.UserAuth != null && user.UserAuth != null && (u.Username == user.Username || u.UserAuth.Email == user.UserAuth.Email))
            .ToList();
        
        if (userQueryList.Any())
        {
            _logger.LogError("User {UserUsername} already exists", user.Username);
            throw new Exception("User already exists!");
        }
        
        //Hash password
        user.UserAuth!.Password = BCrypt.Net.BCrypt.HashPassword(user.UserAuth!.Password!);
        
        //Create user
        await _users.CreateItemAsync(user);
        _logger.LogInformation("Created user {UserUsername}", user.Username);
        return true;
    }
    
    public Task<UserModel?> GetUser(UserAuthUsernameAndPasswordDto userAuth)
    {
        var userQueryList = _users
            .GetItemLinqQueryable<UserModel>(true)
            .Where(u => u.UserAuth != null && (u.Username == userAuth.Username || u.UserAuth.Email == userAuth.Username))
            .ToList();
        
        if (!userQueryList.Any())
        {
            _logger.LogError("User {UserUsername} does not exist", userAuth.Username);
            throw new Exception("User does not exist!");
        }
        _logger.LogInformation("Getting user {UserUsername}", userAuth.Username);
        

        var userQuery = userQueryList.FirstOrDefault();
        
        
        return Task.FromResult(userQuery);
    }
    
    public async Task<UserModel> UpdateUser(UserModel user)
    {
        _logger.LogInformation("Updating user {UserUsername}", user.Username);
        var userQueryList = _users.GetItemLinqQueryable<UserModel>(true).Where(u => u.Username == user.Username).ToList();
        if (userQueryList.Any())
        {

            if (string.IsNullOrEmpty(user.UserAuth!.Password))
            {
                var password = userQueryList.FirstOrDefault()?.UserAuth!.Password;
                user.UserAuth!.Password = password;
            }
            else
            {
                user.UserAuth!.Password = BCrypt.Net.BCrypt.HashPassword(user.UserAuth!.Password!);
            }
            

            var id = userQueryList.FirstOrDefault()?.Id;
            user.Id = id;
            await _users.ReplaceItemAsync(user, id);
            _logger.LogInformation("Updated user {UserUsername}", user.Username);
            return user;
        }
        _logger.LogError("User {UserUsername} does not exist", user.Username);
        throw new Exception("User does not exist!");
    }
    
    public async Task<bool> DeleteUser(string username)
    {
        _logger.LogInformation("Deleting user {UserUsername}", username);
        var userQueryList = _users.GetItemLinqQueryable<UserModel>(true).Where(u => u.Username == username).ToList();
        
        if (userQueryList.Any())
        {
            var id = userQueryList.FirstOrDefault()?.Id;
            await _users.DeleteItemAsync<UserModel>(id, new PartitionKey(id));
            _logger.LogInformation("Deleted user {UserUsername}", username);
            return true;
        }
        _logger.LogError("User {UserUsername} does not exist", username);
        throw new Exception("User does not exist!");
    }
    
}