namespace CoreDocker.Sdk.RestApi
{
    public class GraphQlFragments
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

        public static string Project = @"fragment projectData on Project {
              id
              name
            }";
    }
}