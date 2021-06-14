using System;
using System.Linq;
using Chess.Engine;
using Chess.Model;

namespace Chess
{
    internal class Program
    {
        public static Board Board;
        public static string Color;

        private static void Main(string[] args)
        {
            //ResetBoard();

            //var piece = Board.GetPiece(new Point(1, 4));
            //var move = piece.PossibleMoves.FirstOrDefault(x => x.To == new Point(3, 4));

            //Board.MovePiece(move, true);
            //Color = "Black";
            //Board.Update("Black");

            //Console.WriteLine(GetBestMove(10, 20000));

            while (true)
            {
                var input = Console.ReadLine();
                Communication(input);
            }
        }

        public static void Communication(string msg)
        {
            if (msg == "uci")
            {
                Console.WriteLine("id name Chess");
                Console.WriteLine("id author bluefine");
                Console.WriteLine("uciok");
            }
            else if (msg == "isready")
            {
                Console.WriteLine("readyok");
            }
            else if (msg == "ucinewgame")
            {
                ResetBoard();
            }
            else if (msg.Contains("position"))
            {
                ResetBoard();
                var info = msg.Split(" ");
                if (info.Length > 2)
                    foreach (var move in info.Skip(3))
                    {
                        var piece = GetPieceFromUser(move[0] + move[1].ToString());
                        var pos = GetDestination(move[2] + move[3].ToString());

                        if (piece != null && piece.Color == Color)
                        {
                            var dest = piece.PossibleMoves.FirstOrDefault(x => x.To == pos);
                            if (dest != null)
                            {
                                Board.MovePiece(dest, true);
                                Color = Color == "White" ? "Black" : "White";
                                Board.Update(Color);
                            }
                        }
                    }
            }
            else if (msg.Contains("go"))
            {
                var depth = 10;
                var time = -1;
                var info = msg.Split(" ");
                if (info.Length > 1)
                {
                    if (info[1] == "wtime")
                    {
                        var whiteTime = info[2];
                        var blackTime = info[4]; //add later the info about time inc

                        if (Color=="White")
                        {
                            time = (int)(Convert.ToInt32(whiteTime) * 0.1);
                        }
                        else
                        {
                            time = (int)(Convert.ToInt32(blackTime) * 0.1);
                        }
                    }
                    else if (info[1] == "depth")
                    {
                        depth = Convert.ToInt32(info[2]);
                    }
                }
                Console.WriteLine(GetBestMove(depth, time));
            }
        }

        public static void ResetBoard()
        {
            Board = new Board();
            Board.New();
            Color = "White";
            Board.Update(Color);
        }

        public static string GetBestMove(int depth, int time)
        {
            //lets say we will use every time 10% of our time to move, probably i isn;t best tactic but maybe it will work well

            var main = new Main(time);
            main.Search(Board, Color, depth);
            return "bestmove " + main.AlphaBeta.BestMove.From.ToNotation() + main.AlphaBeta.BestMove.To.ToNotation();
        }

        public static Piece GetPieceFromUser(string from)
        {
            var piece = Board.GetPieceByNotation(from);
            return piece;
        }

        public static Point GetDestination(string to)
        {
            var pos = Point.GetPointByNotation(to);
            return pos;
        }
    }
}