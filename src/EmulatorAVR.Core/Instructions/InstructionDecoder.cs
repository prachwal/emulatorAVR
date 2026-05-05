namespace EmulatorAVR.Core.Instructions;

public class InstructionDecoder
{
    public Instruction Decode(ushort opcode, ushort nextWord = 0)
    {
        if (opcode == 0x0000)
            return new Instruction(opcode, InstructionKind.Nop);

        // SLEEP 1001 0101 1000 1000
        if (opcode == 0x9588)
            return new Instruction(opcode, InstructionKind.Sleep);

        // WDR 1001 0101 1010 1000
        if (opcode == 0x95A8)
            return new Instruction(opcode, InstructionKind.Wdr);

        // BREAK 1001 0101 0110 1000
        if (opcode == 0x9578)
            return new Instruction(opcode, InstructionKind.Break);

        // JMP (2-word) 1001 010k kkkk 110k + 2nd word address
        if ((opcode & 0xFE0E) == 0x940C)
        {
            int kBits = ((opcode >> 4) & 0x0F) | ((opcode & 0x01) << 4);
            int target = (kBits << 16) | nextWord;
            return new Instruction(opcode, InstructionKind.Jmp, offset: target & 0x7FFF);
        }

        // CALL (2-word) 1001 010k kkkk 111k + 2nd word address
        if ((opcode & 0xFE0E) == 0x940E)
        {
            int kBits = ((opcode >> 4) & 0x0F) | ((opcode & 0x01) << 4);
            int target = (kBits << 16) | nextWord;
            return new Instruction(opcode, InstructionKind.Call, offset: target & 0x7FFF);
        }

        // IJMP 1001 0100 0000 1001
        if (opcode == 0x9409)
            return new Instruction(opcode, InstructionKind.Ijmp);

        // ICALL 1001 0101 0000 1001
        if (opcode == 0x9509)
            return new Instruction(opcode, InstructionKind.Icall);

        // RET 1001 0101 0000 1000
        if (opcode == 0x9508)
            return new Instruction(opcode, InstructionKind.Ret);

        // RETI 1001 0101 0001 1000
        if (opcode == 0x9518)
            return new Instruction(opcode, InstructionKind.Reti);

        // MUL 1001 11rd dddd rrrr
        if ((opcode & 0xF000) == 0xE000)
        {
            int d = (opcode >> 4) & 0x0F;
            int rd = d + 16;
            byte k = (byte)(((opcode >> 4) & 0xF0) | (opcode & 0x0F));
            var kind = k == 0xFF ? InstructionKind.Ser : InstructionKind.Ldi;
            return new Instruction(opcode, kind, rd: rd, immediate: k);
        }

        // LPM 1001 0101 1100 1000 (implicit R0)
        if (opcode == 0x95C8)
            return new Instruction(opcode, InstructionKind.Lpm);

        // LPM Rd, Z+ 1001 000d dddd 0101
        if ((opcode & 0xFE0F) == 0x9005)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LpmZPlus, rd: rd);
        }

        // LDS (2-word) 1001 000d dddd 0000 + 2nd word data address
        if ((opcode & 0xFE0F) == 0x9000)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Lds, rd: rd, offset: nextWord);
        }

        // STS (2-word) 1001 001r rrrr 0000 + 2nd word data address
        if ((opcode & 0xFE0F) == 0x9200)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Sts, rd: rr, offset: nextWord);
        }

        // LD Rd, X 1001 000d dddd 1100
        if ((opcode & 0xFE0F) == 0x900C)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LdX, rd: rd);
        }

        // LD Rd, X+ 1001 000d dddd 1101
        if ((opcode & 0xFE0F) == 0x900D)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LdXPlus, rd: rd);
        }

        // LD Rd, -X 1001 000d dddd 1110
        if ((opcode & 0xFE0F) == 0x900E)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LdMinusX, rd: rd);
        }

        // ST X, Rr 1001 001r rrrr 1100
        if ((opcode & 0xFE0F) == 0x920C)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.StX, rd: rr);
        }

        // ST X+, Rr 1001 001r rrrr 1101
        if ((opcode & 0xFE0F) == 0x920D)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.StXPlus, rd: rr);
        }

        // ST -X, Rr 1001 001r rrrr 1110
        if ((opcode & 0xFE0F) == 0x920E)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.StMinusX, rd: rr);
        }

        // LD Rd, Y 1000 000d dddd 1000
        if ((opcode & 0xFE0F) == 0x8008)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LdY, rd: rd);
        }

        // LD Rd, Y+ 1000 000d dddd 1001
        if ((opcode & 0xFE0F) == 0x8009)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LdYPlus, rd: rd);
        }

        // LD Rd, -Y 1000 000d dddd 1010
        if ((opcode & 0xFE0F) == 0x800A)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LdMinusY, rd: rd);
        }

        // ST Y, Rr 1000 001r rrrr 1000
        if ((opcode & 0xFE0F) == 0x8208)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.StY, rd: rr);
        }

        // ST Y+, Rr 1000 001r rrrr 1001
        if ((opcode & 0xFE0F) == 0x8209)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.StYPlus, rd: rr);
        }

        // ST -Y, Rr 1000 001r rrrr 1010
        if ((opcode & 0xFE0F) == 0x820A)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.StMinusY, rd: rr);
        }

        // LD Rd, Z 1000 000d dddd 0000
        if ((opcode & 0xFE0F) == 0x8000)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LdZ, rd: rd);
        }

        // LD Rd, Z+ 1000 000d dddd 0001
        if ((opcode & 0xFE0F) == 0x8001)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LdZPlus, rd: rd);
        }

        // LD Rd, -Z 1000 000d dddd 0010
        if ((opcode & 0xFE0F) == 0x8002)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.LdMinusZ, rd: rd);
        }

        // ST Z, Rr 1000 001r rrrr 0000
        if ((opcode & 0xFE0F) == 0x8200)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.StZ, rd: rr);
        }

        // ST Z+, Rr 1000 001r rrrr 0001
        if ((opcode & 0xFE0F) == 0x8201)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.StZPlus, rd: rr);
        }

        // ST -Z, Rr 1000 001r rrrr 0010
        if ((opcode & 0xFE0F) == 0x8202)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.StMinusZ, rd: rr);
        }

        // MUL 1001 11rd dddd rrrr
        if ((opcode & 0xFC00) == 0x9C00)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Mul, rd: rd, rr: rr);
        }

        // MULS 0000 0010 dddd rrrr (signed multiply, R16-R23)
        if ((opcode & 0xFF00) == 0x0200)
        {
            int rd = ((opcode >> 4) & 0x0F) + 16;
            int rr = (opcode & 0x0F) + 16;
            return new Instruction(opcode, InstructionKind.Muls, rd: rd, rr: rr);
        }

        // FMUL 0000 0011 0ddd 1000 (Rd in R16-R23, Rr=R16)
        if ((opcode & 0xFF8F) == 0x0308)
        {
            int rd = ((opcode >> 4) & 0x07) + 16;
            return new Instruction(opcode, InstructionKind.Fmul, rd: rd, rr: 16);
        }

        // FMULS 0000 0011 0ddd 1010 (signed)
        if ((opcode & 0xFF8F) == 0x030A)
        {
            int rd = ((opcode >> 4) & 0x07) + 16;
            return new Instruction(opcode, InstructionKind.Fmuls, rd: rd, rr: 16);
        }

        // FMULSU 0000 0011 0ddd 1011 (signed × unsigned)
        if ((opcode & 0xFF8F) == 0x030B)
        {
            int rd = ((opcode >> 4) & 0x07) + 16;
            return new Instruction(opcode, InstructionKind.Fmulsu, rd: rd, rr: 16);
        }

        // MULSU 0000 0011 0ddd 0rrr (signed × unsigned)
        if ((opcode & 0xFF88) == 0x0300)
        {
            int rd = ((opcode >> 4) & 0x07) + 16;
            int rr = (opcode & 0x07) + 16;
            return new Instruction(opcode, InstructionKind.Mulsu, rd: rd, rr: rr);
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

        // CPSE 0001 00rd dddd rrrr (compare skip if equal)
        if ((opcode & 0xFC00) == 0x1000)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int rr = (opcode & 0x0F) | ((opcode >> 5) & 0x10);
            return new Instruction(opcode, InstructionKind.Cpse, rd: rd, rr: rr);
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

        // POP 1001 000d dddd 1111
        if ((opcode & 0xFE0F) == 0x900F)
        {
            int rd = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Pop, rd: rd);
        }

        // PUSH 1001 001r rrrr 1111
        if ((opcode & 0xFE0F) == 0x920F)
        {
            int rr = (opcode >> 4) & 0x1F;
            return new Instruction(opcode, InstructionKind.Push, rd: rr);
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

        // BSET 1001 0100 0sss 1000 / flag aliases
        if ((opcode & 0xFF8F) == 0x9408)
        {
            int s = (opcode >> 4) & 0x07;
            InstructionKind kind = s switch
            {
                0 => InstructionKind.Sec,
                1 => InstructionKind.Sez,
                2 => InstructionKind.Sen,
                3 => InstructionKind.Sev,
                4 => InstructionKind.Ses,
                5 => InstructionKind.Seh,
                6 => InstructionKind.Set,
                7 => InstructionKind.Sei,
                _ => InstructionKind.Bset
            };
            return new Instruction(opcode, kind, rd: s);
        }

        // BCLR 1001 0100 1sss 1000 / flag aliases
        if ((opcode & 0xFF8F) == 0x9488)
        {
            int s = (opcode >> 4) & 0x07;
            InstructionKind kind = s switch
            {
                0 => InstructionKind.Clc,
                1 => InstructionKind.Clz,
                2 => InstructionKind.Cln,
                3 => InstructionKind.Clv,
                4 => InstructionKind.Cls,
                5 => InstructionKind.Clh,
                6 => InstructionKind.Clt,
                7 => InstructionKind.Cli,
                _ => InstructionKind.Bclr
            };
            return new Instruction(opcode, kind, rd: s);
        }

        // IN 1011 0AAd dddd AAAA
        if ((opcode & 0xF800) == 0xB000)
        {
            int rd = ((opcode >> 4) & 0x1F);
            int addr = ((opcode >> 5) & 0x30) | (opcode & 0x0F);
            int dataAddr = addr + 0x20;
            return new Instruction(opcode, InstructionKind.In, rd: rd, immediate: (byte)dataAddr);
        }

        // OUT 1011 1AAr rrrr AAAA
        if ((opcode & 0xF800) == 0xB800)
        {
            int rr = ((opcode >> 4) & 0x1F);
            int addr = ((opcode >> 5) & 0x30) | (opcode & 0x0F);
            int dataAddr = addr + 0x20;
            return new Instruction(opcode, InstructionKind.Out, rd: rr, immediate: (byte)dataAddr);
        }

        // SBI 1001 1010 AAAA Abbb
        if ((opcode & 0xFE00) == 0x9A00)
        {
            int addr = (opcode >> 3) & 0x1F;
            int bit = opcode & 0x07;
            return new Instruction(opcode, InstructionKind.Sbi, rd: addr, immediate: (byte)bit);
        }

        // CBI 1001 1000 AAAA Abbb
        if ((opcode & 0xFE00) == 0x9800)
        {
            int addr = (opcode >> 3) & 0x1F;
            int bit = opcode & 0x07;
            return new Instruction(opcode, InstructionKind.Cbi, rd: addr, immediate: (byte)bit);
        }

        // SBRC 1111 110r rrrr 0bbb
        if ((opcode & 0xFC08) == 0xF800)
        {
            int rr = (opcode >> 5) & 0x1F;
            int bit = opcode & 0x07;
            return new Instruction(opcode, InstructionKind.Sbrc, rd: rr, immediate: (byte)bit);
        }

        // SBRS 1111 111r rrrr 0bbb
        if ((opcode & 0xFC08) == 0xFC00)
        {
            int rr = (opcode >> 5) & 0x1F;
            int bit = opcode & 0x07;
            return new Instruction(opcode, InstructionKind.Sbrs, rd: rr, immediate: (byte)bit);
        }

        // RJMP 1100 kkkk kkkk kkkk
        if ((opcode & 0xF000) == 0xC000)
        {
            int k = opcode & 0x0FFF;
            if ((k & 0x0800) != 0) k |= unchecked((int)0xFFFFF000);
            return new Instruction(opcode, InstructionKind.Rjmp, offset: k);
        }

        // RCALL 1101 kkkk kkkk kkkk
        if ((opcode & 0xF000) == 0xD000)
        {
            int k = opcode & 0x0FFF;
            if ((k & 0x0800) != 0) k |= unchecked((int)0xFFFFF000);
            return new Instruction(opcode, InstructionKind.Rcall, offset: k);
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
