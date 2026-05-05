namespace EmulatorAVR.Core.Instructions;

public class InstructionDecoder
{
    public Instruction Decode(ushort opcode, ushort nextWord = 0)
    {
        if (opcode == 0x0000)
            return new Instruction(opcode, InstructionKind.Nop);

        if (opcode == 0x9588) return new Instruction(opcode, InstructionKind.Sleep);
        if (opcode == 0x95A8) return new Instruction(opcode, InstructionKind.Wdr);
        if (opcode == 0x9578) return new Instruction(opcode, InstructionKind.Break);

        if ((opcode & 0xFE0E) == 0x940C)
            return new Instruction(opcode, InstructionKind.Jmp, offset: DecodeAbsolute22(opcode, nextWord), lengthWords: 2);

        if ((opcode & 0xFE0E) == 0x940E)
            return new Instruction(opcode, InstructionKind.Call, offset: DecodeAbsolute22(opcode, nextWord), lengthWords: 2);

        if (opcode == 0x9409) return new Instruction(opcode, InstructionKind.Ijmp);
        if (opcode == 0x9509) return new Instruction(opcode, InstructionKind.Icall);
        if (opcode == 0x9508) return new Instruction(opcode, InstructionKind.Ret);
        if (opcode == 0x9518) return new Instruction(opcode, InstructionKind.Reti);

        if ((opcode & 0xF000) == 0xE000)
        {
            int rd = 16 + ((opcode >> 4) & 0x0F);
            byte k = DecodeImmediate8(opcode);
            return new Instruction(opcode, k == 0xFF ? InstructionKind.Ser : InstructionKind.Ldi, rd: rd, immediate: k);
        }

        if (opcode == 0x95C8)
            return new Instruction(opcode, InstructionKind.Lpm, rd: 0);

        if ((opcode & 0xFE0F) == 0x9004)
            return new Instruction(opcode, InstructionKind.Lpm, rd: (opcode >> 4) & 0x1F);

        if ((opcode & 0xFE0F) == 0x9005)
            return new Instruction(opcode, InstructionKind.LpmZPlus, rd: (opcode >> 4) & 0x1F);

        // ELPM/EIJMP/EICALL are intentionally unsupported for the ATmega328P profile.
        if (opcode == 0x95D8 || (opcode & 0xFE0F) == 0x9006 || (opcode & 0xFE0F) == 0x9007)
            return new Instruction(opcode, InstructionKind.Unsupported);

        if (opcode == 0x95E8 || opcode == 0x95F8)
            return new Instruction(opcode, InstructionKind.Spm);

        if ((opcode & 0xFE0F) == 0x9000)
            return new Instruction(opcode, InstructionKind.Lds, rd: (opcode >> 4) & 0x1F, offset: nextWord, lengthWords: 2);

        if ((opcode & 0xFE0F) == 0x9200)
            return new Instruction(opcode, InstructionKind.Sts, rd: (opcode >> 4) & 0x1F, offset: nextWord, lengthWords: 2);

        if ((opcode & 0xFE0F) == 0x900C) return new Instruction(opcode, InstructionKind.LdX, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x900D) return new Instruction(opcode, InstructionKind.LdXPlus, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x900E) return new Instruction(opcode, InstructionKind.LdMinusX, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x920C) return new Instruction(opcode, InstructionKind.StX, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x920D) return new Instruction(opcode, InstructionKind.StXPlus, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x920E) return new Instruction(opcode, InstructionKind.StMinusX, rd: (opcode >> 4) & 0x1F);

        if ((opcode & 0xFE0F) == 0x8008) return new Instruction(opcode, InstructionKind.LdY, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x8009) return new Instruction(opcode, InstructionKind.LdYPlus, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x800A) return new Instruction(opcode, InstructionKind.LdMinusY, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x8208) return new Instruction(opcode, InstructionKind.StY, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x8209) return new Instruction(opcode, InstructionKind.StYPlus, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x820A) return new Instruction(opcode, InstructionKind.StMinusY, rd: (opcode >> 4) & 0x1F);

        if ((opcode & 0xFE0F) == 0x8000) return new Instruction(opcode, InstructionKind.LdZ, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x8001) return new Instruction(opcode, InstructionKind.LdZPlus, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x8002) return new Instruction(opcode, InstructionKind.LdMinusZ, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x8200) return new Instruction(opcode, InstructionKind.StZ, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x8201) return new Instruction(opcode, InstructionKind.StZPlus, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x8202) return new Instruction(opcode, InstructionKind.StMinusZ, rd: (opcode >> 4) & 0x1F);

        if ((opcode & 0xD208) == 0x8008) return new Instruction(opcode, InstructionKind.LddY, rd: (opcode >> 4) & 0x1F, offset: DecodeQ(opcode));
        if ((opcode & 0xD208) == 0x8208) return new Instruction(opcode, InstructionKind.StdY, rd: (opcode >> 4) & 0x1F, offset: DecodeQ(opcode));
        if ((opcode & 0xD208) == 0x8000) return new Instruction(opcode, InstructionKind.LddZ, rd: (opcode >> 4) & 0x1F, offset: DecodeQ(opcode));
        if ((opcode & 0xD208) == 0x8200) return new Instruction(opcode, InstructionKind.StdZ, rd: (opcode >> 4) & 0x1F, offset: DecodeQ(opcode));

        if ((opcode & 0xFC00) == 0x9C00) return DecodeTwoRegister(opcode, InstructionKind.Mul);
        if ((opcode & 0xFF00) == 0x0200) return new Instruction(opcode, InstructionKind.Muls, rd: 16 + ((opcode >> 4) & 0x0F), rr: 16 + (opcode & 0x0F));
        if ((opcode & 0xFF88) == 0x0300) return DecodeSmallMul(opcode, InstructionKind.Mulsu);
        if ((opcode & 0xFF88) == 0x0308) return DecodeSmallMul(opcode, InstructionKind.Fmul);
        if ((opcode & 0xFF88) == 0x0380) return DecodeSmallMul(opcode, InstructionKind.Fmuls);
        if ((opcode & 0xFF88) == 0x0388) return DecodeSmallMul(opcode, InstructionKind.Fmulsu);

        if ((opcode & 0xFC00) == 0x2C00) return DecodeTwoRegister(opcode, InstructionKind.Mov);
        if ((opcode & 0xFC00) == 0x2000) return DecodeAliasTwoRegister(opcode, InstructionKind.And, InstructionKind.Tst);
        if ((opcode & 0xFC00) == 0x2400) return DecodeAliasTwoRegister(opcode, InstructionKind.Eor, InstructionKind.Clr);
        if ((opcode & 0xFC00) == 0x2800) return DecodeTwoRegister(opcode, InstructionKind.Or);
        if ((opcode & 0xFC00) == 0x1000) return DecodeSkipTwoRegister(opcode, InstructionKind.Cpse, nextWord);
        if ((opcode & 0xFC00) == 0x0C00) return DecodeAliasTwoRegister(opcode, InstructionKind.Add, InstructionKind.Lsl);
        if ((opcode & 0xFC00) == 0x1C00) return DecodeAliasTwoRegister(opcode, InstructionKind.Adc, InstructionKind.Rol);
        if ((opcode & 0xFC00) == 0x1800) return DecodeTwoRegister(opcode, InstructionKind.Sub);
        if ((opcode & 0xFC00) == 0x1400) return DecodeTwoRegister(opcode, InstructionKind.Cp);
        if ((opcode & 0xFC00) == 0x0400) return DecodeTwoRegister(opcode, InstructionKind.Cpc);
        if ((opcode & 0xFC00) == 0x0800) return DecodeTwoRegister(opcode, InstructionKind.Sbc);

        if ((opcode & 0xF000) == 0x6000) return DecodeImmediateInstruction(opcode, InstructionKind.Ori);
        if ((opcode & 0xF000) == 0x7000) return DecodeImmediateInstruction(opcode, InstructionKind.Andi);
        if ((opcode & 0xF000) == 0x5000) return DecodeImmediateInstruction(opcode, InstructionKind.Subi);
        if ((opcode & 0xF000) == 0x4000) return DecodeImmediateInstruction(opcode, InstructionKind.Sbci);
        if ((opcode & 0xF000) == 0x3000) return DecodeImmediateInstruction(opcode, InstructionKind.Cpi);

        if ((opcode & 0xFE0F) == 0x900F) return new Instruction(opcode, InstructionKind.Pop, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x920F) return new Instruction(opcode, InstructionKind.Push, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x9400) return new Instruction(opcode, InstructionKind.Com, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x9403) return new Instruction(opcode, InstructionKind.Inc, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x940A) return new Instruction(opcode, InstructionKind.Dec, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x9401) return new Instruction(opcode, InstructionKind.Neg, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x9402) return new Instruction(opcode, InstructionKind.Swap, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x9405) return new Instruction(opcode, InstructionKind.Asr, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x9406) return new Instruction(opcode, InstructionKind.Lsr, rd: (opcode >> 4) & 0x1F);
        if ((opcode & 0xFE0F) == 0x9407) return new Instruction(opcode, InstructionKind.Ror, rd: (opcode >> 4) & 0x1F);

        if ((opcode & 0xFF00) == 0x9600) return DecodeWordImmediate(opcode, InstructionKind.Adiw);
        if ((opcode & 0xFF00) == 0x9700) return DecodeWordImmediate(opcode, InstructionKind.Sbiw);

        if ((opcode & 0xFF8F) == 0x9408) return DecodeFlagAlias(opcode, true);
        if ((opcode & 0xFF8F) == 0x9488) return DecodeFlagAlias(opcode, false);

        if ((opcode & 0xF800) == 0xB000)
        {
            int rd = (opcode >> 4) & 0x1F;
            int dataAddr = DecodeIo6(opcode) + 0x20;
            return new Instruction(opcode, InstructionKind.In, rd: rd, immediate: (byte)dataAddr);
        }

        if ((opcode & 0xF800) == 0xB800)
        {
            int rr = (opcode >> 4) & 0x1F;
            int dataAddr = DecodeIo6(opcode) + 0x20;
            return new Instruction(opcode, InstructionKind.Out, rd: rr, immediate: (byte)dataAddr);
        }

        if ((opcode & 0xFF00) == 0x9A00) return DecodeIoBit(opcode, InstructionKind.Sbi);
        if ((opcode & 0xFF00) == 0x9800) return DecodeIoBit(opcode, InstructionKind.Cbi);
        if ((opcode & 0xFF00) == 0x9900) return DecodeSkipIoBit(opcode, InstructionKind.Sbic, nextWord);
        if ((opcode & 0xFF00) == 0x9B00) return DecodeSkipIoBit(opcode, InstructionKind.Sbis, nextWord);

        if ((opcode & 0xFE08) == 0xF800) return DecodeRegisterBit(opcode, InstructionKind.Bld);
        if ((opcode & 0xFE08) == 0xFA00) return DecodeRegisterBit(opcode, InstructionKind.Bst);
        if ((opcode & 0xFE08) == 0xFC00) return DecodeSkipRegisterBit(opcode, InstructionKind.Sbrc, nextWord);
        if ((opcode & 0xFE08) == 0xFE00) return DecodeSkipRegisterBit(opcode, InstructionKind.Sbrs, nextWord);

        if ((opcode & 0xF000) == 0xC000)
            return new Instruction(opcode, InstructionKind.Rjmp, offset: SignExtend(opcode & 0x0FFF, 12));

        if ((opcode & 0xF000) == 0xD000)
            return new Instruction(opcode, InstructionKind.Rcall, offset: SignExtend(opcode & 0x0FFF, 12));

        if ((opcode & 0xFC00) == 0xF000)
            return DecodeBranch(opcode);

        return new Instruction(opcode, InstructionKind.Unsupported);
    }

    private static int DecodeAbsolute22(ushort opcode, ushort nextWord)
    {
        int high = ((opcode >> 4) & 0x1F) | ((opcode & 0x01) << 5);
        return (high << 16) | nextWord;
    }

    private static Instruction DecodeImmediateInstruction(ushort opcode, InstructionKind kind)
        => new(opcode, kind, rd: 16 + ((opcode >> 4) & 0x0F), immediate: DecodeImmediate8(opcode));

    private static byte DecodeImmediate8(ushort opcode)
        => (byte)(((opcode >> 4) & 0xF0) | (opcode & 0x0F));

    private static Instruction DecodeTwoRegister(ushort opcode, InstructionKind kind)
        => new(opcode, kind, rd: DecodeRd(opcode), rr: DecodeRr(opcode));

    private static Instruction DecodeAliasTwoRegister(ushort opcode, InstructionKind normalKind, InstructionKind aliasKind)
    {
        int rd = DecodeRd(opcode);
        int rr = DecodeRr(opcode);
        return new Instruction(opcode, rd == rr ? aliasKind : normalKind, rd: rd, rr: rr);
    }

    private static Instruction DecodeSkipTwoRegister(ushort opcode, InstructionKind kind, ushort nextWord)
        => new(opcode, kind, rd: DecodeRd(opcode), rr: DecodeRr(opcode), skipWords: InstructionLength(nextWord));

    private static int DecodeRd(ushort opcode) => (opcode >> 4) & 0x1F;
    private static int DecodeRr(ushort opcode) => (opcode & 0x0F) | ((opcode >> 5) & 0x10);

    private static Instruction DecodeSmallMul(ushort opcode, InstructionKind kind)
        => new(opcode, kind, rd: 16 + ((opcode >> 4) & 0x07), rr: 16 + (opcode & 0x07));

    private static Instruction DecodeWordImmediate(ushort opcode, InstructionKind kind)
    {
        int wordPair = ((opcode >> 4) & 0x03) switch { 0 => 24, 1 => 26, 2 => 28, _ => 30 };
        byte k = (byte)((((opcode >> 6) & 0x03) << 4) | (opcode & 0x0F));
        return new Instruction(opcode, kind, wordRegisterPair: wordPair, immediate: k);
    }

    private static Instruction DecodeFlagAlias(ushort opcode, bool set)
    {
        int bit = (opcode >> 4) & 0x07;
        InstructionKind kind = (set, bit) switch
        {
            (true, 0) => InstructionKind.Sec, (false, 0) => InstructionKind.Clc,
            (true, 1) => InstructionKind.Sez, (false, 1) => InstructionKind.Clz,
            (true, 2) => InstructionKind.Sen, (false, 2) => InstructionKind.Cln,
            (true, 3) => InstructionKind.Sev, (false, 3) => InstructionKind.Clv,
            (true, 4) => InstructionKind.Ses, (false, 4) => InstructionKind.Cls,
            (true, 5) => InstructionKind.Seh, (false, 5) => InstructionKind.Clh,
            (true, 6) => InstructionKind.Set, (false, 6) => InstructionKind.Clt,
            (true, 7) => InstructionKind.Sei, (false, 7) => InstructionKind.Cli,
            _ => set ? InstructionKind.Bset : InstructionKind.Bclr
        };
        return new Instruction(opcode, kind, rd: bit);
    }

    private static int DecodeIo6(ushort opcode) => ((opcode >> 5) & 0x30) | (opcode & 0x0F);

    private static Instruction DecodeIoBit(ushort opcode, InstructionKind kind)
        => new(opcode, kind, rd: (opcode >> 3) & 0x1F, immediate: (byte)(opcode & 0x07));

    private static Instruction DecodeSkipIoBit(ushort opcode, InstructionKind kind, ushort nextWord)
        => new(opcode, kind, rd: (opcode >> 3) & 0x1F, immediate: (byte)(opcode & 0x07), skipWords: InstructionLength(nextWord));

    private static Instruction DecodeRegisterBit(ushort opcode, InstructionKind kind)
        => new(opcode, kind, rd: (opcode >> 4) & 0x1F, immediate: (byte)(opcode & 0x07));

    private static Instruction DecodeSkipRegisterBit(ushort opcode, InstructionKind kind, ushort nextWord)
        => new(opcode, kind, rd: (opcode >> 4) & 0x1F, immediate: (byte)(opcode & 0x07), skipWords: InstructionLength(nextWord));

    private static Instruction DecodeBranch(ushort opcode)
    {
        int offset = SignExtend((opcode >> 3) & 0x7F, 7);
        int bit = opcode & 0x07;
        bool set = (opcode & 0x0400) == 0;
        InstructionKind kind = (set, bit) switch
        {
            (true, 0) => InstructionKind.Brcs, (false, 0) => InstructionKind.Brcc,
            (true, 1) => InstructionKind.Breq, (false, 1) => InstructionKind.Brne,
            (true, 2) => InstructionKind.Brmi, (false, 2) => InstructionKind.Brpl,
            (true, 3) => InstructionKind.Brvs, (false, 3) => InstructionKind.Brvc,
            (true, 4) => InstructionKind.Brlt, (false, 4) => InstructionKind.Brge,
            (true, 5) => InstructionKind.Brhs, (false, 5) => InstructionKind.Brhc,
            (true, 6) => InstructionKind.Brts, (false, 6) => InstructionKind.Brtc,
            (true, 7) => InstructionKind.Brie, (false, 7) => InstructionKind.Brid,
            _ => set ? InstructionKind.Brbs : InstructionKind.Brbc
        };
        return new Instruction(opcode, kind, rd: bit, offset: offset);
    }

    private static int DecodeQ(ushort opcode)
        => ((opcode & 0x2000) != 0 ? 32 : 0)
         | ((opcode & 0x0800) != 0 ? 16 : 0)
         | ((opcode & 0x0400) != 0 ? 8 : 0)
         | (opcode & 0x07);

    private static int InstructionLength(ushort opcode)
        => ((opcode & 0xFE0E) == 0x940C || (opcode & 0xFE0E) == 0x940E || (opcode & 0xFE0F) == 0x9000 || (opcode & 0xFE0F) == 0x9200) ? 2 : 1;

    private static int SignExtend(int value, int bits)
    {
        int signBit = 1 << (bits - 1);
        int mask = (1 << bits) - 1;
        value &= mask;
        return (value ^ signBit) - signBit;
    }
}
