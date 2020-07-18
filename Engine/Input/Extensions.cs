using System;

namespace SE.Input
{
    /// <summary>
    /// Ugly workaround for Extensions namespace missing, since Input isn't coupled with the engine.
    /// </summary>
    internal static class Extensions
    {
        public static bool Contains<T>(this T[] array, T check)
        {
            if (array == null)
                return false;

            return Array.IndexOf(array, check) != -1;
        }

        // .Net standard 2.0 doesn't have Math.Clamp apparently.
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) 
                return min;

            return val.CompareTo(max) > 0 ? max : val;
        }
    }
}
