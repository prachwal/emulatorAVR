# Technical references

Use these as normative references for emulator behavior.

## Primary references

1. Microchip AVR Instruction Set Manual.
2. Microchip ATmega328P datasheet.
3. Arduino Uno Rev3 official technical documentation.

## Secondary references

4. avr-gcc and avr-libc generated fixture firmware.
5. Existing tests in this repository.
6. Existing docs in this repository.

## Reference policy

- Do not copy behavior from random emulator projects as normative behavior.
- If a behavior is not yet verified against a primary reference, mark it as provisional in tests or docs.
- Unsupported opcodes must fail deterministically until implemented.
