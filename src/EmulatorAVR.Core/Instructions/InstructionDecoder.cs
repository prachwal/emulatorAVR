namespace EmulatorAVR.Core.Instructions;

public class InstructionDecoder
{
    public Instruction Decode(ushort opcode)
    {
        if (opcode == 0x0000)
            return new Instruction(opcode, InstructionKind.Nop);

        if ((opcode & 0xF000) == 0xE000)
        {
            int d = (opcode >> 4) & 0x0F;
            int rd = d + 16;
            byte k = (byte)(((opcode >> 4) & 0xF0) | (opcode & 0x0F));
            return new Instruction(opcode, InstructionKind.Ldi, rd: rd, immediate: k);
        }

        if ((opcode & 0xFC00) == 0x2C00)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Mov, rd: rd, rr: rr);
        }

        if ((opcode & 0xFC00) == 0x0C00)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Add, rd: rd, rr: rr);
        }

        return new Instruction(opcode, InstructionKind.Unsupported);
    }
}
