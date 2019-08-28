using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace ProjectoTC
{
    class FileLogicAutomata
    {
        private XmlDataDocument jflap = new XmlDataDocument();
        private List<Node> nodes = new List<Node>();
        private List<AllTransition> ts = new List<AllTransition>();
        public string nameFile="";

        public FileLogicAutomata(string path,string nameFile)
        {
            jflap.Load(path);
            this.nameFile = nameFile;
        }

        public void LoadFile()
        {
            XmlNodeList states = jflap.GetElementsByTagName("state");
            XmlNodeList transitions = jflap.GetElementsByTagName("transition");

            foreach(XmlNode node in states)
            {
                string id = node.Attributes["id"].Value;
                string name = node.Attributes["name"].Value;
                bool initial = false;
                bool finish = false;

                foreach (XmlNode end in node)
                {
                    if (end.Name == "initial")
                        initial = true;
                    if (end.Name == "final")
                        finish = true;
                }

                Node n = new Node(id, name, initial, finish);
                nodes.Add(n);
            }
            foreach(XmlNode trans in transitions)
            {
                 AllTransition te = new AllTransition();
                 foreach(XmlNode atributes in trans)
                {
                    string fse = atributes.Name;

                    if(atributes.Name == "from")
                    {
                        te.from = atributes.InnerText;
                    }
                    else if (atributes.Name == "to")
                    {
                        te.from = atributes.InnerText;
                    }
                    else if(atributes.Name == "read")
                    {
                        te.read = atributes.InnerText;
                        fse = null;
                    }
                    if (string.IsNullOrEmpty(fse))                    
                        ts.Add(te);                                         
                }
            }

        }

        public void RemoveLongState()
        {
            XmlNodeList states = jflap.GetElementsByTagName("state");
            XmlNodeList transitions = jflap.GetElementsByTagName("transition");

            for(int i=0; i < states.Count; i++)
            {
                for(int y=0; y < nodes.Count; y++)
                {
                    if(states[i].Attributes["id"].Value == nodes[y].id)
                    {
                        for(int j =0; j < ts.Count; j++)
                        {
                            if(states[i].Attributes["id"].Value == ts[j].to || nodes[y].initial)
                            {
                                break;
                            }
                            if(j == ts.Count - 1)
                            {
                                for(int z =0; z < transitions.Count;z++) { 
                                    foreach(XmlNode x in transitions[z].SelectNodes("from"))
                                    {
                                        if (x.InnerText == states[i].Attributes["id"].Value)
                                        {
                                            transitions[z].ParentNode.RemoveChild(transitions[z]);
                                            ts.Remove(ts[z]);
                                        }
                                     }
                                }
                                states[i].ParentNode.RemoveChild(states[i]);
                                nodes.Remove(nodes[i]);
                            }
                        }
                    }
                }
            }
            jflap.Save(nameFile);
        }

        public void determineState(int i, int j, int[,] Arr, int prev)
        {
            Node tmp1 = nodes[i];
            Node tmp2 = nodes[j];

            string ind1 = "";
            string ind2 = "";

            int idx = 0;
            int idx1 = 0;

            bool w = false;
            bool w1 = false;
            bool wrote = false;

            for (int x = 0; x < ts.Count; x++)
            {
                if (ts[x].from == tmp1.id)
                {
                    ind1 = ts[x].to;
                    for (int c = 0; c < ts.Count; c++)
                    {
                        string read1 = ts[x].read.Replace(",", "");
                        string read2 = ts[c].read.Replace(",", "");

                        if (ts[c].from == tmp2.id && (read1.Contains(read2) || read2.Contains(read1)))
                        {
                            ind2 = ts[c].to;

                            for (int y = 0; y < nodes.Count; y++)
                            {

                                if (nodes[y].id == ind1)
                                {
                                    idx = y;
                                    w = true;
                                }
                                if (nodes[y].id == ind2)
                                {
                                    idx1 = y;
                                    w1 = true;
                                }

                                if ((Arr[idx, idx1] == prev || Arr[idx1, idx] == prev) && (w && w1))
                                {
                                    Arr[i, j] = prev + 1;
                                    w = false;
                                    w1 = false;
                                    wrote = true;
                                    break;
                                }
                                if (!wrote)
                                {
                                    if (w1 && w)
                                    {
                                        w = false;
                                        w1 = false;
                                    }
                                }
                                wrote = false;
                            }
                        }
                    }
                }
            }
        }

        public void minimizerUpToStep()
        {
            int[,] nodeArray = new int[nodes.Count, nodes.Count];
            int delIndex = 0;

            for(int i=0; i < nodes.Count; i++)
            {
                for(int j = delIndex; j < nodes.Count; j++)
                {
                    nodeArray[i, j] = -1;
                }
                delIndex += 1;
            }

            for(int i =0; i < nodes.Count; i++)
            {
                for(int j =0; j < nodes.Count; j++)
                {
                    if(nodeArray[i,j] != -1)
                    {
                        nodeArray[i, j] = -2;
                    }
                }
            }

            //step 0
            for(int i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < nodes.Count; j++)
                {
                     if((nodes[i].final && !nodes[j].final || (!nodes[i].final && nodes[j].final))){
                          if(nodeArray[i,j] == -2)
                          {
                              nodeArray[i, j] = 0;
                          }                                
                     }
                }
            }

            //step 1
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (nodeArray[i, j] == -2)
                    {
                        determineState(i, j, nodeArray, 0);
                    }
                }
            }

            //step 3
            for(int i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < nodes.Count; j++)
                {
                    if(nodeArray[i,j] == -2)
                    {
                        determineState(i, j, nodeArray, 2);
                    }
                }
            }

            algorithmXML(nodeArray);

        }

        public void algorithmXML(int[,] Arr)
        {
            XmlNodeList auto = jflap.GetElementsByTagName("automaton");
            XmlNodeList states = jflap.GetElementsByTagName("state");

            XmlNodeList transitions = jflap.GetElementsByTagName("transition");

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (Arr[i, j] == -2)
                    {
                        for (int c = 0; c < states.Count; c++)
                        {
                            if (!string.IsNullOrEmpty(states[c].Attributes["id"].Value))
                            {
                                if (states[c].Attributes["id"].Value == nodes[i].id || states[c].Attributes["id"].Value == nodes[j].id)
                                {
                                    states[c].ParentNode.RemoveChild(states[c]);
                                    c--;
                                }
                            }
                        }

                        XmlElement nState = jflap.CreateElement("state");
                        XmlNode xVal = jflap.CreateElement("x");
                        XmlNode yVal = jflap.CreateElement("y");
                        XmlNode ini = jflap.CreateElement("initial");
                        XmlNode fin = jflap.CreateElement("final");

                        nState.SetAttribute("id", (states.Count + 100).ToString());
                        nState.SetAttribute("name", "q" + nodes[i].id + "/q" + nodes[j].id);

                        xVal.InnerText = "0";
                        yVal.InnerText = "0";


                        nState.AppendChild(xVal);
                        nState.AppendChild(yVal);

                        if (nodes[i].initial || nodes[j].initial)
                        {
                            nState.AppendChild(ini);
                        }
                        if (nodes[i].final || nodes[j].final)
                        {
                            nState.AppendChild(fin);
                        }

                        auto[0].AppendChild(nState);
                    }
                }
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (Arr[i, j] == -2)
                    {
                        string nameF = nodes[i].id;
                        string nameT = nodes[j].id;

                        for (int c = 0; c < transitions.Count; c++)
                        {
                            foreach (XmlNode t in transitions[c])
                            {
                                if (t.Name == "from")
                                {
                                    if (t.InnerText == nameF || t.InnerText == nameT)
                                    {
                                        transitions[c].ParentNode.RemoveChild(transitions[c]);
                                        c--;
                                        break;
                                    }
                                }
                                if (t.Name == "to")
                                {
                                    if (t.InnerText == nameF || t.InnerText == nameT)
                                    {
                                        transitions[c].ParentNode.RemoveChild(transitions[c]);
                                        c--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (Arr[i, j] == -2)
                    {
                        string name = nodes[i].id;
                        string name2 = nodes[j].id;

                        for (int z = 0; z < ts.Count; z++)
                        {
                            if (ts[z].from == nodes[i].id || ts[z].from == nodes[j].id)
                            {
                                XmlElement nTrans = jflap.CreateElement("transition");
                                XmlNode from = jflap.CreateElement("from");
                                XmlNode to = jflap.CreateElement("to");
                                XmlNode read = jflap.CreateElement("read");

                                for (int b = 0; b < states.Count; b++)
                                {
                                    if (states[b].Attributes["name"].Value == "q" + nodes[i].id + "/q" + nodes[j].id)
                                    {
                                        from.InnerText = states[b].Attributes["id"].Value;
                                        for (int u = 0; u < nodes.Count; u++)
                                        {
                                            for (int p = 0; p < nodes.Count; p++)
                                            {
                                                if (Arr[u, p] == -2)
                                                {
                                                    if (nodes[u].id == ts[z].to || nodes[p].id == ts[z].to)
                                                    {
                                                        foreach (XmlNode st in states)
                                                        {
                                                            if (st.Attributes["name"].Value == "q" + nodes[u].id + "/q" + nodes[p].id)
                                                            {
                                                                to.InnerText = st.Attributes["id"].Value;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ts[z].to != nodes[i].id && ts[z].to != nodes[j].id)
                                {
                                    foreach (XmlNode st in states)
                                    {
                                        if (st.Attributes["id"].Value == ts[z].to)
                                        {
                                            to.InnerText = ts[z].to;
                                            break;
                                        }
                                    }
                                }


                                read.InnerText = ts[z].read;

                                nTrans.AppendChild(from);
                                nTrans.AppendChild(to);
                                nTrans.AppendChild(read);
                                if (!string.IsNullOrEmpty(to.InnerText))
                                {
                                    auto[0].AppendChild(nTrans);
                                }
                                else
                                {

                                }

                            }

                            if (ts[z].to == nodes[i].id || ts[z].to == nodes[j].id)
                            {
                                XmlElement nTrans = jflap.CreateElement("transition");
                                XmlNode from = jflap.CreateElement("from");
                                XmlNode to = jflap.CreateElement("to");
                                XmlNode read = jflap.CreateElement("read");

                                for (int b = 0; b < states.Count; b++)
                                {
                                    if (states[b].Attributes["name"].Value == "q" + nodes[i].id + "/q" + nodes[j].id)
                                    {
                                        to.InnerText = states[b].Attributes["id"].Value;
                                        if (ts[z].from == nodes[i].id || ts[z].from == nodes[j].id)
                                            for (int u = 0; u < nodes.Count; u++)
                                            {
                                                for (int p = 0; p < nodes.Count; p++)
                                                {
                                                    if (Arr[u, p] == -2)
                                                    {
                                                        if (nodes[u].id == ts[z].from || nodes[p].id == ts[z].from)
                                                        {
                                                            foreach (XmlNode st in states)
                                                            {
                                                                if (st.Attributes["name"].Value == "q" + nodes[u].id + "/q" + nodes[p].id)
                                                                {
                                                                    from.InnerText = st.Attributes["id"].Value;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                    }
                                }

                                if (ts[z].from != nodes[i].id && ts[z].from != nodes[j].id)
                                {
                                    foreach (XmlNode st in states)
                                    {
                                        if (st.Attributes["id"].Value == ts[z].from)
                                        {
                                            from.InnerText = ts[z].from;
                                            break;
                                        }
                                    }
                                }

                                read.InnerText = ts[z].read;

                                nTrans.AppendChild(from);
                                nTrans.AppendChild(to);
                                nTrans.AppendChild(read);

                                if (!string.IsNullOrEmpty(from.InnerText))
                                {
                                    auto[0].AppendChild(nTrans);
                                }
                                else
                                {

                                }

                            }
                        }
                    }
                }
            }
                        
            jflap.Save(nameFile);
        }

    }
}
