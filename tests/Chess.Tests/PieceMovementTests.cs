using Chess.Core.Enums;
using Chess.Core.Game;
using Chess.Core.Models;

namespace Chess.Tests;

public class PieceMovementTests
{
    [Fact]
    public void StartPosition_WhiteHasTwentyMoves()
    {
        var game = new ChessGame();
        int count = Core.Rules.MoveValidator.GetAllLegalMoves(game.Board, PieceColor.White).Count;
        Assert.Equal(20, count); // 16 pawn moves + 4 knight moves
    }

    [Fact]
    public void Pawn_CanMoveOneOrTwoSquaresFromStart()
    {
        var game = new ChessGame();
        Assert.Equal(new[] { "e3", "e4" }, game.Destinations("e2"));
    }

    [Fact]
    public void Pawn_CannotMoveTwoSquaresAfterMoving()
    {
        var game = new ChessGame();
        game.PlayAll("e2e3", "a7a6");
        Assert.Equal(new[] { "e4" }, game.Destinations("e3"));
    }

    [Fact]
    public void Pawn_CannotMoveForwardIntoAPiece()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/4p3/4P3/8/4K3 w - - 0 1");
        Assert.Empty(game.Destinations("e3"));
    }

    [Fact]
    public void Pawn_CapturesDiagonally()
    {
        var game = ChessGame.FromFen("4k3/8/8/3p4/4P3/8/8/4K3 w - - 0 1");
        Assert.Equal(new[] { "d5", "e5" }, game.Destinations("e4"));
    }

    [Fact]
    public void Knight_JumpsOverPieces()
    {
        var game = new ChessGame();
        Assert.Equal(new[] { "a3", "c3" }, game.Destinations("b1"));
    }

    [Fact]
    public void Rook_StopsAtOwnPieceAndCapturesEnemy()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/p3R2P/8/8/4K3 w - - 0 1");
        // Left: d4, c4, b4, and a capture on a4. Right: f4, g4, then own pawn on h4 blocks.
        var squares = game.Destinations("e4");
        Assert.Contains("a4", squares);
        Assert.Contains("g4", squares);
        Assert.DoesNotContain("h4", squares);
    }

    [Fact]
    public void Bishop_MovesOnlyDiagonally()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/3B4/8/8/4K3 w - - 0 1");
        var squares = game.Destinations("d4");
        Assert.Contains("a7", squares);
        Assert.Contains("g1", squares);
        Assert.DoesNotContain("d5", squares);
    }

    [Fact]
    public void Queen_CombinesRookAndBishop()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/3Q4/8/8/4K3 w - - 0 1");
        Assert.Equal(27, game.Destinations("d4").Count);
    }

    [Fact]
    public void King_MovesOneSquare()
    {
        var game = ChessGame.FromFen("7k/8/8/8/3K4/8/8/8 w - - 0 1");
        Assert.Equal(8, game.Destinations("d4").Count);
    }

    [Fact]
    public void CannotMoveOpponentsPiece()
    {
        var game = new ChessGame();
        var result = game.Play("e7e5");
        Assert.False(result.Success);
    }

    [Fact]
    public void IllegalMove_IsRefusedAndBoardIsUnchanged()
    {
        var game = new ChessGame();
        var result = game.Play("e2e5");
        Assert.False(result.Success);
        Assert.NotNull(game.Board[Position.Parse("e2")]);
        Assert.Equal(PieceColor.White, game.CurrentTurn);
    }

    [Fact]
    public void Turns_AlternateAfterEachMove()
    {
        var game = new ChessGame();
        game.PlayAll("e2e4");
        Assert.Equal(PieceColor.Black, game.CurrentTurn);
    }

    [Fact]
    public void Undo_RestoresThePreviousPosition()
    {
        var game = new ChessGame();
        game.PlayAll("e2e4");
        Assert.True(game.Undo());
        Assert.Equal(PieceColor.White, game.CurrentTurn);
        Assert.NotNull(game.Board[Position.Parse("e2")]);
        Assert.Null(game.Board[Position.Parse("e4")]);
        Assert.False(game.Undo());
    }
}
