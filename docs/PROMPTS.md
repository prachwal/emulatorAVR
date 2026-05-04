# PROMPTS — Kilo Code / DeepSeek V4 Flash

## Master execution prompt

Use this prompt when starting work in Kilo Code with a weak executor model such as DeepSeek V4 Flash or DeepSeek V4 Flash 2 models.

```text
You are working in repository https://github.com/prachwal/emulatorAVR.

Read these files first:

1. AGENTS.md
2. CONTEXT.md
3. docs/ROADMAP.md
4. docs/TASKS.md
5. docs/references.md
6. The assigned GitHub Issue

Project goal:
Build a binary-compatible AVR emulator for the ATmega328P-class CPU used by Arduino Uno.

Current milestone:
Create a CLI tool that loads externally compiled AVR firmware and executes it with tracing of registers and Arduino Uno ports.

Executor profile:
Treat yourself as a weak executor, not an architect.
Do not make architecture decisions.
Do not expand scope.
Do not infer missing AVR semantics.
Do not decide that an issue is complete without checking every acceptance criterion.

Hard constraints:
- Use C# / .NET.
- Unit tests use MSTest, Moq, FluentAssertions.
- Logging uses NLog.
- CPU logic must live only in EmulatorAVR.Core.
- CLI and optional Avalonia UI must consume the same Core API.
- Every implementation task must add or update tests.
- Run dotnet build and dotnet test before claiming completion.
- Do not implement broad behavior outside the assigned task.
- Do not invent AVR semantics. Use docs/references.md and authoritative manuals.
- Unsupported opcodes must fail deterministically until implemented.

Work style:
- Execute exactly one GitHub Issue at a time.
- Before editing, restate the issue number.
- Before editing, list files you will touch.
- Before editing, list files or areas you will not touch.
- Prefer tests first.
- Keep changes small.
- Do not refactor unrelated files.
- Do not move files unless the issue explicitly requires it.
- Stop after one failed retry.
- At the end, report commands run and whether they passed.
```

## DeepSeek V4 Flash task template

Use this shape for all future DeepSeek V4 Flash tasks:

```text
Execute only GitHub Issue #<number>.

Read first:
- AGENTS.md
- CONTEXT.md
- docs/TASKS.md
- docs/references.md
- GitHub Issue #<number>

Allowed files:
- <exact paths>

Forbidden areas:
- <exact paths or modules>

Non-goals:
- <things not to implement>

Commands to run:
- dotnet build
- dotnet test

Stop condition:
If a command fails, fix only the issue-related cause and retry once. If it fails again, stop and report the error.

Final report:
- Issue number
- Files changed
- Commands run with PASS/FAIL
- Checklist PASS/FAIL
- Can close issue: YES/NO
```

## Phase 00 prompt

```text
Execute GitHub Issue #1 only.
Initialize the .NET solution, test projects, root context files, and docs.
Do not implement CPU behavior.
Do not place test projects under src/.
Test projects must be under tests/.
After changes run:

dotnet sln list
dotnet build
dotnet test
```

## Phase 01 prompt

```text
Execute GitHub Issue #2 only.
Create deterministic core CPU state and memory skeleton.
Add tests for registers, SREG, PC, cycle counter, and memory bounds.
Do not implement instruction execution yet.
After changes run:

dotnet build
dotnet test
```

## Phase 02 prompt

```text
Execute GitHub Issue #3 only.
Implement Intel HEX loader first, then raw binary loader.
Add tests for valid HEX, EOF, invalid checksum, malformed record, and raw binary base address.
Do not parse ELF.
Do not execute CPU instructions.
After changes run:

dotnet build
dotnet test
```
