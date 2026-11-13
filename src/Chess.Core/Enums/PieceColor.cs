namespace Chess.Core.Enums;

/// <summary>The two sides in a chess game.</summary>
public enum PieceColor
{
    White,
    Black
}

public static class PieceColorExtensions
{
    /// <summary>Returns the other side.</summary>
    public static PieceColor Opposite(this PieceColor color) =>
        color == PieceColor.White ? PieceColor.Black : PieceColor.White;
}
