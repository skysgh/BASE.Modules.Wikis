using System.Reflection;

namespace Tests.Modules.Wikis.Static.Quality.Reliability
{
    /// <summary>
    /// A minimal <see cref="DispatchProxy"/> that returns the default value for
    /// any invoked member. Used only to satisfy interface-typed constructor
    /// dependencies when a body sink is instantiated purely to read its
    /// <c>Kind</c> property — no proxied member is ever actually called.
    /// </summary>
    public class NoOpInterfaceProxy : DispatchProxy
    {
        /// <summary>
        /// Creates a no-op proxy implementing the given interface type.
        /// </summary>
        /// <param name="interfaceType">The interface to proxy.</param>
        /// <returns>A proxy instance assignable to <paramref name="interfaceType"/>.</returns>
        public static object Create(Type interfaceType)
        {
            // DispatchProxy exposes two Create overloads, so select the generic
            // Create<T, TProxy>() unambiguously by its generic-parameter arity.
            MethodInfo createDefinition = typeof(DispatchProxy)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m =>
                    m.Name == nameof(DispatchProxy.Create)
                    && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == 2
                    && m.GetParameters().Length == 0);

            MethodInfo createMethod = createDefinition
                .MakeGenericMethod(interfaceType, typeof(NoOpInterfaceProxy));

            return createMethod.Invoke(null, parameters: null)!;
        }

        /// <inheritdoc />
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            Type? returnType = targetMethod?.ReturnType;
            if (returnType is null || returnType == typeof(void))
            {
                return null;
            }

            return returnType.IsValueType ? Activator.CreateInstance(returnType) : null;
        }
    }
}
