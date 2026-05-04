namespace EmulatorAVR.Core.Ports;

public sealed class PortRegister
{
    public string Name { get; }
    public ushort Address { get; }
    public byte Value { get; private set; }

    public PortRegister(string name, ushort address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Register name cannot be null or whitespace.", nameof(name));
        Name = name;
        Address = address;
        Value = 0;
    }

    public byte Read() => Value;

    public void Write(byte value)
    {
        Value = value;
    }
}
