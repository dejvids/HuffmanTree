using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanTree.ViewModel
{
    public class CharacterType
    {
        public char Character { get; set; }
        public int ASCII { get { return (int)Character; } }
        public int Count { get; set; }
        public float Probability { get; set; }
        public string Code { get; set; }
    }
}
