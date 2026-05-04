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
            {
                byte rd = state.Registers[instruction.Rd];
                byte rr = state.Registers[instruction.Rr];
                int result = rd + rr;

                byte resultByte = (byte)(result & 0xFF);

                state.SREG.H = ((rd & 0x0F) + (rr & 0x0F)) >= 0x10;
                state.SREG.C = result > 0xFF;
                state.SREG.N = (resultByte & 0x80) != 0;
                state.SREG.V = ((rd & 0x80) == (rr & 0x80)) && ((rd & 0x80) != (resultByte & 0x80));
                state.SREG.S = state.SREG.N ^ state.SREG.V;
                state.SREG.Z = resultByte == 0;

                state.Registers[instruction.Rd] = resultByte;
                break;
            }

            default:
                return ExecutionResult.Unsupported;
        }

        state.ProgramCounter++;
        state.AddCycles(1);
        return ExecutionResult.Ok;
    }
}
