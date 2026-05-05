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
                break;

            case InstructionKind.Ldi:
                state.Registers[instruction.Rd] = instruction.Immediate;
                break;

            case InstructionKind.Mov:
                state.Registers[instruction.Rd] = state.Registers[instruction.Rr];
                break;

            case InstructionKind.Add:
                ExecuteAdd(state, instruction);
                break;

            case InstructionKind.Adc:
                ExecuteAdc(state, instruction);
                break;

            case InstructionKind.Sub:
                ExecuteSub(state, instruction);
                break;

            case InstructionKind.Subi:
                ExecuteSubi(state, instruction);
                break;

            case InstructionKind.Sbc:
                ExecuteSbc(state, instruction);
                break;

            case InstructionKind.Sbci:
                ExecuteSbci(state, instruction);
                break;

            case InstructionKind.Cp:
                ExecuteCp(state, instruction);
                break;

            case InstructionKind.Cpc:
                ExecuteCpc(state, instruction);
                break;

            case InstructionKind.Cpi:
                ExecuteCpi(state, instruction);
                break;

            case InstructionKind.Inc:
                ExecuteInc(state, instruction);
                break;

            case InstructionKind.Dec:
                ExecuteDec(state, instruction);
                break;

            case InstructionKind.Neg:
                ExecuteNeg(state, instruction);
                break;

            case InstructionKind.Adiw:
                ExecuteAdiw(state, instruction);
                break;

            case InstructionKind.Sbiw:
                ExecuteSbiw(state, instruction);
                break;

            // Group C — logical and bitwise
            case InstructionKind.And:
                ExecuteAnd(state, instruction);
                break;

            case InstructionKind.Andi:
                ExecuteAndi(state, instruction);
                break;

            case InstructionKind.Or:
                ExecuteOr(state, instruction);
                break;

            case InstructionKind.Ori:
                ExecuteOri(state, instruction);
                break;

            case InstructionKind.Eor:
                ExecuteEor(state, instruction);
                break;

            case InstructionKind.Com:
                ExecuteCom(state, instruction);
                break;

            case InstructionKind.Tst:
                ExecuteTst(state, instruction);
                break;

            case InstructionKind.Clr:
                ExecuteClr(state, instruction);
                break;

            case InstructionKind.Ser:
                ExecuteSer(state, instruction);
                break;

            // Group D — shifts, rotates, swap
            case InstructionKind.Lsl:
                ExecuteAdd(state, instruction);
                break;

            case InstructionKind.Lsr:
                ExecuteLsr(state, instruction);
                break;

            case InstructionKind.Rol:
                ExecuteAdc(state, instruction);
                break;

            case InstructionKind.Ror:
                ExecuteRor(state, instruction);
                break;

            case InstructionKind.Asr:
                ExecuteAsr(state, instruction);
                break;

            case InstructionKind.Swap:
                ExecuteSwap(state, instruction);
                break;

            // Group E — branches and control flow
            case InstructionKind.Rjmp:
                ExecuteRjmp(state, instruction);
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
                ExecuteBranchIfSet(state, instruction);
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
                ExecuteBranchIfClear(state, instruction);
                break;

            default:
                return ExecutionResult.Unsupported;
        }

        state.ProgramCounter++;
        state.AddCycles(1);
        return ExecutionResult.Ok;
    }

    private void ExecuteAdd(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        int result = rd + rr;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = StatusRegisterMath.HalfCarryAdd(rd, rr);
        state.SREG.C = result > 0xFF;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = StatusRegisterMath.OverflowAdd(rd, rr, resultByte);
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(resultByte);

        state.Registers[instruction.Rd] = resultByte;
    }

    private void ExecuteAdc(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        int carryIn = state.SREG.C ? 1 : 0;
        int result = rd + rr + carryIn;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = ((rd & 0x0F) + (rr & 0x0F) + carryIn) >= 0x10;
        state.SREG.C = result > 0xFF;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = ((rd & 0x80) == (rr & 0x80)) && ((rd & 0x80) != (resultByte & 0x80));
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(resultByte);

        state.Registers[instruction.Rd] = resultByte;
    }

    private void ExecuteSub(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        int result = rd - rr;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = StatusRegisterMath.HalfCarrySub(rd, rr);
        state.SREG.C = result < 0;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = StatusRegisterMath.OverflowSub(rd, rr, resultByte);
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(resultByte);

        state.Registers[instruction.Rd] = resultByte;
    }

    private void ExecuteSubi(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte k = instruction.Immediate;
        int result = rd - k;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = StatusRegisterMath.HalfCarrySub(rd, k);
        state.SREG.C = result < 0;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = StatusRegisterMath.OverflowSub(rd, k, resultByte);
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(resultByte);

        state.Registers[instruction.Rd] = resultByte;
    }

    private void ExecuteSbc(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        int carryIn = state.SREG.C ? 1 : 0;
        int result = rd - rr - carryIn;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = ((rd & 0x0F) - (rr & 0x0F) - carryIn) < 0;
        state.SREG.C = result < 0;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = ((rd & 0x80) != (rr & 0x80)) && ((rd & 0x80) != (resultByte & 0x80));
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z &= StatusRegisterMath.Zero(resultByte);

        state.Registers[instruction.Rd] = resultByte;
    }

    private void ExecuteSbci(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte k = instruction.Immediate;
        int carryIn = state.SREG.C ? 1 : 0;
        int result = rd - k - carryIn;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = ((rd & 0x0F) - (k & 0x0F) - carryIn) < 0;
        state.SREG.C = result < 0;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = ((rd & 0x80) != (k & 0x80)) && ((rd & 0x80) != (resultByte & 0x80));
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z &= StatusRegisterMath.Zero(resultByte);

        state.Registers[instruction.Rd] = resultByte;
    }

    private void ExecuteCp(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        int result = rd - rr;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = StatusRegisterMath.HalfCarrySub(rd, rr);
        state.SREG.C = result < 0;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = StatusRegisterMath.OverflowSub(rd, rr, resultByte);
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(resultByte);
    }

    private void ExecuteCpc(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        int carryIn = state.SREG.C ? 1 : 0;
        int result = rd - rr - carryIn;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = ((rd & 0x0F) - (rr & 0x0F) - carryIn) < 0;
        state.SREG.C = result < 0;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = ((rd & 0x80) != (rr & 0x80)) && ((rd & 0x80) != (resultByte & 0x80));
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z &= StatusRegisterMath.Zero(resultByte);
    }

    private void ExecuteCpi(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte k = instruction.Immediate;
        int result = rd - k;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = StatusRegisterMath.HalfCarrySub(rd, k);
        state.SREG.C = result < 0;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = StatusRegisterMath.OverflowSub(rd, k, resultByte);
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(resultByte);
    }

    private void ExecuteInc(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        int result = rd + 1;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = rd == 0x7F;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(resultByte);

        state.Registers[instruction.Rd] = resultByte;
    }

    private void ExecuteDec(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        int result = rd - 1;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = rd == 0x80;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(resultByte);

        state.Registers[instruction.Rd] = resultByte;
    }

    private void ExecuteNeg(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        int result = 0 - rd;
        byte resultByte = (byte)(result & 0xFF);

        state.SREG.H = ((0 & 0x0F) - (rd & 0x0F)) < 0;
        state.SREG.C = rd != 0;
        state.SREG.N = StatusRegisterMath.Negative(resultByte);
        state.SREG.V = rd == 0x80;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(resultByte);

        state.Registers[instruction.Rd] = resultByte;
    }

    private void ExecuteAdiw(AvrCpuState state, Instruction instruction)
    {
        int pairBase = instruction.WordRegisterPair;
        int value = StatusRegisterMath.WordRegisterValue(state.Registers, pairBase);
        int k = instruction.Immediate;
        int result = value + k;
        int resultLow = result & 0xFFFF;

        byte resultLowByte = (byte)(resultLow & 0xFF);
        byte resultHighByte = (byte)((resultLow >> 8) & 0xFF);

        state.SREG.C = result > 0xFFFF;
        state.SREG.N = StatusRegisterMath.Negative(resultHighByte);
        state.SREG.V = ((value & 0x8000) == 0) && ((resultLow & 0x8000) != 0);
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = (resultLow & 0xFFFF) == 0;

        StatusRegisterMath.SetWordRegisterValue(state.Registers, pairBase, resultLow);
    }

    private void ExecuteSbiw(AvrCpuState state, Instruction instruction)
    {
        int pairBase = instruction.WordRegisterPair;
        int value = StatusRegisterMath.WordRegisterValue(state.Registers, pairBase);
        int k = instruction.Immediate;
        int result = value - k;
        int resultLow = result & 0xFFFF;

        byte resultHighByte = (byte)((resultLow >> 8) & 0xFF);

        state.SREG.C = result < 0;
        state.SREG.N = StatusRegisterMath.Negative(resultHighByte);
        state.SREG.V = ((value & 0x8000) != 0) && ((resultLow & 0x8000) == 0);
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = (resultLow & 0xFFFF) == 0;

        StatusRegisterMath.SetWordRegisterValue(state.Registers, pairBase, resultLow);
    }

    private void ExecuteAnd(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        byte result = (byte)(rd & rr);

        state.SREG.N = StatusRegisterMath.Negative(result);
        state.SREG.V = false;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(result);

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteAndi(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte k = instruction.Immediate;
        byte result = (byte)(rd & k);

        state.SREG.N = StatusRegisterMath.Negative(result);
        state.SREG.V = false;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(result);

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteOr(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        byte result = (byte)(rd | rr);

        state.SREG.N = StatusRegisterMath.Negative(result);
        state.SREG.V = false;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(result);

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteOri(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte k = instruction.Immediate;
        byte result = (byte)(rd | k);

        state.SREG.N = StatusRegisterMath.Negative(result);
        state.SREG.V = false;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(result);

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteEor(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        byte result = (byte)(rd ^ rr);

        state.SREG.N = StatusRegisterMath.Negative(result);
        state.SREG.V = false;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(result);

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteCom(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte result = (byte)(~rd);

        state.SREG.C = true;
        state.SREG.N = StatusRegisterMath.Negative(result);
        state.SREG.V = false;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(result);

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteTst(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte result = rd;

        state.SREG.N = StatusRegisterMath.Negative(result);
        state.SREG.V = false;
        state.SREG.S = StatusRegisterMath.Sign(state.SREG.N, state.SREG.V);
        state.SREG.Z = StatusRegisterMath.Zero(result);
    }

    private void ExecuteClr(AvrCpuState state, Instruction instruction)
    {
        byte result = 0;

        state.SREG.N = false;
        state.SREG.V = false;
        state.SREG.S = false;
        state.SREG.Z = true;

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteSer(AvrCpuState state, Instruction instruction)
    {
        state.Registers[instruction.Rd] = 0xFF;
    }

    private void ExecuteLsr(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte result = (byte)(rd >> 1);

        state.SREG.C = (rd & 0x01) != 0;
        state.SREG.N = false;
        state.SREG.V = state.SREG.N ^ state.SREG.C;
        state.SREG.S = state.SREG.N ^ state.SREG.V;
        state.SREG.Z = StatusRegisterMath.Zero(result);

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteRor(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        int carryIn = state.SREG.C ? 0x80 : 0;
        byte result = (byte)((rd >> 1) | carryIn);

        state.SREG.C = (rd & 0x01) != 0;
        state.SREG.N = StatusRegisterMath.Negative(result);
        state.SREG.V = state.SREG.N ^ state.SREG.C;
        state.SREG.S = state.SREG.N ^ state.SREG.V;
        state.SREG.Z = StatusRegisterMath.Zero(result);

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteAsr(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte result = (byte)((rd >> 1) | (rd & 0x80));

        state.SREG.C = (rd & 0x01) != 0;
        state.SREG.N = StatusRegisterMath.Negative(result);
        state.SREG.V = state.SREG.N ^ state.SREG.C;
        state.SREG.S = state.SREG.N ^ state.SREG.V;
        state.SREG.Z = StatusRegisterMath.Zero(result);

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteSwap(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte result = (byte)((rd << 4) | (rd >> 4));

        state.Registers[instruction.Rd] = result;
    }

    private void ExecuteRjmp(AvrCpuState state, Instruction instruction)
    {
        uint currentPc = state.ProgramCounter;
        state.ProgramCounter = (uint)((int)currentPc + instruction.Offset);
    }

    private void ExecuteBranchIfSet(AvrCpuState state, Instruction instruction)
    {
        if (StatusRegisterMath.GetSregBit(state.SREG, instruction.Rd))
        {
            uint currentPc = state.ProgramCounter;
            state.ProgramCounter = (uint)((int)currentPc + instruction.Offset);
        }
    }

    private void ExecuteBranchIfClear(AvrCpuState state, Instruction instruction)
    {
        if (!StatusRegisterMath.GetSregBit(state.SREG, instruction.Rd))
        {
            uint currentPc = state.ProgramCounter;
            state.ProgramCounter = (uint)((int)currentPc + instruction.Offset);
        }
    }
}
