using Chess.Core.Enums;
using Chess.WinForms.Rendering;

namespace Chess.WinForms.Forms;

/// <summary>A small dialog that asks which piece a pawn should become.</summary>
public sealed class PromotionDialog : Form
{
    /// <summary>The piece the player picked. Only meaningful when the dialog returns OK.</summary>
    public PieceType Choice { get; private set; } = PieceType.Queen;

    public PromotionDialog()
    {
        Text = "Promote pawn";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(4 * 80 + 10, 100);

        var choices = new[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight };
        for (int i = 0; i < choices.Length; i++)
        {
            var type = choices[i];
            var button = new Button
            {
                Text = PieceImageProvider.GetGlyph(type),
                Font = new Font("Segoe UI Symbol", 32f),
                ForeColor = Color.Black, // Buttons show the symbol in black for both sides.
                Bounds = new Rectangle(5 + i * 80, 10, 75, 80),
                AccessibleName = type.ToString()
            };
            // Hover text names the piece for people who do not recognise the symbol.
            new ToolTip().SetToolTip(button, type.ToString());
            button.Click += (_, _) =>
            {
                Choice = type;
                DialogResult = DialogResult.OK;
            };
            Controls.Add(button);
        }
    }
}
