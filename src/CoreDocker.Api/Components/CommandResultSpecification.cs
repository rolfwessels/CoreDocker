using CoreDocker.Core.Framework.CommandQuery;
using GraphQL.Types;

namespace CoreDocker.Api.Components
{
    public class CommandResultSpecification : ObjectGraphType<CommandResult>
    {
       public CommandResultSpecification()
        {
            Name = "CommandResult";
            Field(d => d.CorrelationId).Description("Command correlation id.");
            Field(d => d.Id,true).Description("The id of the command.");
            Field(d => d.CreatedAt,type:typeof(DateTimeGraphType)).Description("When command was created.");
        }

       
    }
}