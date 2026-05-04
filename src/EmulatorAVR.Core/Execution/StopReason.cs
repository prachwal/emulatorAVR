namespace EmulatorAVR.Core.Execution;

public enum StopReason
{
    MaxCycles,
    UnsupportedInstruction,
    ProgramEnd,
    Error
}
