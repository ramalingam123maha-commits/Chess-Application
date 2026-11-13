using Chess.Core.Enums;
using Chess.Core.Models;
using Chess.WinForms.Controllers;
using Chess.WinForms.Rendering;

namespace Chess.WinForms.Controls;

/// <summary>
/// Draws the board and reports which square was clicked. It always stays square:
/// the board fills the smaller of the control's width and height.
/// </summary>
public sealed class BoardControl : Control
{
    private readonly GameController _controller;
    private bool _flipped;

    /// <summary>Raised with the clicked square. The form passes it to the controller.</summary>
    public event EventHandler<Position>? SquareClicked;

    public BoardControl(GameController controller)
    {
        _controller = controller;
        // Double buffering stops the board flickering while it redraws.
        DoubleBuffered = true;
        SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        MinimumSize = new Size(320, 320);
        _controller.StateChanged += (_, _) => Invalidate();
    }

    /// <summary>When true, Black is drawn at the bottom.</summary>
    public bool Flipped
    {
        get => _flipped;
        set
        {
            _flipped = value;
            Invalidate();
        }
    }

    // Layout: a margin on the left and bottom holds the file and rank labels.
    private int Margin_ => Math.Max(16, Math.Min(Width, Height) / 24);
    private int SquareSize => (Math.Min(Width, Height) - Margin_) / 8;
    private Point Origin => new(Margin_, 0);

    /// <summary>The pixel rectangle of a square, taking the flip into account.</summary>
    private Rectangle SquareRect(Position p)
    {
        int column = _flipped ? 7 - p.Col : p.Col;
        int rowFromTop = _flipped ? p.Row : 7 - p.Row;
        return new Rectangle(Origin.X + column * SquareSize, Origin.Y + rowFromTop * SquareSize, SquareSize, SquareSize);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(BackColor);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var game = _controller.Game;
        Position? checkedKing = game.Status is GameStatus.Check or GameStatus.Checkmate
            ? game.Board.FindKing(game.CurrentTurn)
            : null;

        for (int row = 0; row < 8; row++)
        for (int col = 0; col < 8; col++)
        {
            var pos = new Position(row, col);
            var rect = SquareRect(pos);

            // a1 is a dark square, so a square is dark when row + col is even.
            Color colour = (row + col) % 2 == 0 ? BoardTheme.DarkSquare : BoardTheme.LightSquare;
            if (pos == checkedKing) colour = BoardTheme.CheckSquare;
            else if (pos == _controller.Selected) colour = BoardTheme.SelectedSquare;

            using (var brush = new SolidBrush(colour))
                g.FillRectangle(brush, rect);

            if (game.Board[pos] is { } piece)
                PieceImageProvider.Draw(g, piece, rect);
        }

        DrawLegalMoveMarkers(g);
        DrawLabels(g);
    }

    /// <summary>Dots on empty destinations, rings on squares where the move captures.</summary>
    private void DrawLegalMoveMarkers(Graphics g)
    {
        using var brush = new SolidBrush(BoardTheme.LegalMoveDot);
        using var ring = new Pen(BoardTheme.LegalMoveDot, Math.Max(3, SquareSize / 12f));

        foreach (var move in _controller.SelectedMoves.Select(m => m.To).Distinct())
        {
            var rect = SquareRect(move);
            bool captures = _controller.Game.Board[move] is not null;
            // En passant lands on an empty square but still captures, so it gets a ring too.
            captures |= _controller.SelectedMoves.Any(m => m.To == move && m.IsEnPassant);

            if (captures)
            {
                rect.Inflate(-4, -4);
                g.DrawEllipse(ring, rect);
            }
            else
            {
                int d = SquareSize / 3;
                g.FillEllipse(brush, rect.X + (rect.Width - d) / 2, rect.Y + (rect.Height - d) / 2, d, d);
            }
        }
    }

    /// <summary>Files a–h under the board and ranks 1–8 beside it.</summary>
    private void DrawLabels(Graphics g)
    {
        using var font = new Font(Font.FontFamily, Math.Max(8f, SquareSize / 6f), FontStyle.Bold);
        using var brush = new SolidBrush(BoardTheme.LabelColor);
        var centre = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        for (int i = 0; i < 8; i++)
        {
            var fileSquare = SquareRect(new Position(_flipped ? 7 : 0, i));
            g.DrawString(((char)('a' + i)).ToString(), font, brush,
                new Rectangle(fileSquare.X, SquareSize * 8, SquareSize, Margin_), centre);

            var rankSquare = SquareRect(new Position(i, 0));
            g.DrawString((i + 1).ToString(), font, brush,
                new Rectangle(0, rankSquare.Y, Margin_, SquareSize), centre);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButtons.Left) return;

        int column = (e.X - Origin.X) / SquareSize;
        int rowFromTop = (e.Y - Origin.Y) / SquareSize;
        if (e.X < Origin.X || e.Y < 0 || column > 7 || rowFromTop > 7) return;

        var square = new Position(_flipped ? rowFromTop : 7 - rowFromTop, _flipped ? 7 - column : column);
        SquareClicked?.Invoke(this, square);
    }
}
