using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Extensions;

namespace Chess.Model
{
    public class Piece
    {
        public Piece(Piece piece)
        {
            PieceType = piece.PieceType;
            IsWhite = piece.IsWhite;
            Position = piece.Position;
            MovesCount = piece.MovesCount;
            Value = piece.Value;
            InCheckPositions = piece.InCheckPositions;
            LegalMoves = piece.LegalMoves;
        }

        public Piece()
        {
        }


        public List<Point> InCheckPositions { get; set; }
        public List<Move> LegalMoves { get; set; }
        public PieceType PieceType { get; set; }
        public bool IsWhite { get; set; }
        public Point Position { get; set; }
        public int MovesCount { get; set; }
        public double Value { get; set; }

        public void UpdateInCheckPositions(Board board3)
        {
            InCheckPositions = new List<Point>();

            switch (PieceType)
            {
                case PieceType.Pawn:
                {
                    InCheckPositions = GetPawnHit();
                    break;
                }
                case PieceType.King:
                {
                    InCheckPositions = GetKingHit();
                    break;
                }
                case PieceType.Queen:
                {
                    InCheckPositions.AddRange(GetMovesDiagonal(board3));
                    InCheckPositions.AddRange(GetMovesAxis(board3));
                    break;
                }
                case PieceType.Bishop:
                {
                    InCheckPositions = GetMovesDiagonal(board3);
                    break;
                }
                case PieceType.Knight:
                {
                    InCheckPositions = GetKnightHit();
                    break;
                }
                case PieceType.Rook:
                {
                    InCheckPositions = GetMovesAxis(board3);
                    break;
                }
            }
        }

        public void FindLegalMoves(Board board3)
        {
            LegalMoves = new List<Move>();
            switch (PieceType)
            {
                case PieceType.Pawn:
                {
                    FindPawnLegalMoves(board3);
                    break;
                }
                case PieceType.King:
                {
                    CheckPoints(GetKingHit(), board3);
                    if (!board3.IsCheck(IsWhite)) //cant castle under check
                    {
                        FindKingCastles(board3);
                    }
                    break;
                }
                case PieceType.Queen:
                {
                    CheckPoints(GetMovesDiagonal(board3), board3);
                    CheckPoints(GetMovesAxis(board3), board3);
                    break;
                }
                case PieceType.Bishop:
                {
                    CheckPoints(GetMovesDiagonal(board3), board3);
                    break;
                }
                case PieceType.Knight:
                {
                    CheckPoints(GetKnightHit(), board3);
                    break;
                }
                case PieceType.Rook:
                {
                    CheckPoints(GetMovesAxis(board3), board3);
                    break;
                }
            }
        }

        private void FindKingCastles(Board board3)
        {
            var castles = new List<Move>();

            var side = 7;
            if (IsWhite) 
                side = 0;

            var f = board3.GameBoard[side, 5];
            var g = board3.GameBoard[side, 6];
            var h = board3.GameBoard[side, 7];

            if (f == null && g == null && h != null)
                if (MovesCount == 0 && h.MovesCount == 0 && h.PieceType == PieceType.Rook)
                {
                    var castle = new Move(MoveType.Castle) {Piece = this, CastleSide = true, Piece2 = h, Destination = new Point(Position.X,6)};
                    castles.Add(castle);
                }

            var d = board3.GameBoard[side, 3];
            var c = board3.GameBoard[side, 2];
            var b = board3.GameBoard[side, 1];
            var a = board3.GameBoard[side, 0];

            if (d == null && c == null && b == null && a != null)
                if (MovesCount == 0 && a.MovesCount == 0 && a.PieceType == PieceType.Rook)
                {
                    var castle = new Move(MoveType.Castle) { Piece = this, CastleSide = false, Piece2 = a, Destination = new Point(Position.X, 2) };
                    castles.Add(castle);
                }

            foreach (var castle in castles)
            {
                if (board3.IsMoveLegal(castle))
                {
                    LegalMoves.Add(castle);
                }
            }
        }

        private void CheckPoints(List<Point> points, Board board3)
        {
            foreach (var pos in points)
            {
                var p = board3.GetPiece(pos);
                if (p != null)
                    if (p.IsWhite == IsWhite || p.PieceType == PieceType.King)
                        continue;

                var move = new Move(MoveType.Normal) { Piece = this, Destination = pos };

                if (board3.IsMoveLegal(move))
                {
                    LegalMoves.Add(move);
                }
            }
        }

        private void FindPawnLegalMoves(Board board3)
        {
            var moves = GetPawnHit();
            var temp = -1;
            if (IsWhite)
                temp = 1;

            var temp2 = -2;
            if (IsWhite)
                temp2 = 2;

            var up1 = new Point(Position.X + temp, Position.Y);
            if (board3.GameBoard[up1.X, up1.Y] == null) //position of course need to be empty for pawn
            {
                var move = new Move(MoveType.Normal) { Piece = this, Destination = up1 };

                if (board3.IsMoveLegal(move))
                {
                    LegalMoves.Add(move);
                }

                if (MovesCount == 0) //2 places can only be done if the piece wasn't moved
                {
                    var up2 = new Point(Position.X + temp2, Position.Y);

                    if (board3.GameBoard[up2.X, up2.Y] == null)
                    {
                        var move2 = new Move(MoveType.Normal) { Piece = this, Destination = up2 };

                        if (board3.IsMoveLegal(move2))
                        {
                            LegalMoves.Add(move2);
                        }
                    }
                }
            }

            foreach (var point in moves)
            {
                var p = board3.GetPiece(point);
                if (p != null) //we need piece to perform diagonal move
                {
                    if (p.PieceType != PieceType.King && p.IsWhite != IsWhite)
                    {
                        var move = new Move(MoveType.Normal) { Piece = this, Destination = point };

                        if (board3.IsMoveLegal(move))
                        {
                            LegalMoves.Add(move);
                        }
                    }
                }
                else
                {
                    var pos = new Point(point.X + -temp, point.Y);
                    var pieceUnder = board3.GetPiece(pos);
                    if (pieceUnder != null)
                    {
                        if (pieceUnder.PieceType == PieceType.Pawn)
                        {
                            if (pieceUnder.MovesCount == 1)
                            {
                                if (pieceUnder.IsWhite != IsWhite)
                                {
                                    var lastMove = board3.MovesHistory.Last();
                                    if (lastMove.Destination == pieceUnder.Position)
                                    {
                                        if (pieceUnder.IsWhite)
                                        {
                                            if (pieceUnder.Position.X == 3)
                                            {
                                                var move = new Move(MoveType.EnPassant) { Piece = this, Destination = point, Hit = pos };

                                                if (board3.IsMoveLegal(move))
                                                {
                                                    LegalMoves.Add(move);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (pieceUnder.Position.X == 4)
                                            {
                                                var move = new Move(MoveType.EnPassant) { Piece = this, Destination = point, Hit = pos };

                                                if (board3.IsMoveLegal(move))
                                                {
                                                    LegalMoves.Add(move);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private List<Point> GetMovesAxis(Board board) //i guess it can be combined the axis and diagonal
        {
            var points = new List<Point>();

            for (var i = 0; i < 4; i++)
                for (var j = 1; j < 8; j++)
                {
                    var p = new Point();
                    switch (i)
                    {
                        case 0:
                            p = new Point(Position.X + j, Position.Y);
                            break;
                        case 1:
                            p = new Point(Position.X - j, Position.Y);
                            break;
                        case 2:
                            p = new Point(Position.X, Position.Y - j);
                            break;
                        case 3:
                            p = new Point(Position.X, Position.Y + j);
                            break;
                    }

                    if (p.IsPointOutsideBoard()) break;

                    var piece = board.GetPiece(p);
                    if (piece == null)
                    {
                        points.Add(p);
                    }
                    else
                    {
                        points.Add(p);
                        break;
                    }
                }

            return points;
        }

        private List<Point> GetMovesDiagonal(Board board)
        {
            var points = new List<Point>();

            for (var i = 0; i < 4; i++)
                for (var j = 1; j < 8; j++)
                {
                    var p = new Point();
                    switch (i)
                    {
                        case 0:
                            p = new Point(Position.X + j, Position.Y - j);
                            break;
                        case 1:
                            p = new Point(Position.X - j, Position.Y - j);
                            break;
                        case 2:
                            p = new Point(Position.X + j, Position.Y + j);
                            break;
                        case 3:
                            p = new Point(Position.X - j, Position.Y + j);
                            break;
                    }

                    if (p.IsPointOutsideBoard()) break;

                    var piece = board.GetPiece(p);
                    if (piece == null)
                    {
                        points.Add(p);
                    }
                    else
                    {
                        points.Add(p);
                        break;
                    }
                }

            return points;
        }

        private List<Point> GetPawnHit()
        {
            var moves = new List<Point>();

            var temp = -1;
            if (IsWhite)
                temp = 1;

            var pLeft = new Point(Position.X + temp, Position.Y - 1);
            if (!pLeft.IsPointOutsideBoard())
                moves.Add(pLeft);

            var pRight = new Point(Position.X + temp, Position.Y + 1);
            if (!pRight.IsPointOutsideBoard())
                moves.Add(pRight);

            return moves;
        }

        private List<Point> GetKingHit()
        {
            var points = new List<Point>
            {
                new (Position.X + 1, Position.Y - 1),
                new (Position.X + 1, Position.Y),
                new (Position.X + 1, Position.Y + 1),
                new (Position.X, Position.Y + 1),
                new (Position.X - 1, Position.Y + 1),
                new (Position.X - 1, Position.Y),
                new (Position.X - 1, Position.Y - 1),
                new (Position.X, Position.Y - 1)
            };

            return points.Where(point => !point.IsPointOutsideBoard()).ToList();
        }

        private List<Point> GetKnightHit()
        {
            var points = new List<Point>
            {
                new(Position.X + 1, Position.Y - 2),
                new(Position.X - 1, Position.Y - 2),
                new(Position.X + 2, Position.Y - 1),
                new(Position.X + 2, Position.Y + 1),
                new(Position.X + 1, Position.Y + 2),
                new(Position.X - 1, Position.Y + 2),
                new(Position.X - 2, Position.Y - 1),
                new(Position.X - 2, Position.Y + 1)
            };

            return points.Where(point => !point.IsPointOutsideBoard()).ToList();
        }

    }
}
