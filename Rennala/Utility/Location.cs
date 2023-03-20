using System.Reflection;

namespace Rennala.Utility
{
    internal class Location
    {
        /// <summary>
        /// Gets the exe directory of the program
        /// </summary>
        /// <returns> Exe directory </returns>
        public static string GetExeDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
        }

        /// <summary>
        /// Gets the root of Black Ops III
        /// </summary>
        /// <returns> Black Ops III path </returns>
        public static string? GetBlackOpsRoot()
        {
            return Environment.GetEnvironmentVariable("TA_GAME_PATH");
        }
    }
}
