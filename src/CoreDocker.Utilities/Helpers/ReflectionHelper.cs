using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreDocker.Utilities.Helpers
{
    public class ReflectionHelper
    {
        public static Type FindOfType(Assembly ns, string typeName)
        {
            return ns.GetTypes().FirstOrDefault(x => x.Name == typeName);
        }


        public static PropertyInfo GetPropertyInfo<TSource, TType>(Expression<Func<TSource, TType>> propertyLambda)
        {
            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            return propInfo;
        }

        public static string GetPropertyString<T, TType>(Expression<Func<T, TType>> propertyLambda)
        {
            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            var name = propInfo.Name;
            var e = member.Expression as MemberExpression;
            while (e != null)
            {
                name = e.Member.Name + '.' + name;
                e = e.Expression as MemberExpression;
            }

            return name;
        }

        public static void ExpressionToAssign<TObj, TValue>(TObj obj, Expression<Func<TObj, TValue>> expression, TValue value)
        {
            ParameterExpression valueParameterExpression = Expression.Parameter(typeof(TValue));
            Expression targetExpression =
                expression.Body is UnaryExpression ? ((UnaryExpression)expression.Body).Operand : expression.Body;

            var assign = Expression.Lambda<Action<TObj, TValue>>
            (
                Expression.Assign(targetExpression, Expression.Convert(valueParameterExpression, targetExpression.Type)),
                expression.Parameters.Single(),
                valueParameterExpression
            );

            assign.Compile().Invoke(obj, value);
        }
    }
}