using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semi_automatic_trace
{
    public class Element_Atr
    {
        private string name;

        public string Name
        {
            get => name;
            set => name = value;    
        }

        private Dictionary<string, string> attributes;

        public Dictionary<string, string> Attributes 
        { 
            get => attributes; 
            set => attributes = value; 
        }
    }
}
