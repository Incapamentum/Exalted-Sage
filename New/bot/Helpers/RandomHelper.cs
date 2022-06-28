using System;

namespace Bot.Helpers
{
    /// <summary>
    ///     Static class accessible to entire application due
    ///     to Random not being thread safe.
    /// </summary>
    internal static class RandomHelper
    {
        internal static readonly Random rand;

        static RandomHelper()
        {
            rand = new Random(16);
        }
    }
}
