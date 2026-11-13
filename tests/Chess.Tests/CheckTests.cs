using Chess.Core.Enums;
using Chess.Core.Game;

namespace Chess.Tests;

public class CheckTests
{
    [Fact]
    public void Status_IsCheck_WhenKingIsAttackedButCanEscape()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/8/8/4R3/4K3 b - - 0 1");
        Assert.Equal(GameStatus.Check, game.Status);
    }

    [Fact]
    public void PinnedPiece_CannotLeaveTheLineOfAttack()
    {
        // The white bishop on e2 is pinned to its king by the black rook on e8.
        var game = ChessGame.FromFen("4r2k/8/8/8/8/8/4B3/4K3 w - - 0 1");
        Assert.Empty(game.Destinations("e2"));
    }

    [Fact]
    public void King_CannotStepIntoCheck()
    {
        var game = ChessGame.FromFen("7k/8/8/8/8/8/r7/4K3 w - - 0 1");
        var squares = game.Destinations("e1");
        Assert.Contains("d1", squares);       // The rook on a2 does not attack rank 1, so d1 is safe.
        Assert.Contains("f1", squares);
        Assert.DoesNotContain("e2", squares); // The rook on a2 attacks the whole second rank.
        Assert.DoesNotContain("d2", squares);
        Assert.DoesNotContain("f2", squares);
    }

    [Fact]
    public void MustAnswerACheck()
    {
        // White is in check from the rook on e8. A rook move elsewhere does not help.
        var game = ChessGame.FromFen("4r2k/8/8/8/8/8/R7/4K3 w - - 0 1");
        Assert.False(game.Play("a2a3").Success);
        Assert.True(game.Play("e1d1").Success);
    }

    [Fact]
    public void CanBlockACheck()
    {
        var game = ChessGame.FromFen("4r2k/8/8/8/8/8/R7/4K3 w - - 0 1");
        Assert.True(game.Play("a2e2").Success);
    }

    [Fact]
    public void CanCaptureTheCheckingPiece()
    {
        var game = ChessGame.FromFen("7k/8/8/8/8/8/4r3/4K3 w - - 0 1");
        Assert.True(game.Play("e1e2").Success);
    }

    [Fact]
    public void KingsCannotStandNextToEachOther()
    {
        var game = ChessGame.FromFen("8/8/8/8/8/3k4/8/4K3 w - - 0 1");
        var squares = game.Destinations("e1");
        Assert.DoesNotContain("d2", squares);
        Assert.DoesNotContain("e2", squares);
    }
}
