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

            for (var i = 1; i < 99; i++)
            {
                AlphaBeta.DepthLimit = i;
                AlphaBeta.Search(board, 0, -10000, 10000, color == "White");
                var nps = (int) (AlphaBeta.Nodes / (((float) AlphaBeta.Stopwatch.ElapsedMilliseconds + 1) / 1000));
                Console.WriteLine($"info depth {i} nodes {AlphaBeta.Nodes} nps {nps} cp {AlphaBeta.BestScore}");
                if (i >= depth) break;

                if (AlphaBeta.Stopwatch.ElapsedMilliseconds > AlphaBeta.TimeLimit) break;
            }

            AlphaBeta.Stopwatch.Stop();
        }
    }
}