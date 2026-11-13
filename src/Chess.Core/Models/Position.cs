namespace Chess.Core.Models;

/// <summary>
/// A square on the board. Row 0 is rank 1 (White's back rank) and row 7 is rank 8.
/// Column 0 is file a and column 7 is file h. So (0, 4) is e1.
/// </summary>
public readonly record struct Position(int Row, int Col)
{
    /// <summary>True when the square lies on the 8x8 board.</summary>
    public bool IsValid => Row >= 0 && Row < 8 && Col >= 0 && Col < 8;

    /// <summary>Returns the square that is the given number of rows and columns away.</summary>
    public Position Offset(int rowDelta, int colDelta) => new(Row + rowDelta, Col + colDelta);

    /// <summary>Algebraic name such as "e4".</summary>
    public override string ToString() =>
        IsValid ? $"{(char)('a' + Col)}{Row + 1}" : $"({Row},{Col})";

    /// <summary>Converts a name such as "e4" to a position.</summary>
    public static Position Parse(string text)
    {
        if (text is null || text.Length != 2)
            throw new FormatException($"'{text}' is not a square name.");

        var position = new Position(text[1] - '1', char.ToLowerInvariant(text[0]) - 'a');
        if (!position.IsValid)
            throw new FormatException($"'{text}' is not a square on the board.");
        return position;
    }
}
