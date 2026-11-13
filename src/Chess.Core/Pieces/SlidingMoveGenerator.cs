using Chess.Core.Boards;
using Chess.Core.Enums;
using Chess.Core.Models;

namespace Chess.Core.Pieces;

/// <summary>
/// Rooks, bishops and queens slide any distance along a line until they meet a piece.
/// One class serves all three; the piece type decides which directions are used.
/// </summary>
public sealed class SlidingMoveGenerator : IMoveGenerator
{
    private static readonly (int Row, int Col)[] Straight = { (1, 0), (-1, 0), (0, 1), (0, -1) };
    private static readonly (int Row, int Col)[] Diagonal = { (1, 1), (1, -1), (-1, 1), (-1, -1) };

    public IEnumerable<Move> Generate(Board board, Position from)
    {
        var mover = board[from]!;
        var directions = mover.Type switch
        {
            PieceType.Rook => Straight,
            PieceType.Bishop => Diagonal,
            _ => Straight.Concat(Diagonal).ToArray() // Queen
        };

        foreach (var (dr, dc) in directions)
        {
            var to = from.Offset(dr, dc);
            while (to.IsValid)
            {
                var target = board[to];
                if (target is null)
                {
                    yield return new Move(from, to);
                }
                else
                {
                    // An enemy piece can be captured; either way the line stops here.
                    if (target.Color != mover.Color)
                        yield return new Move(from, to);
                    break;
                }
                to = to.Offset(dr, dc);
            }
        }
    }
}
