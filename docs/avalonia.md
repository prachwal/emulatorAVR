# Avalonia State Viewer

## Purpose

The Avalonia viewer provides a graphical display of `EmulatorAVR.Core` emulator state for development and debugging. It is a thin UI layer that consumes snapshots from Core and renders register values, SREG flags, and cycle/PC state.

The UI is optional and does not block the CLI milestone.

The UI must never contain emulator logic (instruction decoding, firmware loading, run loop, port behavior).

## Run command

```powershell
dotnet run --project src/EmulatorAVR.Avalonia
```

## Environment

The development environment supports WSL with X desktop output. Ensure an X server is running before launching the UI (e.g., `vcxsrv`, `Xming`, or `xlaunch` on Windows; `XQuartz` on macOS; native X on Linux).

## Current limitations

- The viewer displays a static/demo snapshot of emulator state. It does not support live stepping or run controls.
- Port register sections are placeholders — no Arduino port behavior is implemented in the UI.
- No firmware loading or file picker is available.
- No breakpoint or debugger integration.

## Validation command

```powershell
dotnet run --project src/EmulatorAVR.Avalonia --no-build
```
