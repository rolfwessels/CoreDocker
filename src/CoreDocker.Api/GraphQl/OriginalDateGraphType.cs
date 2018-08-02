using System;
using System.Globalization;
using CoreDocker.Utilities.Helpers;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace CoreDocker.Api.GraphQl
{
    /// <summary>
    ///     Keep dates in the original format.
    /// </summary>
    public class OriginalDateGraphType : DateGraphType
    {
        public OriginalDateGraphType()
        {
            Name = "Date";
            Description =
                "The `Date` scalar type represents a timestamp provided in UTC. `Date` expects timestamps " +
                "to be formatted in accordance with the [ISO-8601](https://en.wikipedia.org/wiki/ISO_8601) standard.";
        }

        public override object Serialize(object value)
        {
            return ParseValue(value);
        }

        public override object ParseValue(object value)
        {
            if (value is DateTime time) return time.ToUniversalTime();

            var inputValue = value?.ToString().Trim('"');

            if (DateTime.TryParse(
                inputValue,
                CultureInfo.CurrentCulture,
                DateTimeStyles.NoCurrentDateDefault,
                out var outputValue))
                return outputValue.ToUniversalTime();

            return null;
        }

        public override object ParseLiteral(IValue value)
        {
            if (value is DateTimeValue timeValue) return ParseValue(timeValue.Value);
            if (value is StringValue stringValue) return ParseValue(stringValue.Value);

            return null;
        }
    }
}