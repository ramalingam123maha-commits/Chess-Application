using Chess.Core.Enums;
using Chess.Core.Game;
using Chess.Core.Models;

namespace Chess.Tests;

public class EnPassantTests
{
    [Fact]
    public void EnPassant_CapturesThePawnThatJustMovedTwoSquares()
    {
        var game = new ChessGame();
        game.PlayAll("e2e4", "a7a6", "e4e5", "d7d5");
        Assert.Contains("d6", game.Destinations("e5"));

        game.PlayAll("e5d6");
        Assert.Equal(PieceType.Pawn, game.Board[Position.Parse("d6")]!.Type);
        Assert.Equal(PieceColor.White, game.Board[Position.Parse("d6")]!.Color);
        // The captured pawn is removed from d5, not from the landing square.
        Assert.Null(game.Board[Position.Parse("d5")]);
    }

    [Fact]
    public void EnPassant_IsRefusedOneMoveLater()
    {
        var game = new ChessGame();
        game.PlayAll("e2e4", "a7a6", "e4e5", "d7d5", "h2h3", "h7h6");
        Assert.DoesNotContain("d6", game.Destinations("e5"));
    }

    [Fact]
    public void EnPassant_WorksForBlackToo()
    {
        var game = new ChessGame();
        game.PlayAll("a2a3", "e7e5", "a3a4", "e5e4", "d2d4");
        Assert.Contains("d3", game.Destinations("e4"));
        game.PlayAll("e4d3");
        Assert.Null(game.Board[Position.Parse("d4")]);
    }

    [Fact]
    public void EnPassant_NotAvailableAfterASingleStep()
    {
        var game = new ChessGame();
        game.PlayAll("e2e4", "a7a6", "e4e5", "d7d6", "h2h3", "d6d5");
        Assert.DoesNotContain("d6", game.Destinations("e5"));
    }

    [Fact]
    public void EnPassant_IsIllegal_WhenItExposesTheKing()
    {
        // Removing both pawns from rank 5 would open the rook's line to the white king.
        var game = ChessGame.FromFen("7k/8/8/K2pP2r/8/8/8/8 w - d6 0 1");
        Assert.DoesNotContain("d6", game.Destinations("e5"));
    }

    [Fact]
    public void EnPassant_UndoRestoresTheCapturedPawn()
    {
        var game = new ChessGame();
        game.PlayAll("e2e4", "a7a6", "e4e5", "d7d5", "e5d6");
        game.Undo();
        Assert.NotNull(game.Board[Position.Parse("d5")]);
        Assert.Null(game.Board[Position.Parse("d6")]);
    }
}
