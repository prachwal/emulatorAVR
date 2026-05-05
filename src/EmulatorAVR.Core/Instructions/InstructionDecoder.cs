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

        // ADD 0000 11rd dddd rrrr
        if ((opcode & 0xFC00) == 0x0C00)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Add, rd: rd, rr: rr);
        }

        // ADC 0001 11rd dddd rrrr
        if ((opcode & 0xFC00) == 0x1C00)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Adc, rd: rd, rr: rr);
        }

        // SUB 0001 10rd dddd rrrr
        if ((opcode & 0xFC00) == 0x1800)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Sub, rd: rd, rr: rr);
        }

        // CP 0001 01rd dddd rrrr
        if ((opcode & 0xFC00) == 0x1400)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Cp, rd: rd, rr: rr);
        }

        // CPC 0000 01rd dddd rrrr
        if ((opcode & 0xFC00) == 0x0400)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Cpc, rd: rd, rr: rr);
        }

        // SBC 0000 10rd dddd rrrr
        if ((opcode & 0xFC00) == 0x0800)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Sbc, rd: rd, rr: rr);
        }

        // SUBI 0101 KKKK dddd KKKK (Rd 16..31)
        if ((opcode & 0xF000) == 0x5000)
        {
            int d = (opcode >> 4) & 0x0F;
            int rd = d + 16;
            byte k = (byte)(((opcode >> 4) & 0xF0) | (opcode & 0x0F));
            return new Instruction(opcode, InstructionKind.Subi, rd: rd, immediate: k);
        }

        // SBCI 0100 KKKK dddd KKKK (Rd 16..31)
        if ((opcode & 0xF000) == 0x4000)
        {
            int d = (opcode >> 4) & 0x0F;
            int rd = d + 16;
            byte k = (byte)(((opcode >> 4) & 0xF0) | (opcode & 0x0F));
            return new Instruction(opcode, InstructionKind.Sbci, rd: rd, immediate: k);
        }

        // CPI 0011 KKKK dddd KKKK (Rd 16..31)
        if ((opcode & 0xF000) == 0x3000)
        {
            int d = (opcode >> 4) & 0x0F;
            int rd = d + 16;
            byte k = (byte)(((opcode >> 4) & 0xF0) | (opcode & 0x0F));
            return new Instruction(opcode, InstructionKind.Cpi, rd: rd, immediate: k);
        }

        // INC 1001 010d dddd 0011
        if ((opcode & 0xFE0F) == 0x9403)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Inc, rd: rd);
        }

        // DEC 1001 010d dddd 1010
        if ((opcode & 0xFE0F) == 0x940A)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Dec, rd: rd);
        }

        // NEG 1001 010d dddd 0001
        if ((opcode & 0xFE0F) == 0x9401)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Neg, rd: rd);
        }

        // ADIW 1001 0110 KKdd KKKK
        if ((opcode & 0xFF00) == 0x9600)
        {
            int dField = (opcode >> 4) & 0x03;
            int wordPair = dField switch
            {
                0 => 24,
                1 => 26,
                2 => 28,
                3 => 30,
                _ => 24
            };
            int kHigh = (opcode >> 6) & 0x03;
            int kLow = opcode & 0x0F;
            byte k = (byte)((kHigh << 4) | kLow);
            return new Instruction(opcode, InstructionKind.Adiw, wordRegisterPair: wordPair, immediate: k);
        }

        // SBIW 1001 0111 KKdd KKKK
        if ((opcode & 0xFF00) == 0x9700)
        {
            int dField = (opcode >> 4) & 0x03;
            int wordPair = dField switch
            {
                0 => 24,
                1 => 26,
                2 => 28,
                3 => 30,
                _ => 24
            };
            int kHigh = (opcode >> 6) & 0x03;
            int kLow = opcode & 0x0F;
            byte k = (byte)((kHigh << 4) | kLow);
            return new Instruction(opcode, InstructionKind.Sbiw, wordRegisterPair: wordPair, immediate: k);
        }

        return new Instruction(opcode, InstructionKind.Unsupported);
    }
}
