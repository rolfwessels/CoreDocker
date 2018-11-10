namespace CoreDocker.Utilities.Helpers
{
    /// <summary>
    ///     Non-generic class allowing properties to be copied from one instance
    ///     to another existing instance of a potentially different type.
    /// </summary>
    public static class PropertyCopy
    {
        /// <summary>
        ///     Copies all public, readable properties from the source object to the
        ///     target. The target type does not have to have a parameterless constructor,
        ///     as no new instance needs to be created.
        /// </summary>
        /// <remarks>
        ///     Only the properties of the source and target types themselves
        ///     are taken into account, regardless of the actual types of the arguments.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source</typeparam>
        /// <typeparam name="TTarget">Type of the target</typeparam>
        /// <param name="source">Source to copy properties from</param>
        /// <param name="target">Target to copy properties to</param>
        public static void Copy<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            PropertyCopier<TSource, TTarget>.Copy(source, target);
        }
    }

    /// <summary>
    ///     Generic class which copies to its target type from a source
    ///     type specified in the Copy method. The types are specified
    ///     separately to take advantage of type inference on generic
    ///     method arguments.
    /// </summary>
    public static class PropertyCopy<TTarget> where TTarget : class, new()
    {
        /// <summary>
        ///     Copies all readable properties from the source to a new instance
        ///     of TTarget.
        /// </summary>
        public static TTarget CopyFrom<TSource>(TSource source) where TSource : class
        {
            return PropertyCopier<TSource, TTarget>.Copy(source);
        }
    }
}