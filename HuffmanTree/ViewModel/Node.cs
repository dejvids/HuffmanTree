using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace HuffmanTree.ViewModel
{
    public class Node
    {
        [XmlIgnore]
        public char Symbol { get; set; }
        [JsonIgnore]
        [XmlElement("Symbol"), Browsable(false)]
        public string SymbolString { get { return Symbol.ToString(); } set { Symbol = value.FirstOrDefault(); } }
        public int Frequency { get; set; }
        public Node Right { get; set; }
        public Node Left { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public bool IsLeaf { get { return Right == null && Left == null; } }

        public List<bool> Traverse(char symbol, List<bool> data)
        {
            // Leaf
            if (Right == null && Left == null)
            {
                if (symbol.Equals(this.Symbol))
                {
                    return data;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<bool> left = null;
                List<bool> right = null;

                if (Left != null)
                {
                    List<bool> leftPath = new List<bool>();
                    leftPath.AddRange(data);
                    leftPath.Add(false);

                    left = Left.Traverse(symbol, leftPath);
                }

                if (Right != null)
                {
                    List<bool> rightPath = new List<bool>();
                    rightPath.AddRange(data);
                    rightPath.Add(true);
                    right = Right.Traverse(symbol, rightPath);
                }

                if (left != null)
                {
                    return left;
                }
                else
                {
                    return right;
                }
            }
        }
    }

}
