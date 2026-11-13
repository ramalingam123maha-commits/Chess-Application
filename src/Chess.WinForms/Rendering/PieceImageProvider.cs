using Chess.Core.Enums;
using Chess.Core.Models;

namespace Chess.WinForms.Rendering;

/// <summary>
/// Supplies the picture for each piece. Pieces are drawn from Unicode chess symbols,
/// so the project needs no image files. Both colours use the solid glyph; the colour
/// comes from the fill, with an outline so white pieces stay visible on light squares.
/// </summary>
public static class PieceImageProvider
{
    /// <summary>The symbol for a piece type.</summary>
    public static string GetGlyph(PieceType type) => type switch
    {
        PieceType.King => "\u265A",
        PieceType.Queen => "\u265B",
        PieceType.Rook => "\u265C",
        PieceType.Bishop => "\u265D",
        PieceType.Knight => "\u265E",
        _ => "\u265F"
    };

    public static Color FillColor(PieceColor color) =>
        color == PieceColor.White ? Color.White : Color.FromArgb(30, 30, 30);

    public static Color OutlineColor(PieceColor color) =>
        color == PieceColor.White ? Color.FromArgb(30, 30, 30) : Color.FromArgb(220, 220, 220);

    /// <summary>
    /// Draws one piece centred in a square. The glyph is turned into a path so it can be
    /// filled and outlined separately.
    /// </summary>
    public static void Draw(Graphics g, Piece piece, Rectangle square)
    {
        float emSize = square.Height * 0.78f;
        using var family = new FontFamily("Segoe UI Symbol");
        using var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddString(GetGlyph(piece.Type), family, (int)FontStyle.Regular, emSize, Point.Empty,
            StringFormat.GenericTypographic);

        // Centre the glyph by moving its bounding box to the middle of the square.
        var bounds = path.GetBounds();
        using var move = new System.Drawing.Drawing2D.Matrix();
        move.Translate(
            square.X + (square.Width - bounds.Width) / 2f - bounds.X,
            square.Y + (square.Height - bounds.Height) / 2f - bounds.Y);
        path.Transform(move);

        using var fill = new SolidBrush(FillColor(piece.Color));
        using var outline = new Pen(OutlineColor(piece.Color), Math.Max(1.5f, square.Height / 28f))
        {
            LineJoin = System.Drawing.Drawing2D.LineJoin.Round
        };
        g.FillPath(fill, path);
        g.DrawPath(outline, path);
    }
}
