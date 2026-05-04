using System.Collections.Immutable;
using EmulatorAVR.Core.Cpu;

namespace EmulatorAVR.Core.Tracing;

public sealed class EmulatorStateSnapshot
{
    public uint ProgramCounter { get; }
    public ulong CycleCount { get; }
    public byte SregRawValue { get; }
    public bool I { get; }
    public bool T { get; }
    public bool H { get; }
    public bool S { get; }
    public bool V { get; }
    public bool N { get; }
    public bool Z { get; }
    public bool C { get; }
    public ImmutableArray<byte> Registers { get; }

    public EmulatorStateSnapshot(AvrCpuState state)
    {
        ProgramCounter = state.ProgramCounter;
        CycleCount = state.CycleCount;
        SregRawValue = state.SREG.Value;
        I = state.SREG.I;
        T = state.SREG.T;
        H = state.SREG.H;
        S = state.SREG.S;
        V = state.SREG.V;
        N = state.SREG.N;
        Z = state.SREG.Z;
        C = state.SREG.C;

        var regs = new byte[32];
        for (int i = 0; i < 32; i++)
            regs[i] = state.Registers[i];
        Registers = ImmutableArray.Create(regs);
    }
}
