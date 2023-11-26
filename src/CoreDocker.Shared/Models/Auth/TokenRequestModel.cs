namespace CoreDocker.Shared.Models.Auth
{
    public record TokenRequestModel(string UserName,
        string Password,
        string ClientId,
        string ClientSecret,
        string GrantType = "password");
}