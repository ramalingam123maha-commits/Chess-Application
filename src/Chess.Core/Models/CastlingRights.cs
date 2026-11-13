using Chess.Core.Enums;

namespace Chess.Core.Models;

/// <summary>
/// Which castling options are still available. An option is lost for good once the king
/// or that rook moves, or once the rook is captured. This class does not check whether
/// castling is possible right now (empty squares, check); the king move generator does that.
/// </summary>
public sealed class CastlingRights
{
    public bool WhiteKingSide { get; set; }
    public bool WhiteQueenSide { get; set; }
    public bool BlackKingSide { get; set; }
    public bool BlackQueenSide { get; set; }

    public bool Has(PieceColor color, bool kingSide) => (color, kingSide) switch
    {
        (PieceColor.White, true) => WhiteKingSide,
        (PieceColor.White, false) => WhiteQueenSide,
        (PieceColor.Black, true) => BlackKingSide,
        _ => BlackQueenSide
    };

    /// <summary>Removes both options for one side. Used when its king moves.</summary>
    public void RevokeColor(PieceColor color)
    {
        if (color == PieceColor.White)
            WhiteKingSide = WhiteQueenSide = false;
        else
            BlackKingSide = BlackQueenSide = false;
    }

    /// <summary>
    /// Removes the option tied to a rook's starting corner. Called for the square a piece
    /// leaves and the square it lands on, so it covers both a rook move and a rook capture.
    /// </summary>
    public void RevokeForSquare(Position square)
    {
        if (square == new Position(0, 0)) WhiteQueenSide = false;
        else if (square == new Position(0, 7)) WhiteKingSide = false;
        else if (square == new Position(7, 0)) BlackQueenSide = false;
        else if (square == new Position(7, 7)) BlackKingSide = false;
    }

    public CastlingRights Clone() => (CastlingRights)MemberwiseClone();
}
