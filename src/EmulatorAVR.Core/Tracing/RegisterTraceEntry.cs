namespace EmulatorAVR.Core.Tracing;

public class RegisterTraceEntry
{
    public int RegisterIndex { get; }
    public byte OldValue { get; }
    public byte NewValue { get; }

    public RegisterTraceEntry(int registerIndex, byte oldValue, byte newValue)
    {
        RegisterIndex = registerIndex;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
