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
        MCU = mcu;
        MaxCycles = maxCycles;
        TraceRegisters = traceRegisters;
        TracePorts = tracePorts;
        Firmware = firmware;
    }
}
