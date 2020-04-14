using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeSerializationForm
{
    public partial class Form1 : Form
    {

        private Tree tree;
        public Form1()
        {
            InitializeComponent();
        }

        #region generate and populate a dummy tree

        public void generateTree()
        {
            Node rootNode = new Node() { value = 0, height = 3 };
            rootNode.id = 1;//rootNode.GetHashCode();
            rootNode.setAsRoot();
            tree = new Tree(rootNode);
            //populateTreeAuto(3);
            populateTreeCustom();
        }

        private void populateTreeCustom()
        {
            addChildren(tree.root, 2,1,2);

            //addLeaves(tree.root.children[0], 9);

            addChildren(tree.root.children[0], 3, 10, 1);
            //addChildren(tree.root.children[1], 2, 10, 1);

            //addChildren(tree.root.children[0].children[0],2,100,4);

            addLeaves(tree.root.children[1], 4);

            addLeaves(tree.root.children[0].children[0], 3);
            addLeaves(tree.root.children[0].children[1], 1);
            addLeaves(tree.root.children[0].children[2], 2);

            //addLeaves(tree.root.children[1].children[0], 1);
            //addLeaves(tree.root.children[1].children[1], 2);

            //addLeaves(tree.root.children[0].children[0].children[0], 3);
            //addLeaves(tree.root.children[0].children[0].children[1], 2);

        }

        private void populateTreeAuto(int index)
        {
            for (int i = 0; i<index; i++)
            {
                Node n = new Node() { value = 1 * i, height = 2 };
                n.id = n.GetHashCode();
                tree.root.addChild(n);
                addChildren(n, 3,1,1);
            }
        }

        private void addChildren(Node parent)
        {
            double v = parent.value;
            for (int i = 0; i < 4; i++)
            {
                Node n = new Node() { value = v*10 * i, height = 1 };
                n.id = n.GetHashCode();
                parent.addChild(n);
                addLeaves(n,3);
            }
        }

        private void addChildren(Node parent, int numberOfChildren, int valueIndex, int heigthIndex)
        {
            double v = parent.value;
            for (int i = 0; i < numberOfChildren; i++)
            {
                Node n = new Node() { value = valueIndex * 10 * i, height = heigthIndex };
                n.id = i + 10;//n.GetHashCode();
                parent.addChild(n);
            }
        }

        private void addLeaves(Node parent, int numberOfLeaves)
        {
            double v = parent.value;
            for (int i = 0; i < numberOfLeaves; i++)
            {
                Leaf leaf = new Leaf() { leafValue = v+1001 * i, id = i };
                parent.addLeaf(leaf);
            }
        }

        #endregion


        const byte readNode = 0;
        const byte readLeaf = 1;
        const byte goUpOneNode = 2;

        #region serialize tree

        private byte[] serializeTree()
        {
            StringBuilder sb = new StringBuilder();
            MemoryStream stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                serializeNode(tree.root, writer, sb);
            }
            byte[] treeData = stream.ToArray();
            stream.Close();
            saveTxt("serializing.txt", sb.ToString());
            return treeData;
        }

        private void serializeNode(Node node, BinaryWriter writer, StringBuilder sb)
        {

            if (node.hasChildren)   /* this is an internal node in the tree */
            {
                serializeNodeData(node, writer, sb);

                for (int i = 0; i < node.children.Count(); i++)
                {
                    serializeNode(node.children[i], writer, sb);
                }
            }
            else /* this is a leaf node */
            {
                serializeNodeData(node, writer,sb);

                for (int k = 0; k < node.leaves.Count(); k++)
                {
                    serializeLeafData(node.leaves[k], writer,sb);
                }
            }

        }

        private void serializeLeafData(Leaf leaf, BinaryWriter writer, StringBuilder sb)
        {
            writer.Write(leaf.id);
            sb.Append("writer.Write leaf.id " + leaf.id + "\r\n");
        }

        private void serializeNodeData(Node node, BinaryWriter writer, StringBuilder sb)
        {
            writer.Write(node.id);
            sb.Append("writer.Write node.id " + node.id + "\r\n");
            writer.Write(node.height);
            sb.Append("writer.Write node.height " + node.height + "\r\n");

            //określam liczbę dzieci dopiero przed serializacją, bo ten parametr jest mi potrzebny do deserializacji
            //nie mogę zautomatyzować liczenia tego parametru w nodzie, bo zaburzy pętlę podczas deserializacji
            node.numberOfChildren = node.children.Count();  
            writer.Write(node.numberOfChildren);
            sb.Append("writer.Write node.numberOfChildren " + node.numberOfChildren + "\r\n");

            node.numberOfLeaves = node.leaves.Count();
            writer.Write(node.numberOfLeaves);
            sb.Append("writer.Write node.numberOfLeaves " + node.numberOfLeaves + "\r\n");
        }

        #endregion


        #region deserialize tree


        private Tree deserializeTree(byte[] treeData)
        {
            StringBuilder sb = new StringBuilder();
            Tree newTree = null;
            try
            {
                MemoryStream stream = new MemoryStream(treeData);

                Node root = new Node();
                newTree = new Tree(root);
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    deserializeNode(root, reader, sb);
                }
            }
            catch (Exception)
            {
                saveTxt("deserialize.txt", sb.ToString());
            }

            saveTxt("deserialize.txt", sb.ToString());
            return newTree;
        }

        private void deserializeNode(Node node,BinaryReader reader, StringBuilder sb)
        {
            deserializeNodeData(node, reader, sb);
            if (node.numberOfChildren > 0) {
                for (int i = 0; i < node.numberOfChildren; i++)
                {
                    Node child = new Node();
                    node.addChild(child);
                    deserializeNode(child, reader, sb);
                }
            }
            else 
            {
                deserializeLeaves(node, reader, sb);
            }
        }

        private void deserializeNodeData(Node node, BinaryReader reader, StringBuilder sb)
        {
            node.id = reader.ReadInt32();
            sb.Append("node.id = reader.ReadInt32() " + node.id + "\r\n");
            node.height = reader.ReadInt32();
            sb.Append("node.height = reader.ReadInt32() " + node.height + "\r\n");
            node.numberOfChildren = reader.ReadInt32();
            sb.Append("node.numberOfChildren = reader.ReadInt32() " + node.numberOfChildren + "\r\n");
            node.numberOfLeaves = reader.ReadInt32();
            sb.Append("node.numberOfLeaves = reader.ReadInt32() " + node.numberOfLeaves + "\r\n");
        }

        private void deserializeLeaves(Node node, BinaryReader reader, StringBuilder sb)
        {
            for(int i =0; i<node.numberOfLeaves; i++)
            {
                Leaf leaf = new Leaf();
                deserializeLeafData(node, reader, leaf);
                sb.Append("leaf.id = reader.ReadInt32(); " + leaf.id + "\r\n");
                sb.Append("nextAction = reader.ReadByte(); 1\r\n");
            }
        }

        private void deserializeLeafData(Node node, BinaryReader reader, Leaf leaf)
        {
            node.addLeaf(leaf);
            leaf.id = reader.ReadInt32();
        }

        #endregion

        private void Button1_Click(object sender, EventArgs e)
        {
            generateTree();
            //StringBuilder sb = new StringBuilder();
            //saveTreeToTxt(tree.root, sb);
            //saveTxt("RTree.txt", sb.ToString());
            byte[] treeData = serializeTree();

            Tree newTree = deserializeTree(treeData);

        }

        #region save text files
        private void saveTreeToTxt(Node n, StringBuilder sb)
        {

            if (n.hasChildren)   /* this is an internal node in the tree */
            {
                sb.Append("node # = " + n.GetHashCode());
                sb.Append("  node value = " + n.value);
                sb.Append("  node heigth = " + n.height + "\r\n");
                sb.Append("read NODE marker \r\n");

                for (int i = 0; i < n.children.Count(); i++)
                {
                    //sb.Append("child " + i + "  node # = " + n.children[i].GetHashCode() + "  node value = " + n.value + "\r\n");
                    saveTreeToTxt((Node)n.children[i], sb);
                    if (i == n.children.Count() - 1)
                    {
                        sb.Append("go up one node marker \r\n");
                    }
                    else
                    {
                        sb.Append("read one node marker\r\n");
                    }
                }
            }

            else /* this is a leaf node */
            {
                sb.Append("node # = " + n.GetHashCode());
                sb.Append("  node value = " + n.value);
                sb.Append("  node heigth = " + n.height + "\r\n");
                sb.Append("read leaf marker \r\n");

                for (int k = 0; k < n.leaves.Count(); k++)
                {
                    sb.Append("leaf " + k + "  leaf # = " + n.leaves[k].GetHashCode() + "  leaf value = " + n.leaves[k].leafValue + "\r\n");
                    if (k == n.leaves.Count() - 1)
                    {
                        sb.Append("go up one node - end of leaves marker \r\n");
                    }
                    else
                    {
                        sb.Append("read leaf marker \r\n");
                    }
                }
            }

        }


        private void saveTxt(string fileName, string fileContent)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Append))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(fileContent);
                writer.Close();
            }
        }

        #endregion

    }
}
