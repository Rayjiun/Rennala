using Rennala.Utility;

namespace Rennala
{
    internal class Program
    {
        private static readonly Dictionary<string, Action<string, string>> ParseableExtensions = new()
        {
            {".gdt", (line, file) => Logic.GDT.Parser.ParseGDT(line, file)},
            {".szc", (line, file) => Logic.SZC.Parser.ParseSZC(line, file)}
        };

        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
            Console.WriteLine(" Rennala, an asset packager for Call of Duty: Black Ops III to allow easy drag and drop for asset releases. ");
            Console.WriteLine(" Version: 2.0.0");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("");

            if(args.Length == 0)
            {
                CLI.ErrorMessage("! No file was provided");
            }

            foreach (var arg in args)
            {
                if (!File.Exists(arg))
                {
                    CLI.ErrorMessage($"! {arg} is not a file");
                    continue;
                }

                string fileExtension = Path.GetExtension(arg);
                if (!ParseableExtensions.ContainsKey(fileExtension))
                {
                    CLI.ErrorMessage($"! {fileExtension} is not a supported file type");
                    continue;
                }

                string file = Path.GetFileName(arg);
                CLI.WaitMessage($"* Parsing {file}");

                ParseableExtensions[fileExtension](arg, file); // Call the parse function
            }

            CLI.WaitForUserConfirmation();
        }
    }
}