using Chess.Core.Boards;
using Chess.Core.Enums;
using Chess.Core.Models;

namespace Chess.Core.Rules;

/// <summary>
/// Answers one question: is this square attacked by a given side?
/// It works by looking outward from the square for enemy pieces that could reach it.
/// It does not use the move generators, because the king generator needs this class
/// for castling and the two would call each other forever.
/// </summary>
public static class AttackDetector
{
    private static readonly (int Row, int Col)[] KnightJumps =
    {
        (1, 2), (2, 1), (-1, 2), (-2, 1), (1, -2), (2, -1), (-1, -2), (-2, -1)
    };

    private static readonly (int Row, int Col)[] StraightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1) };
    private static readonly (int Row, int Col)[] DiagonalDirections = { (1, 1), (1, -1), (-1, 1), (-1, -1) };

    public static bool IsSquareAttacked(Board board, Position square, PieceColor attacker)
    {
        // Pawns: an attacking pawn stands one row behind the square from its own point of view.
        int pawnRow = attacker == PieceColor.White ? -1 : 1;
        foreach (int dc in new[] { -1, 1 })
        {
            var from = square.Offset(pawnRow, dc);
            if (from.IsValid && board[from] is { Type: PieceType.Pawn } p && p.Color == attacker)
                return true;
        }

        foreach (var (dr, dc) in KnightJumps)
        {
            var from = square.Offset(dr, dc);
            if (from.IsValid && board[from] is { Type: PieceType.Knight } p && p.Color == attacker)
                return true;
        }

        // Adjacent king.
        for (int dr = -1; dr <= 1; dr++)
        for (int dc = -1; dc <= 1; dc++)
        {
            if (dr == 0 && dc == 0) continue;
            var from = square.Offset(dr, dc);
            if (from.IsValid && board[from] is { Type: PieceType.King } p && p.Color == attacker)
                return true;
        }

        return RayHits(board, square, attacker, StraightDirections, PieceType.Rook)
            || RayHits(board, square, attacker, DiagonalDirections, PieceType.Bishop);
    }

    /// <summary>
    /// Walks outward in each direction until it meets a piece. That piece attacks the square
    /// if it belongs to the attacker and is a queen or the given slider type.
    /// </summary>
    private static bool RayHits(Board board, Position square, PieceColor attacker,
        (int Row, int Col)[] directions, PieceType slider)
    {
        foreach (var (dr, dc) in directions)
        {
            var current = square.Offset(dr, dc);
            while (current.IsValid)
            {
                var piece = board[current];
                if (piece is not null)
                {
                    if (piece.Color == attacker && (piece.Type == slider || piece.Type == PieceType.Queen))
                        return true;
                    break; // Any piece blocks the ray.
                }
                current = current.Offset(dr, dc);
            }
        }
        return false;
    }

    /// <summary>True when the given side's king is attacked. A board with no such king is never in check.</summary>
    public static bool IsInCheck(Board board, PieceColor color)
    {
        var king = board.FindKing(color);
        return king is { } square && IsSquareAttacked(board, square, color.Opposite());
    }
}
