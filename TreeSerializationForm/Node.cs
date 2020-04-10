using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeSerializationForm
{
    public class Node
    {
        public double value {get; set;}
        public int id { get; set; }
        public bool isRoot { get; private set; }
        public bool hasChildren { get { return children.Count > 0; } }
        public int numberOfChildren { get; set; } = 0;   //potrzebuję do deserializacji
        public int height { get; set; }
        public List<Node> children { get; }
        public List<Leaf> leaves { get; }

        public Node()
        {
            children = new List<Node>();
            leaves = new List<Leaf>();
        }
        public void addChild (Node other)
        {
            this.children.Add(other);
        }
        public void addLeaf(Leaf l)
        {
            leaves.Add(l);
        }
        public void setAsRoot()
        {
            isRoot = true;
        }

    }
}
