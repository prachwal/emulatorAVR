# emulatorAVR

Binary-compatible AVR emulator targeting the ATmega328P-class CPU used by Arduino Uno.

## Phase 1 objective

Create a deterministic CLI tool that can load externally compiled firmware and execute it while tracing:

- AVR registers `R0..R31`
- `SREG`
- program counter
- cycle count
- Arduino Uno ports: `PORTB`, `PORTC`, `PORTD`, `DDRB`, `DDRC`, `DDRD`, `PINB`, `PINC`, `PIND`

Optional UI: thin Avalonia viewer over the same emulator core API.

## Technology baseline

- C# / .NET
- MSTest
- Moq
- FluentAssertions
- NLog

## Documents

- `AGENTS.md` — AI-agent project rules
- `CONTEXT.md` — persistent situation context
- `docs/ROADMAP.md` — phases
- `docs/TASKS.md` — task backlog
- `docs/PROMPTS.md` — prompts for Kilo Code / DeepSeek V4 Flash
- `docs/references.md` — authoritative technical references

## Initial command target

```powershell
dotnet run --project src/EmulatorAVR.Cli -- run --mcu atmega328p --firmware samples/firmware/blink.hex --max-cycles 100000 --trace registers,ports
```

## Test command

```powershell
dotnet test
```
