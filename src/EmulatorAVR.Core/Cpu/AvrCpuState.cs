namespace EmulatorAVR.Core.Cpu;

public class AvrCpuState
{
    public AvrRegisters Registers { get; }
    public StatusRegister SREG { get; }
    public uint ProgramCounter { get; set; }
    public ulong CycleCount { get; private set; }

    public AvrCpuState()
    {
        Registers = new AvrRegisters();
        SREG = new StatusRegister();
        ProgramCounter = 0;
        CycleCount = 0;
    }

    public void AddCycles(ulong cycles)
    {
        CycleCount += cycles;
    }
}
