namespace CoreDocker.Api.Security
{
    public class OpenIdConfigBase
    {
        public static string HostUrl = "http://localhost:5000";

        public const string ApiResourceName = "api.resource";
        public const string ApiResourceSecret = "a98802aa-e4d4-432a-835e-6c856a05d999";

        public const string ClientName = "coredocker.api";
        public const string ClientSecret = "super_secure_password";

        public const string IdentPath = "identity";
        public const string ScopeApi = "api";
    }
}