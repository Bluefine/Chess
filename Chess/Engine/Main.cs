using System;
using System.Diagnostics;
using Chess.Model;

namespace Chess.Engine
{
    public class Main
    {
        public AlphaBetaPruning AlphaBeta;

        public Main(int time)
        {
            AlphaBeta = new AlphaBetaPruning {TimeLimit = time};
        }

        public void Search(Board board, string color, int depth)
        {
            ZobristHashing.Initialize();
            AlphaBeta.Stopwatch = new Stopwatch();
            AlphaBeta.Stopwatch.Start();

            for (int i = 1; i < 10; i++)
            {
                AlphaBeta.DepthLimit = i;
                AlphaBeta.Search(board, color, 0, -10000, 10000, color == "White");
                Console.WriteLine($"info depth {i} nodes {AlphaBeta.Nodes}");
                if (i >= depth)
                {
                    break;
                }

                if (AlphaBeta.Stopwatch.ElapsedMilliseconds > AlphaBeta.TimeLimit)
                {
                    break;
                }
            }
            AlphaBeta.Stopwatch.Stop();
        }

    }
}