using Rennala.Utility;
using Rennala.Logic.Zone;

namespace Rennala.Logic.GDT
{
    internal static class Parser
    {
        private static readonly Dictionary<string, string?> FileData = new()
        {
            {".gdt",        null},
            {".exr",        null},
            {".png",        null},
            {".tiff",       null},
            {".tif",        null},
            {".xmodel_bin", "model_export\\"},
            {".xanim_bin",  "xanim_export\\"},
            {".efx",        "share\\raw\\"}
        };

        /// <summary>
        /// Gets additional file path information based off of the file extension
        /// </summary>
        /// <param name="extension"> File extension to check </param>
        /// <returns> Additional file path </returns>
        private static string GetAdditionalFilePath(string extension)
        {
            return FileData[extension] ?? "";
        }

        /// <summary>
        /// Checks and returns the correct extension
        /// </summary>
        /// <param name="line"> What line to check on </param>
        /// <returns> File extension </returns>
        private static string? GetAcceptedExtension(string line)
        {
            foreach (string key in FileData.Keys)
            {   
                if (line.ToLower().Contains(key)) // GetExtension() won't suffice, as we'd have to clean up the string a lot for it to work
                {
                    return key;
                }
            }

            return null;
        }

        /// <summary>
        /// Parses the GDT file
        /// </summary>
        /// <param name="file"> File to operate on </param>
        /// <param name="GDT"> Name of the GDT </param>
        public static void ParseGDT(string file, string GDT)
        {
            string[]? lines = File.ReadAllLines(file);

            if (lines.Length == 0)
            {
                return;
            }

            string? root = Location.GetBlackOpsRoot();
            if (root == null)
            {
                CLI.ErrorMessage("* TA_GAME_PATH Environment Variable is not defined");
                return;
            }

            // Add the GDT itself in there
            file = file.Replace(root, ""); // Remove the BO3 root path so we just get the full path name for it
            lines = lines.Append(file).ToArray();

            string outputDir = Path.Combine(Location.GetExeDirectory(), "RennalaOutput");

            foreach (string line in lines)
            {
                // It's way too small to even be possible to be one of our needed strings
                if (line.Length < 8 && !line.Contains(".gdt"))
                {
                    continue;
                }

                if (Writer.IsZoneEntry(line))
                {
                    Writer.HandleZoneEntry(line);
                }

                string? extension = GetAcceptedExtension(line);
                if (extension == null)
                {
                    continue;
                }

                string newLine = line.Replace("/", "\\"); // Make sure our paths are all consistent, makes it easier if we operate on this
                string[] splitString = newLine.Split('"'); // We need to split as we don't want the whole line

                // GDT support, as the GDT name itself will never get split as it has no "'s
                string asset = line;
                if (splitString.Length >= 3)
                {
                    asset = splitString[3];
                }

                // Create the path the asset has to be in
                string assetPath = Path.Combine(GetAdditionalFilePath(extension), asset);
                if (assetPath.Contains(".efx") && !assetPath.Contains("fx\\")) // 3arc's GDT syntax is weird with FX, so ensure the FX folder is in there
                {
                    assetPath = assetPath.Replace("share\\raw", "share\\raw\\fx");
                }

                // If it doesn't exist, it may be an asset that's loaded in APE but not provided in the tools
                string rootAsset = Path.Combine(root, assetPath);
                if (!File.Exists(rootAsset))
                {
                    continue;
                }

                string endAsset = Path.Combine(outputDir, assetPath);
                if (File.Exists(endAsset))
                {
                    continue;
                }

                splitString = assetPath.Split("\\"); // Get the directory the asset has to end up in
                assetPath = assetPath.Replace(splitString[splitString.Length - 1], ""); // Get the path minus the asset
                string endDir = Path.Combine(outputDir, assetPath);
                if (!Directory.Exists(endDir))
                {
                    Directory.CreateDirectory(endDir);
                }

                File.Copy(rootAsset, endAsset);
            }

            Writer.Write(); // Write the zone.txt
            CLI.SuccessMessage($"* Successfully parsed {GDT}");
        }
    }
}
