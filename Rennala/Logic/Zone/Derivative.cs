namespace Rennala.Logic.Zone
{
    static internal class Derivative
    {
        /// <summary>
        /// Checks if the GDT info is a derivative
        /// </summary>
        /// <param name="line"> Which line to check </param>
        /// <returns> Bool </returns>
        public static bool IsDerived(string line)
        {
            return line.Contains('[');
        }

        /// <summary>
        /// Adds the derivative to a dictionary
        /// </summary>
        /// <param name="line"> Which line to gather information from </param>
        /// <param name="derived"> The dictionary to add to </param>
        public static void AddDerivative(string line, Dictionary<string, string> derived)
        {
            string[] lines = line.Split('"');

            string asset = lines[1];
            string derivedFrom = lines[3];

            derived.Add(asset, derivedFrom);
        }
    }
}
