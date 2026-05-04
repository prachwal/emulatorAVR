namespace EmulatorAVR.Core.Instructions;

public class Instruction
{
    public ushort Opcode { get; }
    public InstructionKind Kind { get; }
    public int Rd { get; }
    public int Rr { get; }
    public byte Immediate { get; }

    public Instruction(ushort opcode, InstructionKind kind, int rd = -1, int rr = -1, byte immediate = 0)
    {
        Opcode = opcode;
        Kind = kind;
        Rd = rd;
        Rr = rr;
        Immediate = immediate;
    }
}
