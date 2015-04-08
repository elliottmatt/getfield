using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getfield
{
    class ParseClass
    {
        /// <summary>
        /// Splits the string passed in by the delimiters passed in.
        /// Quoted sections are not split, and all tokens have whitespace
        /// trimmed from the start and end.
        /// Source: http://stackoverflow.com/questions/554013/regular-expression-to-split-on-spaces-unless-in-quotes
        /// </summary>
        public static string[] ParseLine(string stringToSplit, bool trimFields, char delimiter, bool preserveQuotes, char quoteChar)
        {
            if (preserveQuotes)
            {
                List<string> results = new List<string>();
                bool inQuote = false;
                StringBuilder currentToken = new StringBuilder();
                for (int index = 0; index < stringToSplit.Length; ++index)
                {
                    char currentCharacter = stringToSplit[index];
                    if (currentCharacter == quoteChar)
                    {
                        // When we see a ", we need to decide whether we are
                        // at the start or send of a quoted section...
                        inQuote = !inQuote;
                        currentToken.Append(currentCharacter);
                    }
                    else if (delimiter == currentCharacter && !inQuote)
                    {
                        // We've come to the end of a token, so we find the token,
                        // trim it and add it to the collection of results...
                        string result = currentToken.ToString();
                        if (trimFields)
                            result = result.Trim();
                        results.Add(result);

                        // We start a new token...
                        currentToken = new StringBuilder();
                    }
                    else
                    {
                        // We've got a 'normal' character, so we add it to
                        // the curent token...
                        currentToken.Append(currentCharacter);
                    }
                }

                // We've come to the end of the string, so we add the last token...
                string lastResult = currentToken.ToString();
                if (trimFields)
                    lastResult = lastResult.Trim();
                results.Add(lastResult);

                return results.ToArray();
            }
            else
            {
                string[] results = stringToSplit.Split(new char[] { delimiter }, StringSplitOptions.None);
                if (trimFields)
                {
                    for (int i = 0; i < results.Length; i++)
                    {
                        results[i] = results[i].Trim();
                    }
                }
                return results;
            }
        }

        public static int ParseStringArg(string s, out char parsed)
        {
            parsed = '|'; // guaranteed to be overwritten if return = 0;

            if (s.Equals("\\t", StringComparison.CurrentCultureIgnoreCase)
                || s.Equals("tab", StringComparison.CurrentCultureIgnoreCase))
            {
                parsed = '\t';
                return 0;
            }
            else if (s.StartsWith("0x"))
            {
                try
                {
                    parsed = System.Convert.ToChar(System.Convert.ToUInt32(s, 16));
                    return 0;
                }
                catch
                {
                    Console.Error.WriteLine("Unable to parse hex character.");
                    return 1;
                }
            }
            else
            {
                parsed = s[0];
                return 0;
            }
        }

        public static int ParseIntArg(string s, out int parsed)
        {
            parsed = 0; // guaranteed to be overwritten if return = 0;

            int n;
            if (!Int32.TryParse(s, out n))
            {
                Console.Error.WriteLine("Unable to parse given number");
                return 1;
            }
            else
            {
                parsed = n;
                return 0;
            }
        }


        public static bool IsAllDigits(string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (!Char.IsDigit(s, i))
                {
                    return false;
                }
            }

            return (s.Length > 0);
        }
    }
}

