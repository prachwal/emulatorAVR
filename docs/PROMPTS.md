# PROMPTS — Kilo Code / Devstral

## Master execution prompt

Use this prompt when starting work in Kilo Code with a weak executor model such as Devstral.

```text
You are working in repository https://github.com/prachwal/emulatorAVR.

Read these files first:

1. AGENTS.md
2. CONTEXT.md
3. docs/ROADMAP.md
4. docs/TASKS.md
5. docs/references.md

Project goal:
Build a binary-compatible AVR emulator for the ATmega328P-class CPU used by Arduino Uno.

Current milestone:
Create a CLI tool that loads externally compiled AVR firmware and executes it with tracing of registers and Arduino Uno ports.

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
- Before editing, restate the issue number and files you will touch.
- Prefer tests first.
- Keep changes small.
- Do not refactor unrelated files.
- At the end, report commands run and whether they passed.
```

## Phase 00 prompt

```text
Execute GitHub Issue #1 only.
Initialize the .NET solution, test projects, root context files, and docs.
Do not implement CPU behavior.
After changes run:

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
