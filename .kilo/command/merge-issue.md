# Kilo command — merge issue

Use this command after an implementation issue reports `Can close issue #<number>: YES`.

## Purpose

Prepare, verify, merge, close, and clean up exactly one completed GitHub issue branch.

This command must not implement new functionality. It is only for final verification and repository hygiene.

## Required input

Provide:

```text
Issue number: #<number>
Repository: https://github.com/prachwal/emulatorAVR
Source branch: <issue-branch>
Target branch: main
```

If the issue body specifies a branch name, use that branch as the source branch.

## Mandatory first steps

Before creating or merging a PR:

1. Read the GitHub issue body.
2. Read all issue comments.
3. Confirm the latest completion report says:

   ```text
   Can close issue #<number>: YES
   ```

4. Compare source branch against `main`.
5. Confirm the branch is not stale or diverged from `main`.
6. Confirm changed files match the issue scope.
7. Confirm there are no unrelated files in the diff.

If any check fails, do not merge. Add a control note to the issue and report:

```text
Can merge issue #<number>: NO
```

## Branch readiness rules

The issue branch may be merged only if:

- branch is ahead of `main`;
- branch is not behind `main`;
- branch is not diverged from `main`;
- changed files match the issue scope;
- no unrelated files are present;
- validation commands pass or are explicitly allowed as `UNKNOWN` by the issue;
- issue report contains `Can close issue #<number>: YES`.

If branch is stale/diverged:

1. Rebase or recreate from current `main`.
2. Reapply only issue-scoped changes.
3. Rerun validation.
4. Post a new final report.
5. Do not merge until clean.

## Out-of-scope file handling

If the branch contains useful but unrelated files, do not merge them with the issue.

Move them to a preservation branch, for example:

```text
preserve/issue-<number>-out-of-scope-files
```

Then remove them from the issue branch.

Examples of files that usually must not be merged with feature issues unless explicitly allowed:

```text
.kilo/**
AGENTS.md
CONTEXT.md
docs/** unrelated to the issue
README.md unrelated to the issue
```

## Required validation commands

Run the exact validation commands from the issue body.

Default validation sequence:

```powershell
dotnet sln list
dotnet build --no-restore
dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
```

If the issue requires Avalonia startup validation, also run:

```powershell
dotnet run --project src/EmulatorAVR.Avalonia --no-build
```

Do not shorten commands.
Do not replace TRX test command with plain `dotnet test`.
Do not pipe output through `tail`, `grep`, `cat`, `wc`, `od`, `head`, or similar tools.

## PR creation

Create a PR only after readiness checks pass.

PR title format:

```text
Issue #<number>: <short issue title>
```

PR body must include:

```text
Closes #<number>

Validation:
- dotnet sln list: PASS
- dotnet build --no-restore: PASS
- dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS
```

If the issue requires Avalonia startup validation, include:

```text
- dotnet run --project src/EmulatorAVR.Avalonia --no-build: PASS/UNKNOWN
```

## Merge rules

Merge only if:

- PR diff contains only issue-scoped files;
- validation is PASS or issue-allowed UNKNOWN;
- no review/control note blocks the issue;
- no unresolved out-of-scope files remain;
- `Closes #<number>` is present in PR body.

Preferred merge method:

```text
squash
```

If repository policy prefers merge commits, use repository default.

## After merge cleanup

After successful merge:

1. Confirm issue is closed automatically by `Closes #<number>`.
2. If not closed, close the issue manually as completed.
3. Delete the merged source branch.
4. Confirm only expected long-lived branches remain.
5. Do not delete preservation branches unless explicitly requested.

## Final report

Report exactly:

```text
Issue: #<number>
Source branch:
- <issue-branch>
Target branch:
- main
PR:
- <PR URL or number>
Commands run:
- dotnet sln list: PASS/FAIL
- dotnet build --no-restore: PASS/FAIL
- dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS/FAIL/UNKNOWN
Merge:
- merged: YES/NO
- issue closed: YES/NO
- source branch deleted: YES/NO
Cleanup:
- out-of-scope files preserved separately: YES/NO/NOT_APPLICABLE
Notes:
- ...
```

If any merge condition is false, report:

```text
Can merge issue #<number>: NO
```

Do not merge partially complete or stale branches.