using Chess.Core.Boards;
using Chess.Core.Enums;

namespace Chess.Core.Rules;

/// <summary>Decides whether the side to move is in check, checkmated or stalemated.</summary>
public static class GameStatusEvaluator
{
    public static GameStatus Evaluate(Board board, PieceColor sideToMove)
    {
        bool inCheck = AttackDetector.IsInCheck(board, sideToMove);
        bool hasMove = MoveValidator.GetAllLegalMoves(board, sideToMove).Count > 0;

        if (hasMove) return inCheck ? GameStatus.Check : GameStatus.InProgress;
        return inCheck ? GameStatus.Checkmate : GameStatus.Stalemate;
    }
}
