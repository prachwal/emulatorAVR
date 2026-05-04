namespace EmulatorAVR.Core.Cpu;

public class AvrRegisters
{
    private readonly byte[] _registers = new byte[32];

    public const int Count = 32;

    public byte this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _registers[index];
        }
        set
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            _registers[index] = value;
        }
    }

    public AvrRegisters()
    {
    }

    public AvrRegisters(byte[] snapshot)
    {
        if (snapshot.Length != Count)
            throw new ArgumentException($"Snapshot must have exactly {Count} bytes.", nameof(snapshot));
        snapshot.CopyTo(_registers, 0);
    }

    public byte[] Snapshot()
    {
        var copy = new byte[Count];
        _registers.CopyTo(copy, 0);
        return copy;
    }
}
