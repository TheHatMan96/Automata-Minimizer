using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectoTC
{
    class Node
    {
        public string id {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public bool initial
        {
            get; set;
        }
        public bool final
        {
            get; set;
        }
        //(states list) read all the nodes|states
        public List<Node> listnode = new List<Node>();

        public Node()
        {
            id = "";
            name = "";
            initial = false;
            final = false;
        }
       

        public Node(string id, string name, bool initial, bool final)
        {
            this.id = id;
            this.name = name;
            this.initial = initial;
            this.final = final;
        }

    }
}
