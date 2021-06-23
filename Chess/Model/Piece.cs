using System.Collections.Generic;
using System.Linq;

namespace Chess.Model
{
    public class Piece
    {
        public Piece(Piece piece)
        {
            NameShort = piece.NameShort;
            Value = piece.Value;
            Color = piece.Color;
            Position = piece.Position;
            WasMoved = piece.WasMoved;
            MovesDone = piece.MovesDone;
            LegalMoves = piece.LegalMoves;
            HitMoves = piece.HitMoves;
        }

        public Piece(char nameShort, int value, string color, Point position)
        {
            NameShort = nameShort;
            Value = value;
            Color = color;
            Position = position;
        }

        public Piece()
        {
        }

        public char NameShort { get; set; }
        public int Value { get; set; }
        public string Color { get; set; }
        public Point Position { get; set; }
        public bool WasMoved { get; set; }
        public int MovesDone { get; set; }
        public List<Move> LegalMoves { get; set; }
        public List<Point> HitMoves { get; set; }

        public void UpdateHitMoves(Board board)
        {
            var toBeCheck = new List<Move>();
            HitMoves = new List<Point>();
            switch (NameShort)
            {
                case 'P':
                {
                    HitMoves = GetPawnHitPositions();
                    break;
                }

                case 'N':
                {
                    toBeCheck = GetKnightAllMoves().Where(p => !p.To.IsPointOutsideBoard()).ToList();
                    break;
                }
                case 'R':
                {
                    toBeCheck = GetMovesInLine(board);
                    break;
                }
                case 'B':
                {
                    toBeCheck = GetMovesInDiagonal(board);
                    break;
                }
                case 'Q':
                {
                    toBeCheck.AddRange(GetMovesInLine(board)); //combine to make it faster
                    toBeCheck.AddRange(GetMovesInDiagonal(board)); //combine to make it faster
                    break;
                }
                case 'K':
                {
                    toBeCheck = GetKingAllMoves().Where(point => !point.To.IsPointOutsideBoard()).ToList();
                    break;
                }
            }

            foreach (var move in toBeCheck) HitMoves.Add(new Point(move.To.X, move.To.Y));
        }

        public void FindLegalMoves(Board board)
        {
            LegalMoves = new List<Move>();
            var toBeCheck = new List<Move>();
            switch (NameShort)
            {
                case 'P':
                {
                    var moves = GetPawnHitPositions();
                    var temp = -1;
                    if (Color == "White")
                        temp = 1;

                    var temp2 = -2;
                    if (Color == "White")
                        temp2 = 2;

                    var up1 = new Point(Position.X + temp, Position.Y);

                    if (board.Pieces[up1.X, up1.Y] == null) //position of course need to be empty for pawn
                    {
                        var m = new Move {Castle = false, From = Position, Hit = false, To = up1};

                        if (board.ValidMove(m)) LegalMoves.Add(m);

                        if (!WasMoved) //2 places can only be done if the piece wasn't moved
                        {
                            var up2 = new Point(Position.X + temp2, Position.Y);

                            if (board.Pieces[up2.X, up2.Y] == null) //position of course need to be empty for pawn
                            {
                                var m2 = new Move {Castle = false, From = Position, Hit = false, To = up2};
                                if (board.ValidMove(m2)) LegalMoves.Add(m2);
                            }
                        }
                    }

                    //todo add el-passant

                    foreach (var point in moves)
                    {
                        var p = board.Pieces[point.X, point.Y];
                        if (p != null) //we need piece to perform diagonal move
                        {
                            if (p.Color != Color)
                                if (p.NameShort != 'K')
                                {
                                    var move = new Move {Castle = false, From = Position, Hit = true, To = point};
                                    if (board.ValidMove(move)) LegalMoves.Add(move);
                                }
                        }
                    }

                    break;
                }

                case 'N':
                {
                    toBeCheck = GetKnightAllMoves().Where(p => !p.To.IsPointOutsideBoard()).ToList();
                    break;
                }
                case 'R':
                {
                    toBeCheck = GetMovesInLine(board);
                    break;
                }
                case 'B':
                {
                    toBeCheck = GetMovesInDiagonal(board);
                    break;
                }
                case 'Q':
                {
                    toBeCheck.AddRange(GetMovesInLine(board)); //combine to make it faster
                    toBeCheck.AddRange(GetMovesInDiagonal(board)); //combine to make it faster
                    break;
                }
                case 'K':
                {
                    toBeCheck = GetKingAllMoves().Where(point => !point.To.IsPointOutsideBoard()).ToList();
                    if (!board.IsCheck(Color)) //can't castle under check
                        toBeCheck.AddRange(GetKingCastling(board));

                    break;
                }
            }

            if (NameShort != 'P')
                foreach (var move in toBeCheck)
                {
                    //HitMoves.Add(new Point(move.To.X, move.To.Y));

                    var p = board.Pieces[move.To.X, move.To.Y];
                    if (p != null) //hit
                        if (p.Color == Color || p.NameShort == 'K')
                            continue;

                    if (board.ValidMove(move)) LegalMoves.Add(move);
                }
        }

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
                    var move = new Move {From = Position, To = p, Hit = true};
                    moves.Add(move);
                }
                else
                {
                    var move = new Move {From = Position, To = p, Hit = true};
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
                    var move = new Move {From = Position, To = p, Hit = true};
                    moves.Add(move);
                }
                else
                {
                    var move = new Move {From = Position, To = p, Hit = true};
                    moves.Add(move);
                    break;
                }
            }

            return moves;
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
            pointsAroundKing.Add(new Move {From = Position, Hit = true, To = p1});
            pointsAroundKing.Add(new Move {From = Position, Hit = true, To = p2});
            pointsAroundKing.Add(new Move {From = Position, Hit = true, To = p3});
            pointsAroundKing.Add(new Move {From = Position, Hit = true, To = p4});
            pointsAroundKing.Add(new Move {From = Position, Hit = true, To = p5});
            pointsAroundKing.Add(new Move {From = Position, Hit = true, To = p6});
            pointsAroundKing.Add(new Move {From = Position, Hit = true, To = p7});
            pointsAroundKing.Add(new Move {From = Position, Hit = true, To = p8});

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
                    moves.Add(new Move {Castle = true, From = Position, To = new Point(side, 6)});

            //queenside
            var d = board.Pieces[side, 3];
            var c = board.Pieces[side, 2];
            var b = board.Pieces[side, 1];
            var a = board.Pieces[side, 0];

            if (d == null && c == null && b == null && a != null)
                if (!WasMoved && !a.WasMoved)
                    moves.Add(new Move
                        {Castle = true, From = Position, To = new Point(side, 2), CastleQueenSide = true});

            return moves;
        }

        private IEnumerable<Move> GetKnightAllMoves()
        {
            var pointsToCheck = new List<Move>();

            var leftUp = new Point(Position.X + 1, Position.Y - 2);
            pointsToCheck.Add(new Move {From = Position, To = leftUp, Hit = true});

            var leftDown = new Point(Position.X - 1, Position.Y - 2);
            pointsToCheck.Add(new Move {From = Position, To = leftDown, Hit = true});

            var upLeft = new Point(Position.X + 2, Position.Y - 1);
            pointsToCheck.Add(new Move {From = Position, To = upLeft, Hit = true});

            var upRight = new Point(Position.X + 2, Position.Y + 1);
            pointsToCheck.Add(new Move {From = Position, To = upRight, Hit = true});

            var rightUp = new Point(Position.X + 1, Position.Y + 2);
            pointsToCheck.Add(new Move {From = Position, To = rightUp, Hit = true});

            var rightDown = new Point(Position.X - 1, Position.Y + 2);
            pointsToCheck.Add(new Move {From = Position, To = rightDown, Hit = true});

            var downLeft = new Point(Position.X - 2, Position.Y - 1);
            pointsToCheck.Add(new Move {From = Position, To = downLeft, Hit = true});

            var downRight = new Point(Position.X - 2, Position.Y + 1);
            pointsToCheck.Add(new Move {From = Position, To = downRight, Hit = true});

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