# CONTEXT — emulatorAVR

## Persistent situation context

Repository: `https://github.com/prachwal/emulatorAVR`

Project type: AVR emulator for the processor used as the core of Arduino Uno.

Target MCU profile for the first implementation stage: `ATmega328P` / Arduino Uno compatible AVR core.

Primary milestone for the current phase:

- Build a CLI tool that can run externally compiled firmware.
- Support binary-level execution, starting with Intel HEX firmware.
- Trace AVR registers and Arduino Uno I/O ports.

Execution environment and workflow:

- The user will use Kilo Code.
- The likely executor model is weak: Devstral.
- Tasks must be precise, small, file-oriented, command-oriented, and include explicit acceptance criteria.
- Do not rely on broad implicit reasoning by the executor model.
- The user has WSL with working X desktop output, so Avalonia UI can be used and visually checked.

Implementation preferences:

- C# / .NET.
- Unit tests: MSTest + Moq + FluentAssertions.
- Logging: NLog.
- PowerShell commands for system scripts and command examples.

First milestone command target:

```powershell
dotnet run --project src/EmulatorAVR.Cli -- run --mcu atmega328p --firmware samples/firmware/blink.hex --max-cycles 100000 --trace registers,ports
```

First milestone trace target:

- Cycle count.
- Program counter.
- Opcode word.
- Register changes.
- `SREG` flags.
- Arduino Uno I/O port changes: `PORTB`, `PORTC`, `PORTD`, `DDRB`, `DDRC`, `DDRD`, `PINB`, `PINC`, `PIND`.

Design constraint:

- CPU logic lives in `EmulatorAVR.Core` only.
- CLI and Avalonia must call the same core emulator API.
- Avalonia is optional and must be a thin visualization layer.
