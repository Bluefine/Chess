using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Model
{
    public class Piece
    {
        public Piece(Piece piece)
        {
            Name = piece.Name;
            NameShort = piece.NameShort;
            Value = piece.Value;
            Color = piece.Color;
            PossibleMoves = piece.PossibleMoves;
            CheckPositions = piece.CheckPositions;
            Position = piece.Position;
            WasMoved = piece.WasMoved;
            MovesDone = piece.MovesDone;
        }

        public Piece(string name, char nameShort, int value, string color, Point position)
        {
            Name = name;
            NameShort = nameShort;
            Value = value;
            Color = color;
            Position = position;
        }

        public Piece()
        {
        }

        public string Name { get; set; }
        public char NameShort { get; set; }
        public int Value { get; set; }
        public string Color { get; set; }
        public List<Move> PossibleMoves { get; set; } = new();
        public List<Point> CheckPositions { get; set; } = new();
        public Point Position { get; set; }
        public bool WasMoved { get; set; }
        public int MovesDone { get; set; }

        public List<Move> GetMovesInLine(Board board)
        {
            var moves = new List<Move>();

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
                    var move = new Move {From = Position, To = p, Hit = p};
                    moves.Add(move);
                }
                else
                {
                    var move = new Move {From = Position, To = p, Hit = p};
                    moves.Add(move);
                    break;
                }
            }

            return moves;
        }

        public List<Move> GetMovesInDiagonal(Board board) //combine it with the one before
        {
            var moves = new List<Move>();

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
                    var move = new Move {From = Position, To = p, Hit = p};
                    moves.Add(move);
                }
                else
                {
                    var move = new Move {From = Position, To = p, Hit = p};
                    moves.Add(move);
                    break;
                }
            }

            return moves;
        }

        public List<Move> SelectGoodMoves(List<Move> moves, Board board)
        {
            var good = new List<Move>();

            foreach (var p in moves)
            {
                var piece = board.GetPiece(p.To);
                if (piece == null)
                {
                    if (board.SafeMove(new Tuple<Piece, Point>(this, p.To))) good.Add(p);
                }
                else
                {
                    if (piece.Color != Color && piece.NameShort != 'K')
                        if (board.SafeMove(new Tuple<Piece, Point>(this, p.To)))
                            good.Add(p);
                }
            }

            return good;
        }

        public void Update(Board board, bool onlyCheck = false)
        {
            PossibleMoves = new List<Move>();
            CheckPositions = new List<Point>();
            var moves = new List<Move>();
            switch (NameShort)
            {
                case 'P':
                {
                    CheckPositions = GetPawnHitPositions();
                    if (onlyCheck)
                        break;
                    var temp = -1;
                    if (Color == "White")
                        temp = 1;

                    var temp2 = -2;
                    if (Color == "White")
                        temp2 = 2;

                    var up1 = new Point(Position.X + temp, Position.Y);

                    if (board.GetPiece(up1) == null)
                    {
                        if (board.SafeMove(new Tuple<Piece, Point>(this, up1)))
                        {
                            var move = new Move {From = Position, To = up1, Hit = null};
                            PossibleMoves.Add(move);
                        }

                        if (!WasMoved)
                        {
                            var up2 = new Point(Position.X + temp2, Position.Y);

                            if (board.GetPiece(up2) == null)
                                if (board.SafeMove(new Tuple<Piece, Point>(this, up2)))
                                {
                                    var move = new Move {From = Position, To = up2, Hit = null};
                                    PossibleMoves.Add(move);
                                }
                        }
                    }

                    foreach (var move in from point in CheckPositions
                        where !point.IsPointOutsideBoard()
                        let p = board.GetPiece(point)
                        where p != null && p.Color != Color && p.NameShort != 'K'
                        where board.SafeMove(new Tuple<Piece, Point>(this, point))
                        select new Move {From = Position, To = point, Hit = point})
                        PossibleMoves.Add(move);

                    break;
                }
                case 'N':
                {
                    moves = GetKnightAllMoves().Where(p => !p.To.IsPointOutsideBoard()).ToList();
                    break;
                }
                case 'R':
                {
                    moves = GetMovesInLine(board);
                    break;
                }
                case 'B':
                {
                    moves = GetMovesInDiagonal(board);
                    break;
                }
                case 'Q':
                {
                    moves.AddRange(GetMovesInLine(board)); //combine to make it faster
                    moves.AddRange(GetMovesInDiagonal(board)); //combine to make it faster
                    break;
                }
                case 'K':
                {
                    moves = GetKingAllMoves().Where(point => !point.To.IsPointOutsideBoard()).ToList();
                    foreach (var move in moves)
                        CheckPositions.Add(move.To);
                    if (onlyCheck) break;
                    var enemyKing = board.FindKingLocation(ChangeColor(Color));
                    var byDist = (from kingMove in moves
                        let dist = kingMove.To.Distance(enemyKing.Position)
                        where dist > 1.5
                        select kingMove).ToList();

                    if (!board.IsCheck(Color))
                    {
                        var castling = GetKingCastling(board);
                        var good = SelectGoodMoves(castling, board); //to be check
                        foreach (var move in good) PossibleMoves.Add(move);
                    }
                    var good2 = SelectGoodMoves(byDist, board);
                    foreach (var move in good2) PossibleMoves.Add(move);
                    break;
                }
            }

            if (PossibleMoves.Count == 0 && NameShort != 'K')
            {
                foreach (var move in moves)
                    CheckPositions.Add(move.To);
                if (onlyCheck) return;
                var good = SelectGoodMoves(moves, board);
                foreach (var move in good) PossibleMoves.Add(move);
            }
        }

        private string ChangeColor(string color)
        {
            return color == "White" ? "Black" : "White";
        }

        private IEnumerable<Move> GetKingAllMoves() //can be done better?
        {
            var pointsAroundKing = new List<Move>();

            var p1 = new Point(Position.X + 1, Position.Y - 1); //leftUp
            var p2 = new Point(Position.X + 1, Position.Y); //up
            var p3 = new Point(Position.X + 1, Position.Y + 1); //rightUp
            var p4 = new Point(Position.X, Position.Y + 1); //right
            var p5 = new Point(Position.X - 1, Position.Y + 1); //rightDown
            var p6 = new Point(Position.X - 1, Position.Y); //down
            var p7 = new Point(Position.X - 1, Position.Y - 1); //downLeft
            var p8 = new Point(Position.X, Position.Y - 1); //Left
            pointsAroundKing.Add(new Move {From = Position, Hit = p1, To = p1});
            pointsAroundKing.Add(new Move {From = Position, Hit = p2, To = p2});
            pointsAroundKing.Add(new Move {From = Position, Hit = p3, To = p3});
            pointsAroundKing.Add(new Move {From = Position, Hit = p4, To = p4});
            pointsAroundKing.Add(new Move {From = Position, Hit = p5, To = p5});
            pointsAroundKing.Add(new Move {From = Position, Hit = p6, To = p6});
            pointsAroundKing.Add(new Move {From = Position, Hit = p7, To = p7});
            pointsAroundKing.Add(new Move {From = Position, Hit = p8, To = p8});

            return pointsAroundKing;
        }

        private List<Move> GetKingCastling(Board board)
        {
            var moves = new List<Move>();

            var side = 0;
            if (Color == "Black") side = 7;

            //kingside
            var f = board.Pieces[side, 5];
            var g = board.Pieces[side, 6];
            var h = board.Pieces[side, 7];

            if (f == null && g == null && h != null)
                if (!WasMoved && !h.WasMoved)
                {
                    var toCheck = new List<Tuple<Piece, Point>>
                    {
                        new(this, new Point(side, 6)),
                        new(h, new Point(side, 5))
                    };
                    if (board.SafeMove(toCheck))
                    {
                        var m = new Move {Castle = true, From = Position, To = new Point(side, 6)};
                        moves.Add(m);
                    }
                }

            //queenside
            var d = board.Pieces[side, 3];
            var c = board.Pieces[side, 2];
            var b = board.Pieces[side, 1];
            var a = board.Pieces[side, 0];

            if (d == null && c == null && b == null && a != null)
                if (!WasMoved && !a.WasMoved)
                {
                    var toCheck = new List<Tuple<Piece, Point>>
                    {
                        new(this, new Point(side, 2)),
                        new(a, new Point(side, 3))
                    };
                    if (board.SafeMove(toCheck))
                    {
                        var m = new Move {Castle = true, From = Position, To = new Point(side, 2)};
                        moves.Add(m);
                    }
                }

            return moves;
        }

        private IEnumerable<Move> GetKnightAllMoves()
        {
            var pointsToCheck = new List<Move>();

            var leftUp = new Point(Position.X + 1, Position.Y - 2);
            pointsToCheck.Add(new Move {From = Position, To = leftUp, Hit = leftUp});

            var leftDown = new Point(Position.X - 1, Position.Y - 2);
            pointsToCheck.Add(new Move {From = Position, To = leftDown, Hit = leftDown});

            var upLeft = new Point(Position.X + 2, Position.Y - 1);
            pointsToCheck.Add(new Move {From = Position, To = upLeft, Hit = upLeft});

            var upRight = new Point(Position.X + 2, Position.Y + 1);
            pointsToCheck.Add(new Move {From = Position, To = upRight, Hit = upRight});

            var rightUp = new Point(Position.X + 1, Position.Y + 2);
            pointsToCheck.Add(new Move {From = Position, To = rightUp, Hit = rightUp});

            var rightDown = new Point(Position.X - 1, Position.Y + 2);
            pointsToCheck.Add(new Move {From = Position, To = rightDown, Hit = rightDown});

            var downLeft = new Point(Position.X - 2, Position.Y - 1);
            pointsToCheck.Add(new Move {From = Position, To = downLeft, Hit = downLeft});

            var downRight = new Point(Position.X - 2, Position.Y + 1);
            pointsToCheck.Add(new Move {From = Position, To = downRight, Hit = downRight});

            return pointsToCheck;
        }

        private List<Point> GetPawnHitPositions()
        {
            var moves = new List<Point>();

            var temp = -1;
            if (Color == "White")
                temp = 1;

            var pLeft = new Point(Position.X + temp, Position.Y - 1);
            if (!pLeft.IsPointOutsideBoard())
                moves.Add(pLeft);

            var pRight = new Point(Position.X + temp, Position.Y + 1);
            if (!pRight.IsPointOutsideBoard())
                moves.Add(pRight);

            return moves;
        }
    }
}