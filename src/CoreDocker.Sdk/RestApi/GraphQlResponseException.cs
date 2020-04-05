using System;
using System.Linq;
using CoreDocker.Utilities.Helpers;
using GraphQL;

namespace CoreDocker.Sdk.RestApi
{
    public class GraphQlResponseException : Exception
    {
        public GraphQlResponseException(GraphQLResponse<dynamic> graphQlResponse) : base(graphQlResponse.Errors
            .Select(x => x.Message).StringJoin())
        {
            GraphQlResponse = graphQlResponse;
        }

        public GraphQLResponse<dynamic> GraphQlResponse { get; }
    }
}
