namespace Chess.Model
{
    public class Transposition
    {
        public enum Flags
        {
            Exact,
            Lower,
            Upper
        }

        public int Depth;
        public Flags Flag;
        public double Value;

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