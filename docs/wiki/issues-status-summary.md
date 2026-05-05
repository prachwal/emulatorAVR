# Issues status summary

Generated: 2026-05-04
Repository: `prachwal/emulatorAVR`

> Note: GitHub Wiki repository `prachwal/emulatorAVR.wiki` was not accessible through the GitHub integration. This page is stored in the project repository under `docs/wiki/` as a wiki-ready Markdown summary.

## Executive summary

All issues `#1` through `#7` are currently closed in GitHub.

The project has progressed through the planned initial phases:

1. project bootstrap and agent context,
2. deterministic AVR CPU state and memory containers,
3. Intel HEX and raw binary firmware loading,
4. baseline instruction decode/execution,
5. CPU run loop, CLI command, and tracing,
6. Arduino Uno digital port register model,
7. optional Avalonia state and trace viewer.

The final reported validation level after Phase 06 is:

```text
dotnet sln list: PASS
dotnet build --no-restore: PASS
dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS — 138/138 tests
dotnet run --project src/EmulatorAVR.Avalonia --no-build: PASS — app starts, no startup exception
```

## Issue status table

| Issue | Phase | GitHub status | Reported result | Key outcome |
|---:|---|---|---|---|
| #1 | Phase 00 — initialization | Closed | YES | .NET solution, Core, CLI, test projects, docs and agent context created/cleaned up. |
| #2 | Phase 01 — CPU state and memory | Closed | YES | `AvrRegisters`, `StatusRegister`, `AvrCpuState`, `AvrCpu`, `ProgramMemory`, `DataMemory` implemented with tests. |
| #3 | Phase 02 — firmware loaders | Closed | YES | `FirmwareImage`, `IFirmwareLoader`, `IntelHexLoader`, `RawBinaryLoader` implemented with checksum and malformed input tests. |
| #4 | Phase 03 — instruction baseline | Closed | YES | `NOP`, `LDI`, `MOV`, `ADD` decode/execution implemented; MOV/ADD high-register formulas corrected; ADD flags tested. |
| #5 | Phase 04 — run loop, CLI, tracing | Closed | YES in completion reports; verify odd-byte final correction if needed | Deterministic run loop, CLI `run`, register tracing, port trace placeholder and loader selection implemented. |
| #6 | Phase 05 — Arduino Uno port model | Closed | YES | `PINB/DDRB/PORTB`, `PINC/DDRC/PORTC`, `PIND/DDRD/PORTD`, D0..D13 metadata, snapshots and immutable register views implemented. |
| #7 | Phase 06 — Avalonia viewer | Closed | YES | Optional Avalonia viewer added; snapshot immutability fixed via `ImmutableArray<byte>`; app startup validated. |

## Phase details

### #1 — Phase 00: initialization

Implemented/confirmed:

- `EmulatorAVR.sln`
- `src/EmulatorAVR.Core/`
- `src/EmulatorAVR.Cli/`
- `tests/EmulatorAVR.Core.Tests/`
- `tests/EmulatorAVR.Cli.Tests/`
- documentation files: `AGENTS.md`, `CONTEXT.md`, `README.md`, `docs/ROADMAP.md`, `docs/TASKS.md`, `docs/PROMPTS.md`, `docs/references.md`
- test layout corrected: test projects are under `tests/`, not `src/`
- placeholder `DummyLibrary` and dummy tests removed in cleanup

Reported validation:

```text
dotnet sln list: PASS
dotnet build --no-restore: PASS
dotnet test --no-build: PASS — 2/2 tests after cleanup
Can close issue #1: YES
```

### #2 — Phase 01: deterministic AVR CPU state

Implemented:

- `AvrRegisters` — exactly 32 8-bit registers, bounds checks, defensive snapshot
- `StatusRegister` — raw SREG byte and flags `I/T/H/S/V/N/Z/C`
- `AvrCpuState` — word-addressed `ProgramCounter`, `CycleCount`, registers, SREG
- `AvrCpu` — minimal wrapper with reset behavior
- `ProgramMemory` — `ushort` instruction words by word address
- `DataMemory` — ATmega328P-class memory layout covering `0x0000..0x08FF`

Corrections:

- Added explicit SREG bit-mask test for all flags.

Reported validation:

```text
dotnet sln list: PASS
dotnet build --no-restore: PASS
dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS — 33/33 tests
Can close issue #2: YES
```

### #3 — Phase 02: firmware loaders

Implemented:

- `FirmwareImage` with base address and defensive byte storage
- `IFirmwareLoader`
- `IntelHexLoader`
- `RawBinaryLoader`
- `samples/firmware/README.md`

Coverage:

- Intel HEX data records and EOF records
- checksum validation
- malformed input rejection
- unsupported record type rejection
- raw binary loading with explicit base address

Correction:

- EOF checksum validation added.

Reported validation:

```text
dotnet sln list: PASS
dotnet build --no-restore: PASS
dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS — 47/47 tests
Can close issue #3: YES
```

### #4 — Phase 03: instruction decode and execution baseline

Implemented:

- `InstructionKind`
- `Instruction`
- `InstructionDecoder`
- `ExecutionResult`
- `InstructionExecutor`

Supported instruction subset:

- `NOP`
- `LDI Rd,K` for `R16..R31`
- `MOV Rd,Rr`
- `ADD Rd,Rr`

Corrections:

- Fixed MOV/ADD register extraction formulas.
- Added high-register MOV/ADD tests.
- Strengthened ADD `H` and `S` flag tests.

Reported validation:

```text
dotnet sln list: PASS
dotnet build --no-restore: PASS
dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS — 69/69 tests
Can close issue #4: YES
```

### #5 — Phase 04: run loop, CLI, deterministic tracing

Implemented:

- `StopReason`
- `RunOptions`
- `RunResult`
- `AvrRunLoop`
- `RegisterTraceEntry`
- `PortTraceEntry`
- `TraceFrame`
- `CliOptions`
- `CliRunner`
- `Program.cs` delegating to the runner

Behavior:

- deterministic instruction loop
- max-cycle stop
- unsupported-instruction stop
- program-end stop
- byte-addressed firmware base mapped to word-addressed PC
- odd byte count padded with `0x00`
- register trace diffs
- empty deterministic port trace placeholder
- CLI loader selection by extension: `.hex` vs `.bin`
- deterministic CLI errors for invalid args, non-existing firmware, unsupported extension, and loader failures

Corrections recorded in comments:

- Non-existing firmware had to be tested at Program/runner level, not parser-only.
- Unsupported opcode mutation test had to verify registers and SREG, not only PC/cycles.
- Loader exceptions had to be converted to stable CLI errors.
- Deterministic loader selection had to be tested by extension.
- A later control note requested stronger odd trailing byte validation.

Status note:

- GitHub search reports issue `#5` as closed.
- Earlier final correction report says `Can close issue #5: YES` with `95/95` tests passing.
- Retrieved comments also include a later control note saying the odd trailing byte test still needed strengthening. Because the issue is now closed, this was likely resolved later or closed manually; verify this specific test before expanding run-loop behavior.

Recommended follow-up:

```text
Before adding more execution semantics, inspect AvrRunLoopTests for a test proving that an odd trailing firmware byte is reached and interpreted as a padded word with high byte 0x00.
```

### #6 — Phase 05: Arduino Uno digital port register model

Implemented:

- `PortRegister`
- `PortSnapshot`
- `ArduinoPinMapping`
- `ArduinoUnoPortMap`
- `IoRegisterMap`

Port registers use ATmega328P data memory addresses:

```text
PINB  = 0x23
DDRB  = 0x24
PORTB = 0x25
PINC  = 0x26
DDRC  = 0x27
PORTC = 0x28
PIND  = 0x29
DDRD  = 0x2A
PORTD = 0x2B
```

Digital pin metadata:

```text
D0..D7   -> PORTD bits 0..7
D8..D13  -> PORTB bits 0..5
```

Corrections:

- `IoRegisterMap` now exposes stable register list/snapshot.
- `ArduinoUnoPortMap.Registers` no longer exposes internal mutable dictionary.
- Reduced I/O offsets `0x03..0x0B` explicitly tested as rejected.
- Analog pin A0 / pin 14 explicitly tested as not exposed.
- `PortSnapshot` immutability strengthened.
- `PortRegister` constructor null/empty/whitespace validation tested.

Reported validation:

```text
dotnet sln list: PASS
dotnet build --no-restore: PASS
dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS — 129/129 tests
Can close issue #6: YES
```

### #7 — Phase 06: optional Avalonia state viewer

Implemented:

- `EmulatorStateSnapshot`
- `EmulatorStateSnapshotTests`
- `src/EmulatorAVR.Avalonia/`
- `MainWindowViewModel`
- `MainWindow.axaml`
- `docs/avalonia.md`
- solution updated with Avalonia project

UI scope:

- PC display
- cycle count display
- SREG raw value and flags
- `R0..R31` display
- port placeholder section
- no emulator logic in UI
- no firmware parsing in UI
- no run-loop logic in UI

Corrections:

- `EmulatorStateSnapshot.Registers` changed from array-backed `IReadOnlyList<byte>` to `ImmutableArray<byte>`.
- Added test proving caller cannot mutate snapshot register data.
- Final Avalonia startup validation was explicitly rerun.

Final reported validation:

```text
dotnet sln list: PASS
dotnet build --no-restore: PASS
dotnet test --no-build --logger "trx;LogFileName=test-results.trx": PASS — 138/138 tests
dotnet run --project src/EmulatorAVR.Avalonia --no-build: PASS — app starts, blocked waiting for UI, killed after 3s, no startup exception
Can close issue #7: YES
```

## Current project state

Current milestone state:

- CLI firmware runner exists.
- Register tracing exists.
- Port tracing has a placeholder model and separate Arduino Uno port register model.
- Optional Avalonia viewer exists.
- Baseline instruction subset is still very small: `NOP`, `LDI`, `MOV`, `ADD`.

Important limitation:

- This is not yet a complete ATmega328P emulator.
- It can run only firmware composed of the currently implemented instruction subset unless unsupported-instruction stop behavior is desired.

## Recommended next issue

Create the next focused issue before expanding scope:

```text
Phase 07 — Integrate Arduino Uno port model with DataMemory/run-loop tracing
```

Suggested acceptance criteria:

- no new AVR instructions unless explicitly required;
- connect `IoRegisterMap`/`ArduinoUnoPortMap` to data-memory writes only through a controlled abstraction;
- trace changed port registers deterministically;
- keep CLI output stable;
- add tests proving port trace remains empty when disabled and contains changed registers when enabled;
- run exact validation sequence.

Secondary candidate:

```text
Phase 08 — Add next AVR instruction subset for compiled Arduino-style firmware
```

Recommended first instructions to consider only after checking fixture needs:

- `RJMP`
- `OUT`
- `IN`
- `SBI`
- `CBI`
- `INC/DEC`
- `BRNE/BREQ`

Do not implement a broad instruction set in one issue.

## UI refinement issues #15–#18 (Phase 07–10)

All four UI refinement phases are now merged:

- **#15 / Phase 07**: MVVM shell, display models, style resources, dashboard layout foundation
- **#16 / Phase 08**: refined CPU summary cards, SREG flag strip with active/inactive states, register grid with consistent two-digit hex formatting
- **#17 / Phase 09**: port register panel (PINB/DDRB/PORTB, PINC/DDRC/PORTC, PIND/DDRD/PORTD with data memory addresses), Arduino digital pin map (D0..D13 mapped to PORTD/PORTB)
- **#18 / Phase 10**: visual polish (consistent spacing, card styles, typography), responsive layout (MinWidth, scrollable), accessibility basics (ToolTip, text-based labels), documentation updates

The Avalonia viewer now displays CPU summary, SREG flags, registers, port registers, and pin mappings from static/demo data. No emulator logic, live execution, or CLI changes were introduced.
