namespace Chess.Core.Enums;

/// <summary>The state of the game from the point of view of the side to move.</summary>
public enum GameStatus
{
    /// <summary>The game continues and the side to move is not in check.</summary>
    InProgress,

    /// <summary>The side to move is in check but has at least one legal move.</summary>
    Check,

    /// <summary>The side to move is in check and has no legal move. The game is over.</summary>
    Checkmate,

    /// <summary>The side to move is not in check but has no legal move. The game is a draw.</summary>
    Stalemate
}
