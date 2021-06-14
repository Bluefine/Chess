using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Model
{
    public class Transposition
    {
        public double Value;
        public int Depth;
        public Flags Flag;

        public enum Flags
        {
            Exact,
            Lower,
            Upper
        }

        public Transposition(double value, int depth, Flags flag)
        {
            Value = value;
            Depth = depth;
            Flag = flag;
        }

        public Transposition(double value, int depth)
        {
            Value = value;
            Depth = depth;
        }
    }
}
