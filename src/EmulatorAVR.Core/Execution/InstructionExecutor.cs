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

            case InstructionKind.Rcall:
                ExecuteRcall(state, instruction);
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

            // Group F — skip instructions
            case InstructionKind.Cpse:
                ExecuteCpse(state, instruction);
                break;

            case InstructionKind.Sbrc:
                ExecuteSbrc(state, instruction);
                break;

            case InstructionKind.Sbrs:
                ExecuteSbrs(state, instruction);
                break;

            // Group G — I/O and bit operations
            case InstructionKind.Bst:
            case InstructionKind.Bld:
                // BST/BLD deferred — encoding collision with SBRC/SBRS
                break;

            case InstructionKind.Sec:
            case InstructionKind.Sez:
            case InstructionKind.Sen:
            case InstructionKind.Sev:
            case InstructionKind.Ses:
            case InstructionKind.Seh:
            case InstructionKind.Set:
            case InstructionKind.Sei:
            case InstructionKind.Bset:
                ExecuteBset(state, instruction);
                break;

            case InstructionKind.Clc:
            case InstructionKind.Clz:
            case InstructionKind.Cln:
            case InstructionKind.Clv:
            case InstructionKind.Cls:
            case InstructionKind.Clh:
            case InstructionKind.Clt:
            case InstructionKind.Cli:
            case InstructionKind.Bclr:
                ExecuteBclr(state, instruction);
                break;

            case InstructionKind.In:
                ExecuteIn(state, instruction);
                break;

            case InstructionKind.Out:
                ExecuteOut(state, instruction);
                break;

            case InstructionKind.Sbi:
                ExecuteSbi(state, instruction);
                break;

            case InstructionKind.Cbi:
                ExecuteCbi(state, instruction);
                break;

            // Group I — stack operations
            case InstructionKind.Push:
                ExecutePush(state, instruction);
                break;

            case InstructionKind.Pop:
                ExecutePop(state, instruction);
                break;

            case InstructionKind.Ret:
            case InstructionKind.Reti:
                ExecuteRet(state, instruction);
                break;

            case InstructionKind.Lpm:
                ExecuteLpm(state, instruction);
                break;

            case InstructionKind.Lds:
                ExecuteLds(state, instruction);
                break;

            case InstructionKind.Sts:
                ExecuteSts(state, instruction);
                break;

            case InstructionKind.Jmp:
                ExecuteJmp(state, instruction);
                break;

            case InstructionKind.Call:
                ExecuteCall(state, instruction);
                break;

            case InstructionKind.LpmZPlus:
                ExecuteLpm(state, instruction);
                IncrementZ(state);
                break;

            case InstructionKind.LdX:
                ExecuteLdReg(state, instruction, 26);
                break;

            case InstructionKind.LdXPlus:
                ExecuteLdReg(state, instruction, 26);
                IncrementPair(state, 26);
                break;

            case InstructionKind.LdMinusX:
                DecrementPair(state, 26);
                ExecuteLdReg(state, instruction, 26);
                break;

            case InstructionKind.StX:
                ExecuteStReg(state, instruction, 26);
                break;

            case InstructionKind.StXPlus:
                ExecuteStReg(state, instruction, 26);
                IncrementPair(state, 26);
                break;

            case InstructionKind.StMinusX:
                DecrementPair(state, 26);
                ExecuteStReg(state, instruction, 26);
                break;

            case InstructionKind.LdY:
                ExecuteLdReg(state, instruction, 28);
                break;

            case InstructionKind.LdYPlus:
                ExecuteLdReg(state, instruction, 28);
                IncrementPair(state, 28);
                break;

            case InstructionKind.LdMinusY:
                DecrementPair(state, 28);
                ExecuteLdReg(state, instruction, 28);
                break;

            case InstructionKind.StY:
                ExecuteStReg(state, instruction, 28);
                break;

            case InstructionKind.StYPlus:
                ExecuteStReg(state, instruction, 28);
                IncrementPair(state, 28);
                break;

            case InstructionKind.StMinusY:
                DecrementPair(state, 28);
                ExecuteStReg(state, instruction, 28);
                break;

            case InstructionKind.LdZ:
                ExecuteLdReg(state, instruction, 30);
                break;

            case InstructionKind.LdZPlus:
                ExecuteLdReg(state, instruction, 30);
                IncrementPair(state, 30);
                break;

            case InstructionKind.LdMinusZ:
                DecrementPair(state, 30);
                ExecuteLdReg(state, instruction, 30);
                break;

            case InstructionKind.StZ:
                ExecuteStReg(state, instruction, 30);
                break;

            case InstructionKind.StZPlus:
                ExecuteStReg(state, instruction, 30);
                IncrementPair(state, 30);
                break;

            case InstructionKind.StMinusZ:
                DecrementPair(state, 30);
                ExecuteStReg(state, instruction, 30);
                break;

            case InstructionKind.Ijmp:
                ExecuteIjmp(state, instruction);
                break;

            case InstructionKind.Icall:
                ExecuteIcall(state, instruction);
                break;

            // Group K — multiply
            case InstructionKind.Mul:
                ExecuteMul(state, instruction);
                break;

            case InstructionKind.Muls:
                ExecuteMuls(state, instruction);
                break;

            case InstructionKind.Mulsu:
                ExecuteMulsu(state, instruction);
                break;

            case InstructionKind.Fmul:
                ExecuteFmul(state, instruction);
                break;

            case InstructionKind.Fmuls:
                ExecuteFmuls(state, instruction);
                break;

            case InstructionKind.Fmulsu:
                ExecuteFmulsu(state, instruction);
                break;

            // Group L — MCU control
            case InstructionKind.Sleep:
            case InstructionKind.Wdr:
            case InstructionKind.Break:
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

    private void ExecuteSbrc(AvrCpuState state, Instruction instruction)
    {
        byte reg = state.Registers[instruction.Rd];
        int bit = instruction.Immediate;
        if ((reg & (1 << bit)) == 0)
            state.ProgramCounter++;
    }

    private void ExecuteSbrs(AvrCpuState state, Instruction instruction)
    {
        byte reg = state.Registers[instruction.Rd];
        int bit = instruction.Immediate;
        if ((reg & (1 << bit)) != 0)
            state.ProgramCounter++;
    }

    private void ExecuteBst(AvrCpuState state, Instruction instruction)
    {
        byte reg = state.Registers[instruction.Rd];
        int bit = instruction.Immediate;
        state.SREG.T = ((reg >> bit) & 0x01) != 0;
    }

    private void ExecuteBld(AvrCpuState state, Instruction instruction)
    {
        int bit = instruction.Immediate;
        if (state.SREG.T)
            state.Registers[instruction.Rd] |= (byte)(1 << bit);
        else
            state.Registers[instruction.Rd] &= (byte)~(1 << bit);
    }

    private void ExecuteBset(AvrCpuState state, Instruction instruction)
    {
        int bit = instruction.Rd;
        state.SREG.Value |= (byte)(1 << bit);
    }

    private void ExecuteBclr(AvrCpuState state, Instruction instruction)
    {
        int bit = instruction.Rd;
        state.SREG.Value &= (byte)~(1 << bit);
    }

    private void ExecuteIn(AvrCpuState state, Instruction instruction)
    {
        int dataAddr = instruction.Immediate;
        state.Registers[instruction.Rd] = state.DataMemory[dataAddr];
    }

    private void ExecuteOut(AvrCpuState state, Instruction instruction)
    {
        int dataAddr = instruction.Immediate;
        state.DataMemory[dataAddr] = state.Registers[instruction.Rd];
    }

    private void ExecuteSbi(AvrCpuState state, Instruction instruction)
    {
        int ioAddr = instruction.Rd;
        int dataAddr = ioAddr + 0x20;
        int bit = instruction.Immediate;
        state.DataMemory[dataAddr] |= (byte)(1 << bit);
    }

    private void ExecuteCbi(AvrCpuState state, Instruction instruction)
    {
        int ioAddr = instruction.Rd;
        int dataAddr = ioAddr + 0x20;
        int bit = instruction.Immediate;
        state.DataMemory[dataAddr] &= (byte)~(1 << bit);
    }

    private void ExecuteCpse(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        if (rd == rr)
            state.ProgramCounter++;
    }

    private void ExecutePush(AvrCpuState state, Instruction instruction)
    {
        byte reg = state.Registers[instruction.Rd];
        int spl = state.DataMemory[0x5D];
        int sph = state.DataMemory[0x5E];
        int sp = (sph << 8) | spl;
        sp = (sp - 1) & 0xFFFF;
        if (sp >= 0x0100)
            state.DataMemory[sp] = reg;
        state.DataMemory[0x5D] = (byte)(sp & 0xFF);
        state.DataMemory[0x5E] = (byte)((sp >> 8) & 0xFF);
    }

    private static int ReadSp(AvrCpuState state)
    {
        return (state.DataMemory[0x5E] << 8) | state.DataMemory[0x5D];
    }

    private static void WriteSp(AvrCpuState state, int sp)
    {
        state.DataMemory[0x5D] = (byte)(sp & 0xFF);
        state.DataMemory[0x5E] = (byte)((sp >> 8) & 0xFF);
    }

    private void ExecuteRet(AvrCpuState state, Instruction instruction)
    {
        int sp = ReadSp(state);
        byte raLow = sp >= 0x0100 ? state.DataMemory[sp] : (byte)0;
        sp = (sp + 1) & 0xFFFF;
        byte raHigh = sp >= 0x0100 ? state.DataMemory[sp] : (byte)0;
        sp = (sp + 1) & 0xFFFF;
        WriteSp(state, sp);
        uint ra = (uint)((raHigh << 8) | raLow);
        state.ProgramCounter = ra - 1;
        if (instruction.Kind == InstructionKind.Reti)
            state.SREG.I = true;
    }

    private void ExecuteIjmp(AvrCpuState state, Instruction instruction)
    {
        uint z = (uint)((state.Registers[31] << 8) | state.Registers[30]);
        state.ProgramCounter = z - 1;
    }

    private void ExecuteRcall(AvrCpuState state, Instruction instruction)
    {
        uint ra = state.ProgramCounter + 1;
        int sp = ReadSp(state);
        sp = (sp - 1) & 0xFFFF;
        if (sp >= 0x0100) state.DataMemory[sp] = (byte)((ra >> 8) & 0xFF);
        sp = (sp - 1) & 0xFFFF;
        if (sp >= 0x0100) state.DataMemory[sp] = (byte)(ra & 0xFF);
        WriteSp(state, sp);
        state.ProgramCounter = (uint)((int)state.ProgramCounter + instruction.Offset);
    }

    private void ExecuteIcall(AvrCpuState state, Instruction instruction)
    {
        uint ra = state.ProgramCounter + 1;
        int sp = ReadSp(state);
        sp = (sp - 1) & 0xFFFF;
        if (sp >= 0x0100) state.DataMemory[sp] = (byte)((ra >> 8) & 0xFF);
        sp = (sp - 1) & 0xFFFF;
        if (sp >= 0x0100) state.DataMemory[sp] = (byte)(ra & 0xFF);
        WriteSp(state, sp);
        uint z = (uint)((state.Registers[31] << 8) | state.Registers[30]);
        state.ProgramCounter = z - 1;
    }

    private void ExecutePop(AvrCpuState state, Instruction instruction)
    {
        int spl = state.DataMemory[0x5D];
        int sph = state.DataMemory[0x5E];
        int sp = (sph << 8) | spl;
        byte value = sp >= 0x0100 ? state.DataMemory[sp] : (byte)0;
        state.Registers[instruction.Rd] = value;
        sp = (sp + 1) & 0xFFFF;
        state.DataMemory[0x5D] = (byte)(sp & 0xFF);
        state.DataMemory[0x5E] = (byte)((sp >> 8) & 0xFF);
    }

    private void ExecuteLpm(AvrCpuState state, Instruction instruction)
    {
        int z = (state.Registers[31] << 8) | state.Registers[30];
        int wordAddr = z >> 1;
        byte value = 0;
        if (state.ProgramMemory != null && wordAddr >= 0 && wordAddr < state.ProgramMemory.WordCapacity)
        {
            ushort word = state.ProgramMemory[wordAddr];
            value = (z & 1) == 0 ? (byte)(word & 0xFF) : (byte)((word >> 8) & 0xFF);
        }
        if (instruction.Rd >= 0)
            state.Registers[instruction.Rd] = value;
        else
            state.Registers[0] = value;
    }

    private void ExecuteJmp(AvrCpuState state, Instruction instruction)
    {
        state.ProgramCounter = (uint)(instruction.Offset - 1);
    }

    private void ExecuteLds(AvrCpuState state, Instruction instruction)
    {
        int addr = instruction.Offset;
        state.Registers[instruction.Rd] = state.DataMemory[addr];
    }

    private void ExecuteSts(AvrCpuState state, Instruction instruction)
    {
        int addr = instruction.Offset;
        state.DataMemory[addr] = state.Registers[instruction.Rd];
    }

    private void ExecuteCall(AvrCpuState state, Instruction instruction)
    {
        uint ra = state.ProgramCounter + 2;
        int sp = ReadSp(state);
        sp = (sp - 1) & 0xFFFF;
        if (sp >= 0x0100) state.DataMemory[sp] = (byte)((ra >> 8) & 0xFF);
        sp = (sp - 1) & 0xFFFF;
        if (sp >= 0x0100) state.DataMemory[sp] = (byte)(ra & 0xFF);
        WriteSp(state, sp);
        state.ProgramCounter = (uint)(instruction.Offset - 1);
    }

    private void ExecuteMul(AvrCpuState state, Instruction instruction)
    {
        byte rd = state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        int result = rd * rr;
        state.Registers[0] = (byte)(result & 0xFF);
        state.Registers[1] = (byte)((result >> 8) & 0xFF);
        state.SREG.C = (result & 0x8000) != 0;
        state.SREG.Z = (result & 0xFFFF) == 0;
    }

    private void ExecuteMuls(AvrCpuState state, Instruction instruction)
    {
        sbyte rd = (sbyte)state.Registers[instruction.Rd];
        sbyte rr = (sbyte)state.Registers[instruction.Rr];
        int result = rd * rr;
        state.Registers[0] = (byte)(result & 0xFF);
        state.Registers[1] = (byte)((result >> 8) & 0xFF);
        state.SREG.C = (result & 0x8000) != 0;
        state.SREG.Z = (result & 0xFFFF) == 0;
    }

    private void ExecuteMulsu(AvrCpuState state, Instruction instruction)
    {
        sbyte rd = (sbyte)state.Registers[instruction.Rd];
        byte rr = state.Registers[instruction.Rr];
        int result = rd * rr;
        state.Registers[0] = (byte)(result & 0xFF);
        state.Registers[1] = (byte)((result >> 8) & 0xFF);
        state.SREG.C = (result & 0x8000) != 0;
        state.SREG.Z = (result & 0xFFFF) == 0;
    }

    private void ExecuteFmul(AvrCpuState state, Instruction instruction)
    {
        int rd = state.Registers[instruction.Rd];
        int rr = state.Registers[instruction.Rr];
        int result = (rd * rr) << 1;
        state.Registers[0] = (byte)(result & 0xFF);
        state.Registers[1] = (byte)((result >> 8) & 0xFF);
        state.SREG.C = (result & 0x10000) != 0;
        state.SREG.Z = (result & 0xFFFF) == 0;
    }

    private void ExecuteFmuls(AvrCpuState state, Instruction instruction)
    {
        int rd = (sbyte)state.Registers[instruction.Rd];
        int rr = (sbyte)state.Registers[instruction.Rr];
        int result = (rd * rr) << 1;
        state.Registers[0] = (byte)(result & 0xFF);
        state.Registers[1] = (byte)((result >> 8) & 0xFF);
        state.SREG.C = (result & 0x10000) != 0;
        state.SREG.Z = (result & 0xFFFF) == 0;
    }

    private void ExecuteFmulsu(AvrCpuState state, Instruction instruction)
    {
        int rd = (sbyte)state.Registers[instruction.Rd];
        int rr = state.Registers[instruction.Rr];
        int result = (rd * rr) << 1;
        state.Registers[0] = (byte)(result & 0xFF);
        state.Registers[1] = (byte)((result >> 8) & 0xFF);
        state.SREG.C = (result & 0x10000) != 0;
        state.SREG.Z = (result & 0xFFFF) == 0;
    }

    private static int ReadPair(AvrCpuState state, int lowReg)
    {
        return (state.Registers[lowReg + 1] << 8) | state.Registers[lowReg];
    }

    private static void WritePair(AvrCpuState state, int lowReg, int value)
    {
        state.Registers[lowReg] = (byte)(value & 0xFF);
        state.Registers[lowReg + 1] = (byte)((value >> 8) & 0xFF);
    }

    private void IncrementZ(AvrCpuState state)
    {
        int z = (state.Registers[31] << 8) | state.Registers[30];
        z++;
        state.Registers[30] = (byte)(z & 0xFF);
        state.Registers[31] = (byte)((z >> 8) & 0xFF);
    }

    private void IncrementPair(AvrCpuState state, int lowReg)
    {
        int val = (state.Registers[lowReg + 1] << 8) | state.Registers[lowReg];
        val++;
        state.Registers[lowReg] = (byte)(val & 0xFF);
        state.Registers[lowReg + 1] = (byte)((val >> 8) & 0xFF);
    }

    private void DecrementPair(AvrCpuState state, int lowReg)
    {
        int val = (state.Registers[lowReg + 1] << 8) | state.Registers[lowReg];
        val--;
        state.Registers[lowReg] = (byte)(val & 0xFF);
        state.Registers[lowReg + 1] = (byte)((val >> 8) & 0xFF);
    }

    private void ExecuteLdReg(AvrCpuState state, Instruction instruction, int lowReg)
    {
        int addr = (state.Registers[lowReg + 1] << 8) | state.Registers[lowReg];
        state.Registers[instruction.Rd] = addr >= 0x0100 ? state.DataMemory[addr] : (byte)0;
    }

    private void ExecuteStReg(AvrCpuState state, Instruction instruction, int lowReg)
    {
        int addr = (state.Registers[lowReg + 1] << 8) | state.Registers[lowReg];
        if (addr >= 0x0100)
            state.DataMemory[addr] = state.Registers[instruction.Rd];
    }
}
