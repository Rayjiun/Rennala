using Rennala.Utility;

namespace Rennala.Logic.SZC
{
    internal class Parser
    {
        private static readonly List<string> csvPaths = new();
        private static readonly List<string> soundPaths = new();

        /// <summary>
        /// Add CSV paths from SZC file to list
        /// </summary>
        /// <param name="file">SZC file to operate on</param>
        private static void FindAddCSVPaths(string file)
        {
            string[] lines = File.ReadAllLines(file);
            string line, lineLowered;

            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i];
                lineLowered = line.ToLower();

                // We do not care for ambient at all, skip the block
                if (lineLowered.Contains("type") && lineLowered.Contains("ambient"))
                {
                    i += 4;
                }

                if (!lineLowered.Contains("filename"))
                {
                    continue;
                }

                string[] splitStrings = line.Split('\"');

                // Currently unsure if sound paths can be anywhere besides the shared dir, but I won't worry about it until any reports are made
                csvPaths.Add(Path.Combine("share", "raw", "sound", "aliases", splitStrings[3])); // 3rd index is always the csv path
            }
        }

        /// <summary>
        /// Adds sounds from CSV to list
        /// </summary>
        /// <param name="root">BO3 root</param>
        private static void FindAddSoundPaths(string root)
        {
            string[] splitStrings, lines;
            string line;

            foreach (string _ in csvPaths)
            {
                string path = Path.Combine(root, _); // Copy it into our own variable so we have control over it
                if (!File.Exists(path))
                {
                    continue;
                }

                lines = File.ReadAllLines(path);
                if(lines.Length <= 1) // It only has the header data, no reason to continue
                {
                    continue;
                }

                int soundIndex = -1;
                for (int i = 0; i < lines.Length ; i++)
                {
                    line = lines[i];
                    splitStrings = line.Split(',');

                    if (line.Contains('#')) // This means it's commented out
                    {
                        continue;
                    }

                    if (splitStrings.Length == 0 || splitStrings.Length < soundIndex) // The split strings is either on an empty line, or some sort of leftover
                    {
                        continue;
                    }

                    if (i == 0) // Header data
                    {
                        for (int x = 0; x < splitStrings.Length; x++)
                        {
                            if (splitStrings[x].ToLower().Contains("filespec")) // Find the index to use early on, so we don't have to loop through all the lines everytime
                            {
                                soundIndex = x;
                                break;
                            }
                        }

                        if (soundIndex == -1) // If it's -1, we might as well break out of this file as it means it's empty or something went wrong
                        {
                            break;
                        }
                    }
                    else
                    {
                        string sound = splitStrings[soundIndex];
                        if (sound.Length == 0) // It's only whitespace, we don't want it
                        {
                            continue;
                        }

                        soundPaths.Add(Path.Combine("sound_assets", sound));
                    }
                }
          
            }
        }

        /// <summary>
        /// Parses SZC files
        /// </summary>
        /// <param name="file">Which file path to parse</param>
        /// <param name="SZC">File name</param>
        public static void ParseSZC(string file, string SZC)
        {
            string? root = Location.GetBlackOpsRoot();
            if (root == null)
            {
                CLI.ErrorMessage("! TA_GAME_PATH Environment Variable is not defined!");
                return;
            }

            FindAddCSVPaths(file);
            FindAddSoundPaths(root);

            string outputDir = Path.Combine(Location.GetExeDirectory(), "RennalaOutput");
            IEnumerable<string> szcLists = csvPaths.Concat(soundPaths);
            Duplication dupes = new();

            foreach (string _ in szcLists)
            {
                string path = _; // Copy it into our own variable so we have control over it
                path = path.Replace("//", "\\"); // cool little skateboard flip

                // Get the directory, and create it if it hasn't been already
                string[] splitPath = path.Split("\\");
                string endDir = Path.Combine(outputDir, path.Replace(splitPath[splitPath.Length - 1], ""));
                if (!Directory.Exists(endDir))
                {
                    Directory.CreateDirectory(endDir);
                }

                // User may not have the asset for some odd reason
                string rootAsset = Path.Combine(root, path);
                if (!File.Exists(rootAsset))
                {
                    CLI.ErrorMessage($"! {rootAsset} could not be located. Please ensure the file is there.");
                    continue;
                }

                string endAsset = Path.Combine(Location.GetExeDirectory(), outputDir, path);
                if (dupes.IsDuplicate(endAsset)) // We add the duplication stuff, so it only gets logged once as the same asset can be referenced multiple times in one csv
                {
                    continue;
                }
                else
                {
                    dupes.Add(endAsset);
                }

                if (File.Exists(endAsset)) // It's already been copied, skip!
                {
                    CLI.NoticeMessage($"? {endAsset} was already present or copied. Skipping!");
                    continue;
                }

                File.Copy(rootAsset, endAsset);
            }

            CLI.SuccessMessage($"> Successfully parsed {SZC}");
        }
    }
}
