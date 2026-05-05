# Avalonia State Viewer

## Purpose

The Avalonia viewer provides a graphical display of `EmulatorAVR.Core` emulator state for development and debugging. It is a thin UI layer that consumes snapshots from Core and renders register values, SREG flags, port registers, pin mappings, and cycle/PC state.

The UI is optional and does not block the CLI milestone.

The UI must never contain emulator logic (instruction decoding, firmware loading, run loop, port behavior).

## Run command

```powershell
dotnet run --project src/EmulatorAVR.Avalonia
```

## Environment

The development environment supports WSL with X desktop output. Ensure an X server is running before launching the UI (e.g., `vcxsrv`, `Xming`, or `xlaunch` on Windows; `XQuartz` on macOS; native X on Linux).

## Current state

The following sections are implemented:

- **CPU summary cards**: Program Counter, Cycle Count, SREG raw value
- **SREG flags**: I T H S V N Z C — active/inactive visual states
- **Register grid**: R0..R31 with two-digit hex formatting
- **Port panel**: PINB/DDRB/PORTB, PINC/DDRC/PORTC, PIND/DDRD/PORTD with data memory addresses 0x23..0x2B
- **Digital pin map**: D0..D13 mapped to PORTD and PORTB bit positions

All sections display static/demo snapshot data. No live execution is attached.

## Limitations

- The viewer displays a static/demo snapshot of emulator state. It does not support live stepping or run controls.
- No firmware loading or file picker is available.
- No breakpoint or debugger integration.

## Validation command

```powershell
dotnet run --project src/EmulatorAVR.Avalonia --no-build
```
