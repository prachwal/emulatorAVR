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
- The likely executor model is weak: DeepSeek V4 Flash / DeepSeek V4 Flash 2 class.
- Tasks must be precise, small, file-oriented, command-oriented, and include explicit acceptance criteria.
- Do not rely on broad implicit reasoning by the executor model.
- The user has WSL with working X desktop output, so Avalonia UI can be used and visually checked.

## DeepSeek V4 Flash executor limitations

Treat DeepSeek V4 Flash-class models as weak executors, not architects.

Use DeepSeek V4 Flash only for:

- one GitHub Issue at a time;
- explicit file edits;
- small implementation steps;
- command-based validation;
- mechanical layout fixes;
- adding tests from precise acceptance criteria;
- implementing one narrow class or one narrow instruction group at a time.

Do not rely on DeepSeek V4 Flash for:

- architecture decisions;
- deciding milestone scope;
- interpreting AVR semantics without explicit references;
- broad refactoring;
- moving files unless the issue explicitly requires it;
- guessing repository layout;
- deciding whether an issue can be closed without a checklist;
- inventing peripheral or instruction behavior.

Every task for DeepSeek V4 Flash must include:

- files to read first;
- exact issue number;
- files allowed to edit;
- files or areas explicitly forbidden;
- commands to run;
- exact acceptance checklist;
- failure stop condition;
- required final report format.

Recommended execution settings for Kilo Code when using DeepSeek V4 Flash-class models:

- temperature: `0.0` to `0.1`;
- one issue per session;
- no autonomous task expansion;
- at most one retry after a failed command;
- stop and report if the second attempt fails;
- prefer existing project conventions over generating a new layout.

## Known DeepSeek V4 Flash failure mode: test-output diagnostic loop

Observed failure mode:

- `dotnet test` prints only restore lines or unexpectedly short output;
- the model repeatedly pipes the same command through `tail`, `grep`, `cat`, `wc`, `od`, `head`, or similar tools;
- the user interrupts the loop;
- the model then incorrectly claims that tests passed.

Project rule:

- Repeating output-inspection pipelines is forbidden.
- User-aborted commands are not successful test runs.
- Suspiciously short test output is `UNKNOWN`, not `PASS`, unless the exit code is clearly `0` and the solution contains the expected test projects.
- If test status is unclear after one structured diagnostic command, the executor must stop and report `Can close issue: NO`.

Preferred validation sequence:

```powershell
dotnet sln list
dotnet build --no-restore
dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
```

One allowed hang diagnostic:

```powershell
dotnet test --no-restore --blame-hang --blame-hang-timeout 60s --logger "trx;LogFileName=test-results.trx"
```

After the hang diagnostic, stop and report the result. Do not run `cat`, `od`, `wc`, `grep`, `tail`, or repeated variants against the same output.

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
