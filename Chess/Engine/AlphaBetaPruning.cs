using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chess.Model;

namespace Chess.Engine
{
    public class AlphaBetaPruning
    {
        public Move BestMove;
        public int DepthLimit;
        public int HashTableHits;
        public int Nodes;
        public Stopwatch Stopwatch;
        public int TimeLimit;
        public Dictionary<ulong, Transposition> Transpositions = new();

        public double Search(Board board, string color, int depth, double alpha, double beta, bool player)
        {
            if (Stopwatch.ElapsedMilliseconds >= TimeLimit) return int.MaxValue;

            var entity = Lookup(board.ZobristKey);
            if (entity != null)
                if (entity.Depth >= depth)
                {
                    HashTableHits++;
                    if (entity.Flag == Transposition.Flags.Exact)
                        return entity.Value;
                    if (entity.Flag == Transposition.Flags.Lower)
                        alpha = Math.Max(alpha, entity.Value);
                    else if (entity.Flag == Transposition.Flags.Upper) beta = Math.Min(beta, entity.Value);

                    if (alpha >= beta) return entity.Value;
                }

            if (board.IsCheck(color))
                if (board.IsCheckMate(color))
                {
                    if (color == "White") return -1000 + -100 + depth;

                    return 1000 + 100 - depth;
                }


            if (depth >= DepthLimit) return GetBoardValue(board); //add depth lvl extra bonus

            depth++;

            var scores = new List<double>();
            var moves = new List<Move>();
            var pieces = board.GetPiecesByColor(color);
            foreach (var piece in pieces.OrderBy(x => x.Position.Distance(new Point(4, 4))).ToList()) //change to 3.5, 3.5
            {
                piece.UpdateHitMoves(board);
                piece.FindLegalMoves(board);
                foreach (var move in piece.LegalMoves)
                {
                    Nodes++;
                    var bN = new Board();
                    bN.New();
                    foreach (var item in board.MovesHistory) bN.MovePiece(item, true);

                    if (piece.NameShort == 'K')
                        if (move.To.X == 0 && move.To.Y == 2)
                        {
                        }

                    bN.MovePieceNewInstance(piece, move, true);
                    bN.ZobristKey = ZobristHashing.Hash(bN.GetBoardCharTable());

                    moves.Add(move);
                    scores.Add(Search(bN, ChangeColor(color), depth, alpha, beta, !player));

                    var bestMove = scores.Last();

                    if (player)
                    {
                        alpha = Math.Max(alpha, bestMove);
                        if (beta <= alpha) return bestMove;
                    }
                    else
                    {
                        beta = Math.Min(beta, bestMove);
                        if (beta <= alpha) return bestMove;
                    }
                }
            }

            if (!Transpositions.ContainsKey(board.ZobristKey))
            {
                if (scores.Any() && board.ZobristKey != 0)
                {
                    var bestValue = color == "White" ? scores.Max() : scores.Min();
                    var node = new Transposition(bestValue, depth);
                    if (bestValue <= alpha)
                        node.Flag = Transposition.Flags.Upper;
                    else if (bestValue >= beta)
                        node.Flag = Transposition.Flags.Lower;
                    else
                        node.Flag = Transposition.Flags.Exact;
                    Transpositions.Add(board.ZobristKey, node);
                }
            }
            else //to be tested, higher depth contains more valuable info
            {
                var node = Transpositions[board.ZobristKey];

                if (depth > node.Depth)
                    if (scores.Any())
                    {
                        var bestValue = color == "White" ? scores.Max() : scores.Min();
                        node.Value = bestValue;
                        node.Depth = depth;

                        if (bestValue <= alpha)
                            node.Flag = Transposition.Flags.Upper;
                        else if (bestValue >= beta)
                            node.Flag = Transposition.Flags.Lower;
                        else
                            node.Flag = Transposition.Flags.Exact;
                    }
            }

            if (depth == 1)
            {
                if (scores.Contains(int.MaxValue)) return 0;

                if (color == "White")
                {
                    if (scores.Count > 0)
                    {
                        var s = scores.IndexOf(scores.Max());
                        BestMove = moves[s];
                    }
                }
                else
                {
                    if (scores.Count > 0)
                    {
                        var s = scores.IndexOf(scores.Min());
                        BestMove = moves[s];
                    }
                }
            }

            if (color == "White")
            {
                if (scores.Count > 0) return scores.Max();

                return 0;
            }

            if (scores.Count > 0) return scores.Min();

            return 0;
        }

        private Transposition Lookup(ulong zobristKey)
        {
            if (Transpositions.ContainsKey(zobristKey)) return Transpositions[zobristKey];

            return null;
        }

        private double GetBoardValue(Board board)
        {
            var pieces = board.GetAllPieces();
            var total = 0D;
            foreach (var piece in pieces)
            {
                total += piece.Value;
                total += PieceSquareTables.GetValue(piece);
            }

            return total;
        }

        private string ChangeColor(string color)
        {
            return color == "White" ? "Black" : "White";
        }
    }
}