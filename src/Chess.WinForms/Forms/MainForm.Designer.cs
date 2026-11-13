#nullable enable
namespace Chess.WinForms.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;

    private Label _turnLabel = null!;
    private ListBox _moveList = null!;
    private Button _newGameButton = null!;
    private Button _undoButton = null!;
    private Button _flipButton = null!;
    private StatusStrip _statusStrip = null!;
    private ToolStripStatusLabel _statusLabel = null!;
    private Panel _sidePanel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    /// <summary>Builds the fixed parts of the window. The board control is added in the other file.</summary>
    private void InitializeComponent()
    {
        _turnLabel = new Label { Dock = DockStyle.Top, Height = 36, Font = new Font("Segoe UI", 14f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        _moveList = new ListBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 10f), IntegralHeight = false, AccessibleName = "Move history" };
        _newGameButton = new Button { Text = "New Game", Dock = DockStyle.Top, Height = 34 };
        _undoButton = new Button { Text = "Undo", Dock = DockStyle.Top, Height = 34 };
        _flipButton = new Button { Text = "Flip Board", Dock = DockStyle.Top, Height = 34 };
        _statusLabel = new ToolStripStatusLabel();
        _statusStrip = new StatusStrip();
        _statusStrip.Items.Add(_statusLabel);

        // Docked controls stack from the last added to the first, so buttons are added in reverse.
        _sidePanel = new Panel { Dock = DockStyle.Right, Width = 220, Padding = new Padding(8) };
        _sidePanel.Controls.Add(_moveList);
        _sidePanel.Controls.Add(_flipButton);
        _sidePanel.Controls.Add(_undoButton);
        _sidePanel.Controls.Add(_newGameButton);
        _sidePanel.Controls.Add(_turnLabel);

        SuspendLayout();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(820, 640);
        MinimumSize = new Size(640, 480);
        Text = "Chess";
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(_sidePanel);
        Controls.Add(_statusStrip);
        ResumeLayout(false);
        PerformLayout();
    }
}
