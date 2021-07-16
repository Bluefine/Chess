using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Extensions;

namespace Chess.Model
{
    public class Move
    {
        public Move(MoveType moveType)
        {
            MoveType = moveType;
        }

        public Move(Move move2)
        {
            MoveType = move2.MoveType;
            Piece = move2.Piece;
            Piece2 = move2.Piece2;
            CastleSide = move2.CastleSide;
            Destination = move2.Destination;
            Hit = move2.Hit;
        }

        public Move(Move move2, Piece pieceClone, Piece piece2Clone)
        {
            MoveType = move2.MoveType;
            Piece = pieceClone;
            Piece2 = piece2Clone;
            CastleSide = move2.CastleSide;
            Destination = move2.Destination;
            Hit = move2.Hit;
        }

        public MoveType MoveType { get; set; }
        public Piece Piece { get; set; }
        public Piece Piece2 { get; set; }
        public bool CastleSide { get; set; }
        public Point Destination { get; set; }
        public Point Hit { get; set; }
        public Piece CapturedPiece { get; set; }
    }
}
