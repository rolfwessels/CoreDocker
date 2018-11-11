using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreDocker.Utilities.Helpers
{
    /// <summary>
    ///     Static class to efficiently store the compiled delegate which can
    ///     do the copying. We need a bit of work to ensure that exceptions are
    ///     appropriately propagated, as the exception is generated at type initialization
    ///     time, but we wish it to be thrown as an ArgumentException.
    ///     Note that this type we do not have a constructor constraint on TTarget, because
    ///     we only use the constructor when we use the form which creates a new instance.
    /// </summary>
    internal static class PropertyCopier<TSource, TTarget>
    {
        /// <summary>
        ///     Delegate to create a new instance of the target type given an instance of the
        ///     source type. This is a single delegate from an expression tree.
        /// </summary>
        private static readonly Func<TSource, TTarget> creator;

        /// <summary>
        ///     List of properties to grab values from. The corresponding targetProperties
        ///     list contains the same properties in the target type. Unfortunately we can't
        ///     use expression trees to do this, because we basically need a sequence of statements.
        ///     We could build a DynamicMethod, but that's significantly more work :) Please mail
        ///     me if you really need this...
        /// </summary>
        private static readonly List<PropertyInfo> sourceProperties = new List<PropertyInfo>();

        private static readonly List<PropertyInfo> targetProperties = new List<PropertyInfo>();
        private static readonly Exception initializationException;

        static PropertyCopier()
        {
            try
            {
                creator = BuildCreator();
                initializationException = null;
            }
            catch (Exception e)
            {
                creator = null;
                initializationException = e;
            }
        }

        internal static TTarget Copy(TSource source)
        {
            if (initializationException != null) throw initializationException;
            if (source == null) throw new ArgumentNullException("source");
            return creator(source);
        }

        internal static void Copy(TSource source, TTarget target)
        {
            if (initializationException != null) throw initializationException;
            if (source == null) throw new ArgumentNullException("source");
            for (var i = 0; i < sourceProperties.Count; i++)
                targetProperties[i].SetValue(target, sourceProperties[i].GetValue(source, null), null);
        }

        #region Private Methods

        private static Func<TSource, TTarget> BuildCreator()
        {
            var sourceParameter = Expression.Parameter(typeof(TSource), "source");
            var bindings = new List<MemberBinding>();
            foreach (
                var sourceProperty in
                typeof(TSource).GetRuntimeProperties())
            {
                if (!sourceProperty.CanRead) continue;
                var targetProperty = typeof(TTarget).GetRuntimeProperties()
                    .FirstOrDefault(x => x.Name == sourceProperty.Name);
                if (targetProperty == null)
                    throw new ArgumentException("Property " + sourceProperty.Name +
                                                " is not present and accessible in " +
                                                typeof(TTarget).FullName);
                if (!targetProperty.CanWrite)
                    throw new ArgumentException("Property " + sourceProperty.Name + " is not writable in " +
                                                typeof(TTarget).FullName);
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                    throw new ArgumentException("Property " + sourceProperty.Name + " is static in " +
                                                typeof(TTarget).FullName);
                if (targetProperty.PropertyType != sourceProperty.PropertyType)
                    throw new ArgumentException("Property " + sourceProperty.Name + " has an incompatible type in " +
                                                typeof(TTarget).FullName);
                bindings.Add(Expression.Bind(targetProperty, Expression.Property(sourceParameter, sourceProperty)));
                sourceProperties.Add(sourceProperty);
                targetProperties.Add(targetProperty);
            }

            Expression initializer = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);
            return Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter).Compile();
        }

        #endregion
    }
}