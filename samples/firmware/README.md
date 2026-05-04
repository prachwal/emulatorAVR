# samples/firmware

Place externally compiled AVR firmware files here.

Expected formats:
- Intel HEX (.hex)
- Raw binary (.bin)

Use these files with the CLI runner:

```powershell
dotnet run --project src/EmulatorAVR.Cli -- run --mcu atmega328p --firmware samples/firmware/blink.hex --max-cycles 100000 --trace registers,ports
```
