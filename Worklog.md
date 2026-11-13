# Worklog

## Entry: Setup Initial Tracking Documentation
* **Timestamp**: Initial Setup Session

### Tasks Undertaken
* Created `AgentRules.md` capturing all operating guidelines, preview/approval workflow requirements, documentation rules, and code quality expectations.
* Created `Plan.md` outlining project overview, status, TODO items, implementation roadmap, and validation steps without using table formatting.
* Created `Worklog.md` to record completed work, changes, executed commands, issues, and outcomes.

### Changes Made
* Created `/workspace/chess-application/AgentRules.md`
* Created `/workspace/chess-application/Plan.md`
* Created `/workspace/chess-application/Worklog.md`

### Commands Executed
* File creation via direct write operations.

### Issues Encountered
* None.

### Resolutions
* Not applicable.

### Outcomes
* All three core tracking documents have been successfully initialized following the non-table formatting rule and are ready for ongoing project workflows.

## Entry: Create Empty Folder and File Structure

### Tasks Undertaken
* Created the folder and file skeleton for the C# WinForms chess game, as requested. No code was written.

### Changes Made
1. Created folders: `Docs`, `src/Chess.Core` (Enums, Models, Board, Pieces, Rules, Game), `src/Chess.WinForms` (Forms, Controls, Controllers, Rendering, Resources/Pieces), and `tests/Chess.Tests`.
2. Created empty files: `ChessGame.sln`, `Docs/DesignDocument.md`, the three `.csproj` files, and all planned `.cs` files in Core, WinForms and Tests.
3. Added an empty `.gitkeep` in `Resources/Pieces` so the folder can be tracked by git.

### Commands Executed
* `mkdir -p` for the folders and `touch` for the files.
* `find` to list the result and to confirm every file is empty.

### Issues Encountered
* The first attempt ran in a shell that did not expand brace patterns such as `{a,b}`. It created folders and files with literal brace characters in their names, and most of the files were not created.

### Resolutions
* Removed the wrongly created `src`, `tests`, `Docs` and `ChessGame.sln`, then re-ran the same commands inside `bash -c`.
* Verified with `find` that all entries exist, that no name contains a brace, and that every new file is empty.

### Outcomes
* The structure matches section 4 of the proposed design. All files are zero bytes.

## Entry: Rewrite Plan.md

### Tasks Undertaken
* Rewrote `Plan.md` after approval, because the earlier version was not detailed enough.

### Changes Made
1. Replaced the content of `Plan.md` with seven sections: overview, status, completed steps in order, TODO list, implementation steps, validation steps, and known limits.

### Commands Executed
* A file write to `Plan.md`.

### Issues Encountered
* None.

### Outcomes
* `Plan.md` now lists every step taken so far and every step still pending. No code was changed.

## Entry: Write Readme.md

### Tasks Undertaken
* Wrote the full project description into `Readme.md` after approval.

### Changes Made
1. Replaced the near-empty `Readme.md` with sections for status, features, scope, technology, code organisation, folder structure, move validation, special rules, how to play, testing, development process and known limits.
2. Marked the Readme TODO item as done in `Plan.md` and added step 9 to the completed steps.

### Commands Executed
* A file write to `Readme.md`.
* A small Python script to update `Plan.md`.
* `grep` to check for tables.

### Issues Encountered
* None.

### Outcomes
* `Readme.md` describes the finished project and states clearly that no code exists yet. No code was changed.

## Entry: End-to-End Build of the Chess Game

### Tasks Undertaken
* Built the whole project after approval: rules engine, tests, WinForms interface and documents.

### Changes Made
1. Installed the .NET 8 SDK (8.0.428) in the home folder. The sandbox had no `dotnet`.
2. Replaced the empty `ChessGame.sln` with a real solution. Wrote the three `.csproj` files and a `.gitignore`.
3. `Chess.Core`: enums, `Position`, `Piece`, `Move`, `CastlingRights`, `Board`, four move generators, `AttackDetector`, `MoveValidator`, `GameStatusEvaluator`, `ChessGame` and `MoveResult`.
4. `Chess.Tests`: a helper file and seven test files with 55 tests.
5. `Chess.WinForms`: `Program`, `MainForm` (two files), `BoardControl`, `GameController`, `PromotionDialog`, `BoardTheme` and `PieceImageProvider`.
6. Wrote `Docs/DesignDocument.md` and added status, build commands and limits to `Readme.md`.
7. Updated `Plan.md`.

### Commands Executed
* The official `dotnet-install.sh` script, with channel 8.0, into `$HOME/.dotnet`.
* `dotnet new sln` and `dotnet sln add` for the three projects.
* `dotnet build` on Core, then on the whole solution.
* `dotnet test tests/Chess.Tests`, run three times.

### Issues Encountered and Resolutions
1. A class and a namespace were both named `Board`, which would clash.
   * Renamed the namespace to `Chess.Core.Boards` before the first build.
2. The first test build failed with hundreds of "Fact not found" errors.
   * Cause: the test project had no `using Xunit`.
   * Fix: added a global `<Using Include="Xunit" />` to `Chess.Tests.csproj`.
3. One test failed: `King_CannotStepIntoCheck` (54 passed, 1 failed).
   * Cause: the test was wrong, not the engine. It asserted that d1 was both unsafe and safe, and the unsafe line was a mistake.
   * Fix: removed the contradictory assertion and checked that d1 and f1 are allowed.
4. The UI build gave warning CS8669 in the designer file.
   * Fix: added `#nullable enable` to `MainForm.Designer.cs`.
5. `PromotionDialog` had an unused colour parameter and a meaningless colour expression.
   * Fix: removed the parameter and set the text colour to black.

### Outcomes
* Build: 0 warnings, 0 errors for the whole solution.
* Tests: 55 run, 55 passed, 0 failed.
* Not verified: the WinForms interface was compiled but never run, because WinForms needs Windows. Its drawing, clicking, dialogs and layout are untested.
