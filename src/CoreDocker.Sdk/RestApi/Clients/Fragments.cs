namespace CoreDocker.Sdk.RestApi.Clients
{
    public class Fragments
    {
        public static string User = @"fragment userData on User {
                id,
                name,
                email,
                roles,
                activities,
                createDate,
                updateDate
            } ";
    }
}