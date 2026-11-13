using Chess.Core.Boards;
using Chess.Core.Enums;
using Chess.Core.Models;

namespace Chess.Core.Pieces;

/// <summary>
/// Pawns move forward, capture diagonally, may advance two squares from their start rank,
/// capture en passant, and promote on the last rank.
/// </summary>
public sealed class PawnMoveGenerator : IMoveGenerator
{
    private static readonly PieceType[] PromotionChoices =
        { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight };

    public IEnumerable<Move> Generate(Board board, Position from)
    {
        var pawn = board[from]!;
        int forward = pawn.Color == PieceColor.White ? 1 : -1;
        int startRow = pawn.Color == PieceColor.White ? 1 : 6;
        int lastRow = pawn.Color == PieceColor.White ? 7 : 0;

        // Forward one square, then two from the start rank. Both need empty squares.
        var one = from.Offset(forward, 0);
        if (one.IsValid && board[one] is null)
        {
            foreach (var move in WithPromotions(from, one, lastRow))
                yield return move;

            var two = from.Offset(2 * forward, 0);
            if (from.Row == startRow && board[two] is null)
                yield return new Move(from, two);
        }

        // Diagonal captures, including en passant.
        foreach (int dc in new[] { -1, 1 })
        {
            var to = from.Offset(forward, dc);
            if (!to.IsValid) continue;

            var target = board[to];
            if (target is not null && target.Color != pawn.Color)
            {
                foreach (var move in WithPromotions(from, to, lastRow))
                    yield return move;
            }
            else if (target is null && board.EnPassantTarget == to)
            {
                yield return new Move(from, to, IsEnPassant: true);
            }
        }
    }

    /// <summary>A move to the last rank becomes four moves, one per promotion choice.</summary>
    private static IEnumerable<Move> WithPromotions(Position from, Position to, int lastRow)
    {
        if (to.Row != lastRow)
        {
            yield return new Move(from, to);
            yield break;
        }

        foreach (var choice in PromotionChoices)
            yield return new Move(from, to, Promotion: choice);
    }
}
