# AGENTS.md — emulatorAVR

## Goal

Build a binary-compatible AVR emulator for the ATmega328P-class processor used by Arduino Uno.

First milestone: CLI runner for externally compiled firmware with tracing of AVR registers and Arduino Uno ports.

## Stack

- C# / .NET
- MSTest
- Moq
- FluentAssertions
- NLog
- Optional UI: Avalonia

## Kilo Code model profile

Use this repository with a weak executor model by default.

Recommended executor profile:

- model: Devstral-class coding model, including Devstral 2 class models
- temperature: `0.0` to `0.1`
- mode: one-issue execution
- planning depth: shallow and explicit
- retry policy: at most one retry after a failed command
- command policy: run only the commands required by the current issue
- context policy: read `AGENTS.md`, `CONTEXT.md`, `docs/TASKS.md`, and the assigned GitHub Issue before editing files

Do not use the weak executor model for broad architecture decisions. Use it only for narrow implementation tasks with explicit files, commands, and acceptance criteria.

When Devstral or a similar weak model executes a task, it must:

1. State the issue number being executed.
2. List the exact files it intends to touch.
3. Check whether those files already exist before creating replacements.
4. Add or update tests before production code where practical.
5. Run `dotnet build` and `dotnet test` once after changes.
6. Stop on repeated failures instead of looping.
7. Report the failing command and relevant error output.

## Devstral limitations and safeguards

Treat Devstral-class models as weak executors, not architects.

Allowed use:

- execute one GitHub Issue at a time;
- edit explicitly named files;
- apply mechanical layout corrections;
- add tests from explicit criteria;
- implement a narrow class or a narrow instruction group;
- run validation commands and report results.

Forbidden use unless a human-designed issue explicitly permits it:

- decide emulator architecture;
- expand milestone scope;
- infer AVR instruction semantics without references;
- perform broad refactoring;
- move files outside the requested correction;
- guess project layout;
- implement CPU, memory, ports, firmware loaders, or UI in a documentation/setup issue;
- claim completion without checking the issue checklist.

Required task shape for Devstral:

```text
1. Read AGENTS.md, CONTEXT.md, docs/TASKS.md, and the assigned issue.
2. Restate the issue number.
3. List files to edit.
4. List files or areas that must not be changed.
5. Make the smallest change that satisfies the issue.
6. Run the required commands.
7. Stop after one failed retry.
8. Report PASS/FAIL for every checklist item.
```

## Test command anti-loop policy

Devstral-class models have a known tendency to enter diagnostic loops when `dotnet test` prints only restore messages or unexpectedly short output.

This pattern is forbidden:

```bash
dotnet test --logger "console;verbosity=detailed" 2>&1 | tail -50
dotnet test --logger "console;verbosity=detailed" 2>&1 | grep -i "test"
dotnet test --logger "console;verbosity=detailed" 2>&1 | cat
dotnet test --logger "console;verbosity=detailed" 2>&1 | wc -l
dotnet test --logger "console;verbosity=detailed" 2>&1 | cat -A
dotnet test --logger "console;verbosity=detailed" 2>&1 | od -c
```

Do not inspect test output by repeatedly piping it through `tail`, `grep`, `cat`, `wc`, `od`, `head`, or similar tools. That is considered a loop, not debugging.

Required behavior when `dotnet test` output is suspiciously short:

1. Do not claim success from output text alone.
2. Check the process exit code if available.
3. Run at most one structured diagnostic command.
4. If still unclear, stop and report `Can close issue: NO` with the exact command output.

Allowed validation sequence:

```powershell
dotnet sln list
dotnet build --no-restore
dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
```

If tests appear to hang or emit only restore output, use one diagnostic command only:

```powershell
dotnet test --no-restore --blame-hang --blame-hang-timeout 60s --logger "trx;LogFileName=test-results.trx"
```

After that, stop. Do not run another output-inspection pipeline.

Interpretation rule:

- `dotnet test` with normal test summary and exit code `0` = PASS.
- `dotnet test` with exit code non-zero = FAIL.
- `dotnet test` with no summary and unclear exit status = UNKNOWN, not PASS.
- user interruption or aborted command = UNKNOWN/FAIL, not PASS.

A final report may say `dotnet test: PASS` only if the command completed normally with exit code `0` and the solution contains the expected test projects.

## Repository layout rule

Use this layout for Phase 00 and all later work:

```text
src/EmulatorAVR.Core/
src/EmulatorAVR.Cli/
tests/EmulatorAVR.Core.Tests/
tests/EmulatorAVR.Cli.Tests/
samples/firmware/
docs/
```

Do not place test projects under `src/`. If a generated project appears under `src/EmulatorAVR.*.Tests`, treat that as a layout defect and move it to `tests/` before claiming the issue is complete.

## Rules

- Put CPU logic only in `src/EmulatorAVR.Core`.
- CLI and UI must call the same core API.
- Every implementation task must add or update tests.
- Use deterministic execution. Do not bind CPU tests to wall-clock time.
- Use `ushort` for AVR opcode words.
- Use `byte` for 8-bit registers and memory values.
- Unsupported opcodes must fail deterministically until implemented.
- All work must be tracked in GitHub Issues.
- Avoid infinite loops when running tests. If a command appears to be stuck or repeating, abort and analyze the output before retrying.

## Target CLI

```powershell
dotnet run --project src/EmulatorAVR.Cli -- run --mcu atmega328p --firmware samples/firmware/blink.hex --max-cycles 100000 --trace registers,ports
```

## First milestone scope

Model:

- `R0..R31`
- `SREG`: `I T H S V N Z C`
- word-addressed program counter
- cycle counter
- flash memory
- data memory
- Intel HEX loader
- Arduino Uno port registers: `PORTB`, `PORTC`, `PORTD`, `DDRB`, `DDRC`, `DDRD`, `PINB`, `PINC`, `PIND`

Do not implement in milestone 1 unless explicitly requested:

- ADC accuracy
- full timer waveform behavior
- USART electrical timing
- bootloader protocol

## Required checks

```powershell
dotnet build
dotnet test
```

Completion requires passing tests, updated docs for changed behavior, and no unrelated formatting churn.
