using Chess.Core.Boards;
using Chess.Core.Models;

namespace Chess.Core.Pieces;

/// <summary>
/// Lists pseudo-legal moves for the piece on one square. "Pseudo-legal" means the move
/// follows the piece's movement pattern and is not blocked, but it may still leave the
/// mover's own king in check. <see cref="Rules.MoveValidator"/> removes those moves.
/// </summary>
public interface IMoveGenerator
{
    IEnumerable<Move> Generate(Board board, Position from);
}
