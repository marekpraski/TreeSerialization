using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeSerializationForm
{
    public class Tree
    {
        public Node root { get; }
        public Tree(Node root)
        {
            this.root = root;
        }
    }
}
