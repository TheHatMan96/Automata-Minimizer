using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectoTC
{
    class AllTransition
    {
        public string to
        {
            get;
            set;
        }
        
        public string from
        {
            get;
            set;
        }
        public string read
        {
            get;
            set;
        }

        // (transition list) read all transition to nodes|states
        public List<AllTransition> transition = new List<AllTransition>();

        public AllTransition() {

        }

        public AllTransition(string to, string from, string read)
        {
            this.to = to;
            this.from = from;
            this.read = read;
        }
    }
}
