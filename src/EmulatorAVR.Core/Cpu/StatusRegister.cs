namespace EmulatorAVR.Core.Cpu;

public class StatusRegister
{
    private byte _value;

    public byte Value
    {
        get => _value;
        set => _value = value;
    }

    public bool I
    {
        get => (_value & 0x80) != 0;
        set => _value = value ? (byte)(_value | 0x80) : (byte)(_value & ~0x80);
    }

    public bool T
    {
        get => (_value & 0x40) != 0;
        set => _value = value ? (byte)(_value | 0x40) : (byte)(_value & ~0x40);
    }

    public bool H
    {
        get => (_value & 0x20) != 0;
        set => _value = value ? (byte)(_value | 0x20) : (byte)(_value & ~0x20);
    }

    public bool S
    {
        get => (_value & 0x10) != 0;
        set => _value = value ? (byte)(_value | 0x10) : (byte)(_value & ~0x10);
    }

    public bool V
    {
        get => (_value & 0x08) != 0;
        set => _value = value ? (byte)(_value | 0x08) : (byte)(_value & ~0x08);
    }

    public bool N
    {
        get => (_value & 0x04) != 0;
        set => _value = value ? (byte)(_value | 0x04) : (byte)(_value & ~0x04);
    }

    public bool Z
    {
        get => (_value & 0x02) != 0;
        set => _value = value ? (byte)(_value | 0x02) : (byte)(_value & ~0x02);
    }

    public bool C
    {
        get => (_value & 0x01) != 0;
        set => _value = value ? (byte)(_value | 0x01) : (byte)(_value & ~0x01);
    }

    public StatusRegister()
    {
        _value = 0x00;
    }

    public StatusRegister(byte value)
    {
        _value = value;
    }
}
