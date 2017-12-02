namespace CoreDocker.Api.Security
{
    public class OpenIdConfigBase
    {
        public static string HostUrl = "http://localhost:5000";

        internal const string ApiResourceName = "api.resource";
        internal const string ApiResourceSecret = "a98802aa-e4d4-432a-835e-6c856a05d999";

        internal const string ClientName = "coredocker.api";
        internal const string ClientSecret = "e0acca78-4dc2-46c6-83c6-c6aeacfffd46";

        public const string IdentPath = "identity";
        public const string ScopeApi = "api";
    }
}