using EmulatorAVR.Core.Memory;

namespace EmulatorAVR.Core.Cpu;

public class AvrCpuState
{
    public AvrRegisters Registers { get; }
    public StatusRegister SREG { get; }
    public uint ProgramCounter { get; set; }
    public ulong CycleCount { get; private set; }
    public DataMemory DataMemory { get; }
    public ProgramMemory? ProgramMemory { get; set; }

    public AvrCpuState()
    {
        Registers = new AvrRegisters();
        SREG = new StatusRegister();
        ProgramCounter = 0;
        CycleCount = 0;
        DataMemory = new DataMemory();
    }

    public void AddCycles(ulong cycles)
    {
        CycleCount += cycles;
    }
}
