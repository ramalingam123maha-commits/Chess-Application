# Agent Rules and Operating Guidelines

These rules govern all interactions, workflows, code changes, and documentation practices for this project.

## 1. Analysis and Approval Gate
* Before making any modifications, thoroughly analyze the request.
* Present a detailed preview of all proposed modifications, specifically identifying all files that will be affected.
* Wait for explicit user approval before modifying or creating project files (except for these initial rule files when authorized).

## 2. Tracking Files Maintenance
Maintain three dedicated Markdown files in the root directory:
* **AgentRules.md**: Contains project operational rules, guidelines, and constraints.
* **Plan.md**: Contains task overview, TODO list, implementation steps, validation steps, and current status.
* **Worklog.md**: Contains chronological records of completed work, changes made, commands executed, issues encountered, resolutions, and outcomes.
* Whenever any change is made, automatically update both `Plan.md` and `Worklog.md`.

## 3. Markdown Formatting Standards
* All Markdown files must be human-readable and clean.
* Use proper headings, subheadings, bullet points, and numbered lists.
* **Do not use table-based formats** in any Markdown files.

## 4. Code Quality and Documentation
* Add clear comments and explanations to code where appropriate to maximize readability and maintainability.
* Match the existing language, framework, naming conventions, and style of the project.
* Avoid over-engineering; implement simple, focused, and secure solutions.

## 5. Validation and Reporting
* After completing approved modifications, execute relevant tests or verification steps to validate the results.
* Update `Plan.md` and `Worklog.md` with the latest status, changes, and verification outcomes.
* Provide a comprehensive summary of completed work to the user.
