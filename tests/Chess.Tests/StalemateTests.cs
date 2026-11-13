using Chess.Core.Enums;
using Chess.Core.Game;

namespace Chess.Tests;

public class StalemateTests
{
    [Fact]
    public void KingAndQueenCornering_IsStalemate()
    {
        // Black king on a8 is not attacked but every square it could use is covered.
        var game = ChessGame.FromFen("k7/8/1QK5/8/8/8/8/8 b - - 0 1");
        Assert.Equal(GameStatus.Stalemate, game.Status);
        Assert.True(game.IsGameOver);
    }

    [Fact]
    public void MovingIntoStalemate_EndsTheGame()
    {
        var game = ChessGame.FromFen("k7/8/2K5/8/8/8/1Q6/8 w - - 0 1");
        game.PlayAll("b2b6");
        Assert.Equal(GameStatus.Stalemate, game.Status);
    }

    [Fact]
    public void CheckWithNoEscape_IsCheckmateNotStalemate()
    {
        var game = ChessGame.FromFen("k7/1Q6/1K6/8/8/8/8/8 b - - 0 1");
        Assert.Equal(GameStatus.Checkmate, game.Status);
    }
}
