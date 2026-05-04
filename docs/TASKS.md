# TASKS — emulatorAVR

Backlog is tracked in GitHub Issues. Execute issues in numerical order unless explicitly reprioritized.

## Open phases

| Phase | Issue | Scope |
|---|---:|---|
| 00 | #1 | Project initialization, solution, tests, `AGENTS.md`, `CONTEXT.md` |
| 01 | #2 | Core CPU state and memory skeleton |
| 02 | #3 | Intel HEX and raw binary firmware loaders |
| 03 | TBD | Instruction decode baseline |
| 04 | TBD | Execution loop, tracing, CLI |
| 05 | TBD | Arduino Uno port model |
| 06 | TBD | Optional Avalonia viewer |

## Execution rule for weak models

For Devstral or similar weak executors:

1. Select one issue.
2. Read `AGENTS.md`, `CONTEXT.md`, and this file.
3. Restate files to touch.
4. Write or update tests.
5. Implement only the issue scope.
6. Run `dotnet build`.
7. Run `dotnet test`.
8. Report commands and results.

Do not merge multiple phases into one task.
