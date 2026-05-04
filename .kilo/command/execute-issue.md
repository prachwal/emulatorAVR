# Kilo command — execute issue

Use this command to continue implementation work on one GitHub issue in `prachwal/emulatorAVR`.

## Purpose

Execute exactly one GitHub issue from start to validation without expanding scope.

This command is intended for weak executor models. Treat the model as a narrow implementer, not an architect.

## Required input

Provide:

```text
Issue number: #<number>
Repository: https://github.com/prachwal/emulatorAVR
Branch: issue-<number>-<short-description>
```

If the issue body already specifies a branch name, use the branch name from the issue body.

## Mandatory first steps

Before editing files:

1. Read `AGENTS.md`.
2. Read `CONTEXT.md`.
3. Read `docs/TASKS.md`.
4. Read `docs/ROADMAP.md`.
5. Read the assigned GitHub issue body.
6. Read all existing comments on the issue.
7. Inspect the current repository tree.
8. Confirm the required baseline files from the issue exist.

If any required baseline file is missing, stop and report:

```text
Can close issue #<number>: NO
```

Do not repair earlier phases unless the issue explicitly allows it.

## Scope rules

- Execute only the assigned issue.
- Do not start the next issue.
- Do not infer extra features.
- Do not edit files outside the issue scope.
- Do not perform broad refactoring.
- Do not change CLI/Core/UI behavior unless the issue explicitly allows it.
- Do not add packages unless the issue explicitly allows it.
- Do not add placeholder classes named `Dummy`, `Sample`, `Example`, or equivalent.
- Do not add tests that only assert `true`.

## Branch rules

1. Use the branch specified by the issue.
2. Do not work directly on `main`.
3. If the branch is stale or diverged from `main`, rebase or recreate it from current `main` before final validation.
4. Keep unrelated files out of the issue branch.
5. If unrelated useful files appear, move them to a separate preservation branch and remove them from the issue branch.

## Work sequence

1. Restate the issue number being executed.
2. List intended files to edit.
3. List forbidden files/areas from the issue.
4. Add or update tests first when practical.
5. Implement the smallest production change that satisfies the issue.
6. Run exact validation commands from the issue.
7. If a command fails, fix only the issue-related cause and retry once.
8. Stop after one failed retry and report the failing command.

## Validation command strictness

If the issue provides exact commands, run those exact commands.

Default command sequence when the issue does not specify otherwise:

```powershell
dotnet sln list
dotnet build --no-restore
dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
```

Do not replace these with shortened forms.

Forbidden substitutions:

```text
dotnet build              instead of dotnet build --no-restore
dotnet test               instead of dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
dotnet test --no-build    instead of dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
```

If only a shortened command was run, report the status as `UNKNOWN`, not `PASS`.

## Test-output anti-loop rule

Do not repeatedly pipe test output through diagnostic commands.

Forbidden loop pattern:

```bash
dotnet test ... | tail
dotnet test ... | grep
dotnet test ... | cat
dotnet test ... | wc
dotnet test ... | od
dotnet test ... | head
```

If output is suspiciously short, run at most one structured diagnostic command:

```powershell
dotnet test --no-restore --blame-hang --blame-hang-timeout 60s --logger "trx;LogFileName=test-results.trx"
```

Then stop and report the result.

## Final report format

Use the exact final report format from the issue body.

If the issue does not define one, use:

```text
Issue: #<number>
Branch:
- <branch-name>
Files created or changed:
- ...
Commands run:
- dotnet sln list: PASS/FAIL
- dotnet build --no-restore: PASS/FAIL
- dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS/FAIL/UNKNOWN
Scope check:
- files outside issue scope modified: YES/NO
- unrelated behavior changed: YES/NO
- tests contain only Assert.IsTrue(true): YES/NO
Can close issue #<number>: YES/NO
Notes:
- ...
```

If any required checklist item is false, report:

```text
Can close issue #<number>: NO
```

Do not claim completion from compilation alone.