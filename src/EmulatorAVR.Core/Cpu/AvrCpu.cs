namespace EmulatorAVR.Core.Cpu;

public class AvrCpu
{
    public AvrCpuState State { get; private set; }

    public AvrCpu()
    {
        State = new AvrCpuState();
    }

    public void Reset()
    {
        State = new AvrCpuState();
    }
}
