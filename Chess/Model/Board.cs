﻿using System;
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
            for (var i = 0; i < 8; i++) Pieces[1, i] = new Piece('P', 10, "White", new Point(1, i));

            //pawns
            for (var i = 0; i < 8; i++) Pieces[6, i] = new Piece('P', -10, "Black", new Point(6, i));

            //rooks
            Pieces[0, 0] = new Piece('R', 50, "White", new Point(0, 0));
            Pieces[0, 7] = new Piece('R', 50, "White", new Point(0, 7));
            Pieces[7, 0] = new Piece('R', -50, "Black", new Point(7, 0));
            Pieces[7, 7] = new Piece('R', -50, "Black", new Point(7, 7));

            //knight
            Pieces[0, 1] = new Piece('N', 30, "White", new Point(0, 1));
            Pieces[0, 6] = new Piece('N', 30, "White", new Point(0, 6));
            Pieces[7, 1] = new Piece('N', -30, "Black", new Point(7, 1));
            Pieces[7, 6] = new Piece('N', -30, "Black", new Point(7, 6));

            //bishop
            Pieces[0, 2] = new Piece('B', 30, "White", new Point(0, 2));
            Pieces[0, 5] = new Piece('B', 30, "White", new Point(0, 5));
            Pieces[7, 2] = new Piece('B', -30, "Black", new Point(7, 2));
            Pieces[7, 5] = new Piece('B', -30, "Black", new Point(7, 5));

            //queen
            Pieces[0, 3] = new Piece('Q', 90, "White", new Point(0, 3));
            Pieces[7, 3] = new Piece('Q', -90, "Black", new Point(7, 3));

            //king
            Pieces[0, 4] = new Piece('K', 0, "White", new Point(0, 4));
            Pieces[7, 4] = new Piece('K', 0, "Black", new Point(7, 4));
        }

        public bool Repetition3()
        {
            if (MovesHistory.Count > 9)
            {
                var last1 = MovesHistory.ElementAt(MovesHistory.Count - 1);
                var last2 = MovesHistory.ElementAt(MovesHistory.Count - 2);
                var last3 = MovesHistory.ElementAt(MovesHistory.Count - 3);
                var last4 = MovesHistory.ElementAt(MovesHistory.Count - 4);
                var last5 = MovesHistory.ElementAt(MovesHistory.Count - 5);
                var last6 = MovesHistory.ElementAt(MovesHistory.Count - 6);
                var last7 = MovesHistory.ElementAt(MovesHistory.Count - 7);
                var last8 = MovesHistory.ElementAt(MovesHistory.Count - 8);

                if (last1.From == last5.From && last1.To == last5.To)
                {
                    if (last2.From == last6.From && last2.To == last6.To)
                    {
                        if (last3.From == last7.From && last3.To == last7.To)
                        {
                            if (last4.From == last8.From && last4.To == last8.To)
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

        public bool ValidMove(Move move)
        {
            var myPiece = Pieces[move.From.X, move.From.Y];

            var entityOnDestination = Pieces[move.To.X, move.To.Y];
            Piece entityOnDestinationClone = null;
            if (entityOnDestination != null)
            {
                entityOnDestinationClone =
                    new Piece(entityOnDestination); //make new instance because deep copy will override ours
            }

            if (move.Castle)
                MovePieceCastle(myPiece, move.To, move.CastleQueenSide);
            else
                MovePieceProcess(myPiece, move, false, false); //for sure false?


            var myKing = FindKingLocation(myPiece.Color);

            var check = false;
            //now we need to check if that move wont result in check on our king

            //iterate over enemy pieces w/o pawns and knight and king cuz they got static attack position so won't cause check
            foreach (var p in GetPiecesByColor(ReverseColor(myPiece.Color)))
            {
                var pieceClone = new Piece(p); //new instance too prevent changes on our original piece

                //if (pieceClone.HitMoves != null)
                //{
                //    if (pieceClone.HitMoves.Any(x => x == move.From)) //if the piece wasn't attacking the position on which piece was then don't check it
                //    {
                //        continue;
                //    }
                //}


                pieceClone.UpdateHitMoves(this); //update legal moves after we made one with our piece
                if (pieceClone.HitMoves.Any(x => x == myKing.Position))
                {
                    check = true;
                    break;
                }
            }

            var m = new Move()
            {
                Castle = move.Castle, CastleQueenSide = move.CastleQueenSide, enPassant = move.enPassant, To = move.From, From = move.To, enPassantUndo = true
            };

            if (move.Castle)
                MovePieceCastle(myPiece, m.To, move.CastleQueenSide);
            else
                MovePieceProcess(myPiece, m, false, false); //for sure false?

            if (entityOnDestinationClone != null)
                Pieces[move.To.X, move.To.Y] = entityOnDestinationClone; //bring back old piece if there was any

            if (check) return false;

            return true;
        }

        public string ReverseColor(string color)
        {
            return color == "White" ? "Black" : "White";
        }

        private void MovePieceProcess(Piece _piece, Move move, bool final, bool newInstance)
        {
            var piece = _piece;
            if (newInstance)
            {
                piece = new Piece(_piece);
            }

            if (final)
            {
                piece.WasMoved = true;
                piece.MovesDone++;
                MovesHistory.Add(new Move { From = new Point(piece.Position.X, piece.Position.Y), To = move.To });
                if (IsPromotion(piece, move.To))
                {
                    var value = 90;
                    if (piece.Color == "Black")
                        value = -90;
                    Promotion(piece, 'Q', value);
                }
            }

            Pieces[piece.Position.X, piece.Position.Y] = null;
            Pieces[move.To.X, move.To.Y] = piece;
            piece.Position = move.To;      

            if (move.enPassant)
            {
                var temp = 1;
                if (piece.Color == "White")
                {
                    temp = -1;
                }
                if (move.enPassantUndo)
                {
                    string c;
                    if (piece.Color == "Black")
                    {
                        c = "White";
                    }
                    else
                    {
                        c = "Black";
                    }

                    var undoPiece = new Piece('P', -piece.Value, c, new Point(move.From.X + temp, move.From.Y));
                    undoPiece.MovesDone = 1;
                    undoPiece.WasMoved = true;

                    Pieces[move.From.X + temp, move.From.Y] = undoPiece;
                }
                else
                {             
                    Pieces[move.To.X + temp, move.To.Y] = null;
                }
                
            }
        }

        public void MovePiece(Move move, bool final, bool newInstance)
        {
            var piece = Pieces[move.From.X, move.From.Y];
            if (move.Castle)
                MovePieceCastle(piece, move.To, move.CastleQueenSide, final);
            else
                MovePieceProcess(piece, move, final, newInstance);
        }

        private void MovePieceCastle(Piece piece, Point to, bool queenSide, bool final = false)
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
            else if (to.Y == 4) //back
            {
                if (queenSide)
                {
                    rook = GetPiece(new Point(piece.Position.X, 0));
                    rookTo = new Point(to.X, 3);
                    if (rook == null)
                    {
                        //back move
                        rook = GetPiece(new Point(piece.Position.X, 3));
                        rookTo = new Point(to.X, 0);
                    }
                }
                else
                {
                    rook = GetPiece(new Point(piece.Position.X, 7));
                    rookTo = new Point(to.X, 5);
                    if (rook == null)
                    {
                        //back move
                        rook = GetPiece(new Point(piece.Position.X, 5));
                        rookTo = new Point(to.X, 7);
                    }
                }
            }
            MovePieceProcess(piece, new Move() {To = to}, final, false);
            MovePieceProcess(rook, new Move() { To = rookTo }, final, false); //newInstance for sure false?
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

        public void Promotion(Piece piece, char nameShort, int value)
        {
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
                        piece.FindLegalMoves(this);
            }
        }

        public void UpdateHit(string color)
        {
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = Pieces[i, j];
                if (piece != null)
                    if (piece.Color == color)
                        piece.UpdateHitMoves(this);
            }
        }

        public bool IsCheck(string color)
        {
            var king = FindKingLocation(color);

            var pieces =
                GetPiecesByColor(ReverseColor(color)).OrderByDescending(x => x.Value); //not sure about the order
            foreach (var piece in pieces)
            {
                piece.UpdateHitMoves(this);
                if (piece.HitMoves.Any(x => x == king.Position)) return true;
            }

            return false;
        }

        public bool IsCheckMate(string color)
        {
            //lets check first king if can do any move to prevent check-mate
            var king = FindKingLocation(color);
            king.UpdateHitMoves(this);
            king.FindLegalMoves(this);
            if (king.LegalMoves != null)
                if (king.LegalMoves.Count > 0)
                    return false;

            foreach (var piece in GetPiecesByColor(color))
            {
                if (piece.NameShort == 'K')
                    //we already checked the king before so no point doing it again
                    continue;

                piece.UpdateHitMoves(this);
                piece.FindLegalMoves(this);
                if (piece.LegalMoves != null)
                    if (piece.LegalMoves.Count > 0)
                        return false;
            }

            return true;
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