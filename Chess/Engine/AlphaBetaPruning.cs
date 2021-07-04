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
        public double BestScore;
        public int DepthLimit;
        public int HashTableHits;
        public int Nodes;
        public Stopwatch Stopwatch;
        public int TimeLimit;
        public Dictionary<ulong, Transposition> Transpositions = new();

        public double Search(Board board, int depth, double alpha, double beta, bool player)
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
                    else if (entity.Flag == Transposition.Flags.Upper)
                        beta = Math.Min(beta, entity.Value);

                    if (alpha >= beta) return entity.Value;
                }

            if (board.Repetition3())
            {
                if (!player)
                    return 1000;
                return -1000;
            }

            if (board.IsCheck(player))
                if (board.IsCheckMate(player))
                {
                    if (player)
                        return -1000 + depth;
                    return 1000 - depth;
                }

            if (depth >= DepthLimit) return GetBoardValue(board); //add depth lvl extra bonus

            depth++;

            var scores = new List<double>();
            var moves = new List<Move>();
            var pieces = board.GetPiecesByColor(player);
            foreach (var piece in pieces.OrderBy(x => x.Position.Distance(new Point(4, 4))).ToList()) //change to 3.5, 3.5
            {
                piece.UpdateHitMoves(board);
                piece.FindLegalMoves(board);
                foreach (var move in piece.LegalMoves)
                {
                    Nodes++;
                    var bN = new Board();
                    bN.New();
                    foreach (var item in board.MovesHistory)
                        bN.MovePiece(item, true, false);

                    bN.MovePiece(move, true, true);
                    bN.ZobristKey = ZobristHashing.Hash(bN.GetBoardCharTable());

                    moves.Add(move);
                    scores.Add(Search(bN, depth, alpha, beta, !player));

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

            //if (board.ZobristKey != 0)
            //    if (scores.Count > 0)
            //        StoreTransposition(board, depth, alpha, beta,
            //            player ? scores.Max() : scores.Min()); //there is error with TT which i need to find

            if (depth == 1) //base root
                SetBestMove(scores, moves, player);

            if (player)
            {
                if (scores.Count > 0)
                    return scores.Max();

                return 0;
            }

            if (scores.Count > 0)
                return scores.Min();

            return 0;
        }

        private void SetBestMove(List<double> scores, List<Move> moves, bool player)
        {
            var scoreMax = scores.Max();
            var scoreMin = scores.Min();

            var value = scoreMax;
            if (!player) value = scoreMin;

            BestScore = value;

            if (scores.Contains(int.MaxValue)) //the depth calculation wasn't finished so we need to check if any of the moves are better
            {
                if (player)
                {
                    if (value > BestScore) 
                        BestMove = moves[scores.IndexOf(value)];
                }
                else
                {
                    if (value < BestScore) 
                        BestMove = moves[scores.IndexOf(value)];
                }
            }
            else //the calculation was finished in the depth so we have to select move
            {
                if (scores.Count > 0)
                {
                    BestMove = moves[scores.IndexOf(value)];
                }
            }
        }

        private Transposition Lookup(ulong zobristKey)
        {
            return Transpositions.ContainsKey(zobristKey) ? Transpositions[zobristKey] : null;
        }

        private void StoreTransposition(Board board, int depth, double alpha, double beta, double value)
        {
            if (!Transpositions.ContainsKey(board.ZobristKey))
            {
                var node = new Transposition(value, depth);
                if (value <= alpha)
                    node.Flag = Transposition.Flags.Upper;
                else if (value >= beta)
                    node.Flag = Transposition.Flags.Lower;
                else
                    node.Flag = Transposition.Flags.Exact;
                Transpositions.Add(board.ZobristKey, node);
            }
            else
            {
                var node = Transpositions[board.ZobristKey]; //not sure about the collision override
                if (depth > node.Depth)
                {
                    node.Value = value;
                    node.Depth = depth;

                    if (value <= alpha)
                        node.Flag = Transposition.Flags.Upper;
                    else if (value >= beta)
                        node.Flag = Transposition.Flags.Lower;
                    else
                        node.Flag = Transposition.Flags.Exact;
                }
            }
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
    }
}