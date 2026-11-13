using Chess.Core.Enums;

namespace Chess.Core.Models;

/// <summary>A chess piece. Pieces never change; promotion replaces the pawn with a new piece.</summary>
public sealed record Piece(PieceType Type, PieceColor Color);
