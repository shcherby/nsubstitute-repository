using System;

namespace NSubstitute
{
    public static class SetupCallExtensions
    {
        /// <summary>
        /// Perform an action when this member is called. 
        /// Must be followed by <see cref="SetupCall{T}"/> to provide the callback.
        /// </summary>
        public static void Setup<T>(this T substitute, Action<T> substituteCall) where T : class
        {
            SubstituteRepository.AddCall(substitute, () => substituteCall(substitute));
        }

        /// <summary>
        /// Perform an action when this member is called. 
        /// Must be followed by <see cref="SetupCall{T,TResult}"/> to provide the result callback.
        /// </summary>
        public static TResult Setup<T, TResult>(this T substitute, Func<T, TResult> substituteCall) where T : class
        {
            SubstituteRepository.AddCall(substitute, () => substituteCall(substitute));

            return substituteCall(substitute);
        }


    }
}