# Project Plan

## 1. Task Overview
* Build a two-player chess game as a Windows desktop app in C# with WinForms.
* It must support standard rules, move validation, check and checkmate detection, castling, en passant and pawn promotion.
* Every change needs a preview and your explicit approval first.
* Every change must also be recorded in `Plan.md` and `Worklog.md`.

## 2. Current Status
* Phase: implementation complete. The solution builds with 0 warnings and 0 errors, and all 55 unit tests pass.
* The UI has been compiled but not run, because WinForms needs Windows.
* Work is committed to branch `bc/7cf10857`. No pull request is open.

## 3. Completed Steps, in Order
1. Created `AgentRules.md`, `Plan.md` and `Worklog.md` after approval.
2. Proposed a React/TypeScript structure for a chess app.
   * The direction then changed to C# WinForms, so that structure is dropped.
3. The request for a WinForms design document and structure got no reply, because the model was unavailable.
4. The request to create a local branch named "Chess-demo" was declined.
   * The sandbox allows only branch `bc/7cf10857`, so all work stays there.
5. Presented the WinForms design document as a preview.
   * It covers three projects: Core, WinForms and Tests.
   * It was later saved to `Docs/DesignDocument.md`.
6. Created the empty folders and files only, with no code.
   * The first attempt created wrongly named brace folders.
   * They were deleted and everything was recreated correctly.
   * Verified that all paths exist, that no names contain braces, and that all new files are empty.
7. The request for a complete end-to-end project was stopped before it was answered.
8. Rewrote this `Plan.md` to list every step in detail.
9. Wrote the full project description into `Readme.md` after approval.
10. Approved the end-to-end build. Installed the .NET 8 SDK in the home folder, wrote the project files and the solution, built Core, then the tests, then the UI, and fixed what failed (see `Worklog.md`).
11. Wrote `Docs/DesignDocument.md` and updated `Readme.md`.

## 4. TODO List
* [x] Create the three tracking files
* [x] Create the empty folder and file structure
* [x] Rewrite `Plan.md` with full detail
* [x] Save the design document into `Docs/DesignDocument.md`
* [x] Write the solution and project files
* [x] Implement the enums and models
* [x] Implement the board
* [x] Implement the move generators
* [x] Implement attack detection, move validation and status evaluation
* [x] Implement `ChessGame`, including castling, en passant and promotion
* [x] Implement the WinForms UI
* [x] Write the unit tests
* [x] Build, run the tests, and fix failures
* [x] Fill in `Readme.md` (project description written; update it as code is built)
* [x] Commit the work (commit 1eb00ca on branch bc/7cf10857)

## 5. Implementation Steps (all done)
1. Write `ChessGame.sln` and the three `.csproj` files, targeting .NET 8.
2. Write the enums and models: `PieceColor`, `PieceType`, `GameStatus`, `Position`, `Piece`, `Move` and `CastlingRights`.
3. Write `Board`, with starting setup, lookup and clone.
4. Write the move generators for pawn, knight, sliding pieces and king.
5. Write `AttackDetector`, `MoveValidator` and `GameStatusEvaluator`.
6. Write `ChessGame` and `MoveResult`.
   * This adds turn handling, castling rights, the en passant target, promotion and move history.
7. Write the UI: `BoardControl`, `GameController`, `MainForm`, `PromotionDialog`, `BoardTheme` and `PieceImageProvider`.
   * The piece images will be drawn in code, so no image files are needed.
8. Write the tests in `tests/Chess.Tests`.

## 6. Validation Steps
1. Check that the .NET SDK is available in the sandbox.
   * If it is not, say so and do not claim any test result.
2. Build `Chess.Core` and `Chess.Tests`, then run all tests.
3. Try to compile `Chess.WinForms`.
   * This may need the Windows targeting option on Linux.
4. Confirm that no file in the repo contains a table or a secret.
5. Report what was verified and what was not.
   * The UI itself cannot be run in this sandbox.

## 7. Known Limits and Decisions
* Only branch `bc/7cf10857` can be used.
* The WinForms UI cannot be launched here.
