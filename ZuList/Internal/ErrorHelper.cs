using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZuList.Internal
{
    internal static class ErrorHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowNullReferenceException<T>(T value, string errorText)
            where T : class?
        {
            if (value == null) throw new NullReferenceException(errorText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowArgumentException<T>(T value, string errorText)
        {
            throw new ArgumentException(errorText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowArgumentNullException<T>(T value, string errorText)
            where T : class?
        {
            if (value == null) throw new ArgumentNullException(errorText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowArgumentExceptionIfWrongValueType<T>(object value)
        {
            if (value is not T)
                throw new ArgumentNullException(typeof(T).ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowArgumentExceptionIfNullAndNullsAreIlleagal<T>(object value, string errorText)
        {
            if (value == null && !(default(T) == null))
                throw new ArgumentNullException(errorText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowArgumentNullException(string value, string errorText)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(errorText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowArgumentOutOfRangeExceptionLessThanZero(int value, string errorText)
        {
            if(value < 0) ThrowArgumentOutOfRangeException(errorText); 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowArgumentOutOfRangeException(string errorText)
        {
            throw new ArgumentOutOfRangeException(errorText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowNoElementInArray<T>(T[] value)
        {
            if (value.Length == 0) throw new InvalidOperationException("There is no element of anything.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowNoElementInArray(Array value)
        {
            if (value.Length == 0) throw new InvalidOperationException("There is no element of anything.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowInvalidOperationExceptionIfEnumerationFailed()
        {
            throw new InvalidOperationException();
        }
    }
}
