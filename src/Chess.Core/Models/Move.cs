using Chess.Core.Enums;

namespace Chess.Core.Models;

/// <summary>
/// One move. The flags tell the board about the extra work a move needs:
/// castling also moves a rook, and en passant removes a pawn from a different square.
/// </summary>
/// <param name="From">The square the piece leaves.</param>
/// <param name="To">The square the piece lands on.</param>
/// <param name="Promotion">The piece a pawn becomes, or null when the move is not a promotion.</param>
/// <param name="IsCastling">True for a king move of two squares that also moves a rook.</param>
/// <param name="IsEnPassant">True when a pawn captures the pawn that just moved two squares.</param>
public sealed record Move(
    Position From,
    Position To,
    PieceType? Promotion = null,
    bool IsCastling = false,
    bool IsEnPassant = false);
