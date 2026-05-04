namespace EmulatorAVR.Core.Tracing;

public class TraceFrame
{
    public ulong CycleCount { get; }
    public uint PC { get; }
    public ushort OpcodeWord { get; }
    public IReadOnlyList<RegisterTraceEntry> ChangedRegisters { get; }
    public IReadOnlyList<PortTraceEntry> ChangedPorts { get; }

    public TraceFrame(
        ulong cycleCount,
        uint pc,
        ushort opcodeWord,
        IReadOnlyList<RegisterTraceEntry> changedRegisters,
        IReadOnlyList<PortTraceEntry> changedPorts)
    {
        CycleCount = cycleCount;
        PC = pc;
        OpcodeWord = opcodeWord;
        ChangedRegisters = changedRegisters;
        ChangedPorts = changedPorts;
    }
}
