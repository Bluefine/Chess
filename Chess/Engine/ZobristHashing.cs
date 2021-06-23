using System;

namespace Chess.Engine
{
    public class ZobristHashing
    {
        public static ulong[,,] ZobristTable = new ulong[8, 8, 12];
        public static Random Random = new();

        // Uppercase letters are white pieces
        // Lowercase letters are black pieces
        public static int IndexOf(char piece)
        {
            if (piece == 'P')
                return 0;
            if (piece == 'N')
                return 1;
            if (piece == 'B')
                return 2;
            if (piece == 'R')
                return 3;
            if (piece == 'Q')
                return 4;
            if (piece == 'K')
                return 5;
            if (piece == 'p')
                return 6;
            if (piece == 'n')
                return 7;
            if (piece == 'b')
                return 8;
            if (piece == 'r')
                return 9;
            if (piece == 'q')
                return 10;
            if (piece == 'k')
                return 11;
            return -1;
        }

        public static void Initialize()
        {
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            for (var k = 0; k < 12; k++)
                ZobristTable[i, j, k] = NextInt64();
        }

        public static ulong Hash(char[,] board)
        {
            ulong h = 0;
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
                if (board[i, j] != '-')
                {
                    var piece = IndexOf(board[i, j]);
                    h ^= ZobristTable[i, j, piece];
                }

            return h;
        }

        public static ulong NextInt64()
        {
            var buffer = new byte[sizeof(ulong)];
            Random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}