using System;
using System.Linq;
using CoreDocker.Utilities.Helpers;
using GraphQL.Common.Response;

namespace CoreDocker.Sdk.RestApi
{
    public class GraphQlResponseException : Exception
    {
        public GraphQLResponse GraphQlResponse { get; }

        public GraphQlResponseException(GraphQLResponse graphQlResponse) : base(graphQlResponse.Errors.Select(x=>x.Message).StringJoin())
        {
            GraphQlResponse = graphQlResponse;
        }
    }
}