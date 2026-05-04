# ROADMAP — emulatorAVR

## Milestone 1: CLI firmware runner with tracing

Goal: run externally compiled AVR firmware for ATmega328P-class target and trace CPU registers plus Arduino Uno ports.

## Phase 00 — Project initialization

- Create .NET solution.
- Create Core and CLI projects.
- Create MSTest projects.
- Add Moq and FluentAssertions to tests.
- Add NLog to CLI.
- Add `AGENTS.md`, `CONTEXT.md`, and documentation.

Exit criteria:

- `dotnet build` passes.
- `dotnet test` passes.
- Root context files exist.

## Phase 01 — Core state and memory model

- Register file `R0..R31`.
- `SREG` flags.
- Program counter.
- Cycle counter.
- Flash memory.
- Data memory regions.
- Arduino Uno I/O register names.

Exit criteria:

- Deterministic state API exists.
- Memory and register tests pass.

## Phase 02 — Firmware loaders

- Intel HEX loader.
- Raw binary loader with explicit base address.
- Firmware load result diagnostics.

Exit criteria:

- Loader tests pass with small fixtures.
- Invalid checksum and malformed records are rejected.

## Phase 03 — Instruction decode baseline

- Decode opcode word to instruction descriptor.
- Implement NOP and a small arithmetic/data-movement subset first.
- Unsupported opcodes return deterministic unsupported instruction result.

Exit criteria:

- Decode table has tests.
- Unsupported opcodes are reported cleanly.

## Phase 04 — Execution loop and tracing

- CPU step API.
- Run loop with max cycles.
- Register diff tracer.
- Port diff tracer.
- Stop reasons.

Exit criteria:

- CLI can run a small fixture for limited cycles.
- Trace format is stable and tested.

## Phase 05 — Arduino Uno port model

- `PORTB`, `PORTC`, `PORTD`.
- `DDRB`, `DDRC`, `DDRD`.
- `PINB`, `PINC`, `PIND`.
- Pin aliases for Arduino digital pins.

Exit criteria:

- Writes to DDR/PORT affect traced output deterministically.
- Port tests pass.

## Phase 06 — Optional Avalonia viewer

- Thin UI over core state.
- Register grid.
- `SREG` display.
- Port display.
- Step/run controls.

Exit criteria:

- UI contains no CPU semantics.
- UI can be run under WSL X environment.
