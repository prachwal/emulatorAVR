# AVR Instruction Inventory — ATmega328P

Status categories:
- **Implemented**: Full decode, execution, and tests.
- **Deferred**: Not implemented; documented reason.
- **Unsupported**: Not part of ATmega328P ISA.
- **Alias**: Alternative mnemonic normalizing to a canonical instruction.

## Arithmetic

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| ADD | Implemented | ✓ | ✓ | Rd = Rd + Rr, sets SREG |
| ADC | Implemented | ✓ | ✓ | Add with carry |
| ADIW | Implemented | ✓ | ✓ | Word pair (R24-R30 even) + immediate |
| SUB | Implemented | ✓ | ✓ | |
| SUBI | Implemented | ✓ | ✓ | Rd = R16-R31 |
| SBC | Implemented | ✓ | ✓ | Subtract with carry |
| SBCI | Implemented | ✓ | ✓ | Subtract immediate with carry |
| SBIW | Implemented | ✓ | ✓ | Word pair - immediate |
| CP | Implemented | ✓ | ✓ | Compare, flags only |
| CPC | Implemented | ✓ | ✓ | Compare with carry |
| CPI | Implemented | ✓ | ✓ | Compare immediate (R16-R31) |
| INC | Implemented | ✓ | ✓ | |
| DEC | Implemented | ✓ | ✓ | |
| NEG | Implemented | ✓ | ✓ | Two's complement |

## Logical / Bitwise

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| AND | Implemented | ✓ | ✓ | |
| ANDI | Implemented | ✓ | ✓ | Immediate, R16-R31 |
| OR | Implemented | ✓ | ✓ | |
| ORI | Implemented | ✓ | ✓ | Immediate, R16-R31 |
| EOR | Implemented | ✓ | ✓ | Exclusive OR |
| COM | Implemented | ✓ | ✓ | One's complement |
| TST | Alias | ✓ | ✓ | AND Rd,Rd — alias normalizes to AND kind |
| CLR | Alias | ✓ | ✓ | EOR Rd,Rd — alias normalizes to EOR kind |
| SER | Alias | ✓ | ✓ | LDI Rd,0xFF — alias normalizes to LDI kind |

## Shifts / Rotates

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| LSL | Alias | ✓ | ✓ | ADD Rd,Rd — alias normalizes to ADD kind |
| LSR | Implemented | ✓ | ✓ | Logical shift right |
| ROL | Alias | ✓ | ✓ | ADC Rd,Rd — alias normalizes to ADC kind |
| ROR | Implemented | ✓ | ✓ | Rotate right through carry |
| ASR | Implemented | ✓ | ✓ | Arithmetic shift right |
| SWAP | Implemented | ✓ | ✓ | Swap nibbles |

## Branches / Control Flow

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| RJMP | Implemented | ✓ | ✓ | 12-bit relative offset |
| JMP | Implemented | ✓ | ✓ | 2-word, 22-bit absolute |
| RCALL | Implemented | ✓ | ✓ | Push PC+1, relative jump |
| CALL | Implemented | ✓ | ✓ | 2-word, push PC+2, absolute jump |
| IJMP | Implemented | ✓ | ✓ | Indirect jump via Z |
| ICALL | Implemented | ✓ | ✓ | Push PC+1, indirect via Z |
| RET | Implemented | ✓ | ✓ | Pop PC |
| RETI | Implemented | ✓ | ✓ | Pop PC, set I flag |
| BRBS | Implemented | ✓ | ✓ | Branch if SREG bit set |
| BRBC | Implemented | ✓ | ✓ | Branch if SREG bit clear |
| BREQ | Alias | ✓ | ✓ | BRBS 1 (Z set) |
| BRNE | Alias | ✓ | ✓ | BRBC 1 (Z clear) |
| BRCS | Alias | ✓ | ✓ | BRBS 0 (C set) |
| BRCC | Alias | ✓ | ✓ | BRBC 0 (C clear) |
| BRSH | Alias | — | — | Same as BRCC (BRBC 0) |
| BRLO | Alias | — | — | Same as BRCS (BRBS 0) |
| BRMI | Alias | ✓ | ✓ | BRBS 2 (N set) |
| BRPL | Alias | ✓ | ✓ | BRBC 2 (N clear) |
| BRGE | Alias | ✓ | ✓ | BRBC 4 (S clear) |
| BRLT | Alias | ✓ | ✓ | BRBS 4 (S set) |
| BRHS | Alias | ✓ | ✓ | BRBS 5 (H set) |
| BRHC | Alias | ✓ | ✓ | BRBC 5 (H clear) |
| BRVS | Alias | ✓ | ✓ | BRBS 3 (V set) |
| BRVC | Alias | ✓ | ✓ | BRBC 3 (V clear) |
| BRTS | Alias | ✓ | ✓ | BRBS 6 (T set) |
| BRTC | Alias | ✓ | ✓ | BRBC 6 (T clear) |
| BRIE | Alias | ✓ | ✓ | BRBS 7 (I set) |
| BRID | Alias | ✓ | ✓ | BRBC 7 (I clear) |

## Skip Instructions

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| CPSE | Implemented | ✓ | ✓ | Compare skip if equal |
| SBRC | Implemented | ✓ | ✓ | Skip if bit in register cleared |
| SBRS | Implemented | ✓ | ✓ | Skip if bit in register set |
| SBIC | Implemented | ✓ | ✓ | Skip if I/O bit cleared |
| SBIS | Implemented | ✓ | ✓ | Skip if I/O bit set |

## I/O Operations

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| IN | Implemented | ✓ | ✓ | I/O address → data memory offset +0x20 |
| OUT | Implemented | ✓ | ✓ | Data memory offset +0x20 → I/O address |
| SBI | Implemented | ✓ | ✓ | Set bit in I/O register |
| CBI | Implemented | ✓ | ✓ | Clear bit in I/O register |

## Bit Operations

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| BST | Deferred | ✗ | ✗ | Encoding collision: opcode 0xFA00 overlaps with SBRC R16+ (both use bit-9=1). SBRC takes priority. Decoder test BstEncoding_DecodesAsSbrc_ProvingCollision proves opcode returns Sbrc, not Bst. |
| BLD | Deferred | ✗ | ✗ | Encoding collision: opcode 0xF800 overlaps with SBRC R0+ (both use bit-9=0). SBRC takes priority. Decoder test BldEncoding_DecodesAsSbrc_ProvingCollision proves opcode returns Sbrc, not Bld. |
| BSET | Implemented | ✓ | ✓ | Set SREG bit |
| BCLR | Implemented | ✓ | ✓ | Clear SREG bit |
| SEC | Alias | ✓ | ✓ | BSET 0 |
| CLC | Alias | ✓ | ✓ | BCLR 0 |
| SEZ | Alias | ✓ | ✓ | BSET 1 |
| CLZ | Alias | ✓ | ✓ | BCLR 1 |
| SEN | Alias | ✓ | ✓ | BSET 2 |
| CLN | Alias | ✓ | ✓ | BCLR 2 |
| SEV | Alias | ✓ | ✓ | BSET 3 |
| CLV | Alias | ✓ | ✓ | BCLR 3 |
| SES | Alias | ✓ | ✓ | BSET 4 |
| CLS | Alias | ✓ | ✓ | BCLR 4 |
| SEH | Alias | ✓ | ✓ | BSET 5 |
| CLH | Alias | ✓ | ✓ | BCLR 5 |
| SET | Alias | ✓ | ✓ | BSET 6 |
| CLT | Alias | ✓ | ✓ | BCLR 6 |
| SEI | Alias | ✓ | ✓ | BSET 7 |
| CLI | Alias | ✓ | ✓ | BCLR 7 |

## Data Memory Load/Store

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| LD Rd, X | Implemented | ✓ | ✓ | |
| LD Rd, X+ | Implemented | ✓ | ✓ | Post-increment |
| LD Rd, -X | Implemented | ✓ | ✓ | Pre-decrement |
| ST X, Rr | Implemented | ✓ | ✓ | |
| ST X+, Rr | Implemented | ✓ | ✓ | |
| ST -X, Rr | Implemented | ✓ | ✓ | |
| LD Rd, Y | Implemented | ✓ | ✓ | |
| LD Rd, Y+ | Implemented | ✓ | ✓ | |
| LD Rd, -Y | Implemented | ✓ | ✓ | |
| ST Y, Rr | Implemented | ✓ | ✓ | |
| ST Y+, Rr | Implemented | ✓ | ✓ | |
| ST -Y, Rr | Implemented | ✓ | ✓ | |
| LD Rd, Z | Implemented | ✓ | ✓ | |
| LD Rd, Z+ | Implemented | ✓ | ✓ | |
| LD Rd, -Z | Implemented | ✓ | ✓ | |
| ST Z, Rr | Implemented | ✓ | ✓ | |
| ST Z+, Rr | Implemented | ✓ | ✓ | |
| ST -Z, Rr | Implemented | ✓ | ✓ | |
| LDD Y+q | Implemented | ✓ | ✓ | 6-bit displacement |
| STD Y+q | Implemented | ✓ | ✓ | |
| LDD Z+q | Implemented | ✓ | ✓ | |
| STD Z+q | Implemented | ✓ | ✓ | |
| LDS | Implemented | ✓ | ✓ | 2-word, 16-bit address |
| STS | Implemented | ✓ | ✓ | 2-word, 16-bit address |
| PUSH | Implemented | ✓ | ✓ | SP decrement, store byte |
| POP | Implemented | ✓ | ✓ | Load byte, SP increment |

## Program Memory Access

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| LPM | Implemented | ✓ | ✓ | R0 = flash[Z] |
| LPM Rd, Z+ | Implemented | ✓ | ✓ | Rd = flash[Z], Z++ |
| ELPM | Implemented | ✓ | — | Same as LPM (RAMPZ=0 always on ATmega328P) |
| SPM | Implemented | ✓ | — | Stub (flash read-only in emulator) |

## Stack Operations

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| PUSH | Implemented | ✓ | ✓ | |
| POP | Implemented | ✓ | ✓ | |

## Multiply

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| MUL | Implemented | ✓ | ✓ | Unsigned 8×8 → R1:R0 |
| MULS | Implemented | ✓ | ✓ | Signed, R16-R23 × R16-R23 |
| MULSU | Implemented | ✓ | ✓ | Signed × unsigned |
| FMUL | Implemented | ✓ | ✓ | Fractional, (Rd×Rr)<<1 |
| FMULS | Implemented | ✓ | ✓ | Signed fractional |
| FMULSU | Implemented | ✓ | ✓ | Signed × unsigned fractional |

## MCU Control

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| SLEEP | Implemented | ✓ | ✓ | No-op stub |
| WDR | Implemented | ✓ | ✓ | No-op stub |
| BREAK | Implemented | ✓ | ✓ | No-op stub (debug) |

## NOP / MOV / LDI

| Mnemonic | Status | Decode test | Exec test | Notes |
|----------|--------|-------------|-----------|-------|
| NOP | Implemented | ✓ | ✓ | |
| MOV | Implemented | ✓ | ✓ | |
| LDI | Implemented | ✓ | ✓ | R16-R31, 8-bit immediate |

## Not on ATmega328P (Unsupported)

These instructions are not part of the ATmega328P ISA:

| Mnemonic | Status | Reason |
|----------|--------|--------|
| EIJMP | Unsupported | Needs EIND register (not on ATmega328P) |
| EICALL | Unsupported | Needs EIND register (not on ATmega328P) |
| XCH | Unsupported | XMEGA-only exchange instruction |
| LAS | Unsupported | XMEGA-only |
| LAC | Unsupported | XMEGA-only |
| LAT | Unsupported | XMEGA-only |

## Alias Policy

- **SER**, **TST**, **CLR**, **LSL**, **ROL**: decode returns the alias `InstructionKind` (e.g. `InstructionKind.Ser`). The alias has its own execution test. The canonical instruction (LDI, AND, EOR, ADD, ADC) is NOT replaced — the alias is distinguished at decode time.
- **Branch aliases** (BREQ, BRNE, etc.): decode returns the alias `InstructionKind`. Executor treats all BRBS variants the same and all BRBC variants the same.
- **Flag aliases** (SEC, CLC, etc.): decode returns the alias `InstructionKind`. Executor treats BSET variants (set bit) and BCLR variants (clear bit) the same.
- **BRSH/BRLO**: not explicitly decoded — they are equivalent to BRCC/BRCS respectively. A future disassembler addition could add these aliases.

## Cycle Count Policy

All instructions simulate 1 cycle by default. This is a simplification for milestone 1. Multi-cycle instructions (JMP, CALL, RET, branches, skips) are documented but not cycle-accurate. A future milestone should add instruction-specific cycle counts.

## Instruction Length

Most instructions are 1 word (16 bits). The following are 2-word:

- JMP, CALL, LDS, STS

Skip instructions that skip over a 2-word instruction advance PC by 2 (one extra word beyond the normal increment).
