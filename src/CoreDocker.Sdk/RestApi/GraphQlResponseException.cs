using System;
using System.Linq;
using CoreDocker.Utilities.Helpers;
using GraphQL.Common.Response;

namespace CoreDocker.Sdk.RestApi
{
    public class GraphQlResponseException : Exception
    {
        public GraphQlResponseException(GraphQLResponse graphQlResponse) : base(graphQlResponse.Errors
            .Select(x => x.Message).StringJoin())
        {
            GraphQlResponse = graphQlResponse;
        }

        public GraphQLResponse GraphQlResponse { get; }
    }
}
