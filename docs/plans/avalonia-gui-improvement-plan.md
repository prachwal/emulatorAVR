# Avalonia GUI improvement plan

Repository: `prachwal/emulatorAVR`
Scope: open Avalonia UI refinement issues `#15` to `#18`
Generated: 2026-05-04

## Purpose

This document defines the shared design and implementation plan for improving the Avalonia GUI into a clean, readable, MVVM-based visual state viewer for the AVR emulator.

The plan covers:

- MVVM shell and layout foundation;
- visual design resources;
- CPU register dashboard;
- SREG flag visualization;
- Arduino Uno port and digital pin mapping visualization;
- responsive layout and documentation polish.

This is not a plan for adding emulator behavior. The UI must remain a viewer/demo dashboard unless a later issue explicitly introduces live execution or debugging.

## Related issues

| Issue | Phase | Purpose |
|---:|---|---|
| #15 | Phase 07 | MVVM shell, visual design system, layout foundation |
| #16 | Phase 08 | CPU register dashboard and SREG visual refinement |
| #17 | Phase 09 | Arduino Uno port panel and pin mapping visualization |
| #18 | Phase 10 | Visual polish, responsiveness, and documentation update |

## Current baseline

Already available before these issues:

- `src/EmulatorAVR.Avalonia/` project exists;
- `MainWindowViewModel` exists;
- `MainWindow.axaml` exists;
- `EmulatorStateSnapshot` exists in Core;
- viewer can show basic PC, cycle count, SREG, registers and port placeholders;
- snapshot register storage was fixed to `ImmutableArray<byte>`;
- Avalonia app startup was previously validated.

## Global design principles

1. Keep Avalonia as a thin viewer.
2. Keep emulator logic in `EmulatorAVR.Core`, not in UI.
3. Keep CLI behavior unchanged.
4. Use MVVM boundaries consistently.
5. Prefer display-ready ViewModel data over complex XAML formatting logic.
6. Use reusable Avalonia resources for colors, typography and controls.
7. Use dark-friendly visual defaults.
8. Avoid external assets, icon packs and font files.
9. Avoid large framework additions unless already present.
10. Keep every phase buildable and testable.

## Global non-goals

Do not implement in these issues:

- live run-loop integration;
- start/pause/step controls;
- breakpoints;
- firmware loading UI;
- file picker;
- debugger controls;
- memory viewer;
- disassembler;
- serial/UART terminal;
- timers;
- animation;
- emulator semantics;
- port electrical simulation;
- Core instruction changes;
- CLI behavior changes.

## Target visual layout

Recommended final screen structure:

```text
+--------------------------------------------------------------+
| Header: emulatorAVR / ATmega328P state viewer                |
| Subtitle: static/demo snapshot viewer                        |
+--------------------------------------------------------------+
| CPU Summary Cards                                            |
| [PC] [Cycle Count] [SREG Raw]                                |
+--------------------------------------------------------------+
| SREG Flags                                                   |
| [I] [T] [H] [S] [V] [N] [Z] [C]                              |
+--------------------------------------------------------------+
| Registers                                                    |
| R0  0x00 | R1  0x00 | ... | R31 0x00                         |
+--------------------------------------------------------------+
| Ports                                                        |
| PINB/DDRB/PORTB | PINC/DDRC/PORTC | PIND/DDRD/PORTD          |
+--------------------------------------------------------------+
| Arduino Digital Pin Map                                      |
| D0..D13 -> PORTD/PORTB bit mappings                          |
+--------------------------------------------------------------+
```

Use vertical scrolling if content does not fit.

## MVVM architecture

### View

`MainWindow.axaml` responsibilities:

- layout only;
- bindings only;
- styling through resources;
- no business logic;
- no firmware or emulator logic;
- no event handlers except template-generated boilerplate if unavoidable.

### ViewModel

`MainWindowViewModel` responsibilities:

- expose app title and subtitle;
- expose PC display text;
- expose cycle count display text;
- expose SREG raw display text;
- expose SREG flag display collection;
- expose register display collection;
- expose port register display collection;
- expose digital pin mapping display collection;
- provide deterministic demo/static data.

### Display models

Recommended immutable display records:

```text
DisplayRegister
DisplayFlag
DisplayPortRegister
DisplayPinMapping
```

These should contain display-ready strings where possible:

- register name;
- formatted hex value;
- flag name;
- active/inactive state;
- port register address display;
- port register value display;
- pin name;
- mapped port name;
- bit index display.

## Style resources

Recommended style files:

```text
src/EmulatorAVR.Avalonia/Styles/Colors.axaml
src/EmulatorAVR.Avalonia/Styles/Typography.axaml
src/EmulatorAVR.Avalonia/Styles/Controls.axaml
```

Resources should cover:

- app background;
- card background;
- border color;
- primary text;
- secondary text;
- muted text;
- value text;
- active flag style;
- inactive flag style;
- section/card style.

Do not add external font files. Use system/default fonts.

## Phase 07 plan — issue #15

Goal:

- establish MVVM shell;
- add reusable display models;
- add design-time/demo ViewModel data;
- add style/resource files;
- rework MainWindow layout into dashboard foundation.

Expected files:

```text
src/EmulatorAVR.Avalonia/ViewModels/ViewModelBase.cs
src/EmulatorAVR.Avalonia/ViewModels/MainWindowViewModel.cs
src/EmulatorAVR.Avalonia/ViewModels/DesignTimeMainWindowViewModel.cs
src/EmulatorAVR.Avalonia/Models/DisplayRegister.cs
src/EmulatorAVR.Avalonia/Models/DisplayFlag.cs
src/EmulatorAVR.Avalonia/Models/DisplayPortRegister.cs
src/EmulatorAVR.Avalonia/Views/MainWindow.axaml
src/EmulatorAVR.Avalonia/Styles/Colors.axaml
src/EmulatorAVR.Avalonia/Styles/Typography.axaml
src/EmulatorAVR.Avalonia/Styles/Controls.axaml
```

Acceptance:

- app builds;
- all sections visible;
- R0..R31 available from bound collection;
- SREG flags available from bound collection;
- port placeholders visible;
- no Core/CLI behavior changed.

## Phase 08 plan — issue #16

Goal:

- refine CPU summary cards;
- improve register grid readability;
- improve SREG chip strip.

Register grid requirements:

- exactly `R0..R31`;
- stable order;
- two-digit hex values: `0x00` to `0xFF`;
- compact cells;
- readable label/value hierarchy.

SREG requirements:

- order: `I T H S V N Z C`;
- active/inactive states visually distinguishable;
- inactive state readable;
- no fake live behavior.

Acceptance:

- CPU cards visually distinct;
- register values consistently formatted;
- SREG flag order correct;
- XAML remains binding-driven;
- no Core/CLI changes.

## Phase 09 plan — issue #17

Goal:

- add proper port register visual section;
- add Arduino digital pin mapping visual section.

Port registers:

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

Pin mappings:

```text
D0..D7   -> PORTD bits 0..7
D8..D13  -> PORTB bits 0..5
```

Acceptance:

- all nine port registers visible;
- addresses displayed as data memory addresses, not reduced I/O offsets;
- D0..D13 visible;
- no analog pins exposed;
- static/demo status clear;
- no port behavior simulated in UI.

## Phase 10 plan — issue #18

Goal:

- visual polish;
- responsive behavior;
- accessibility basics;
- documentation update.

Polish requirements:

- consistent spacing;
- consistent card backgrounds/borders;
- consistent section headers;
- readable value typography;
- no overcrowding.

Responsiveness requirements:

- avoid fragile fixed widths;
- use wrapping/grid/scrolling where needed;
- register grid remains accessible;
- port/pin sections remain accessible;
- vertical scrolling acceptable.

Documentation requirements:

- update `docs/avalonia.md`;
- update `docs/wiki/issues-status-summary.md` with short note about UI refinement issues.

Acceptance:

- build passes;
- tests pass;
- Avalonia starts or reports exact display error;
- docs updated;
- no Core/CLI changes.

## Required validation for every phase

Run exactly:

```powershell
dotnet sln list
dotnet build --no-restore
dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
dotnet run --project src/EmulatorAVR.Avalonia --no-build
```

If graphical display is unavailable, report:

```text
dotnet run --project src/EmulatorAVR.Avalonia --no-build: UNKNOWN — <exact display error>
```

Do not claim PASS without startup validation.

## Final acceptance for the UI improvement sequence

The UI improvement sequence is complete when:

- [ ] MVVM display model structure exists;
- [ ] dashboard foundation exists;
- [ ] reusable styles exist;
- [ ] CPU summary cards are readable;
- [ ] SREG flags are visible and correctly ordered;
- [ ] `R0..R31` are visible and consistently formatted;
- [ ] port registers are visible with correct data memory addresses;
- [ ] D0..D13 pin mappings are visible;
- [ ] layout remains usable with scrolling/wrapping;
- [ ] docs are updated;
- [ ] no emulator logic was added to UI;
- [ ] no Core behavior was changed;
- [ ] no unrelated CLI behavior was changed;
- [ ] validation commands pass or Avalonia run is `UNKNOWN` with exact display error.
