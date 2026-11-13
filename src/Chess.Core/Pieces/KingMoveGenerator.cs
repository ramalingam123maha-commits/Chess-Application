using Chess.Core.Boards;
using Chess.Core.Enums;
using Chess.Core.Models;
using Chess.Core.Rules;

namespace Chess.Core.Pieces;

/// <summary>Kings step one square in any direction and may castle.</summary>
public sealed class KingMoveGenerator : IMoveGenerator
{
    public IEnumerable<Move> Generate(Board board, Position from)
    {
        var king = board[from]!;

        for (int dr = -1; dr <= 1; dr++)
        for (int dc = -1; dc <= 1; dc++)
        {
            if (dr == 0 && dc == 0) continue;
            var to = from.Offset(dr, dc);
            if (to.IsValid && (board[to] is null || board[to]!.Color != king.Color))
                yield return new Move(from, to);
        }

        foreach (var castle in CastlingMoves(board, from, king))
            yield return castle;
    }

    /// <summary>
    /// Castling needs all of these:
    /// the king and that rook have not moved (tracked by castling rights),
    /// the squares between them are empty,
    /// the king is not in check, and
    /// the king does not cross or land on an attacked square.
    /// </summary>
    private static IEnumerable<Move> CastlingMoves(Board board, Position from, Piece king)
    {
        int homeRow = king.Color == PieceColor.White ? 0 : 7;
        if (from != new Position(homeRow, 4)) yield break;

        var enemy = king.Color.Opposite();
        if (AttackDetector.IsSquareAttacked(board, from, enemy)) yield break;

        // King side: f and g must be empty and not attacked.
        if (board.Castling.Has(king.Color, kingSide: true)
            && IsRookHome(board, new Position(homeRow, 7), king.Color)
            && board[new Position(homeRow, 5)] is null
            && board[new Position(homeRow, 6)] is null
            && !AttackDetector.IsSquareAttacked(board, new Position(homeRow, 5), enemy)
            && !AttackDetector.IsSquareAttacked(board, new Position(homeRow, 6), enemy))
        {
            yield return new Move(from, new Position(homeRow, 6), IsCastling: true);
        }

        // Queen side: b, c and d must be empty; only c and d must be safe (the rook may pass b under attack).
        if (board.Castling.Has(king.Color, kingSide: false)
            && IsRookHome(board, new Position(homeRow, 0), king.Color)
            && board[new Position(homeRow, 1)] is null
            && board[new Position(homeRow, 2)] is null
            && board[new Position(homeRow, 3)] is null
            && !AttackDetector.IsSquareAttacked(board, new Position(homeRow, 3), enemy)
            && !AttackDetector.IsSquareAttacked(board, new Position(homeRow, 2), enemy))
        {
            yield return new Move(from, new Position(homeRow, 2), IsCastling: true);
        }
    }

    private static bool IsRookHome(Board board, Position square, PieceColor color) =>
        board[square] is { Type: PieceType.Rook } rook && rook.Color == color;
}
