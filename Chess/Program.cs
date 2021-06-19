using System;
using System.Diagnostics;
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
            //Console.WriteLine(GetBestMove(3, 3333333));
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
                            var dest = piece.LegalMoves.FirstOrDefault(x => x.To == pos);
                            if (dest != null)
                            {
                                Board.MovePiece(dest, true);
                                Color = Color == "White" ? "Black" : "White";
                                Board.Update(Color);
                                //DebugBoard(true, Board);
                            }
                        }
                    }
            }
            else if (msg.Contains("go"))
            {
                var depth = 10;
                var time = 99999999;
                var info = msg.Split(" ");
                if (info.Length > 1)
                {
                    if (info[1] == "wtime")
                    {
                        var whiteTime = info[2];
                        var blackTime = info[4]; //add later the info about time inc

                        if (Color == "White")
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

        public static void DebugBoard(bool flip, Board board)
        {
            var gameBoardClone = (Piece[,])board.Pieces.Clone();
            if (flip)
            {
                gameBoardClone = FlipArray(gameBoardClone);
            }
            Debug.WriteLine("");
            //Console.Clear();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (gameBoardClone[i, j] != null)
                    {
                        Debug.Write(gameBoardClone[i, j].NameShort + " ");
                    }
                    else
                    {
                        Debug.Write("X ");
                    }
                }
                Debug.WriteLine("");
            }
        }

        private static Piece[,] FlipArray(Piece[,] arrayToFlip)
        {
            var rows = arrayToFlip.GetLength(0);
            var columns = arrayToFlip.GetLength(1);
            var flippedArray = new Piece[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    flippedArray[i, j] = arrayToFlip[(rows - 1) - i, j];
                }
            }
            return flippedArray;
        }

        public static void ResetBoard()
        {
            Board = new Board();
            Board.New();
            Color = "White";
            var st = new Stopwatch();
            st.Start();
            Board.UpdateHit((Color));
            Board.UpdateHit((ReverseColor(Color)));
            Board.Update(Color);
            Board.Update(ReverseColor(Color));


            st.Stop();
            Debug.WriteLine(st.ElapsedMilliseconds);
        }

        public static string ReverseColor(string color)
        {
            return color == "White" ? "Black" : "White";
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