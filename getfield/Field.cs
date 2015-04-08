using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getfield
{
    public class Field
    {
        /// <summary>
        /// Create a special-case field
        /// </summary>
        /// <param name="s"></param>
        public Field(string s)
        {
            FieldNums = null;
            if (s.ToUpper() == "*" || s.ToUpper() == "A")
            {
                AllFields = true;
            }
            else if (s.ToUpper() == "R")
            {
                Remaining = true;
            }
            else
            {
                throw new Exception(String.Format("Unable to parse Field |{0}|", s));
            }
        }

        /// <summary>
        /// Create a field number
        /// </summary>
        /// <param name="n"></param>
        public Field(int n)
        {
            FieldNum = n;
            FieldNums = null;
        }

        /// <summary>
        /// Create an append field
        /// </summary>
        public Field()
        {
            FieldNums = new List<int>();
        }

        public int FieldNum { get; private set; }
        
        public bool Remaining { get; private set; }
        
        public bool AllFields { get; private set; }

        public bool IsSingleField
        {
            get { return (FieldNums == null && !Remaining && !AllFields); }
        }
        
        public bool IsAppend
        {
            get { return FieldNums != null && FieldNums.Count > 0; }
        }

        public List<int> FieldNums { get; private set; }

        public string VerboseType
        {
            get
            {
                if (IsSingleField) return "SingleField";
                if (IsAppend) return "IsAppend";
                if (Remaining) return "IsRemaining";
                return "UNKNOWN - ERROR";
            }
        }

        public string VerboseDefinition
        {
            get
            {
                if (IsSingleField) return FieldNum.ToString();
                if (IsAppend)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < FieldNums.Count ; ++i)
                    {
                        if (i > 0) { sb.Append("+"); }
                        sb.Append(FieldNums[i].ToString());
                    }
                    return sb.ToString();
                }
                if (Remaining) return "IsRemaining";
                return "UNKNOWN - ERROR";
            }
        }
    }
}
