using Chess.Core.Enums;
using Chess.Core.Game;
using Chess.Core.Models;

namespace Chess.Tests;

public class PromotionTests
{
    [Theory]
    [InlineData("e7e8Q", PieceType.Queen)]
    [InlineData("e7e8R", PieceType.Rook)]
    [InlineData("e7e8B", PieceType.Bishop)]
    [InlineData("e7e8N", PieceType.Knight)]
    public void Pawn_PromotesToTheChosenPiece(string move, PieceType expected)
    {
        var game = ChessGame.FromFen("7k/4P3/8/8/8/8/8/K7 w - - 0 1");
        Assert.True(game.Play(move).Success);
        var piece = game.Board[Position.Parse("e8")]!;
        Assert.Equal(expected, piece.Type);
        Assert.Equal(PieceColor.White, piece.Color);
    }

    [Fact]
    public void Promotion_WithoutAChoice_IsRefused()
    {
        var game = ChessGame.FromFen("7k/4P3/8/8/8/8/8/K7 w - - 0 1");
        Assert.False(game.Play("e7e8").Success);
    }

    [Fact]
    public void Promotion_CanHappenByCapture()
    {
        var game = ChessGame.FromFen("3rk3/4P3/8/8/8/8/8/K7 w - - 0 1");
        Assert.True(game.Play("e7d8Q").Success);
        Assert.Equal(PieceType.Queen, game.Board[Position.Parse("d8")]!.Type);
    }

    [Fact]
    public void BlackPawn_PromotesOnRankOne()
    {
        var game = ChessGame.FromFen("7k/8/8/8/8/8/4p3/K7 b - - 0 1");
        Assert.True(game.Play("e2e1N").Success);
        Assert.Equal(PieceType.Knight, game.Board[Position.Parse("e1")]!.Type);
        Assert.Equal(PieceColor.Black, game.Board[Position.Parse("e1")]!.Color);
    }

    [Fact]
    public void IsPromotionMove_TellsTheUiToAskForAChoice()
    {
        var game = ChessGame.FromFen("7k/4P3/8/8/8/8/8/K7 w - - 0 1");
        Assert.True(game.IsPromotionMove(Position.Parse("e7"), Position.Parse("e8")));
        Assert.False(game.IsPromotionMove(Position.Parse("a1"), Position.Parse("a2")));
    }

    [Fact]
    public void Promotion_CanDeliverCheckmate()
    {
        var game = ChessGame.FromFen("6k1/4P1pp/8/8/8/8/8/K7 w - - 0 1");
        game.PlayAll("e7e8Q");
        Assert.Equal(GameStatus.Checkmate, game.Status);
    }
}
