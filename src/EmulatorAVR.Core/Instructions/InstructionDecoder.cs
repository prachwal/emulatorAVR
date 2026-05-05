namespace EmulatorAVR.Core.Instructions;

public class InstructionDecoder
{
    public Instruction Decode(ushort opcode)
    {
        if (opcode == 0x0000)
            return new Instruction(opcode, InstructionKind.Nop);

        // LDI 1110 KKKK dddd KKKK / SER when K=0xFF
        if ((opcode & 0xF000) == 0xE000)
        {
            int d = (opcode >> 4) & 0x0F;
            int rd = d + 16;
            byte k = (byte)(((opcode >> 4) & 0xF0) | (opcode & 0x0F));
            var kind = k == 0xFF ? InstructionKind.Ser : InstructionKind.Ldi;
            return new Instruction(opcode, kind, rd: rd, immediate: k);
        }

        // MOV 0010 11rd dddd rrrr
        if ((opcode & 0xFC00) == 0x2C00)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Mov, rd: rd, rr: rr);
        }

        // AND 0010 00rd dddd rrrr / TST when Rr == Rd
        if ((opcode & 0xFC00) == 0x2000)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            var kind = rd == rr ? InstructionKind.Tst : InstructionKind.And;
            return new Instruction(opcode, kind, rd: rd, rr: rr);
        }

        // EOR 0010 01rd dddd rrrr / CLR when Rr == Rd
        if ((opcode & 0xFC00) == 0x2400)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            var kind = rd == rr ? InstructionKind.Clr : InstructionKind.Eor;
            return new Instruction(opcode, kind, rd: rd, rr: rr);
        }

        // OR 0010 10rd dddd rrrr
        if ((opcode & 0xFC00) == 0x2800)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Or, rd: rd, rr: rr);
        }

        // ADD 0000 11rd dddd rrrr / LSL when Rr == Rd
        if ((opcode & 0xFC00) == 0x0C00)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            var kind = rd == rr ? InstructionKind.Lsl : InstructionKind.Add;
            return new Instruction(opcode, kind, rd: rd, rr: rr);
        }

        // ADC 0001 11rd dddd rrrr / ROL when Rr == Rd
        if ((opcode & 0xFC00) == 0x1C00)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            var kind = rd == rr ? InstructionKind.Rol : InstructionKind.Adc;
            return new Instruction(opcode, kind, rd: rd, rr: rr);
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

        // ORI 0110 KKKK dddd KKKK (Rd 16..31)
        if ((opcode & 0xF000) == 0x6000)
        {
            int d = (opcode >> 4) & 0x0F;
            int rd = d + 16;
            byte k = (byte)(((opcode >> 4) & 0xF0) | (opcode & 0x0F));
            return new Instruction(opcode, InstructionKind.Ori, rd: rd, immediate: k);
        }

        // ANDI 0111 KKKK dddd KKKK (Rd 16..31)
        if ((opcode & 0xF000) == 0x7000)
        {
            int d = (opcode >> 4) & 0x0F;
            int rd = d + 16;
            byte k = (byte)(((opcode >> 4) & 0xF0) | (opcode & 0x0F));
            return new Instruction(opcode, InstructionKind.Andi, rd: rd, immediate: k);
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

        // COM 1001 010d dddd 0000
        if ((opcode & 0xFE0F) == 0x9400)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Com, rd: rd);
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

        // SWAP 1001 010d dddd 0010
        if ((opcode & 0xFE0F) == 0x9402)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Swap, rd: rd);
        }

        // ASR 1001 010d dddd 0101
        if ((opcode & 0xFE0F) == 0x9405)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Asr, rd: rd);
        }

        // LSR 1001 010d dddd 0110
        if ((opcode & 0xFE0F) == 0x9406)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Lsr, rd: rd);
        }

        // ROR 1001 010d dddd 0111
        if ((opcode & 0xFE0F) == 0x9407)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Ror, rd: rd);
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

        // RJMP 1100 kkkk kkkk kkkk
        if ((opcode & 0xF000) == 0xC000)
        {
            int k = opcode & 0x0FFF;
            if ((k & 0x0800) != 0) k |= unchecked((int)0xFFFFF000);
            return new Instruction(opcode, InstructionKind.Rjmp, offset: k);
        }

        // BRBS 1111 00kk kkkk ksss / branch aliases
        if ((opcode & 0xFC00) == 0xF000)
        {
            int bit = opcode & 0x07;
            int k = (opcode >> 3) & 0x7F;
            if ((k & 0x40) != 0) k |= unchecked((int)0xFFFFFF80);

            InstructionKind kind = bit switch
            {
                0 => InstructionKind.Brcs,
                1 => InstructionKind.Breq,
                2 => InstructionKind.Brmi,
                3 => InstructionKind.Brvs,
                4 => InstructionKind.Brlt,
                5 => InstructionKind.Brhs,
                6 => InstructionKind.Brts,
                7 => InstructionKind.Brie,
                _ => InstructionKind.Brbs
            };

            return new Instruction(opcode, kind, rd: bit, offset: k);
        }

        // BRBC 1111 01kk kkkk ksss / branch aliases
        if ((opcode & 0xFC00) == 0xF400)
        {
            int bit = opcode & 0x07;
            int k = (opcode >> 3) & 0x7F;
            if ((k & 0x40) != 0) k |= unchecked((int)0xFFFFFF80);

            InstructionKind kind = bit switch
            {
                0 => InstructionKind.Brcc,
                1 => InstructionKind.Brne,
                2 => InstructionKind.Brpl,
                3 => InstructionKind.Brvc,
                4 => InstructionKind.Brge,
                5 => InstructionKind.Brhc,
                6 => InstructionKind.Brtc,
                7 => InstructionKind.Brid,
                _ => InstructionKind.Brbc
            };

            return new Instruction(opcode, kind, rd: bit, offset: k);
        }

        return new Instruction(opcode, InstructionKind.Unsupported);
    }
}
