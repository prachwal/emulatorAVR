# AVR Instruction Inventory Corrections — Issue #19

This document records correction-pass decisions that override stale or ambiguous entries in `docs/plans/avr-instruction-inventory.md` until that inventory is fully regenerated.

## BLD / BST / SBRC / SBRS opcode classification

Correct opcode families:

```text
BLD  Rd,b: 1111 100d dddd 0bbb
BST  Rd,b: 1111 101d dddd 0bbb
SBRC Rr,b: 1111 110r rrrr 0bbb
SBRS Rr,b: 1111 111r rrrr 0bbb
```

Required examples now covered by regression tests:

```text
BLD  R16,3 -> 0xF903
BST  R16,3 -> 0xFB03
SBRC R16,3 -> 0xFD03
SBRS R16,3 -> 0xFF03
```

`BLD` and `SBRC` are not treated as an intentional collision.

## Skip instruction policy

Skip instructions carry `SkipWords` metadata derived from the next instruction word.

The following skip instructions must support skipping over both one-word and two-word instructions:

```text
CPSE
SBRC
SBRS
SBIC
SBIS
```

Two-word instructions currently recognized for skip-length purposes:

```text
JMP
CALL
LDS
STS
```

## LPM policy

Implemented:

```text
LPM          -> implicit R0, no Z increment
LPM Rd,Z    -> explicit destination register, no Z increment
LPM Rd,Z+   -> explicit destination register, increments Z
```

## ELPM policy

For the ATmega328P profile, `ELPM` is intentionally unsupported in the corrected decoder.

Rejected forms:

```text
ELPM
ELPM Rd,Z
ELPM Rd,Z+
```

Reason: ATmega328P profile does not require extended program memory access via RAMPZ.

## SPM policy

`SPM` remains implemented as a deterministic no-op stub because flash write/self-programming is outside the emulator milestone. It now has a regression test proving no-op behavior with PC/cycle advancement.
