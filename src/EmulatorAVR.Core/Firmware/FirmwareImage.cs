namespace EmulatorAVR.Core.Firmware;

public class FirmwareImage
{
    public uint BaseAddress { get; }
    public int Length => _data.Length;
    private readonly byte[] _data;

    public FirmwareImage(uint baseAddress, byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        BaseAddress = baseAddress;
        _data = new byte[data.Length];
        data.CopyTo(_data, 0);
    }

    public byte[] ToArray()
    {
        var copy = new byte[_data.Length];
        _data.CopyTo(copy, 0);
        return copy;
    }
}
