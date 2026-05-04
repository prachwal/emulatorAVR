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
