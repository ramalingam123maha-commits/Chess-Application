namespace Chess.WinForms.Rendering;

/// <summary>All colours used to draw the board, kept in one place so the look is easy to change.</summary>
public static class BoardTheme
{
    public static readonly Color LightSquare = Color.FromArgb(240, 217, 181);
    public static readonly Color DarkSquare = Color.FromArgb(181, 136, 99);
    public static readonly Color SelectedSquare = Color.FromArgb(246, 246, 105);
    public static readonly Color LastMoveSquare = Color.FromArgb(205, 210, 106);
    public static readonly Color CheckSquare = Color.FromArgb(235, 97, 80);
    public static readonly Color LegalMoveDot = Color.FromArgb(120, 20, 85, 30);
    public static readonly Color LabelColor = Color.FromArgb(90, 60, 40);
}
