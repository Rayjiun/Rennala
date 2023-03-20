using Rennala.Logic.GDT;
using Rennala.Utility;

namespace Rennala
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine(" Rennala, a GDT Packager for Call of Duty: Black Ops III to allow easy drag and drop for asset releases. ");
            Console.WriteLine(" Version: 1.0.0");
            Console.WriteLine("--------------------------------------------------------------------------------------------------------");
            Console.WriteLine("");

            if(args.Length == 0)
            {
                CLI.ErrorMessage("* No file was provided");
            }

            foreach (var arg in args)
            {
                if (!File.Exists(arg))
                {
                    CLI.ErrorMessage("* Not a file");
                    return;
                }

                if (!arg.Contains(".gdt"))
                {
                    CLI.ErrorMessage("* Not a GDT");
                    return;
                }

                string GDT = Path.GetFileName(arg);
                CLI.WaitMessage($"* Parsing {GDT}");
                Parser.ParseGDT(arg, GDT); // Parse the GDT
            }

            CLI.WaitForUserConfirmation();
        }
    }
}