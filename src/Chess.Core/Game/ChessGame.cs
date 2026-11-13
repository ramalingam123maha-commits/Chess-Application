using Chess.Core.Boards;
using Chess.Core.Enums;
using Chess.Core.Models;
using Chess.Core.Rules;

namespace Chess.Core.Game;

/// <summary>
/// The one class the UI talks to. It owns the board, the side to move, the move history
/// and the game status, and refuses illegal moves.
/// </summary>
public sealed class ChessGame
{
    // Each history entry keeps the position before the move, which makes undo a simple restore.
    private readonly record struct Snapshot(Board Board, PieceColor Turn, GameStatus Status);

    private readonly List<Snapshot> _undoStack = new();
    private readonly List<string> _moveNotes = new();

    public Board Board { get; private set; }
    public PieceColor CurrentTurn { get; private set; }
    public GameStatus Status { get; private set; }

    /// <summary>Simple coordinate notes such as "e2-e4", one per move played.</summary>
    public IReadOnlyList<string> MoveHistory => _moveNotes;

    /// <summary>True once the game is over by checkmate or stalemate.</summary>
    public bool IsGameOver => Status is GameStatus.Checkmate or GameStatus.Stalemate;

    /// <summary>Starts a standard game with White to move.</summary>
    public ChessGame() : this(Board.CreateStartPosition(), PieceColor.White)
    {
    }

    /// <summary>Starts from any position. Used by the tests to set up exact situations.</summary>
    public ChessGame(Board board, PieceColor turn)
    {
        Board = board;
        CurrentTurn = turn;
        Status = GameStatusEvaluator.Evaluate(Board, CurrentTurn);
    }

    /// <summary>Starts a game from a FEN string.</summary>
    public static ChessGame FromFen(string fen)
    {
        var board = Board.FromFen(fen, out var turn);
        return new ChessGame(board, turn);
    }

    /// <summary>Legal moves for the piece on a square. Empty when it is not that side's turn.</summary>
    public IReadOnlyList<Move> GetLegalMoves(Position from)
    {
        if (IsGameOver || Board[from] is not { } piece || piece.Color != CurrentTurn)
            return Array.Empty<Move>();
        return MoveValidator.GetLegalMoves(Board, from);
    }

    /// <summary>
    /// Plays a move if it is legal. For a promotion the move must name the chosen piece.
    /// </summary>
    public MoveResult TryMakeMove(Move move)
    {
        if (IsGameOver)
            return MoveResult.Fail("The game is over.");

        var legal = GetLegalMoves(move.From)
            .FirstOrDefault(m => m.To == move.To && m.Promotion == move.Promotion);
        if (legal is null)
            return MoveResult.Fail($"{move.From}-{move.To} is not a legal move.");

        _undoStack.Add(new Snapshot(Board.Clone(), CurrentTurn, Status));
        Board.ApplyMove(legal);
        CurrentTurn = CurrentTurn.Opposite();
        Status = GameStatusEvaluator.Evaluate(Board, CurrentTurn);

        var note = $"{legal.From}-{legal.To}";
        if (legal.Promotion is { } promoted) note += $"={promoted}";
        _moveNotes.Add(note);
        return MoveResult.Ok();
    }

    /// <summary>True when moving from a square to another needs the player to choose a promotion piece.</summary>
    public bool IsPromotionMove(Position from, Position to) =>
        GetLegalMoves(from).Any(m => m.To == to && m.Promotion is not null);

    /// <summary>Takes back the last move. Returns false when there is nothing to undo.</summary>
    public bool Undo()
    {
        if (_undoStack.Count == 0) return false;

        var last = _undoStack[^1];
        _undoStack.RemoveAt(_undoStack.Count - 1);
        _moveNotes.RemoveAt(_moveNotes.Count - 1);
        Board = last.Board;
        CurrentTurn = last.Turn;
        Status = last.Status;
        return true;
    }
}
