using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getfield
{
    public partial class Getfield
    {
        #region Properties

        string FilenameIn { get; set; } // <file>
        string FilenameOut { get; set; } // -F
        bool ReadStdin { get { return (FilenameIn == null); } }
        bool WriteStdout { get; set; }
        TextWriter OutStream { get; set; }

        List<Field> Fields { get; set; }

        bool PrintEmptyFields { get; set; } // -e
        bool PrintTrailingDelim { get; set; } // -td

        bool TrimFields { get; set; } // -t

        char QuoteChar { get; set; }
        char Delimiter { get; set; } // -d
        char OutDelimiter { get; set; } // -d

        bool PreserveQuotes { get; set; } // -Q

        public bool Verbose { get; set; } // -v

        #endregion

        public Getfield()
        {
            FilenameIn = null;
            WriteStdout = true;
            OutStream = null;

            TrimFields = false;

            QuoteChar = '"';
            Delimiter = '|';
            OutDelimiter = '|';

            PreserveQuotes = false;

            Verbose = false;
            PrintTrailingDelim = false;
            PrintEmptyFields = false;

            Fields = new List<Field>();
        }

    
        public int Run()
        {
            TextReader instream = null;
            try
            {
                OutStream = (WriteStdout ? Console.Out : new StreamWriter(FilenameOut));
                instream = (ReadStdin ? Console.In : new StreamReader(FilenameIn));

                if (Verbose)
                {
                    for (int i = 0; i < Fields.Count; ++i)
                    {
                        Field f = Fields[i];
                        OutStream.WriteLine(String.Format("Field{0}|{1}|{2}|",
                            i,
                            f.VerboseType,
                            f.VerboseDefinition
                            )
                        );                            
                    }
                }

                for (int row = 0; ; row++)
                {
                    string line = instream.ReadLine();
                    if (line == null)
                        break;

                    string[] arr = ParseClass.ParseLine(line, TrimFields, Delimiter, PreserveQuotes, QuoteChar);
                    PrintFields(arr);
                    if (PrintTrailingDelim) { OutStream.Write(OutDelimiter); }
                    OutStream.WriteLine();
                }

                return 0;
            }
            finally
            {
                if (instream != null && instream != Console.In)
                {
                    try { instream.Close(); }
                    catch { }
                    try { instream.Dispose(); }
                    catch { }
                }

                if (OutStream != null && OutStream != Console.Out)
                {
                    try { OutStream.Close(); }
                    catch { }
                    try { OutStream.Dispose(); }
                    catch { }
                }
            }
        }

        public void PrintFields(string[] arr)
        {
            bool needDelim = false;
            for (int i = 0; i < Fields.Count; ++i)
            {
                if (needDelim) { OutStream.Write(OutDelimiter); }

                Field f = Fields[i];
                needDelim = PrintField(f, arr);
            }
        }

        /// <summary>
        /// Print out a field
        /// </summary>
        /// <param name="f"></param>
        /// <param name="arr"></param>
        /// <returns>True if delim is needed</returns>
        public bool PrintField(Field f, string[] arr)
        {
            if (f.IsSingleField)
            {
                if (f.FieldNum < arr.Length)
                {
                    OutStream.Write(arr[f.FieldNum]);
                    return true;
                }
                else 
                {
                    // if want an empty field, return true
                    return PrintEmptyFields;
                }
            }
            else if (f.IsAppend)
            {
                bool printed = false;
                for (int i = 0; i < f.FieldNums.Count; ++i)
                {
                    int n = f.FieldNums[i];
                    if (n < arr.Length)
                    {
                        OutStream.Write(arr[n]);
                        printed = true;
                    }
                }
                return printed || PrintEmptyFields;
            }
            else if (f.AllFields)
            {
                for (int i = 0; i < arr.Length; ++i)
                {
                    if (i > 0) { OutStream.Write(OutDelimiter); }
                    OutStream.Write(arr[i]);
                }
                return true;
            }
            else if (f.Remaining)
            {
                bool needDelim = false;
                bool printed = false;
                for (int i = 0; i < arr.Length; ++i)
                {
                    if (needDelim) { OutStream.Write(OutDelimiter); }
                    needDelim = false;

                    // need to see if 'i' is somewhere else in Fields
                    bool printedElsewhere = false;
                    foreach (Field ff in Fields)
                    {
                        if (ff.IsSingleField && ff.FieldNum == i)
                        {
                            printedElsewhere = true;
                            break;
                        }
                    }
                    if (!printedElsewhere)
                    {
                        OutStream.Write(arr[i]);
                        printed = true;
                        needDelim = true;
                    }
                }
                return printed;
            }
            else
            {
                throw new Exception("Unexpected code");
            }
        }
    }
}
