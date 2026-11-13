using Chess.Core.Boards;
using Chess.Core.Enums;
using Chess.Core.Models;
using Chess.Core.Pieces;

namespace Chess.Core.Rules;

/// <summary>
/// Turns pseudo-legal moves into legal moves. It plays each candidate on a copy of the board
/// and keeps the move only if the mover's own king is safe afterwards. This single test
/// covers pinned pieces, a king stepping into check, and ignoring a check.
/// </summary>
public static class MoveValidator
{
    private static readonly PawnMoveGenerator Pawn = new();
    private static readonly KnightMoveGenerator Knight = new();
    private static readonly SlidingMoveGenerator Sliding = new();
    private static readonly KingMoveGenerator King = new();

    private static IMoveGenerator GeneratorFor(PieceType type) => type switch
    {
        PieceType.Pawn => Pawn,
        PieceType.Knight => Knight,
        PieceType.King => King,
        _ => Sliding // Rook, Bishop, Queen
    };

    /// <summary>Legal moves for the piece on one square. Empty when the square is empty.</summary>
    public static List<Move> GetLegalMoves(Board board, Position from)
    {
        var piece = board[from];
        if (piece is null) return new List<Move>();

        var legal = new List<Move>();
        foreach (var move in GeneratorFor(piece.Type).Generate(board, from))
        {
            var trial = board.Clone();
            trial.ApplyMove(move);
            if (!AttackDetector.IsInCheck(trial, piece.Color))
                legal.Add(move);
        }
        return legal;
    }

    /// <summary>Every legal move for one side.</summary>
    public static List<Move> GetAllLegalMoves(Board board, PieceColor color)
    {
        var all = new List<Move>();
        for (int row = 0; row < 8; row++)
        for (int col = 0; col < 8; col++)
        {
            var square = new Position(row, col);
            if (board[square] is { } piece && piece.Color == color)
                all.AddRange(GetLegalMoves(board, square));
        }
        return all;
    }
}
