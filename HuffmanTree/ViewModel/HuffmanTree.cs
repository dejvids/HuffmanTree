using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace HuffmanTree.ViewModel
{
    public class HuffmanTree
    {
        public Node Root { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public List<Node> Nodes { get; private set; } = new List<Node>();

        [JsonIgnore]
        [XmlIgnore]
        public Dictionary<char, int> Frequencies = new Dictionary<char, int>();

        [JsonIgnore]
        [XmlIgnore]
        public int NumberOfChars { get { return Content.Length; } }

        public string Content { get;  set; }
        public int Height { get; set; }
        public HuffmanTree(string content)
        {
            Height = 0;
            Content = content;
            Build();
        }
        public HuffmanTree()
        {

        }
        public void Build()
        {
            for (int i = 0; i < Content.Length; i++)
            {
                if (!Frequencies.ContainsKey(Content[i]))
                {
                    Frequencies.Add(Content[i], 0);
                }

                Frequencies[Content[i]]++;
            }

            foreach (KeyValuePair<char, int> symbol in Frequencies)
            {
                Nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (Nodes.Count > 1)
            {
                List<Node> orderedNodes = Nodes.OrderBy(node => node.Frequency).ToList<Node>();

                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                    // Create a parent node by combining the frequencies
                    Node parent = new Node()
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    Nodes.Remove(taken[0]);
                    Nodes.Remove(taken[1]);
                    Nodes.Add(parent);
                }

                this.Root = Nodes.FirstOrDefault();

            }
            var height = CalculateHight(this.Root);

        }

        public BitArray Encode(string content)
        {
            BitArray bitArray;
            if (this.Root == null)
                return null;
            List<bool> encoded = new List<bool>();
            for (int i = 0; i < Content.Length; i++)
            {
                var code = this.Root.Traverse(Content[i], new List<bool>());
                encoded.AddRange(code);
            }
            bitArray = new BitArray(encoded.ToArray());
            return bitArray;
        }

        public Dictionary<char, List<bool>> GetCodes()
        {
            Dictionary<char, List<bool>> encodedSource = new Dictionary<char, List<bool>>();
            if (this.Root == null)
                return null;
            for (int i = 0; i < Content.Length; i++)
            {
                if (!encodedSource.Keys.Any(c => c == Content[i]))
                {
                    List<bool> encodedSymbol = this.Root.Traverse(Content[i], new List<bool>());
                    if (encodedSymbol.Count > Height)
                        Height = encodedSymbol.Count;
                    encodedSource.Add(Content[i], encodedSymbol);
                }
            }

            return encodedSource;
        }

        public string Decode(BitArray bits)
        {
            Node current = this.Root;
            string decoded = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (current.IsLeaf)
                {
                    decoded += current.Symbol;
                    current = this.Root;
                }
            }

            return decoded;
        }

        private int CalculateHight(Node node, int height=0)
        {
            int max = height;
            if(node.Left != null)
            {
                CalculateHight(node.Left, height++);
            }
            if(node.Right != null)
            {
                CalculateHight(node.Right, height++);
            }
            return height;
        }

    }

}
