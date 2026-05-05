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
}
