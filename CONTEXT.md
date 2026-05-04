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
- The likely executor model is weak: Devstral / Devstral 2 class.
- Tasks must be precise, small, file-oriented, command-oriented, and include explicit acceptance criteria.
- Do not rely on broad implicit reasoning by the executor model.
- The user has WSL with working X desktop output, so Avalonia UI can be used and visually checked.

## Devstral executor limitations

Treat Devstral-class models as weak executors, not architects.

Use Devstral only for:

- one GitHub Issue at a time;
- explicit file edits;
- small implementation steps;
- command-based validation;
- mechanical layout fixes;
- adding tests from precise acceptance criteria;
- implementing one narrow class or one narrow instruction group at a time.

Do not rely on Devstral for:

- architecture decisions;
- deciding milestone scope;
- interpreting AVR semantics without explicit references;
- broad refactoring;
- moving files unless the issue explicitly requires it;
- guessing repository layout;
- deciding whether an issue can be closed without a checklist;
- inventing peripheral or instruction behavior.

Every task for Devstral must include:

- files to read first;
- exact issue number;
- files allowed to edit;
- files or areas explicitly forbidden;
- commands to run;
- exact acceptance checklist;
- failure stop condition;
- required final report format.

Recommended execution settings for Kilo Code when using Devstral-class models:

- temperature: `0.0` to `0.1`;
- one issue per session;
- no autonomous task expansion;
- at most one retry after a failed command;
- stop and report if the second attempt fails;
- prefer existing project conventions over generating a new layout.

Operational rule:

```text
repo = durable project memory
GitHub Issues = current executable work items
chat = planning, review, and correction loop
```

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
