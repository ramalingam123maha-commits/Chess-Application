using Chess.Core.Game;
using Chess.Core.Models;

namespace Chess.Tests;

/// <summary>Shortcuts that keep the tests short and readable.</summary>
internal static class TestHelpers
{
    /// <summary>Plays a move written as "e2e4" (or "e7e8Q" for a promotion) and returns the result.</summary>
    public static MoveResult Play(this ChessGame game, string text)
    {
        var from = Position.Parse(text.Substring(0, 2));
        var to = Position.Parse(text.Substring(2, 2));
        Core.Enums.PieceType? promotion = text.Length > 4
            ? char.ToUpperInvariant(text[4]) switch
            {
                'Q' => Core.Enums.PieceType.Queen,
                'R' => Core.Enums.PieceType.Rook,
                'B' => Core.Enums.PieceType.Bishop,
                'N' => Core.Enums.PieceType.Knight,
                _ => throw new ArgumentException("Bad promotion letter.")
            }
            : null;
        return game.TryMakeMove(new Move(from, to, promotion));
    }

    /// <summary>Plays a list of moves and fails the test at once if any is refused.</summary>
    public static void PlayAll(this ChessGame game, params string[] moves)
    {
        foreach (var move in moves)
        {
            var result = game.Play(move);
            Assert.True(result.Success, $"{move} was refused: {result.Error}");
        }
    }

    public static List<string> Destinations(this ChessGame game, string square) =>
        game.GetLegalMoves(Position.Parse(square))
            .Select(m => m.To.ToString()).Distinct().OrderBy(s => s).ToList();
}
