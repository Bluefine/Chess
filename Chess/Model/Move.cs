namespace Chess.Model
{
    public class Move
    {
        public Move(Move move)
        {
            From = move.From;
            To = move.To;
            Hit = move.Hit;
            Castle = move.Castle;
        }

        public Move()
        {
        }

        public Point From { get; set; }
        public Point To { get; set; }
        public Point Hit { get; set; }
        public bool Castle { get; set; }
    }
}