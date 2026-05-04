# AVR instruction implementation plan

Repository: `prachwal/emulatorAVR`
Target MCU profile: ATmega328P / Arduino Uno
Target core family: 8-bit AVR enhanced RISC profile
Plan status: implementation planning document
Generated: 2026-05-04

## Purpose

This document defines a detailed implementation plan for adding full AVR instruction support to the emulator.

The user request is to plan **all instructions in one phase**. This document therefore treats the work as one large umbrella phase, but internally splits the implementation into controlled sub-batches. The reason is practical: a weak executor model should not implement all instructions in one unstructured patch.

## Current baseline

Already implemented before this plan:

- CPU state:
  - `AvrCpu`
  - `AvrCpuState`
  - `AvrRegisters`
  - `StatusRegister`
- memory:
  - `ProgramMemory`
  - `DataMemory`
- firmware:
  - `FirmwareImage`
  - `IntelHexLoader`
  - `RawBinaryLoader`
- decode/execution baseline:
  - `NOP`
  - `LDI`
  - `MOV`
  - `ADD`
- run loop:
  - `AvrRunLoop`
  - `RunOptions`
  - `RunResult`
  - `StopReason`
- tracing:
  - `TraceFrame`
  - `RegisterTraceEntry`
  - `PortTraceEntry`
- Arduino Uno ports model exists, but run-loop integration is incomplete.

## Core rule for this phase

Implement the full AVR instruction set as a single umbrella phase, but not as one chaotic commit.

Use this internal order:

1. decoder/test matrix infrastructure;
2. arithmetic and logic;
3. branches and relative control flow;
4. I/O and bit operations;
5. data memory load/store;
6. stack and subroutine control flow;
7. program memory and indirect access;
8. multiplication and extended arithmetic;
9. status/control instructions;
10. skip instructions and edge cases;
11. integration fixtures;
12. final coverage audit.

Each sub-batch must compile and pass tests before moving to the next.

## Non-goals

This plan does not require implementing:

- timers;
- interrupts delivery;
- UART;
- ADC;
- PWM hardware;
- analog pins;
- bootloader protocol;
- ELF loader;
- GDB server;
- live GUI debugger.

Some instructions manipulate interrupt flags or return from interrupt. The instruction behavior may be implemented, but real interrupt scheduling is out of scope unless explicitly stated.

## Reference priority

Use references in this order:

1. Microchip AVR Instruction Set Manual.
2. Microchip ATmega328P datasheet.
3. Existing emulator tests and existing code conventions.
4. avr-gcc / avr-libc generated tiny fixture firmware.
5. Arduino Uno official documentation for port/pin mapping only.

Do not guess opcode formats. Every opcode pattern must be copied from the instruction manual and verified by tests.

## Required implementation architecture

### Decoder structure

Required decoder behavior:

- input: one `ushort` opcode word;
- output: immutable `Instruction` descriptor;
- unsupported opcodes return `InstructionKind.Unsupported`;
- two-word instructions must be represented explicitly;
- decoding must not mutate CPU state;
- decoding must not read data memory;
- decoding must not execute instruction side effects.

Recommended additions:

```text
src/EmulatorAVR.Core/Instructions/InstructionFormat.cs
src/EmulatorAVR.Core/Instructions/InstructionOperand.cs
src/EmulatorAVR.Core/Instructions/InstructionLength.cs
src/EmulatorAVR.Core/Instructions/OpcodePatterns.cs
```

Do not over-engineer if simple immutable descriptor fields are enough.

### Executor structure

Required executor behavior:

- input: `AvrCpuState`, decoded instruction, memory services where needed;
- deterministic result;
- no wall-clock time;
- no UI dependencies;
- no CLI dependencies;
- PC and cycle updates must be instruction-specific;
- SREG updates must match AVR semantics;
- invalid internal state should fail deterministically.

Recommended additions:

```text
src/EmulatorAVR.Core/Execution/InstructionTiming.cs
src/EmulatorAVR.Core/Execution/InstructionExecutionContext.cs
src/EmulatorAVR.Core/Execution/ExecutionStatus.cs
```

### Memory integration

The current `DataMemory` and `ProgramMemory` will need to support:

- byte read/write by data address;
- word read/write by program word address;
- stack pointer mapping through SPL/SPH in data memory;
- I/O register access;
- indirect addressing through X/Y/Z register pairs;
- program memory reads for `LPM`.

### Status register helpers

Add helper functions only if they reduce duplicated flag logic.

Recommended:

```text
src/EmulatorAVR.Core/Cpu/StatusRegisterMath.cs
```

This may contain deterministic static methods for flag calculations.

Do not hide correctness. Every helper must have focused tests.

## Instruction groups

The list below is intentionally grouped by implementation dependencies rather than alphabetical order.

### Group A — already implemented baseline

Status: already implemented, must remain covered by regression tests.

Instructions:

- `NOP`
- `LDI`
- `MOV`
- `ADD`

Required regression tests:

- decode examples;
- execution examples;
- PC/cycle behavior;
- SREG flags for `ADD`;
- high-register decode cases for `MOV` and `ADD`.

### Group B — arithmetic and compare

Instructions:

- `ADC`
- `ADIW`
- `SUB`
- `SUBI`
- `SBC`
- `SBCI`
- `SBIW`
- `CP`
- `CPC`
- `CPI`
- `INC`
- `DEC`
- `NEG`

Primary behavior:

- update destination registers where applicable;
- compare instructions update flags but do not mutate operands;
- word operations affect register pairs allowed by AVR encoding;
- update correct SREG flags per instruction.

Required SREG coverage:

- `H`
- `S`
- `V`
- `N`
- `Z`
- `C`

For `ADC`, `SBC`, and `CPC`, include carry-in behavior tests.

Required tests per instruction:

- one decode test;
- one nominal execution test;
- one boundary/flag test;
- PC increment;
- cycle increment.

Special checks:

- `CP`, `CPC`, `CPI` must not mutate registers;
- `ADIW` and `SBIW` operate only on valid word register pairs;
- zero flag behavior for `CPC` and `SBC` must follow AVR rules.

### Group C — logical and bitwise register operations

Instructions:

- `AND`
- `ANDI`
- `OR`
- `ORI`
- `EOR`
- `COM`
- `TST`
- `CLR`
- `SER`

Notes:

- Some mnemonics are aliases or semantic equivalents depending on encoding.
- Decide whether aliases are represented as their own `InstructionKind` or normalized internally.
- The test suite must make this decision explicit.

Required behavior:

- correct result values;
- `N`, `Z`, `V`, `S` updates where applicable;
- `COM` sets carry according to AVR semantics;
- alias instructions decode deterministically.

Required tests:

- decode canonical opcode examples;
- execution result;
- SREG flags;
- alias handling for `TST`, `CLR`, and `SER`.

### Group D — shifts, rotates, swap

Instructions:

- `LSL`
- `LSR`
- `ROL`
- `ROR`
- `ASR`
- `SWAP`

Notes:

- Some are aliases over existing encodings, e.g. `LSL` can be represented through `ADD Rd,Rd` depending on decoder strategy.
- Explicitly document the chosen representation.

Required behavior:

- correct bit movement;
- carry in/out for rotate instructions;
- SREG updates, especially `C`, `N`, `Z`, `V`, `S`.

Required tests:

- bit 7/bit 0 carry cases;
- zero result;
- negative result;
- carry-in behavior for rotate instructions.

### Group E — direct, relative, and conditional control flow

Instructions:

- `RJMP`
- `JMP`
- `IJMP`
- `EIJMP` if applicable/unsupported decision documented
- `RCALL`
- `CALL`
- `ICALL`
- `EICALL` if applicable/unsupported decision documented
- `RET`
- `RETI`
- `BRBC`
- `BRBS`
- named branch aliases:
  - `BRCC`
  - `BRCS`
  - `BREQ`
  - `BRNE`
  - `BRGE`
  - `BRLT`
  - `BRHC`
  - `BRHS`
  - `BRID`
  - `BRIE`
  - `BRLO`
  - `BRMI`
  - `BRPL`
  - `BRSH`
  - `BRTC`
  - `BRTS`
  - `BRVC`
  - `BRVS`

Required behavior:

- PC-relative signed offsets;
- absolute jump/call decoding for two-word instructions;
- stack push/pop for call/return;
- cycle counts for taken and not-taken branches;
- `RETI` sets interrupt flag if AVR semantics require it.

Required tests:

- forward and backward branch offsets;
- taken and not taken cases;
- PC update exactness;
- call pushes return address;
- return pops address;
- stack pointer updates;
- invalid stack state fails deterministically.

Important:

- Two-word instructions require run-loop support.
- Implement instruction length before adding `JMP` and `CALL`.

### Group F — skip instructions

Instructions:

- `CPSE`
- `SBIC`
- `SBIS`
- `SBRC`
- `SBRS`

Required behavior:

- skip next instruction when condition matches;
- skip length must consider whether next instruction is one-word or two-word;
- PC update and cycle count must follow AVR semantics;
- no side effects from skipped instruction.

Required tests:

- condition false;
- condition true skipping one-word instruction;
- condition true skipping two-word instruction;
- skip does not execute skipped instruction;
- cycle count differs correctly if applicable.

### Group G — I/O register operations and bit operations

Instructions:

- `IN`
- `OUT`
- `SBI`
- `CBI`
- `BST`
- `BLD`
- `BSET`
- `BCLR`
- flag aliases:
  - `SEC`
  - `CLC`
  - `SEN`
  - `CLN`
  - `SEZ`
  - `CLZ`
  - `SEI`
  - `CLI`
  - `SES`
  - `CLS`
  - `SEV`
  - `CLV`
  - `SET`
  - `CLT`
  - `SEH`
  - `CLH`

Required behavior:

- I/O instruction address mapping must be correct:
  - AVR I/O instruction offsets are not the same as data memory addresses;
  - offset must map to data memory address by adding `0x20` where applicable for ATmega328P.
- `SBI` and `CBI` modify one bit in target I/O register.
- `IN` and `OUT` move between registers and I/O space.
- `BST` and `BLD` use SREG `T` bit.
- `BSET/BCLR` and aliases modify SREG bits.

Required tests:

- `OUT` to `DDRB` and `PORTB` using correct reduced I/O offset mapping;
- `IN` from port register into general register;
- `SBI DDRB,5` sets bit 5;
- `CBI PORTB,5` clears bit 5;
- `BST/BLD` transfer bit through `T`;
- `SEI/CLI` modify `I` only;
- alias instructions map correctly.

This group is necessary for a minimal blink-like firmware.

### Group H — data memory load/store, direct and indirect

Instructions:

- `LDS`
- `STS`
- `LD X`
- `LD X+`
- `LD -X`
- `LD Y`
- `LD Y+`
- `LD -Y`
- `LDD Y+q`
- `LD Z`
- `LD Z+`
- `LD -Z`
- `LDD Z+q`
- `ST X`
- `ST X+`
- `ST -X`
- `ST Y`
- `ST Y+`
- `ST -Y`
- `STD Y+q`
- `ST Z`
- `ST Z+`
- `ST -Z`
- `STD Z+q`

Required behavior:

- X = R27:R26;
- Y = R29:R28;
- Z = R31:R30;
- pre-decrement and post-increment semantics;
- displacement `q` decoding for Y/Z variants;
- direct address two-word `LDS/STS` support;
- cycle counts by addressing mode.

Required tests:

- X/Y/Z address pair construction;
- post-increment updates pointer after read/write;
- pre-decrement updates pointer before read/write;
- displacement addressing reads/writes correct address;
- direct address read/write;
- boundary behavior.

### Group I — stack operations

Instructions:

- `PUSH`
- `POP`

Required behavior:

- use stack pointer from SPL/SPH data memory registers;
- stack grows downward;
- `PUSH` stores register value then decrements SP according to AVR semantics;
- `POP` increments SP then loads register according to AVR semantics;
- deterministic behavior when SP is invalid/out of range.

Required tests:

- push single register;
- pop single register;
- push/pop roundtrip;
- SP low/high byte update;
- stack bounds.

This group is also required for `CALL`, `RET`, and real avr-gcc firmware.

### Group J — program memory access

Instructions:

- `LPM`
- `LPM Rd,Z`
- `LPM Rd,Z+`
- `ELPM` variants if applicable/unsupported decision documented
- `SPM` decision: unsupported or minimal deterministic behavior documented

Required behavior:

- program memory read by Z pointer;
- byte addressing into word-oriented `ProgramMemory`;
- post-increment for `Z+`;
- `R0` default destination variant for `LPM`.

Required tests:

- read low byte from program word;
- read high byte from program word;
- `Z+` increments pointer;
- unsupported `ELPM/SPM` behavior documented if not applicable.

### Group K — multiplication instructions

Instructions:

- `MUL`
- `MULS`
- `MULSU`
- `FMUL`
- `FMULS`
- `FMULSU`

Required behavior:

- result stored in `R1:R0`;
- signed/unsigned semantics;
- fractional variants left shift result according to AVR semantics;
- update `Z` and `C` flags.

Required tests:

- zero result;
- carry/high-bit result;
- signed negative cases;
- fractional multiply examples;
- `R1:R0` result ordering.

Check ATmega328P support before implementing each variant. If unsupported for target profile, explicitly document and test unsupported decode behavior.

### Group L — MCU control and status operations

Instructions:

- `BREAK`
- `SLEEP`
- `WDR`
- `NOP` regression

Required behavior:

- `BREAK` can return deterministic execution status such as breakpoint/trap if supported by run loop;
- `SLEEP` may return deterministic status without modeling power state;
- `WDR` may be a no-op or deterministic status if watchdog is not modeled;
- behavior must be explicitly documented.

Required tests:

- decode;
- deterministic execution status;
- PC/cycle behavior;
- no hidden wall-clock behavior.

### Group M — exchange and special data operations

Instructions:

- `XCH`
- `LAS`
- `LAC`
- `LAT`

Check ATmega328P support. If unsupported or not applicable, document and test unsupported behavior.

Required behavior if implemented:

- use Z-addressed data memory location;
- atomicity is not modeled, but result must match instruction semantics;
- update register and memory correctly.

Required tests:

- data memory value changes;
- register value changes;
- Z pointer unchanged unless instruction requires otherwise.

### Group N — aliases and pseudo-instructions policy

The decoder must define a clear policy for mnemonics that are aliases or assembler conveniences.

Potential aliases/pseudo-instructions:

- `CLR` as `EOR Rd,Rd`;
- `TST` as `AND Rd,Rd`;
- `LSL` as `ADD Rd,Rd`;
- `ROL` as `ADC Rd,Rd`;
- `SER` as `LDI Rd,0xFF`;
- branch aliases over `BRBS/BRBC`;
- flag set/clear aliases over `BSET/BCLR`.

Allowed strategies:

1. decode canonical opcode as the canonical instruction kind only;
2. expose alias instruction kinds when the opcode uniquely maps to user-facing mnemonic;
3. support display/disassembly aliases later, not in execution.

The chosen policy must be documented in tests and in code comments.

## Required test strategy

### Unit tests per instruction

Each instruction should have at minimum:

- decode test;
- execution nominal test;
- PC/cycle test;
- flag test if it touches SREG;
- edge/boundary test if it uses memory, stack, branch offset, or two-word format.

### Golden fixture tests

Add small hand-authored firmware fixtures before using compiler-generated ones.

Suggested fixtures:

1. `ldi_mov_add.hex`
2. `branch_loop.hex`
3. `io_portb_bit5.hex`
4. `stack_call_ret.hex`
5. `load_store_data.hex`
6. `minimal_blink_like.hex`

Fixtures should be tiny and deterministic.

### avr-gcc fixture tests

After basic manual fixtures pass, add externally compiled fixtures:

- minimal `main` with no Arduino core;
- direct register write for `DDRB` and `PORTB`;
- simple counted loop;
- optional Arduino-style blink later.

Do not start with full Arduino framework output. It pulls in more instructions and runtime setup.

## Completion target for blink-like scenario

A minimal blink-like firmware is considered supported when:

- firmware sets `DDRB bit 5` as output;
- firmware toggles or writes `PORTB bit 5`;
- emulator trace can show D13/PORTB5 transitions;
- CLI can run fixture and print stable trace;
- tests assert port state transitions deterministically.

Required likely instruction subset for first blink-like firmware:

- `LDI`
- `OUT`
- `SBI`
- `CBI`
- `RJMP`
- `BRNE`
- `DEC`
- possibly `IN`

Full Arduino `Blink.ino` may additionally require:

- `CALL`
- `RET`
- `PUSH`
- `POP`
- `LDS`
- `STS`
- `LD/ST`
- more arithmetic and branch instructions.

## Required files to add or update

Production files likely to be touched:

```text
src/EmulatorAVR.Core/Instructions/Instruction.cs
src/EmulatorAVR.Core/Instructions/InstructionKind.cs
src/EmulatorAVR.Core/Instructions/InstructionDecoder.cs
src/EmulatorAVR.Core/Execution/InstructionExecutor.cs
src/EmulatorAVR.Core/Execution/ExecutionResult.cs
src/EmulatorAVR.Core/Execution/AvrRunLoop.cs
src/EmulatorAVR.Core/Memory/DataMemory.cs
src/EmulatorAVR.Core/Memory/ProgramMemory.cs
src/EmulatorAVR.Core/Cpu/AvrCpuState.cs
src/EmulatorAVR.Core/Cpu/AvrRegisters.cs
src/EmulatorAVR.Core/Cpu/StatusRegister.cs
```

New production files may be added only if they reduce complexity and remain in Core:

```text
src/EmulatorAVR.Core/Instructions/InstructionLength.cs
src/EmulatorAVR.Core/Instructions/InstructionOperand.cs
src/EmulatorAVR.Core/Execution/InstructionExecutionContext.cs
src/EmulatorAVR.Core/Cpu/RegisterPair.cs
src/EmulatorAVR.Core/Cpu/StatusRegisterMath.cs
```

Test files likely to be added:

```text
tests/EmulatorAVR.Core.Tests/Instructions/*Tests.cs
tests/EmulatorAVR.Core.Tests/Execution/*Tests.cs
tests/EmulatorAVR.Core.Tests/Memory/*Tests.cs
tests/EmulatorAVR.Core.Tests/Fixtures/*Tests.cs
```

Fixture files may be added under:

```text
samples/firmware/manual/
samples/firmware/avr-gcc/
```

## Forbidden changes during instruction implementation

Do not edit:

```text
src/EmulatorAVR.Avalonia/
```

unless a later UI-specific issue explicitly requests visualizing new trace data.

Do not modify CLI output format unless the instruction implementation requires a new deterministic result field and tests are updated.

Do not add unrelated NuGet packages.

Do not add threading, timers, async background execution, or wall-clock behavior to the emulator core.

## Validation sequence after each sub-batch

Run exactly:

```powershell
dotnet sln list
dotnet build --no-restore
dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
```

For fixture-based CLI checks, additionally run targeted commands such as:

```powershell
dotnet run --project src/EmulatorAVR.Cli -- run --mcu atmega328p --firmware samples/firmware/manual/minimal_blink_like.hex --max-cycles 1000 --trace registers,ports
```

Do not replace the required test command with shorter variants.

## Sub-batch acceptance gates

Before moving to the next group:

- all newly added instruction decode tests pass;
- all execution tests pass;
- previous regression tests pass;
- no unsupported instruction behavior regresses;
- no UI or CLI unrelated files are modified;
- code remains deterministic;
- no `Assert.IsTrue(true)` style tests are added.

## Final acceptance checklist

The umbrella phase is complete only if:

- [ ] instruction inventory is documented;
- [ ] every target-supported instruction is either implemented or explicitly documented as unsupported/not applicable for ATmega328P;
- [ ] each implemented instruction has decode tests;
- [ ] each implemented instruction has execution tests;
- [ ] all SREG-affecting instructions have flag tests;
- [ ] branch/call/return instructions have PC and stack tests;
- [ ] skip instructions correctly handle one-word and two-word skipped instructions;
- [ ] load/store instructions cover direct and indirect addressing modes;
- [ ] I/O instructions use correct reduced I/O offset to data memory address mapping;
- [ ] stack instructions use SPL/SPH correctly;
- [ ] multiplication instructions store results in `R1:R0` correctly;
- [ ] aliases/pseudo-instruction policy is documented and tested;
- [ ] minimal blink-like firmware fixture passes;
- [ ] no Avalonia files were modified;
- [ ] no unrelated CLI behavior was changed;
- [ ] exact validation commands pass.

## Recommended issue body using this plan

Use this document as the source of truth for the implementation issue.

Recommended issue title:

```text
Phase 11: Full AVR instruction set implementation plan and execution umbrella
```

Recommended issue rule:

```text
Do not implement all instructions in one uncontrolled patch. Use this plan as one umbrella phase with internal sub-batches. Each sub-batch must build and pass tests before continuing.
```

## Risk notes

High-risk areas:

- two-word instruction handling;
- skip over two-word instructions;
- stack pointer semantics;
- I/O offset vs data memory address mapping;
- SREG flag formulas;
- branch signed offset calculation;
- alias instruction policy;
- avr-gcc startup code pulling in more instructions than expected.

Mitigation:

- implement by group;
- write manual opcode fixtures;
- add regression tests before expanding;
- compare against official instruction examples;
- keep unsupported behavior deterministic.
