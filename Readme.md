# Chess Game (C# WinForms)

A two-player chess game for Windows desktop. Two people play on one computer and take turns.

## Project Status
- The rules engine is complete and tested: 55 unit tests pass.
- The WinForms interface is written and compiles. It has not been run, because WinForms needs Windows and development used a Linux sandbox.
- See `Docs/DesignDocument.md` for the full design.

## Features
1. Two players, White and Black, sharing one screen. White moves first.
2. All standard piece movement.
3. Move validation. The game rejects illegal moves, including any move that leaves your own king in check.
4. Check and checkmate detection.
5. Stalemate detection, because a stalemate also ends the game.
6. Castling on both sides.
7. En passant capture.
8. Pawn promotion. The player picks a Queen, Rook, Bishop or Knight.

## Out of Scope
- A computer opponent.
- Online play.
- Game clocks.
- Saving and loading games.

## Technology
- C# on .NET 8.
- Windows Forms for the interface. It runs only on Windows.
- xUnit for the tests.

## How the Code Is Organised
The project has three parts. The rules engine knows nothing about the UI, so tests can run without opening a window.

1. `Chess.Core` is the rules engine. It holds the board, the move rules and the game state.
2. `Chess.WinForms` is the desktop interface. It draws the board and sends mouse clicks to the engine.
3. `Chess.Tests` holds the unit tests for the rules engine.

The dependencies point one way: the UI uses the engine, and the tests use the engine. The engine uses neither.

## Folder Structure
- `Docs/` holds the design document.
- `src/Chess.Core/` holds the rules engine.
  - `Enums/` holds piece colours, piece types and game status.
  - `Models/` holds positions, pieces, moves and castling rights.
  - `Board/` holds the 8x8 board.
  - `Pieces/` holds one move generator for each kind of piece movement.
  - `Rules/` holds attack detection, move validation and game status checks.
  - `Game/` holds the main `ChessGame` class that the UI calls.
- `src/Chess.WinForms/` holds the interface.
  - `Forms/` holds the main window and the promotion dialog.
  - `Controls/` holds the custom board control.
  - `Controllers/` holds the class that turns clicks into moves.
  - `Rendering/` holds board colours and piece images.
  - `Resources/Pieces/` is an unused placeholder. Pieces are drawn from Unicode symbols.
- `tests/Chess.Tests/` holds the test files.

## How Move Validation Works
Validation runs in two stages.

1. Each piece generator lists the moves that fit its movement pattern and are not blocked.
2. The validator plays each move on a copy of the board. It discards the move if the mover's king is attacked afterwards.

The second stage also covers pinned pieces and stops the king from moving into check.

## Special Rules
- **Check:** the king's square is attacked by the opponent.
- **Checkmate:** the king is in check and the player has no legal move.
- **Stalemate:** the king is not in check and the player has no legal move.
- **Castling** is allowed only when all of these are true:
  1. The king and that rook have not moved.
  2. The squares between them are empty.
  3. The king is not in check.
  4. The king does not cross or land on an attacked square.
- **En passant:** after a pawn moves two squares, an enemy pawn beside it may capture it as if it had moved one square. This is allowed only on the very next move. The captured pawn is removed from its own square.
- **Promotion:** a pawn that reaches the last rank becomes a Queen, Rook, Bishop or Knight, chosen by the player.

## Playing the Game
1. Click one of your pieces. The board highlights it and marks its legal destinations.
2. Click a destination square to move.
3. If the move is a promotion, choose the new piece in the dialog.
4. The king's square turns red when it is in check.
5. When the game ends, a message shows the result and offers a new game.

The window also has a move list, a turn indicator, and buttons for New Game, Undo and Flip Board.

## Build and Test
You need the .NET 8 SDK. Run these commands from the repository root.

```
dotnet build ChessGame.sln
dotnet test tests/Chess.Tests
dotnet run --project src/Chess.WinForms
```

The last command runs the game and works on Windows only. On other systems the build still succeeds.

## Testing
All tests target `Chess.Core`. They cover piece movement, check, checkmate (including fool's mate and scholar's mate), stalemate, castling, en passant and promotion.

## Development Process
Work on this project follows the rules in `AgentRules.md`.
- `Plan.md` holds the task list and current status.
- `Worklog.md` records completed work, commands, problems and results.

## Known Limits
- WinForms runs only on Windows. The UI was compiled but never launched.
- The game does not detect draws by repetition, the fifty-move rule or insufficient material.
