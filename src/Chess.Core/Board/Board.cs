using Chess.Core.Enums;
using Chess.Core.Models;

namespace Chess.Core.Boards;

/// <summary>
/// The pieces on the 8x8 grid, plus the two facts that belong to a position but are not
/// visible on the grid: the en passant target square and the castling rights.
/// Keeping them here means a cloned board is a complete position, which the move
/// validator relies on when it tries a move and checks for check.
/// </summary>
public sealed class Board
{
    private readonly Piece?[,] _squares = new Piece?[8, 8];

    /// <summary>The square a pawn skipped on the last move, or null. Only valid for one move.</summary>
    public Position? EnPassantTarget { get; private set; }

    /// <summary>The castling options still available.</summary>
    public CastlingRights Castling { get; private set; } = new();

    public Piece? this[Position position]
    {
        get => _squares[position.Row, position.Col];
        set => _squares[position.Row, position.Col] = value;
    }

    /// <summary>Creates a board with the standard starting position.</summary>
    public static Board CreateStartPosition()
    {
        var board = new Board();
        PieceType[] backRank =
        {
            PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen,
            PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook
        };

        for (int col = 0; col < 8; col++)
        {
            board[new Position(0, col)] = new Piece(backRank[col], PieceColor.White);
            board[new Position(1, col)] = new Piece(PieceType.Pawn, PieceColor.White);
            board[new Position(6, col)] = new Piece(PieceType.Pawn, PieceColor.Black);
            board[new Position(7, col)] = new Piece(backRank[col], PieceColor.Black);
        }

        board.Castling = new CastlingRights
        {
            WhiteKingSide = true,
            WhiteQueenSide = true,
            BlackKingSide = true,
            BlackQueenSide = true
        };
        return board;
    }

    /// <summary>
    /// Reads a position from Forsyth-Edwards Notation (FEN), for example
    /// "4k3/8/8/8/8/8/8/4K3 w - - 0 1". Only the first four fields are used.
    /// </summary>
    /// <param name="fen">The FEN text.</param>
    /// <param name="sideToMove">Receives the side whose turn it is.</param>
    public static Board FromFen(string fen, out PieceColor sideToMove)
    {
        var fields = fen.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length < 2)
            throw new FormatException("FEN needs at least a piece field and a side-to-move field.");

        var board = new Board();
        var ranks = fields[0].Split('/');
        if (ranks.Length != 8)
            throw new FormatException("FEN must describe 8 ranks.");

        for (int i = 0; i < 8; i++)
        {
            int row = 7 - i; // FEN lists rank 8 first.
            int col = 0;
            foreach (char c in ranks[i])
            {
                if (char.IsDigit(c))
                {
                    col += c - '0';
                    continue;
                }

                var color = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
                PieceType type = char.ToLowerInvariant(c) switch
                {
                    'k' => PieceType.King,
                    'q' => PieceType.Queen,
                    'r' => PieceType.Rook,
                    'b' => PieceType.Bishop,
                    'n' => PieceType.Knight,
                    'p' => PieceType.Pawn,
                    _ => throw new FormatException($"Unknown piece '{c}' in FEN.")
                };

                var position = new Position(row, col);
                if (!position.IsValid)
                    throw new FormatException("FEN rank is longer than 8 squares.");
                board[position] = new Piece(type, color);
                col++;
            }
        }

        sideToMove = fields[1] == "b" ? PieceColor.Black : PieceColor.White;

        if (fields.Length > 2)
        {
            string castling = fields[2];
            board.Castling = new CastlingRights
            {
                WhiteKingSide = castling.Contains('K'),
                WhiteQueenSide = castling.Contains('Q'),
                BlackKingSide = castling.Contains('k'),
                BlackQueenSide = castling.Contains('q')
            };
        }

        if (fields.Length > 3 && fields[3] != "-")
            board.EnPassantTarget = Position.Parse(fields[3]);

        return board;
    }

    /// <summary>Makes an independent copy of the whole position.</summary>
    public Board Clone()
    {
        var copy = new Board();
        Array.Copy(_squares, copy._squares, _squares.Length);
        copy.EnPassantTarget = EnPassantTarget;
        copy.Castling = Castling.Clone();
        return copy;
    }

    /// <summary>Finds the king of the given colour, or null if there is none (only in test positions).</summary>
    public Position? FindKing(PieceColor color)
    {
        for (int row = 0; row < 8; row++)
        for (int col = 0; col < 8; col++)
        {
            var piece = _squares[row, col];
            if (piece is { Type: PieceType.King } && piece.Color == color)
                return new Position(row, col);
        }
        return null;
    }

    /// <summary>
    /// Plays a move on the board. The move is assumed to be legal; this method only does the
    /// mechanical work: moving pieces, removing the en passant victim, moving the castling rook,
    /// promoting, and updating the en passant target and castling rights.
    /// </summary>
    public void ApplyMove(Move move)
    {
        var piece = this[move.From]
            ?? throw new InvalidOperationException($"No piece on {move.From}.");

        // En passant: the captured pawn sits beside the mover, not on the destination square.
        if (move.IsEnPassant)
            this[new Position(move.From.Row, move.To.Col)] = null;

        // Castling: the rook jumps over the king to the square the king crossed.
        if (move.IsCastling)
        {
            bool kingSide = move.To.Col > move.From.Col;
            var rookFrom = new Position(move.From.Row, kingSide ? 7 : 0);
            var rookTo = new Position(move.From.Row, kingSide ? 5 : 3);
            this[rookTo] = this[rookFrom];
            this[rookFrom] = null;
        }

        // Castling rights are lost when the king moves, or when a rook leaves or is captured
        // on its starting corner.
        if (piece.Type == PieceType.King)
            Castling.RevokeColor(piece.Color);
        Castling.RevokeForSquare(move.From);
        Castling.RevokeForSquare(move.To);

        // A pawn that moves two squares leaves behind the square it skipped.
        EnPassantTarget = piece.Type == PieceType.Pawn && Math.Abs(move.To.Row - move.From.Row) == 2
            ? new Position((move.From.Row + move.To.Row) / 2, move.From.Col)
            : null;

        // Promotion replaces the pawn with the chosen piece.
        this[move.To] = move.Promotion is { } promoted ? new Piece(promoted, piece.Color) : piece;
        this[move.From] = null;
    }
}
