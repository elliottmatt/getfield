using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getfield
{
    public partial class Getfield
    {

        public int ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; )
            {
                if (!args[i].StartsWith("-"))
                {
                    if (ReadStdin)
                    {
                        FilenameIn = args[i];
                        i++;
                        continue;
                    }
                    else
                    {
                        Console.Error.WriteLine("Expected input of |{0}|. (Did you give two files?)", args[i]);
                        return 1;
                    }
                }

                switch (args[i])
                {
                    case "-f":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected a delimiter after -f.");
                                return 1;
                            }

                            List<Field> tempFields = ParseFields(args[i]);
                            if (tempFields.Count == 0)
                            {
                                Console.Error.WriteLine("No fields were given after -f");
                                return 1;
                            }
                            Fields.AddRange(tempFields);
                            i++;
                            break;
                        }
                    case "-t": TrimFields = true; i++; break;
                    case "-e": PrintEmptyFields = true; i++; break;
                    case "-td": PrintTrailingDelim = true; i++; break;
                    case "-d":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected a delimiter after -d.");
                                return 1;
                            }
                            char parsed;
                            int n = ParseClass.ParseStringArg(args[i], out parsed);
                            if (n > 0) return n;
                            Delimiter = parsed;
                            i++;
                            break;
                        }
                    case "-od":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected a delimiter after -od.");
                                return 1;
                            }
                            char parsed;
                            int n = ParseClass.ParseStringArg(args[i], out parsed);
                            if (n > 0) return n;
                            OutDelimiter = parsed;
                            i++;
                            break;
                        }
                    //case "-Q": PreserveQuotes = true; i++; break;
                    case "-Q":
                        {
                            PreserveQuotes = true;
                            i++;
                            if (i >= args.Length)
                            {
                                // no quote char given
                                break;
                            }
                            if (args[i].StartsWith("-"))
                            {
                                // no quote char given
                                break;
                            }
                            
                            // it appears i have a delim after it!
                            char parsed;
                            int n = ParseClass.ParseStringArg(args[i], out parsed);
                            if (n > 0) return n;
                            QuoteChar = parsed;
                            i++;
                            break;
                        }
                    case "-o":
                    case "-out":
                        {
                            i++;
                            if (i >= args.Length)
                            {
                                Console.Error.WriteLine("Expected another input after -o.");
                                return 1;
                            }
                            FilenameOut = args[i];
                            WriteStdout = false;
                            i++;
                            break;
                        }
                    case "-v": Verbose = true; i++; break;

                    default:
                        {
                            bool isError = true;
                            if (args[i].StartsWith("-"))
                            {
                                List<Field> tempFields = ParseFields(args[i].Substring(1));
                                if (tempFields != null && tempFields.Count > 0)
                                {
                                    isError = false;
                                    if (tempFields.Count > 0)
                                    {
                                        Fields.AddRange(tempFields);
                                    }
                                    i++;
                                    break;
                                }
                            }
                            if (isError)
                            {
                                Console.WriteLine("Expected input of |{0}|.", args[i]);
                                return 1;
                            }
                            break;
                        }
                }
            }

            if (Fields.Count == 0)
            {
                throw new Exception("No fields given to print out. (Check for -f arg)");
            }
            
            return 0;
        }
        
        public static List<Field> ParseFields(string p)
        {
            List<Field> f = new List<Field>();
            string[] arr = p.Split(new char[] { ',' });
            for (int i = 0; i < arr.Length; ++i)
            {
                List<Field> inner = ParseField(arr[i].Trim());
                f.AddRange(inner);
            }

            return f;
        }

        private static List<Field> ParseField(string p)
        {
            string[] num = p.Split(new char[] { '-' });
            if (num.Length == 1) // single field such as A R * 1 2 5
            {
                if (num[0].Contains("+"))
                {
                    return ParseAppend(num[0]);
                }
                else if (ParseClass.IsAllDigits(num[0]))
                {
                    return new List<Field>() { new Field(Int32.Parse(num[0]) - 1) };
                }
                else if (IsValidField(num[0]))
                {
                    return new List<Field>() { new Field(num[0]) };
                }
                else
                {
                    throw new Exception(String.Format("Unable to parse fields |{0}| (Expected A R * 1 2 1+2, etc)", p));
                }
            }
            else if (num.Length == 2) // multiple such as 1-10 3-4 6-1
            {
                if (ParseClass.IsAllDigits(num[0]) && ParseClass.IsAllDigits(num[1]))
                {
                    List<Field> list = new List<Field>();
                    int n1 = Int32.Parse(num[0]);
                    int n2 = Int32.Parse(num[1]);
                    int diff = (n1 < n2 ? 1 : -1);

                    for (int i = n1; i != (n2 + diff); i += diff)
                    {
                        Field f = new Field(i-1);
                        list.Add(f);
                    }
                    return list;
                }
                else
                {
                    throw new Exception(String.Format("Unable to parse fields |{0}| (Expected two numbers in range?)", p));
                }
            }

            throw new Exception(String.Format("Unable to parse fields |{0}|", p));
        }

        private static List<Field> ParseAppend(string p)
        {
            string[] num = p.Split(new char[] { '+' });
            if (num.Length < 2)
                throw new Exception(String.Format("Unable to parse the append fields |{0}| (Expected multiple fields?)", p));
            
            Field app = new Field();
            for (int i = 0; i < num.Length; ++i)
            {
                if (ParseClass.IsAllDigits(num[i]))
                {
                    int fn = Int32.Parse(num[i]);
                    app.FieldNums.Add(fn - 1);
                }
            }
            return new List<Field>() { app };
        }

        private static bool IsValidField(string s)
        {
            if (s.ToUpper() == "A"
                || s.ToUpper() == "*"
                || s.ToUpper() == "R")
            {
                return true;
            }
            return false;
        }
    }
}
