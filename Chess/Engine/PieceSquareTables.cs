using Chess.Model;

namespace Chess.Engine
{
    public class PieceSquareTables
    {
        public static readonly double[,] PawnWhitePositionValues = {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {5, 5, 5, 5, 5, 5, 5, 5},
            {1, 1, 2, 3, 3, 2, 1, 1},
            {0.5, 0.5, 1, 2.5, 2.5, 1, 0.5, 0.5},
            {0, 0, 0, 3.5, 3.5, 0, 0, 0}, //2.5 -> 3.5
            {0.5, -0.5, -1, 0, 0, -1, -0.5, 0.5},
            {0.5, 1, 1, -2, -2, 1, 1, 0.5},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };

        public static readonly double[,] PawnBlackPositionValues = {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0.5, 1, 1, -2, -2, 1, 1, 0.5},
            {0.5, -0.5, -1, 0, 0, -1, -0.5, 0.5},
            {0, 0, 0, 3.5, 3.5, 0, 0, 0}, //2.5 -> 3.5
            {0.5, 0.5, 1, 2.5, 2.5, 1, 0.5, 0.5},
            {1, 1, 2, 3, 3, 2, 1, 1},
            {5, 5, 5, 5, 5, 5, 5, 5},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };

        public static readonly double[,] KnightPositionValues = {
            {-5, -4, -3, -3, -3, -3, -4, -5},
            {-4, -2, 0, 0, 0, 0, -2, -4},
            {-3, 0, 1, 1.5, 1.5, 1, 0, -3},
            {-3, 0.5, 1.5, 2, 2, 1.5, 0.5, -3},
            {-3, 0, 1.5, 2, 2, 1.5, 0, -3},
            {-3, 0.5, 1, 1.5, 1.5, 1, 0.5, -3},
            {-4, -2, 0, 0.5, 0.5, 0, -2, -4},
            {-5, -4, -3, -3, -3, -3, -4, -5}
        };

        public static readonly double[,] BishopWhitePositionValues = {
            {-2, -1, -1, -1, -1, -1, -1, -2},
            {-1, 0, 0, 0, 0, 0, 0, -1},
            {-1, 0, 0.5, 1, 1, 0.5, 0, -1},
            {-1, 0.5, 0.5, 1, 1, 0.5, 0.5, -1},
            {-1, 0, 1, 1, 1, 1, 0, -1},
            {-1, 1, 1, 1, 1, 1, 1, -1},
            {-1, 0.5, 0, 0, 0, 0, 0.5, -1},
            {-2, -1, -1, -1, -1, -1, -1, -2}
        };

        public static readonly double[,] BishopBlackPositionValues = {
            {-2, -1, -1, -1, -1, -1, -1, -2},
            {-1, 0.5, 0, 0, 0, 0, 0.5, -1},
            {-1, 1, 1, 1, 1, 1, 1, -1},
            {-1, 0, 1, 1, 1, 1, 0, -1},
            {-1, 0.5, 0.5, 1, 1, 0.5, 0.5, -1},
            {-1, 0, 0.5, 1, 1, 0.5, 0, -1},
            {-1, 0, 0, 0, 0, 0, 0, -1},
            {-2, -1, -1, -1, -1, -1, -1, -2}
        };

        public static readonly double[,] RookWhitePositionValues = {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0.5, 1, 1, 1, 1, 1, 1, 0.5},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {0, 0, 0, 0.5, 0.5, 0, 0, 0}
        };

        public static readonly double[,] RookBlackPositionValues = {
            {0, 0, 0, 0.5, 0.5, 0, 0, 0},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {-0.5, 0, 0, 0, 0, 0, 0, -0.5},
            {0.5, 1, 1, 1, 1, 1, 1, 0.5},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };

        public static readonly double[,] QueenPositionValues = {
            {-2, -1, -1, -0.5, -0.5, -1, -1, -2},
            {-1, 0, 0, 0, 0, 0, 0, -1},
            {-1, 0, 0.5, 0.5, 0.5, 0.5, 0, -1},
            {-0.5, 0, 0.5, 0.5, 0.5, 0.5, 0, -0.5},
            {-0.5, 0, 0.5, 0.5, 0.5, 0.5, 0, -0.5},
            {-1, 0, 0.5, 0.5, 0.5, 0.5, 0, -1},
            {-1, 0, 0, 0, 0, 0, 0, -1},
            {-2, -1, -1, -0.5, -0.5, -1, -1, -2}
        };

        public static readonly double[,] KingWhitePositionValues = {
            {-3, -4, -4, -5, -5, -4, -4, -3},
            {-3, -4, -4, -5, -5, -4, -4, -3},
            {-3, -4, -4, -5, -5, -4, -4, -3},
            {-3, -4, -4, -5, -5, -4, -4, -3},
            {-2, -3, -3, -4, -4, -3, -3, -2},
            {-1, -2, -2, -2, -2, -2, -2, -1},
            {2, 2, 0, 0, 0, 0, 2, 2},
            {2, 3, 1, 0, 0, 1, 3, 2}
        };

        public static readonly double[,] KingBlackPositionValues = {
            {2, 3, 1, 0, 0, 1, 3, 2},
            {2, 2, 0, 0, 0, 0, 2, 2},
            {-1, -2, -2, -2, -2, -2, -2, -1},
            {-2, -3, -3, -4, -4, -3, -3, -2},
            {-3, -4, -4, -5, -5, -4, -4, -3},
            {-3, -4, -4, -5, -5, -4, -4, -3},
            {-3, -4, -4, -5, -5, -4, -4, -3},
            {-3, -4, -4, -5, -5, -4, -4, -3}
        };

        public static double GetValue(Piece piece)
        {
            return piece.NameShort switch
            {
                'P' => piece.Color == "Black"
                    ? -PawnWhitePositionValues[piece.Position.X, piece.Position.Y]
                    : PawnBlackPositionValues[piece.Position.X, piece.Position.Y],
                'R' => piece.Color == "Black"
                    ? -RookWhitePositionValues[piece.Position.X, piece.Position.Y]
                    : RookBlackPositionValues[piece.Position.X, piece.Position.Y],
                'N' => KnightPositionValues[piece.Position.X, piece.Position.Y],
                'B' => piece.Color == "Black"
                    ? -BishopWhitePositionValues[piece.Position.X, piece.Position.Y]
                    : BishopBlackPositionValues[piece.Position.X, piece.Position.Y],
                'Q' => QueenPositionValues[piece.Position.X, piece.Position.Y],
                'K' => piece.Color == "Black"
                    ? -KingWhitePositionValues[piece.Position.X, piece.Position.Y]
                    : KingBlackPositionValues[piece.Position.X, piece.Position.Y],
                _ => 0
            };
        }
    }
}