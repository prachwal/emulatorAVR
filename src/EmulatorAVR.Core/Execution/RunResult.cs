using EmulatorAVR.Core.Tracing;

namespace EmulatorAVR.Core.Execution;

public class RunResult
{
    public StopReason Reason { get; }
    public uint FinalPC { get; }
    public ulong FinalCycleCount { get; }
    public IReadOnlyList<TraceFrame> TraceFrames { get; }
    public string? ErrorMessage { get; }

    public RunResult(StopReason reason, uint finalPC, ulong finalCycleCount, IReadOnlyList<TraceFrame> traceFrames, string? errorMessage = null)
    {
        Reason = reason;
        FinalPC = finalPC;
        FinalCycleCount = finalCycleCount;
        TraceFrames = traceFrames;
        ErrorMessage = errorMessage;
    }
}
