namespace EmulatorAVR.Core.Instructions;

public class Instruction
{
    public ushort Opcode { get; }
    public InstructionKind Kind { get; }
    public int Rd { get; }
    public int Rr { get; }
    public byte Immediate { get; }
    public int WordRegisterPair { get; }
    public int Offset { get; }
    public int LengthWords { get; }
    public int SkipWords { get; }

    public Instruction(
        ushort opcode,
        InstructionKind kind,
        int rd = -1,
        int rr = -1,
        byte immediate = 0,
        int wordRegisterPair = -1,
        int offset = 0,
        int lengthWords = 1,
        int skipWords = 1)
    {
        Opcode = opcode;
        Kind = kind;
        Rd = rd;
        Rr = rr;
        Immediate = immediate;
        WordRegisterPair = wordRegisterPair;
        Offset = offset;
        LengthWords = lengthWords;
        SkipWords = skipWords;
    }
}
