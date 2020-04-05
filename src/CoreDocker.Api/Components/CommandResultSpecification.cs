using CoreDocker.Core.Framework.CommandQuery;
using HotChocolate.Types;

namespace CoreDocker.Api.Components
{
    public class CommandResultSpecification : ObjectType<CommandResult>
    {
        protected override void Configure(IObjectTypeDescriptor<CommandResult> descriptor)
        {
            Name = "CommandResult";
            descriptor.Field(d => d.CorrelationId).Description("Command correlation id.");
            descriptor.Field(d => d.Id).Type<StringType>().Description("The id of the command.");
            descriptor.Field(d => d.CreatedAt).Type<NonNullType<DateTimeType>>()
                .Description("When command was created.");
        }
    }
}