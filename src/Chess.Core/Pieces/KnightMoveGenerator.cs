using Chess.Core.Boards;
using Chess.Core.Models;

namespace Chess.Core.Pieces;

/// <summary>Knights jump in an L shape and ignore pieces in between.</summary>
public sealed class KnightMoveGenerator : IMoveGenerator
{
    private static readonly (int Row, int Col)[] Jumps =
    {
        (1, 2), (2, 1), (-1, 2), (-2, 1), (1, -2), (2, -1), (-1, -2), (-2, -1)
    };

    public IEnumerable<Move> Generate(Board board, Position from)
    {
        var mover = board[from]!;
        foreach (var (dr, dc) in Jumps)
        {
            var to = from.Offset(dr, dc);
            // The target must be on the board and empty or hold an enemy piece.
            if (to.IsValid && (board[to] is null || board[to]!.Color != mover.Color))
                yield return new Move(from, to);
        }
    }
}
