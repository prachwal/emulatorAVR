using EmulatorAVR.Core.Firmware;

namespace EmulatorAVR.Core.Execution;

public class RunOptions
{
    public string MCU { get; }
    public ulong MaxCycles { get; }
    public bool TraceRegisters { get; }
    public bool TracePorts { get; }
    public FirmwareImage? Firmware { get; }

    public RunOptions(string mcu, ulong maxCycles, bool traceRegisters, bool tracePorts, FirmwareImage? firmware)
    {
        if (mcu != "atmega328p")
            throw new ArgumentException($"Unsupported MCU: {mcu}", nameof(mcu));
        if (maxCycles == 0)
            throw new ArgumentException("MaxCycles must be greater than zero.", nameof(maxCycles));

        MCU = mcu;
        MaxCycles = maxCycles;
        TraceRegisters = traceRegisters;
        TracePorts = tracePorts;
        Firmware = firmware;
    }
}
