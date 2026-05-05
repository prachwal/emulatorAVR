namespace EmulatorAVR.Core.Cpu;

public static class StatusRegisterMath
{
    public static bool HalfCarryAdd(byte a, byte b)
    {
        return ((a & 0x0F) + (b & 0x0F)) >= 0x10;
    }

    public static bool HalfCarrySub(byte a, byte b)
    {
        return ((a & 0x0F) - (b & 0x0F)) < 0;
    }

    public static bool CarryAdd(int result)
    {
        return result > 0xFF;
    }

    public static bool CarrySub(int result)
    {
        return result < 0;
    }

    public static bool OverflowAdd(byte a, byte b, byte result)
    {
        return ((a & 0x80) == (b & 0x80)) && ((a & 0x80) != (result & 0x80));
    }

    public static bool OverflowSub(byte a, byte b, byte result)
    {
        return ((a & 0x80) != (b & 0x80)) && ((a & 0x80) != (result & 0x80));
    }

    public static bool Negative(byte result)
    {
        return (result & 0x80) != 0;
    }

    public static bool Sign(bool negative, bool overflow)
    {
        return negative ^ overflow;
    }

    public static bool Zero(byte result)
    {
        return result == 0;
    }

    public static int WordRegisterValue(AvrRegisters registers, int pairBase)
    {
        return (registers[pairBase + 1] << 8) | registers[pairBase];
    }

    public static void SetWordRegisterValue(AvrRegisters registers, int pairBase, int value)
    {
        registers[pairBase] = (byte)(value & 0xFF);
        registers[pairBase + 1] = (byte)((value >> 8) & 0xFF);
    }

    public static bool GetSregBit(StatusRegister sreg, int bit)
    {
        return (sreg.Value & (1 << bit)) != 0;
    }
}
