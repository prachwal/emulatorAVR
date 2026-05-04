namespace EmulatorAVR.Core.Memory;

public class DataMemory
{
    public const int RegisterFileStart = 0x0000;
    public const int RegisterFileSize = 32;
    public const int IoRegistersStart = 0x0020;
    public const int IoRegistersSize = 64;
    public const int ExtendedIoStart = 0x0060;
    public const int ExtendedIoSize = 160;
    public const int SramStart = 0x0100;
    public const int SramSize = 2048;

    public const int Size = 0x0900;

    private readonly byte[] _memory = new byte[Size];

    public byte this[int address]
    {
        get
        {
            if (address < 0 || address >= Size)
                throw new ArgumentOutOfRangeException(nameof(address));
            return _memory[address];
        }
        set
        {
            if (address < 0 || address >= Size)
                throw new ArgumentOutOfRangeException(nameof(address));
            _memory[address] = value;
        }
    }
}
