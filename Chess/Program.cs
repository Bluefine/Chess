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
        public static bool Turn = true;

        private static void Main(string[] args)
        {
            //var bbb = new Board();
            //bbb.New();
            //bbb.UpdateBoard();

            //while (true)
            //{
            //    var main = new Main(333333);
            //    main.Search(bbb, Turn, 10);
            //}
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
                {
                    if (info[1] == "fen")
                    {
                        return; //add fen 
                    }

                    foreach (var move in info.Skip(3))
                    {
                        var piece = GetPieceFromUser(move[0] + move[1].ToString());
                        var pos = GetDestination(move[2] + move[3].ToString());

                        if (piece != null && piece.IsWhite == Turn)
                        {
                            var dest = piece.LegalMoves.FirstOrDefault(x => x.Destination == pos);
                            if (dest != null)
                            {
                                Board.MakeMove(dest);
                                Board.UpdateBoard();
                                Turn = !Turn;
                                //DebugBoard(true, Board);
                            }
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

                        if (Turn)
                        {
                            time = (Convert.ToInt32(whiteTime) / 20);
                        }
                        else
                        {
                            time = (Convert.ToInt32(blackTime) / 20);
                        }
                    }
                    else if (info[1] == "depth")
                    {
                        depth = Convert.ToInt32(info[2]);
                    }
                    else if (info[1] == "movetime")
                    {
                        time = Convert.ToInt32(info[2]);
                    }
                }
                Console.WriteLine(GetBestMove(depth, time));
            }
        }

        public static void DebugBoard(bool flip, Board board)
        {
            var gameBoardClone = (Piece[,])board.GameBoard.Clone();
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
                        Debug.Write(gameBoardClone[i, j].PieceType.ToString()[0] + " ");
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
            Turn = true;
            var st = new Stopwatch();
            st.Start();
            Board.UpdateBoard();
            st.Stop();
            Debug.WriteLine(st.ElapsedMilliseconds);
        }

        public static string GetBestMove(int depth, int time)
        {
            DebugBoard(true, Board);
            Board.UpdateBoard();
            var main = new Main(time);
            main.Search(Board, Turn, depth);
            return "bestmove " + main.AlphaBeta.BestMove.Piece.Position.ToNotation() + main.AlphaBeta.BestMove.Destination.ToNotation();
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