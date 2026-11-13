using Chess.Core.Enums;
using Chess.WinForms.Controllers;
using Chess.WinForms.Controls;

namespace Chess.WinForms.Forms;

/// <summary>The main window: the board, a turn label, the move list and the buttons.</summary>
public partial class MainForm : Form
{
    private readonly GameController _controller = new();
    private readonly BoardControl _board;

    public MainForm()
    {
        InitializeComponent();

        _board = new BoardControl(_controller) { Dock = DockStyle.Fill, BackColor = SystemColors.Control };
        Controls.Add(_board);
        _board.BringToFront(); // Fill the space left after the side panel and status bar.

        _board.SquareClicked += (_, square) =>
        {
            var error = _controller.Click(square, AskForPromotion);
            if (error is not null)
                MessageBox.Show(this, error, "Illegal move", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        };
        _controller.StateChanged += (_, _) => RefreshView();

        _newGameButton.Click += (_, _) => _controller.NewGame();
        _undoButton.Click += (_, _) => _controller.Undo();
        _flipButton.Click += (_, _) => _board.Flipped = !_board.Flipped;

        RefreshView();
    }

    /// <summary>Shows the promotion dialog. Returns null when the player closes it.</summary>
    private PieceType? AskForPromotion()
    {
        using var dialog = new PromotionDialog();
        return dialog.ShowDialog(this) == DialogResult.OK ? dialog.Choice : null;
    }

    /// <summary>Updates the labels and move list from the current game, and announces the end of the game.</summary>
    private void RefreshView()
    {
        var game = _controller.Game;

        _moveList.BeginUpdate();
        _moveList.Items.Clear();
        for (int i = 0; i < game.MoveHistory.Count; i++)
        {
            // Moves are numbered per pair: White's move starts a new number.
            string number = i % 2 == 0 ? $"{i / 2 + 1}. " : "   ";
            _moveList.Items.Add(number + game.MoveHistory[i]);
        }
        _moveList.EndUpdate();
        if (_moveList.Items.Count > 0)
            _moveList.TopIndex = _moveList.Items.Count - 1;

        string side = game.CurrentTurn.ToString();
        _turnLabel.Text = game.IsGameOver ? "Game over" : $"{side} to move";
        _undoButton.Enabled = game.MoveHistory.Count > 0;

        string winner = game.CurrentTurn.Opposite().ToString();
        _statusLabel.Text = game.Status switch
        {
            GameStatus.Check => $"{side} is in check.",
            GameStatus.Checkmate => $"Checkmate. {winner} wins.",
            GameStatus.Stalemate => "Stalemate. The game is a draw.",
            _ => $"{side} to move."
        };

        // Announce the end of the game once, when the final move has just been made.
        if (game.IsGameOver && !_gameOverShown)
        {
            _gameOverShown = true;
            BeginInvoke(() => MessageBox.Show(this, _statusLabel.Text + "\nChoose New Game to play again.",
                "Game over", MessageBoxButtons.OK, MessageBoxIcon.Information));
        }
        else if (!game.IsGameOver)
        {
            _gameOverShown = false;
        }
    }

    private bool _gameOverShown;
}
