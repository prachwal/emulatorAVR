# Kilo command — continue after feedback

Use this command when an issue or PR has received a control note, review comment, or correction request and the implementation branch needs a focused follow-up pass.

## Purpose

Continue work after feedback without restarting the whole issue, expanding scope, or introducing unrelated changes.

This command is designed for weak executor models. Treat the model as a narrow correction executor, not an architect.

## Required input

Provide:

```text
Issue number: #<number>
Repository: https://github.com/prachwal/emulatorAVR
Branch: <current issue branch>
Feedback source: issue comment / PR review / control note
```

If there are multiple feedback comments, use the newest unresolved control note as the source of truth.

## Mandatory first steps

Before editing files:

1. Read `AGENTS.md`.
2. Read `CONTEXT.md`.
3. Read the assigned GitHub issue body.
4. Read all comments on the issue.
5. Read all PR review comments if a PR exists.
6. Identify the latest unresolved feedback/control note.
7. Restate the exact requested corrections.
8. List the exact files that need changes.
9. List files that must not be changed.
10. Inspect the current branch diff against `main`.

Do not start coding until the requested correction is clearly identified.

## Feedback priority

Use this priority order:

1. Latest explicit human control note.
2. Latest PR review requesting changes.
3. Latest issue comment that says `Can close issue #<number>: NO`.
4. Original issue body checklist.
5. Existing repository tests.

If feedback conflicts with the issue body, stop and report the conflict instead of guessing.

## Scope rules

- Fix only the feedback items.
- Do not redo unrelated completed parts of the issue.
- Do not add new features.
- Do not edit files outside the feedback scope.
- Do not modify CLI/Core/UI behavior unless feedback explicitly requires it.
- Do not add packages unless feedback explicitly requires it.
- Do not perform broad refactoring.
- Do not rename files unless feedback explicitly requires it.
- Do not create new architecture.
- Do not add placeholder classes named `Dummy`, `Sample`, `Example`, or equivalent.
- Do not add tests that only assert `true`.

## Branch hygiene

Before final report:

1. Compare branch against `main`.
2. Confirm branch is not stale/diverged.
3. Confirm changed files are still issue-scoped.
4. Remove unrelated files from the branch.
5. If unrelated useful files exist, preserve them on a separate branch instead of keeping them in the issue branch.

If the branch is diverged from `main`, rebase or recreate it before final validation.

## Correction workflow

1. Restate the issue number.
2. Quote or summarize the latest correction request in your own words.
3. List the specific checklist items currently failing.
4. Add or strengthen tests that would have caught the defect.
5. Apply the smallest production change required.
6. Run exact validation commands from the issue.
7. Post a corrected final report.

Prefer adding a regression test for every correction when practical.

## Validation command strictness

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
Do not replace the TRX test command with plain `dotnet test`.
Do not pipe output through `tail`, `grep`, `cat`, `wc`, `od`, `head`, or similar tools.

If only a shortened command was run, report the status as `UNKNOWN`, not `PASS`.

## Test-output anti-loop rule

Do not repeatedly inspect test output through shell pipelines.

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

## Required correction report

After applying feedback, post this exact report format to the issue or PR:

```text
Issue: #<number>
Branch:
- <branch-name>
Feedback addressed:
- ...
Files changed in this correction pass:
- ...
Commands run:
- dotnet sln list: PASS/FAIL
- dotnet build --no-restore: PASS/FAIL
- dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS/FAIL/UNKNOWN
Scope check:
- files outside feedback scope modified: YES/NO
- files outside issue scope modified: YES/NO
- unrelated behavior changed: YES/NO
- tests contain only Assert.IsTrue(true): YES/NO
Correctness check:
- regression test added or strengthened: YES/NO/NOT_APPLICABLE
- latest control note fully addressed: YES/NO
- branch clean against main: YES/NO
Can close issue #<number>: YES/NO
Notes:
- ...
```

If any required feedback item remains unresolved, report:

```text
Can close issue #<number>: NO
```

Do not claim completion from compilation alone.

## When to stop

Stop immediately and report `Can close issue #<number>: NO` if:

- feedback is ambiguous and cannot be resolved from the issue/PR context;
- required baseline files are missing;
- branch is diverged and cannot be rebased cleanly;
- validation fails after one focused retry;
- requested correction would require changing files forbidden by the issue;
- feedback asks for behavior outside the issue scope.

## Minimal response discipline

For correction passes, keep the final issue comment short and structured.

Do not include long explanations unless the feedback asks for them.

Do not include speculation.

Do not include unrelated implementation notes.