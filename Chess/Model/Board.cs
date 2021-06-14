using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Extensions;

namespace Chess.Model
{
    public class Board
    {
        public List<Move> MovesHistory = new();
        public Piece[,] Pieces;
        public ulong ZobristKey;

        public void New()
        {
            Pieces = new Piece[8, 8];
            //pawns
            for (var i = 0; i < 8; i++) Pieces[1, i] = new Piece("Pawn", 'P', 10, "White", new Point(1, i));

            //pawns
            for (var i = 0; i < 8; i++) Pieces[6, i] = new Piece("Pawn", 'P', -10, "Black", new Point(6, i));

            //rooks
            Pieces[0, 0] = new Piece("Rook", 'R', 50, "White", new Point(0, 0));
            Pieces[0, 7] = new Piece("Rook", 'R', 50, "White", new Point(0, 7));
            Pieces[7, 0] = new Piece("Rook", 'R', -50, "Black", new Point(7, 0));
            Pieces[7, 7] = new Piece("Rook", 'R', -50, "Black", new Point(7, 7));

            //knight
            Pieces[0, 1] = new Piece("Knight", 'N', 30, "White", new Point(0, 1));
            Pieces[0, 6] = new Piece("Knight", 'N', 30, "White", new Point(0, 6));
            Pieces[7, 1] = new Piece("Knight", 'N', -30, "Black", new Point(7, 1));
            Pieces[7, 6] = new Piece("Knight", 'N', -30, "Black", new Point(7, 6));

            //bishop
            Pieces[0, 2] = new Piece("Bishop", 'B', 30, "White", new Point(0, 2));
            Pieces[0, 5] = new Piece("Bishop", 'B', 30, "White", new Point(0, 5));
            Pieces[7, 2] = new Piece("Bishop", 'B', -30, "Black", new Point(7, 2));
            Pieces[7, 5] = new Piece("Bishop", 'B', -30, "Black", new Point(7, 5));

            //queen
            Pieces[0, 3] = new Piece("Queen", 'Q', 90, "White", new Point(0, 3));
            Pieces[7, 3] = new Piece("Queen", 'Q', -90, "Black", new Point(7, 3));

            //king
            Pieces[0, 4] = new Piece("King", 'K', 0, "White", new Point(0, 4));
            Pieces[7, 4] = new Piece("King", 'K', 0, "Black", new Point(7, 4));
        }

        public char[,] GetBoardCharTable()
        {
            var table = new char[8, 8];
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = Pieces[i, j];
                if (piece == null)
                {
                    table[i, j] = '-';
                }
                else
                {
                    if (piece.Color == "White")
                        table[i, j] = piece.NameShort;
                    else
                        table[i, j] = char.ToLower(piece.NameShort);
                }
            }

            return table;
        }

        public Piece GetPiece(Point point)
        {
            return Pieces[point.X, point.Y];
        }

        public Piece GetPieceByNotation(string pos)
        {
            var x = Notation.GetPositionByLetter(pos[0].ToString());
            var y = Convert.ToInt32(pos[1].ToString()) - 1;
            return Pieces[y, x];
        }

        public Piece FindKingLocation(string color)
        {
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = Pieces[i, j];
                if (piece != null)
                    if (piece.NameShort == 'K' && piece.Color == color)
                        return piece;
            }

            return null;
        }

        public bool SafeMove(List<Tuple<Piece, Point>> moves)
        {
            var king = FindKingLocation(moves.First().Item1.Color);

            if (king == null) return false;

            var color = "White";
            if (king.Color == "White")
                color = "Black";

            var old = moves.Select(tuple =>
                new Tuple<Piece, Point, Piece, Point>(Pieces[tuple.Item1.Position.X, tuple.Item1.Position.Y],
                    Pieces[tuple.Item1.Position.X, tuple.Item1.Position.Y]?.Position,
                    Pieces[tuple.Item2.X, tuple.Item2.Y], tuple.Item2)).ToList();

            foreach (var move in moves) MovePiece(move.Item1, move.Item2);

            var changedPieces = new List<Piece>();
            var notSafe = false;
            foreach (var p in GetPiecesByColor(color))
            {
                if (p.NameShort == 'P')
                    if (p.Position.Distance(king.Position) > 1.5)
                        continue;

                if (p.NameShort == 'N')
                    if (p.Position.Distance(king.Position) > 3)
                        continue;

                if (p.NameShort == 'K')
                    if (p.Position.Distance(king.Position) > 1.5)
                        continue;

                p.Update(this, true);
                changedPieces.Add(p);
                if (p.CheckPositions.Any(x => x == king.Position))
                {
                    notSafe = true;
                    break;
                }
            }

            foreach (var tuple in old)
            {
                MovePiece(tuple.Item1, tuple.Item2);
                Pieces[tuple.Item4.X, tuple.Item4.Y] = tuple.Item3;
            }

            foreach (var changedPiece in changedPieces) changedPiece.Update(this, true);

            if (!notSafe) return true;

            return false;
        }

        public bool SafeMove(Tuple<Piece, Point> move)
        {
            return SafeMove(new List<Tuple<Piece, Point>> {move});
        }

        private List<Point> GetAllCheckPositions(string color)
        {
            var check = new List<Point>();

            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = Pieces[i, j];
                if (piece != null)
                    if (piece.Color == color)
                        check.AddRange(piece.CheckPositions);
            }

            return check;
        }

        private void MovePiece(Piece piece, Point to, bool final = false)
        {
            if (final)
            {
                piece.WasMoved = true;
                piece.MovesDone++;
                MovesHistory.Add(new Move {From = new Point(piece.Position.X, piece.Position.Y), To = to});
                if (IsPromotion(piece, to))
                {
                    var value = 90;
                    if (piece.Color == "Black") value = -90;

                    Promotion(piece, "Queen", 'Q', value); //undo promo? 
                }
            }

            Pieces[piece.Position.X, piece.Position.Y] = null;
            Pieces[to.X, to.Y] = piece;
            piece.Position = to;
        }

        public void MovePiece(Move move, bool final = false)
        {
            var piece = Pieces[move.From.X, move.From.Y];
            if (move.Castle)
                MovePieceCastle(piece, move.To, final);
            else
                MovePiece(piece, move.To, final);
        }

        public void MovePieceNewInstance(Piece piece, Move move, bool final = false)
        {
            var pieceNew = new Piece(piece);
            if (move.Castle)
                MovePieceCastle(pieceNew, move.To, final);
            else
                MovePiece(pieceNew, move.To, final);
        }

        private void MovePieceCastle(Piece piece, Point to, bool final = false)
        {
            Piece rook = null;
            Point rookTo = null;
            if (to.Y == 6)
            {
                rook = GetPiece(new Point(to.X, to.Y + 1));
                rookTo = new Point(to.X, to.Y - 1);
            }
            else if (to.Y == 2)
            {
                rook = GetPiece(new Point(to.X, 0));
                rookTo = new Point(to.X, to.Y + 1);
            }

            MovePiece(piece, to, final);
            MovePiece(rook, rookTo, true);
        }

        public bool IsPromotion(Piece piece, Point to)
        {
            if (piece.NameShort == 'P')
            {
                if (piece.Color == "White")
                {
                    if (to.X == 7) return true;
                }
                else
                {
                    if (to.X == 0) return true;
                }
            }


            return false;
        }

        public void Promotion(Piece piece, string name, char nameShort, int value)
        {
            piece.Name = name;
            piece.NameShort = nameShort;
            piece.Value = value;
        }

        public void Update(string color)
        {
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = Pieces[i, j];
                if (piece != null)
                    if (piece.Color == color)
                        piece.Update(this);
            }
        }

        public bool IsCheck(string color)
        {
            var c = string.Empty;
            if (color == "Black")
                c = "White";
            else
                c = "Black";
            var king = FindKingLocation(color);
            var checkPos = GetAllCheckPositions(c);
            if (checkPos.Any(x => x == king.Position)) return true;
            return false;
        }

        public bool IsCheckMate(string color)
        {
            var moves = new List<Move>();
            foreach (var piece in GetPiecesByColor(color)) moves.AddRange(piece.PossibleMoves);

            if (moves.Any()) return false;

            var check = IsCheck(color);
            if (check) return true;
            return false;
        }

        public List<Piece> GetPiecesByColor(string color)
        {
            var pieces = new List<Piece>();
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = Pieces[i, j];
                if (piece != null)
                    if (piece.Color == color)
                        pieces.Add(piece);
            }

            return pieces;
        }

        public List<Piece> GetAllPieces()
        {
            var pieces = new List<Piece>();
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = Pieces[i, j];
                if (piece != null) pieces.Add(piece);
            }

            return pieces;
        }
    }
}