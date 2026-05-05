using EmulatorAVR.Core.Cpu;
using EmulatorAVR.Core.Instructions;

namespace EmulatorAVR.Core.Execution;

public class InstructionExecutor
{
    public ExecutionResult Execute(AvrCpuState state, Instruction instruction)
    {
        if (instruction.Kind == InstructionKind.Unsupported)
            return ExecutionResult.Unsupported;

        switch (instruction.Kind)
        {
            case InstructionKind.Nop:
            case InstructionKind.Spm:
            case InstructionKind.Sleep:
            case InstructionKind.Wdr:
            case InstructionKind.Break:
                break;
            case InstructionKind.Ldi:
            case InstructionKind.Ser:
                state.Registers[instruction.Rd] = instruction.Immediate;
                break;
            case InstructionKind.Mov:
                state.Registers[instruction.Rd] = state.Registers[instruction.Rr];
                break;
            case InstructionKind.Add:
            case InstructionKind.Lsl:
                Add(state, instruction.Rd, instruction.Rr);
                break;
            case InstructionKind.Adc:
            case InstructionKind.Rol:
                Adc(state, instruction.Rd, instruction.Rr);
                break;
            case InstructionKind.Sub:
                Sub(state, instruction.Rd, state.Registers[instruction.Rr], true);
                break;
            case InstructionKind.Subi:
                Sub(state, instruction.Rd, instruction.Immediate, true);
                break;
            case InstructionKind.Sbc:
                Sbc(state, instruction.Rd, state.Registers[instruction.Rr], true);
                break;
            case InstructionKind.Sbci:
                Sbc(state, instruction.Rd, instruction.Immediate, true);
                break;
            case InstructionKind.Cp:
                Sub(state, instruction.Rd, state.Registers[instruction.Rr], false);
                break;
            case InstructionKind.Cpi:
                Sub(state, instruction.Rd, instruction.Immediate, false);
                break;
            case InstructionKind.Cpc:
                Sbc(state, instruction.Rd, state.Registers[instruction.Rr], false);
                break;
            case InstructionKind.Inc:
                Inc(state, instruction.Rd);
                break;
            case InstructionKind.Dec:
                Dec(state, instruction.Rd);
                break;
            case InstructionKind.Neg:
                Neg(state, instruction.Rd);
                break;
            case InstructionKind.Adiw:
                Adiw(state, instruction.WordRegisterPair, instruction.Immediate);
                break;
            case InstructionKind.Sbiw:
                Sbiw(state, instruction.WordRegisterPair, instruction.Immediate);
                break;
            case InstructionKind.And:
            case InstructionKind.Tst:
                Logic(state, instruction.Rd, (byte)(state.Registers[instruction.Rd] & state.Registers[instruction.Rr]), instruction.Kind != InstructionKind.Tst);
                break;
            case InstructionKind.Andi:
                Logic(state, instruction.Rd, (byte)(state.Registers[instruction.Rd] & instruction.Immediate), true);
                break;
            case InstructionKind.Or:
                Logic(state, instruction.Rd, (byte)(state.Registers[instruction.Rd] | state.Registers[instruction.Rr]), true);
                break;
            case InstructionKind.Ori:
                Logic(state, instruction.Rd, (byte)(state.Registers[instruction.Rd] | instruction.Immediate), true);
                break;
            case InstructionKind.Eor:
                Logic(state, instruction.Rd, (byte)(state.Registers[instruction.Rd] ^ state.Registers[instruction.Rr]), true);
                break;
            case InstructionKind.Clr:
                Logic(state, instruction.Rd, 0, true);
                break;
            case InstructionKind.Com:
                Logic(state, instruction.Rd, (byte)~state.Registers[instruction.Rd], true);
                state.SREG.C = true;
                break;
            case InstructionKind.Lsr:
                Lsr(state, instruction.Rd);
                break;
            case InstructionKind.Ror:
                Ror(state, instruction.Rd);
                break;
            case InstructionKind.Asr:
                Asr(state, instruction.Rd);
                break;
            case InstructionKind.Swap:
                state.Registers[instruction.Rd] = (byte)((state.Registers[instruction.Rd] << 4) | (state.Registers[instruction.Rd] >> 4));
                break;
            case InstructionKind.Rjmp:
                state.ProgramCounter = (uint)((int)state.ProgramCounter + instruction.Offset);
                break;
            case InstructionKind.Jmp:
                state.ProgramCounter = (uint)(instruction.Offset - instruction.LengthWords);
                break;
            case InstructionKind.Ijmp:
                state.ProgramCounter = (uint)(GetWord(state, 30) - 1);
                break;
            case InstructionKind.Rcall:
                PushReturnAddress(state, (int)state.ProgramCounter + 1);
                state.ProgramCounter = (uint)((int)state.ProgramCounter + instruction.Offset);
                break;
            case InstructionKind.Call:
                PushReturnAddress(state, (int)state.ProgramCounter + 2);
                state.ProgramCounter = (uint)(instruction.Offset - instruction.LengthWords);
                break;
            case InstructionKind.Icall:
                PushReturnAddress(state, (int)state.ProgramCounter + 1);
                state.ProgramCounter = (uint)(GetWord(state, 30) - 1);
                break;
            case InstructionKind.Ret:
            case InstructionKind.Reti:
                state.ProgramCounter = (uint)(PopReturnAddress(state) - 1);
                if (instruction.Kind == InstructionKind.Reti)
                    state.SREG.I = true;
                break;
            case InstructionKind.Brbs:
            case InstructionKind.Breq:
            case InstructionKind.Brcs:
            case InstructionKind.Brmi:
            case InstructionKind.Brvs:
            case InstructionKind.Brlt:
            case InstructionKind.Brhs:
            case InstructionKind.Brts:
            case InstructionKind.Brie:
                if (GetSregBit(state, instruction.Rd))
                    state.ProgramCounter = (uint)((int)state.ProgramCounter + instruction.Offset);
                break;
            case InstructionKind.Brbc:
            case InstructionKind.Brne:
            case InstructionKind.Brcc:
            case InstructionKind.Brpl:
            case InstructionKind.Brvc:
            case InstructionKind.Brge:
            case InstructionKind.Brhc:
            case InstructionKind.Brtc:
            case InstructionKind.Brid:
                if (!GetSregBit(state, instruction.Rd))
                    state.ProgramCounter = (uint)((int)state.ProgramCounter + instruction.Offset);
                break;
            case InstructionKind.Cpse:
                if (state.Registers[instruction.Rd] == state.Registers[instruction.Rr])
                    state.ProgramCounter += (uint)instruction.SkipWords;
                break;
            case InstructionKind.Sbrc:
                if ((state.Registers[instruction.Rd] & (1 << instruction.Immediate)) == 0)
                    state.ProgramCounter += (uint)instruction.SkipWords;
                break;
            case InstructionKind.Sbrs:
                if ((state.Registers[instruction.Rd] & (1 << instruction.Immediate)) != 0)
                    state.ProgramCounter += (uint)instruction.SkipWords;
                break;
            case InstructionKind.Sbic:
                if ((state.DataMemory[instruction.Rd + 0x20] & (1 << instruction.Immediate)) == 0)
                    state.ProgramCounter += (uint)instruction.SkipWords;
                break;
            case InstructionKind.Sbis:
                if ((state.DataMemory[instruction.Rd + 0x20] & (1 << instruction.Immediate)) != 0)
                    state.ProgramCounter += (uint)instruction.SkipWords;
                break;
            case InstructionKind.In:
                state.Registers[instruction.Rd] = state.DataMemory[instruction.Immediate];
                break;
            case InstructionKind.Out:
                state.DataMemory[instruction.Immediate] = state.Registers[instruction.Rd];
                break;
            case InstructionKind.Sbi:
                state.DataMemory[instruction.Rd + 0x20] |= (byte)(1 << instruction.Immediate);
                break;
            case InstructionKind.Cbi:
                state.DataMemory[instruction.Rd + 0x20] &= (byte)~(1 << instruction.Immediate);
                break;
            case InstructionKind.Bst:
                state.SREG.T = (state.Registers[instruction.Rd] & (1 << instruction.Immediate)) != 0;
                break;
            case InstructionKind.Bld:
                if (state.SREG.T) state.Registers[instruction.Rd] |= (byte)(1 << instruction.Immediate);
                else state.Registers[instruction.Rd] &= (byte)~(1 << instruction.Immediate);
                break;
            case InstructionKind.Bset:
            case InstructionKind.Sec: case InstructionKind.Sez: case InstructionKind.Sen: case InstructionKind.Sev:
            case InstructionKind.Ses: case InstructionKind.Seh: case InstructionKind.Set: case InstructionKind.Sei:
                state.SREG.Value |= (byte)(1 << instruction.Rd);
                break;
            case InstructionKind.Bclr:
            case InstructionKind.Clc: case InstructionKind.Clz: case InstructionKind.Cln: case InstructionKind.Clv:
            case InstructionKind.Cls: case InstructionKind.Clh: case InstructionKind.Clt: case InstructionKind.Cli:
                state.SREG.Value &= (byte)~(1 << instruction.Rd);
                break;
            case InstructionKind.Push:
                PushByte(state, state.Registers[instruction.Rd]);
                break;
            case InstructionKind.Pop:
                state.Registers[instruction.Rd] = PopByte(state);
                break;
            case InstructionKind.Lpm:
                Lpm(state, instruction.Rd < 0 ? 0 : instruction.Rd, incrementZ: false);
                break;
            case InstructionKind.LpmZPlus:
                Lpm(state, instruction.Rd, incrementZ: true);
                break;
            case InstructionKind.Lds:
                state.Registers[instruction.Rd] = state.DataMemory[instruction.Offset];
                break;
            case InstructionKind.Sts:
                state.DataMemory[instruction.Offset] = state.Registers[instruction.Rd];
                break;
            case InstructionKind.LdX:
                state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 26)];
                break;
            case InstructionKind.LdXPlus:
                state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 26)]; IncrementPair(state, 26); break;
            case InstructionKind.LdMinusX:
                DecrementPair(state, 26); state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 26)]; break;
            case InstructionKind.StX:
                state.DataMemory[GetWord(state, 26)] = state.Registers[instruction.Rd]; break;
            case InstructionKind.StXPlus:
                state.DataMemory[GetWord(state, 26)] = state.Registers[instruction.Rd]; IncrementPair(state, 26); break;
            case InstructionKind.StMinusX:
                DecrementPair(state, 26); state.DataMemory[GetWord(state, 26)] = state.Registers[instruction.Rd]; break;
            case InstructionKind.LdY:
                state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 28)]; break;
            case InstructionKind.LdYPlus:
                state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 28)]; IncrementPair(state, 28); break;
            case InstructionKind.LdMinusY:
                DecrementPair(state, 28); state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 28)]; break;
            case InstructionKind.StY:
                state.DataMemory[GetWord(state, 28)] = state.Registers[instruction.Rd]; break;
            case InstructionKind.StYPlus:
                state.DataMemory[GetWord(state, 28)] = state.Registers[instruction.Rd]; IncrementPair(state, 28); break;
            case InstructionKind.StMinusY:
                DecrementPair(state, 28); state.DataMemory[GetWord(state, 28)] = state.Registers[instruction.Rd]; break;
            case InstructionKind.LdZ:
                state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 30)]; break;
            case InstructionKind.LdZPlus:
                state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 30)]; IncrementPair(state, 30); break;
            case InstructionKind.LdMinusZ:
                DecrementPair(state, 30); state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 30)]; break;
            case InstructionKind.StZ:
                state.DataMemory[GetWord(state, 30)] = state.Registers[instruction.Rd]; break;
            case InstructionKind.StZPlus:
                state.DataMemory[GetWord(state, 30)] = state.Registers[instruction.Rd]; IncrementPair(state, 30); break;
            case InstructionKind.StMinusZ:
                DecrementPair(state, 30); state.DataMemory[GetWord(state, 30)] = state.Registers[instruction.Rd]; break;
            case InstructionKind.LddY:
                state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 28) + instruction.Offset]; break;
            case InstructionKind.StdY:
                state.DataMemory[GetWord(state, 28) + instruction.Offset] = state.Registers[instruction.Rd]; break;
            case InstructionKind.LddZ:
                state.Registers[instruction.Rd] = state.DataMemory[GetWord(state, 30) + instruction.Offset]; break;
            case InstructionKind.StdZ:
                state.DataMemory[GetWord(state, 30) + instruction.Offset] = state.Registers[instruction.Rd]; break;
            case InstructionKind.Mul:
                Mul(state, state.Registers[instruction.Rd], state.Registers[instruction.Rr]); break;
            case InstructionKind.Muls:
                Mul(state, (sbyte)state.Registers[instruction.Rd], (sbyte)state.Registers[instruction.Rr]); break;
            case InstructionKind.Mulsu:
                Mul(state, (sbyte)state.Registers[instruction.Rd], state.Registers[instruction.Rr]); break;
            case InstructionKind.Fmul:
                Mul(state, state.Registers[instruction.Rd] * state.Registers[instruction.Rr] << 1); break;
            case InstructionKind.Fmuls:
                Mul(state, ((sbyte)state.Registers[instruction.Rd] * (sbyte)state.Registers[instruction.Rr]) << 1); break;
            case InstructionKind.Fmulsu:
                Mul(state, ((sbyte)state.Registers[instruction.Rd] * state.Registers[instruction.Rr]) << 1); break;
            default:
                return ExecutionResult.Unsupported;
        }

        state.ProgramCounter += (uint)instruction.LengthWords;
        state.AddCycles(1);
        return ExecutionResult.Ok;
    }

    private static void Add(AvrCpuState s, int rd, int rr)
    {
        byte a = s.Registers[rd], b = s.Registers[rr]; int r = a + b; byte rb = (byte)r;
        s.SREG.H = ((a & 0x0F) + (b & 0x0F)) > 0x0F; s.SREG.C = r > 0xFF; s.SREG.N = (rb & 0x80) != 0;
        s.SREG.V = ((a & 0x80) == (b & 0x80)) && ((a & 0x80) != (rb & 0x80)); s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = rb == 0; s.Registers[rd] = rb;
    }
    private static void Adc(AvrCpuState s, int rd, int rr)
    {
        byte a = s.Registers[rd], b = s.Registers[rr]; int c = s.SREG.C ? 1 : 0; int r = a + b + c; byte rb = (byte)r;
        s.SREG.H = ((a & 0x0F) + (b & 0x0F) + c) > 0x0F; s.SREG.C = r > 0xFF; s.SREG.N = (rb & 0x80) != 0;
        s.SREG.V = ((a & 0x80) == (b & 0x80)) && ((a & 0x80) != (rb & 0x80)); s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = rb == 0; s.Registers[rd] = rb;
    }
    private static void Sub(AvrCpuState s, int rd, byte b, bool write)
    {
        byte a = s.Registers[rd]; int r = a - b; byte rb = (byte)r; SetSubFlags(s, a, b, rb, r < 0, preserveZ: false); if (write) s.Registers[rd] = rb;
    }
    private static void Sbc(AvrCpuState s, int rd, byte b, bool write)
    {
        byte a = s.Registers[rd]; int c = s.SREG.C ? 1 : 0; int r = a - b - c; byte rb = (byte)r; SetSubFlags(s, a, (byte)(b + c), rb, r < 0, preserveZ: true); if (write) s.Registers[rd] = rb;
    }
    private static void SetSubFlags(AvrCpuState s, byte a, byte b, byte rb, bool carry, bool preserveZ)
    {
        s.SREG.H = ((a & 0x0F) - (b & 0x0F)) < 0; s.SREG.C = carry; s.SREG.N = (rb & 0x80) != 0; s.SREG.V = ((a & 0x80) != (b & 0x80)) && ((a & 0x80) != (rb & 0x80)); s.SREG.S = s.SREG.N ^ s.SREG.V; if (preserveZ) s.SREG.Z &= rb == 0; else s.SREG.Z = rb == 0;
    }
    private static void Inc(AvrCpuState s, int rd) { byte r = (byte)(s.Registers[rd] + 1); s.SREG.N = (r & 0x80) != 0; s.SREG.V = r == 0x80; s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = r == 0; s.Registers[rd] = r; }
    private static void Dec(AvrCpuState s, int rd) { byte old = s.Registers[rd]; byte r = (byte)(old - 1); s.SREG.N = (r & 0x80) != 0; s.SREG.V = old == 0x80; s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = r == 0; s.Registers[rd] = r; }
    private static void Neg(AvrCpuState s, int rd) { byte old = s.Registers[rd]; byte r = (byte)(0 - old); s.SREG.H = (old & 0x0F) != 0; s.SREG.C = old != 0; s.SREG.N = (r & 0x80) != 0; s.SREG.V = old == 0x80; s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = r == 0; s.Registers[rd] = r; }
    private static void Adiw(AvrCpuState s, int pair, int k) { int v = GetWord(s, pair); int r = (v + k) & 0xFFFF; SetWord(s, pair, r); byte hi = (byte)(r >> 8); s.SREG.C = v + k > 0xFFFF; s.SREG.N = (hi & 0x80) != 0; s.SREG.V = (v & 0x8000) == 0 && (r & 0x8000) != 0; s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = r == 0; }
    private static void Sbiw(AvrCpuState s, int pair, int k) { int v = GetWord(s, pair); int full = v - k; int r = full & 0xFFFF; SetWord(s, pair, r); byte hi = (byte)(r >> 8); s.SREG.C = full < 0; s.SREG.N = (hi & 0x80) != 0; s.SREG.V = (v & 0x8000) != 0 && (r & 0x8000) == 0; s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = r == 0; }
    private static void Logic(AvrCpuState s, int rd, byte r, bool write) { s.SREG.N = (r & 0x80) != 0; s.SREG.V = false; s.SREG.S = s.SREG.N; s.SREG.Z = r == 0; if (write) s.Registers[rd] = r; }
    private static void Lsr(AvrCpuState s, int rd) { byte v = s.Registers[rd]; byte r = (byte)(v >> 1); s.SREG.C = (v & 1) != 0; s.SREG.N = false; s.SREG.V = s.SREG.N ^ s.SREG.C; s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = r == 0; s.Registers[rd] = r; }
    private static void Ror(AvrCpuState s, int rd) { byte v = s.Registers[rd]; byte r = (byte)((v >> 1) | (s.SREG.C ? 0x80 : 0)); s.SREG.C = (v & 1) != 0; s.SREG.N = (r & 0x80) != 0; s.SREG.V = s.SREG.N ^ s.SREG.C; s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = r == 0; s.Registers[rd] = r; }
    private static void Asr(AvrCpuState s, int rd) { byte v = s.Registers[rd]; byte r = (byte)((v >> 1) | (v & 0x80)); s.SREG.C = (v & 1) != 0; s.SREG.N = (r & 0x80) != 0; s.SREG.V = s.SREG.N ^ s.SREG.C; s.SREG.S = s.SREG.N ^ s.SREG.V; s.SREG.Z = r == 0; s.Registers[rd] = r; }
    private static void Lpm(AvrCpuState s, int rd, bool incrementZ) { int z = GetWord(s, 30); ushort word = s.ProgramMemory![z / 2]; s.Registers[rd] = (byte)((z & 1) == 0 ? word & 0xFF : word >> 8); if (incrementZ) SetWord(s, 30, (z + 1) & 0xFFFF); }
    private static int GetWord(AvrCpuState s, int pair) => s.Registers[pair] | (s.Registers[pair + 1] << 8);
    private static void SetWord(AvrCpuState s, int pair, int value) { s.Registers[pair] = (byte)value; s.Registers[pair + 1] = (byte)(value >> 8); }
    private static void IncrementPair(AvrCpuState s, int pair) => SetWord(s, pair, (GetWord(s, pair) + 1) & 0xFFFF);
    private static void DecrementPair(AvrCpuState s, int pair) => SetWord(s, pair, (GetWord(s, pair) - 1) & 0xFFFF);
    private static int GetSp(AvrCpuState s) => s.DataMemory[0x5D] | (s.DataMemory[0x5E] << 8);
    private static void SetSp(AvrCpuState s, int sp) { s.DataMemory[0x5D] = (byte)sp; s.DataMemory[0x5E] = (byte)(sp >> 8); }
    private static void PushByte(AvrCpuState s, byte v) { int sp = (GetSp(s) - 1) & 0xFFFF; SetSp(s, sp); s.DataMemory[sp] = v; }
    private static byte PopByte(AvrCpuState s) { int sp = GetSp(s); byte v = s.DataMemory[sp]; SetSp(s, (sp + 1) & 0xFFFF); return v; }
    private static void PushReturnAddress(AvrCpuState s, int value) { PushByte(s, (byte)(value & 0xFF)); PushByte(s, (byte)((value >> 8) & 0xFF)); }
    private static int PopReturnAddress(AvrCpuState s) { byte high = PopByte(s); byte low = PopByte(s); return low | (high << 8); }
    private static bool GetSregBit(AvrCpuState s, int bit) => (s.SREG.Value & (1 << bit)) != 0;
    private static void Mul(AvrCpuState s, int a, int b) => Mul(s, a * b);
    private static void Mul(AvrCpuState s, int result) { ushort r = (ushort)result; s.Registers[0] = (byte)r; s.Registers[1] = (byte)(r >> 8); s.SREG.Z = r == 0; s.SREG.C = (r & 0x8000) != 0; }
}
