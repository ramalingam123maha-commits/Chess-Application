using Chess.Core.Enums;
using Chess.Core.Game;
using Chess.Core.Models;

namespace Chess.Tests;

public class CastlingTests
{
    [Fact]
    public void WhiteCanCastleKingSide()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/8/8/8/4K2R w K - 0 1");
        Assert.True(game.Play("e1g1").Success);
        Assert.Equal(PieceType.King, game.Board[Position.Parse("g1")]!.Type);
        Assert.Equal(PieceType.Rook, game.Board[Position.Parse("f1")]!.Type);
        Assert.Null(game.Board[Position.Parse("h1")]);
    }

    [Fact]
    public void WhiteCanCastleQueenSide()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/8/8/8/R3K3 w Q - 0 1");
        Assert.True(game.Play("e1c1").Success);
        Assert.Equal(PieceType.King, game.Board[Position.Parse("c1")]!.Type);
        Assert.Equal(PieceType.Rook, game.Board[Position.Parse("d1")]!.Type);
        Assert.Null(game.Board[Position.Parse("a1")]);
    }

    [Fact]
    public void BlackCanCastleBothSides()
    {
        var game = ChessGame.FromFen("r3k2r/8/8/8/8/8/8/4K3 b kq - 0 1");
        Assert.Contains("g8", game.Destinations("e8"));
        Assert.Contains("c8", game.Destinations("e8"));
    }

    [Fact]
    public void Castling_NotAllowed_WhenPieceIsBetween()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/8/8/8/4KB1R w K - 0 1");
        Assert.DoesNotContain("g1", game.Destinations("e1"));
    }

    [Fact]
    public void Castling_NotAllowed_QueenSideWhenKnightIsBetween()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/8/8/8/RN2K3 w Q - 0 1");
        Assert.DoesNotContain("c1", game.Destinations("e1"));
    }

    [Fact]
    public void Castling_NotAllowed_WhenKingIsInCheck()
    {
        var game = ChessGame.FromFen("4r2k/8/8/8/8/8/8/4K2R w K - 0 1");
        Assert.DoesNotContain("g1", game.Destinations("e1"));
    }

    [Fact]
    public void Castling_NotAllowed_WhenKingCrossesAttackedSquare()
    {
        // The black rook on f8 attacks f1, which the king would cross.
        var game = ChessGame.FromFen("5r1k/8/8/8/8/8/8/4K2R w K - 0 1");
        Assert.DoesNotContain("g1", game.Destinations("e1"));
    }

    [Fact]
    public void Castling_NotAllowed_WhenKingLandsOnAttackedSquare()
    {
        var game = ChessGame.FromFen("6rk/8/8/8/8/8/8/4K2R w K - 0 1");
        Assert.DoesNotContain("g1", game.Destinations("e1"));
    }

    [Fact]
    public void Castling_QueenSide_AllowedWhenOnlyTheRookCrossesAnAttackedSquare()
    {
        // b1 is attacked, but only the rook passes over it.
        var game = ChessGame.FromFen("1r5k/8/8/8/8/8/8/R3K3 w Q - 0 1");
        Assert.Contains("c1", game.Destinations("e1"));
    }

    [Fact]
    public void Castling_NotAllowed_AfterKingHasMoved()
    {
        var game = ChessGame.FromFen("r3k3/8/8/8/8/8/8/4K2R w K - 0 1");
        game.PlayAll("e1e2", "e8d8", "e2e1", "d8e8");
        Assert.DoesNotContain("g1", game.Destinations("e1"));
    }

    [Fact]
    public void Castling_NotAllowed_AfterRookHasMoved()
    {
        var game = ChessGame.FromFen("4k3/8/8/8/8/8/8/4K2R w K - 0 1");
        game.PlayAll("h1h2", "e8d8", "h2h1", "d8e8");
        Assert.DoesNotContain("g1", game.Destinations("e1"));
    }

    [Fact]
    public void Castling_NotAllowed_AfterRookIsCaptured()
    {
        var game = ChessGame.FromFen("4k2r/8/8/8/8/8/8/4K2R w Kk - 0 1");
        game.PlayAll("h1h8", "e8f7", "h8h1", "f7e8");
        // The black rook was taken on h8, so black has lost king-side castling.
        game.PlayAll("e1e2");
        Assert.DoesNotContain("g8", game.Destinations("e8"));
    }
}
