using GuardianGate.DTOs;
using GuardianGate.Models;

namespace GuardianGate.Services;

public interface ICosmosDbService
{
    //CRUD User
    Task<bool> CreateUser(UserModel user);
    Task<UserModel?> GetUser(UserAuthUsernameAndPasswordDto userAuth);
    Task<UserModel> UpdateUser(UserModel userAuth);
    Task<bool> DeleteUser(string username);
}