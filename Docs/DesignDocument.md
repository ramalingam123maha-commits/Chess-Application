# Chess Game Design Document

## 1. Purpose
This document describes how the C# WinForms chess game is built and why. The game is a local two-player chess game for Windows. Both players share one computer.

## 2. Requirements
1. Two players take turns, White first.
2. Every piece moves by the standard rules.
3. The game rejects illegal moves, including moves that leave your own king in check.
4. The game detects check and checkmate. It also detects stalemate, because stalemate ends the game.
5. Castling works on both sides.
6. En passant works on the move right after a two-square pawn advance.
7. A pawn that reaches the last rank is promoted to a piece the player chooses.

Out of scope: a computer opponent, online play, clocks, and saving games.

## 3. Architecture
The solution has three projects.

1. `Chess.Core` is the rules engine. It has no UI code, so tests run without a window.
2. `Chess.WinForms` is the desktop interface. It references `Chess.Core`.
3. `Chess.Tests` holds the xUnit tests. It references `Chess.Core`.

Dependencies point one way: the UI and the tests use the engine, and the engine uses neither.

Technology: C# on .NET 8, Windows Forms, xUnit.

## 4. Chess.Core Design

### 4.1 Coordinates
`Position` holds a row and a column. Row 0 is rank 1 and column 0 is file a, so `(0, 4)` is e1. `Position.Parse("e4")` and `ToString()` convert to and from algebraic names.

### 4.2 Board
`Board` holds the 8x8 grid of pieces. It also holds two facts that are part of a position but cannot be seen on the grid:
- The en passant target, which is the square a pawn skipped on the last move.
- The castling rights.

Because these live on the board, a cloned board is a complete position. The move validator depends on this.

`Board.ApplyMove` does the mechanical work of a move:
1. For en passant, it removes the captured pawn from the square beside the mover.
2. For castling, it moves the rook next to the king.
3. It revokes castling rights when a king moves, or when a rook leaves or is captured on its start corner.
4. It sets the en passant target after a two-square pawn move, and clears it after any other move.
5. For promotion, it puts the chosen piece on the destination square.

`Board.FromFen` reads a position from FEN text. The tests use it to set up exact situations.

### 4.3 Move generation
Each generator lists pseudo-legal moves. These follow the piece's pattern and respect blocking pieces, but may still expose the mover's king.
- `PawnMoveGenerator`: one and two steps forward, diagonal captures, en passant, and four moves (Queen, Rook, Bishop, Knight) for each move to the last rank.
- `KnightMoveGenerator`: eight jumps.
- `SlidingMoveGenerator`: rook, bishop and queen lines. The piece type picks the directions.
- `KingMoveGenerator`: one step in any direction, plus castling.

### 4.4 Move validation
`MoveValidator` turns pseudo-legal moves into legal moves in two steps:
1. Play the move on a copy of the board.
2. Keep the move only if the mover's king is not attacked afterwards.

This one test also covers pinned pieces, a king stepping into check, and ignoring a check.

`AttackDetector` answers "is this square attacked?" by looking outward from the square for enemy pieces. It does not call the move generators. The king generator needs it for castling, and calling the generators would make the two loop forever.

### 4.5 Game status
`GameStatusEvaluator` checks the side to move:
- In check with a legal move: Check.
- In check with no legal move: Checkmate.
- Not in check with no legal move: Stalemate.
- Otherwise: InProgress.

### 4.6 Special rules
Castling is allowed only when all of these hold:
1. The king and that rook have not moved.
2. The squares between them are empty.
3. The king is not in check.
4. The king does not cross or land on an attacked square.

On the queen side, the rook may pass over an attacked b-file square. Only the king's path matters.

En passant is allowed only on the move right after a two-square pawn advance. The captured pawn is removed from its own square, not from the destination. The validator rejects an en passant capture that would expose the king along the rank.

Promotion produces one move per choice. A promotion move without a choice is refused.

### 4.7 ChessGame
`ChessGame` is the only class the UI uses. It offers:
- `CurrentTurn`, `Status`, `IsGameOver` and `MoveHistory`.
- `GetLegalMoves(position)`, which is empty when the square is not the mover's piece.
- `TryMakeMove(move)`, which returns a `MoveResult` with an error message on failure.
- `IsPromotionMove(from, to)`, so the UI knows when to ask for a choice.
- `Undo()`, which restores a saved copy of the position before the last move.

## 5. Chess.WinForms Design
- `Program` starts the application.
- `MainForm` shows the board, a turn label, a move list, a status bar, and New Game, Undo and Flip Board buttons. It announces checkmate and stalemate.
- `BoardControl` draws the board with GDI+. It is double buffered and always square. It shows rank and file labels, the selected square, dots on legal destinations, rings on captures, and a red king in check.
- `GameController` turns clicks into actions. A click on your own piece selects it. A click on a marked square plays the move. It holds no chess rules.
- `PromotionDialog` asks for Queen, Rook, Bishop or Knight.
- `BoardTheme` keeps the colours in one place.
- `PieceImageProvider` draws pieces from Unicode chess symbols, so no image files are needed.

## 6. Interaction Flow
1. The player clicks a piece. The board marks its legal destinations.
2. The player clicks a marked square.
3. For a promotion, the dialog opens first.
4. The controller asks the engine to play the move, and the board redraws.
5. At checkmate or stalemate, a message shows the result.

## 7. Testing
There are 55 tests, all on `Chess.Core`.
- Piece movement, turn order and undo.
- Check: pins, escaping, blocking, capturing the attacker, and kings that cannot touch.
- Checkmate: fool's mate, scholar's mate and a back-rank mate.
- Stalemate, and the difference between stalemate and checkmate.
- Castling: both sides and both colours, blocked paths, check, crossing or landing on an attacked square, a moved king, a moved rook, and a captured rook.
- En passant: allowed at once, refused a move later, both colours, illegal when it exposes the king, and undo.
- Promotion: all four choices, by capture, for Black, and promotion that gives checkmate.

## 8. Decisions and Deviations From the First Plan
1. The namespace for the board is `Chess.Core.Boards`. A namespace and a class both named `Board` clash.
2. Pieces are drawn from Unicode symbols, not image files. `Resources/Pieces/` is an unused placeholder.
3. Castling rights and the en passant target live on `Board`, not on `ChessGame`.
4. Undo restores a saved copy of the position. It is simple and the cost is small.
5. The move list uses coordinate notes such as `e2-e4`, not full algebraic notation.

## 9. Limits
- The game does not detect draws by repetition, the fifty-move rule or insufficient material.
- The UI has been compiled but not run. WinForms needs Windows, and development used a Linux sandbox.
