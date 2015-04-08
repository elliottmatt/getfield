using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getfield
{
    class Program
    {
        static int Main(string[] args)
        {
            Getfield g = null;
            try
            {
                if ((args.Length == 0) || (args.Length > 0 && (args[0] == "--help" || args[0] == "/?")))
                {
                    PrintHelp();
                    return 1;
                }

                g = new Getfield();
                int n = g.ParseArgs(args);
                if (n > 0)
                    return n;

                return g.Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(String.Format("Fatal error! {0}", ex.Message));
                if (g == null || g.Verbose) { Console.Error.WriteLine(ex.ToString()); }
                else { Console.Error.WriteLine("(Run with -v for more details)"); }
                return 2;
            }
        }


        static void PrintHelp()
        {
            string help = @"
getfield by Matt Elliott rewrite in C# (April 2015)

Usage: getfield <file> -f X [options]

Options:
     -f n,*,A,R    Fields to get.
                      n = any number (1-based), such as 1,2,3-6,9-7,10+11
                      R = rest of fields not in [n]     
                      *,A = all fields
                      (Special Case: If you use '+' between fields,
                         it will append with no delimiter. Append fields
                         are ignored in the 'R' value)
                      (Optionally can just write -1,4-10,R as well)
     -d ?          Input delimiter. Defaults to '|' if not given
     -Q ?          Preserve quotes. Defaults to '""' if not given
     -od ?         Output delimiter. Defaults to '|' if not given
     -t            Trims fields on output. Defaults to false
     -o, -out ?    Writes output to file
     -e            Print empty fields not in the input. Defaults to false
                      (e.g. 1-100 prints 100 fields regardless of input)
     -td           Print a trailing delimiter of same as -od
     -v            Print verbose error message (Hint: list as first arg)

If no file is given, reads from stdin. If no outfile is given, writes to stdout.
";
            Console.Error.WriteLine(help.Trim());
        }
    }
}
