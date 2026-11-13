using Chess.Core.Enums;
using Chess.Core.Game;
using Chess.Core.Models;

namespace Chess.WinForms.Controllers;

/// <summary>
/// Turns board clicks into chess actions. It remembers which piece is selected and which
/// squares it can reach, and asks the engine to play the move. It holds no chess rules.
/// </summary>
public sealed class GameController
{
    public ChessGame Game { get; private set; } = new();

    /// <summary>The selected square, or null when nothing is selected.</summary>
    public Position? Selected { get; private set; }

    /// <summary>Legal destinations for the selected piece.</summary>
    public IReadOnlyList<Move> SelectedMoves { get; private set; } = Array.Empty<Move>();

    /// <summary>Raised whenever the position or selection changes, so the view can redraw.</summary>
    public event EventHandler? StateChanged;

    public void NewGame()
    {
        Game = new ChessGame();
        ClearSelection();
    }

    public void Undo()
    {
        if (Game.Undo())
            ClearSelection();
    }

    /// <summary>
    /// Handles a click on a square.
    /// Clicking your own piece selects it. Clicking a highlighted square plays the move.
    /// Anything else clears the selection.
    /// </summary>
    /// <param name="square">The clicked square.</param>
    /// <param name="choosePromotion">Asks the player which piece a pawn becomes. Returns null if cancelled.</param>
    /// <returns>An error message if the engine refused a move, otherwise null.</returns>
    public string? Click(Position square, Func<PieceType?> choosePromotion)
    {
        if (Game.IsGameOver)
            return null;

        // A click on a reachable square plays the move.
        if (Selected is { } from && SelectedMoves.Any(m => m.To == square))
        {
            PieceType? promotion = null;
            if (Game.IsPromotionMove(from, square))
            {
                promotion = choosePromotion();
                if (promotion is null) return null; // The player cancelled the dialog.
            }

            var result = Game.TryMakeMove(new Move(from, square, promotion));
            ClearSelection();
            return result.Success ? null : result.Error;
        }

        // Otherwise select a piece of the side to move, or clear the selection.
        var moves = Game.GetLegalMoves(square);
        if (moves.Count > 0)
        {
            Selected = square;
            SelectedMoves = moves;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            ClearSelection();
        }
        return null;
    }

    private void ClearSelection()
    {
        Selected = null;
        SelectedMoves = Array.Empty<Move>();
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
