using Newtonsoft.Json;

namespace GuardianGate.Models;

public class UserModel
{
    //UserDataModel.cs
    [JsonProperty("id")]
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string? Username { get; set; }
    public UserAuthModel? UserAuth { get; set; }
    public UserDetailsModel? UserDetails { get; set; }
}