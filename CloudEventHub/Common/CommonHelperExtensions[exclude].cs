using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.CloudEventHub.Common
{
    public static class CommonHelperExtensions
    {
        /// <summary>
        /// Creates an instance of type, invokes the given action on it and returns it.
        /// </summary>
        /// <typeparam name="T">The type of action argument. Must be a reference type and have
        /// a public default constructor.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The created instance of <paramref name="T"/>.</returns>
        public static T CreateTargetAndInvoke<T>(this Action<T> action)
            where T : new()
        {
            T target = new T();

            action.Invoke(target);

            return target;
        }
    }
}
