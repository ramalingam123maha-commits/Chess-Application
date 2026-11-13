using Chess.Core.Enums;
using Chess.Core.Game;

namespace Chess.Tests;

public class CheckmateTests
{
    [Fact]
    public void FoolsMate_BlackWinsInTwoMoves()
    {
        var game = new ChessGame();
        game.PlayAll("f2f3", "e7e5", "g2g4", "d8h4");
        Assert.Equal(GameStatus.Checkmate, game.Status);
        Assert.True(game.IsGameOver);
        Assert.Equal(PieceColor.White, game.CurrentTurn); // White is the side mated.
    }

    [Fact]
    public void ScholarsMate_WhiteWinsInFourMoves()
    {
        var game = new ChessGame();
        game.PlayAll("e2e4", "e7e5", "f1c4", "b8c6", "d1h5", "g8f6", "h5f7");
        Assert.Equal(GameStatus.Checkmate, game.Status);
    }

    [Fact]
    public void NoMovesAreAllowedAfterCheckmate()
    {
        var game = new ChessGame();
        game.PlayAll("f2f3", "e7e5", "g2g4", "d8h4");
        Assert.False(game.Play("a2a3").Success);
    }

    [Fact]
    public void BackRankMate_IsDetected()
    {
        var game = ChessGame.FromFen("6k1/5ppp/8/8/8/8/8/R3K3 w - - 0 1");
        game.PlayAll("a1a8");
        Assert.Equal(GameStatus.Checkmate, game.Status);
    }
}
