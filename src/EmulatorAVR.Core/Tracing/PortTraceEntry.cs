namespace EmulatorAVR.Core.Tracing;

public class PortTraceEntry
{
    public string PortName { get; }
    public byte OldValue { get; }
    public byte NewValue { get; }

    public PortTraceEntry(string portName, byte oldValue, byte newValue)
    {
        PortName = portName;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
