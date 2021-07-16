using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Extensions;

namespace Chess.Model
{
    public class Board
    {
        public Piece[,] GameBoard = new Piece[8, 8];
        public List<Move> MovesHistory;
        public ulong ZobristKey;

        public void New()
        {
            MovesHistory = new List<Move>();
            GameBoard = new Piece[8, 8];
            //pawns
            for (var i = 0; i < 8; i++)
                GameBoard[1, i] = new Piece
                { IsWhite = true, PieceType = PieceType.Pawn, Value = 10, Position = new Point(1, i) };

            //pawns
            for (var i = 0; i < 8; i++)
                GameBoard[6, i] = new Piece
                { IsWhite = false, PieceType = PieceType.Pawn, Value = -10, Position = new Point(6, i) };

            //rooks
            GameBoard[0, 0] = new Piece { IsWhite = true, PieceType = PieceType.Rook, Value = 50, Position = new Point(0, 0) };
            GameBoard[0, 7] = new Piece { IsWhite = true, PieceType = PieceType.Rook, Value = 50, Position = new Point(0, 7) };
            GameBoard[7, 0] = new Piece { IsWhite = false, PieceType = PieceType.Rook, Value = -50, Position = new Point(7, 0) };
            GameBoard[7, 7] = new Piece { IsWhite = false, PieceType = PieceType.Rook, Value = -50, Position = new Point(7, 7) };

            //knight
            GameBoard[0, 1] = new Piece { IsWhite = true, PieceType = PieceType.Knight, Value = 30, Position = new Point(0, 1) };
            GameBoard[0, 6] = new Piece { IsWhite = true, PieceType = PieceType.Knight, Value = 30, Position = new Point(0, 6) };
            GameBoard[7, 1] = new Piece { IsWhite = false, PieceType = PieceType.Knight, Value = -30, Position = new Point(7, 1) };
            GameBoard[7, 6] = new Piece { IsWhite = false, PieceType = PieceType.Knight, Value = -30, Position = new Point(7, 6) };

            //bishop
            GameBoard[0, 2] = new Piece { IsWhite = true, PieceType = PieceType.Bishop, Value = 30, Position = new Point(0, 2) };
            GameBoard[0, 5] = new Piece { IsWhite = true, PieceType = PieceType.Bishop, Value = 30, Position = new Point(0, 5) };
            GameBoard[7, 2] = new Piece { IsWhite = false, PieceType = PieceType.Bishop, Value = -30, Position = new Point(7, 2) };
            GameBoard[7, 5] = new Piece { IsWhite = false, PieceType = PieceType.Bishop, Value = -30, Position = new Point(7, 5) };

            //queen
            GameBoard[0, 3] = new Piece { IsWhite = true, PieceType = PieceType.Queen, Value = 90, Position = new Point(0, 3) };
            GameBoard[7, 3] = new Piece { IsWhite = false, PieceType = PieceType.Queen, Value = -90, Position = new Point(7, 3) };

            //king
            GameBoard[0, 4] = new Piece { IsWhite = true, PieceType = PieceType.King, Value = 0, Position = new Point(0, 4) };
            GameBoard[7, 4] = new Piece { IsWhite = false, PieceType = PieceType.King, Value = 0, Position = new Point(7, 4) };
        }

        public bool IsRepetition(Move bestMove)
        {
            if (MovesHistory.Count > 7)
            {
                var last1 = MovesHistory.ElementAt(MovesHistory.Count - 1);
                var last2 = MovesHistory.ElementAt(MovesHistory.Count - 2);
                var last3 = MovesHistory.ElementAt(MovesHistory.Count - 3);
                var last4 = MovesHistory.ElementAt(MovesHistory.Count - 4);
                var last5 = MovesHistory.ElementAt(MovesHistory.Count - 5);
                var last6 = MovesHistory.ElementAt(MovesHistory.Count - 6);
                var last7 = MovesHistory.ElementAt(MovesHistory.Count - 7);

                if (bestMove.Piece.Position == last4.Piece.Position)
                {
                    if (last2.Piece.Position == last6.Piece.Position)
                    {
                        if (last1.Piece.Position == last5.Piece.Position)
                        {
                            if (last3.Piece.Position == last7.Piece.Position)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public char[,] GetBoardCharTable()
        {
            var table = new char[8, 8];
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = GameBoard[i, j];
                if (piece == null)
                {
                    table[i, j] = '-';
                }
                else
                {
                    if (piece.IsWhite)
                        table[i, j] = Notation.GetChessFigureShort(piece.PieceType);
                    else
                        table[i, j] = char.ToLower(Notation.GetChessFigureShort(piece.PieceType));
                }
            }

            return table;
        }


        public Piece GetPiece(Point point)
        {
            return GameBoard[point.X, point.Y];
        }

        public bool IsCheck(bool white)
        {
            var king = FindKingPiece(white);

            var pieces = GetPiecesByColor(!white);
            foreach (var piece in pieces)
            {
                piece.UpdateInCheckPositions(this);
                if (piece.InCheckPositions.Contains(king.Position))
                    return true;
            }

            return false;
        }

        public bool IsCheckMate(bool white)
        {
            //lets check first king if can do any move to prevent check-mate
            var king = FindKingPiece(white);
            king.FindLegalMoves(this);
            if (king.LegalMoves != null)
                if (king.LegalMoves.Count > 0)
                    return false;

            foreach (var piece in GetPiecesByColor(white))
            {
                if (piece.PieceType == PieceType.King)
                    continue; //we already checked the king before so no point doing it again

                piece.FindLegalMoves(this);
                if (piece.LegalMoves != null)
                    if (piece.LegalMoves.Count > 0)
                        return false;
            }

            return true;
        }


        public List<Piece> GetPiecesByColor(bool v)
        {
            var pieces = new List<Piece>();
            
            for (var i = 0; i < 8; i++) //maybe binary search instead of that crap? or cast it to list then use linq?
            for (var j = 0; j < 8; j++)
            {
                var p = GameBoard[i, j];
                if (p != null)
                {
                    if (p.IsWhite == v)
                    {
                        pieces.Add(p);
                    }
                }
            }

            return pieces;
        }

        public Piece FindKingPiece(bool isWhite)
        {
            for (var i = 0; i < 8; i++) //maybe binary search instead of that crap? or cast it to list then use linq?
            for (var j = 0; j < 8; j++)
            {
                var p = GameBoard[i, j];
                if (p != null)
                {
                    if (p.PieceType == PieceType.King)
                    {
                        if (p.IsWhite == isWhite)
                        {
                            return p;
                        }
                    }
                }
            }

            return null; //should never happen
        }

        public List<Piece> GetAllPieces()
        {
            var pieces = new List<Piece>();
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = GameBoard[i, j];
                if (piece != null) pieces.Add(piece);
            }

            return pieces;
        }
        public bool IsPromotion(Piece piece)
        {
            if (piece.PieceType == PieceType.Pawn)
            {
                if (piece.IsWhite)
                {
                    if (piece.Position.X == 7) 
                        return true;
                }
                else
                {
                    if (piece.Position.X == 0) 
                        return true;
                }
            }


            return false;
        }

        public void MakeMove(Move move)
        {
            var lastMove = new Move(move);
            if (move.Piece != null)
            {
                lastMove.Piece = new Piece(move.Piece);
            }
            if (move.Piece2 != null)
            {
                lastMove.Piece2 = new Piece(move.Piece2);
            }
            
            switch (move.MoveType)
            {
                case MoveType.Normal:
                    {
                        lastMove.CapturedPiece = GameBoard[move.Destination.X, move.Destination.Y];
                        GameBoard[move.Destination.X, move.Destination.Y] = move.Piece;
                        GameBoard[move.Piece.Position.X, move.Piece.Position.Y] = null;
                        move.Piece.Position = move.Destination;
                        move.Piece.MovesCount++;
                        if (IsPromotion(move.Piece))
                        {
                            move.Piece.PieceType = PieceType.Queen;

                            var value = -90;
                            if (move.Piece.IsWhite)
                                value = 90;
                            move.Piece.Value = value;
                        }
                        break;
                    }
                case MoveType.Castle:
                    {
                        var kingDestination = move.CastleSide ? new Point(move.Piece.Position.X, 6) : new Point(move.Piece.Position.X, 2);
                        var rookDestination = move.CastleSide ? new Point(move.Piece2.Position.X, 5) : new Point(move.Piece2.Position.X, 3);

                        //move king
                        GameBoard[kingDestination.X, kingDestination.Y] = move.Piece;
                        GameBoard[move.Piece.Position.X, move.Piece.Position.Y] = null;
                        move.Piece.Position = kingDestination;
                        move.Piece.MovesCount++;

                        //move rook
                        GameBoard[rookDestination.X, rookDestination.Y] = move.Piece2;
                        GameBoard[move.Piece2.Position.X, move.Piece2.Position.Y] = null;
                        move.Piece2.Position = rookDestination;
                        move.Piece2.MovesCount++;

                        break;
                    }
                case MoveType.EnPassant:
                    {
                        //make move on board
                        lastMove.CapturedPiece = GameBoard[move.Hit.X, move.Hit.Y];
                        GameBoard[move.Destination.X, move.Destination.Y] = move.Piece;
                        GameBoard[move.Piece.Position.X, move.Piece.Position.Y] = null;
                        GameBoard[move.Hit.X, move.Hit.Y] = null;
                        move.Piece.Position = move.Destination;
                        move.Piece.MovesCount++;
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            MovesHistory.Add(lastMove);
        }

        public void UndoLastMove(Move move)
        {
            var lastMove = MovesHistory.Last();

            switch (lastMove.MoveType)
            {
                case MoveType.Normal:
                {
                    GameBoard[lastMove.Destination.X, lastMove.Destination.Y] = lastMove.CapturedPiece;
                    GameBoard[lastMove.Piece.Position.X, lastMove.Piece.Position.Y] = lastMove.Piece;
                    move.Piece.Position = lastMove.Piece.Position;
                    move.Piece.MovesCount--;
                    if (lastMove.Piece.PieceType == PieceType.Pawn)
                    {
                        if (lastMove.Piece.Position.X == 6 && lastMove.Piece.IsWhite)
                        {
                            move.Piece.PieceType = PieceType.Pawn;
                            move.Piece.Value = 10;
                        }

                        if (lastMove.Piece.Position.X == 1 && !lastMove.Piece.IsWhite)
                        {
                            move.Piece.PieceType = PieceType.Pawn;
                            move.Piece.Value = -10;
                        }
                    }
                    break;
                }
                case MoveType.Castle:
                {
                    var kingDestination = lastMove.CastleSide ? new Point(lastMove.Piece.Position.X, 6) : new Point(lastMove.Piece.Position.X, 2);
                    var rookDestination = lastMove.CastleSide ? new Point(lastMove.Piece2.Position.X, 5) : new Point(lastMove.Piece2.Position.X, 3);

                    GameBoard[lastMove.Piece.Position.X, lastMove.Piece.Position.Y] = lastMove.Piece;
                    GameBoard[kingDestination.X, kingDestination.Y] = null;
                    GameBoard[lastMove.Piece2.Position.X, lastMove.Piece2.Position.Y] = lastMove.Piece2;
                    GameBoard[rookDestination.X, rookDestination.Y] = null;

                    move.Piece.Position = lastMove.Piece.Position;
                    move.Piece2.Position = lastMove.Piece2.Position;
                    move.Piece.MovesCount--;
                    move.Piece2.MovesCount--;
                    break;
                }
                case MoveType.EnPassant:
                {
                    GameBoard[lastMove.Destination.X, lastMove.Destination.Y] = null;
                    GameBoard[lastMove.Piece.Position.X, lastMove.Piece.Position.Y] = lastMove.Piece;
                    GameBoard[lastMove.Hit.X, lastMove.Hit.Y] = lastMove.CapturedPiece;
                    move.Piece.Position = lastMove.Piece.Position;
                    move.Piece.MovesCount--;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            MovesHistory.RemoveAt(MovesHistory.Count-1);
        }

        public bool IsMoveLegal(Move move)
        {
            MakeMove(move);
            var isSafe = IsCheck(move.Piece.IsWhite);
            UndoLastMove(move);
            return !isSafe;
        }

        public Piece GetPieceByNotation(string pos)
        {
            var x = Notation.GetPositionByLetter(pos[0].ToString());
            var y = Convert.ToInt32(pos[1].ToString()) - 1;
            return GameBoard[y, x];
        }

        public void UpdateBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var p = GameBoard[i, j];
                    if (p != null)
                    {
                        p.UpdateInCheckPositions(this);
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var p = GameBoard[i, j];
                    if (p != null)
                    {
                        p.FindLegalMoves(this);
                    }
                }
            }
        }
    }
}
