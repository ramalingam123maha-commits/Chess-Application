namespace Chess.Core.Game;

/// <summary>The outcome of asking the game to make a move.</summary>
/// <param name="Success">True when the move was played.</param>
/// <param name="Error">Why the move was refused. Null on success.</param>
public sealed record MoveResult(bool Success, string? Error = null)
{
    public static MoveResult Ok() => new(true);
    public static MoveResult Fail(string error) => new(false, error);
}
