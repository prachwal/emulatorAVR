using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Cpu;
using EmulatorAVR.Core.Tracing;

namespace EmulatorAVR.Core.Tests.Tracing;

[TestClass]
public class EmulatorStateSnapshotTests
{
    [TestMethod]
    public void PcIsCopiedFromAvrCpuState()
    {
        var state = new AvrCpuState();
        state.ProgramCounter = 0x1234;
        var snapshot = new EmulatorStateSnapshot(state);
        snapshot.ProgramCounter.Should().Be(0x1234u);
    }

    [TestMethod]
    public void CycleCountIsCopiedFromAvrCpuState()
    {
        var state = new AvrCpuState();
        state.AddCycles(42);
        var snapshot = new EmulatorStateSnapshot(state);
        snapshot.CycleCount.Should().Be(42u);
    }

    [TestMethod]
    public void SregRawValueIsCopied()
    {
        var state = new AvrCpuState();
        state.SREG.Value = 0xA5;
        var snapshot = new EmulatorStateSnapshot(state);
        snapshot.SregRawValue.Should().Be(0xA5);
    }

    [TestMethod]
    public void SregFlagsAreCopiedCorrectly()
    {
        var state = new AvrCpuState();
        state.SREG.Value = 0xFF;
        var snapshot = new EmulatorStateSnapshot(state);
        snapshot.I.Should().BeTrue();
        snapshot.T.Should().BeTrue();
        snapshot.H.Should().BeTrue();
        snapshot.S.Should().BeTrue();
        snapshot.V.Should().BeTrue();
        snapshot.N.Should().BeTrue();
        snapshot.Z.Should().BeTrue();
        snapshot.C.Should().BeTrue();
    }

    [TestMethod]
    public void Exactly32RegistersAreCopied()
    {
        var state = new AvrCpuState();
        var snapshot = new EmulatorStateSnapshot(state);
        snapshot.Registers.Should().HaveCount(32);
    }

    [TestMethod]
    public void RegisterSnapshotIsDefensiveCopy()
    {
        var state = new AvrCpuState();
        state.Registers[0] = 0xAB;
        var snapshot = new EmulatorStateSnapshot(state);
        snapshot.Registers[0].Should().Be(0xAB);
    }

    [TestMethod]
    public void ChangingCpuRegistersAfterSnapshotDoesNotMutateSnapshot()
    {
        var state = new AvrCpuState();
        state.Registers[5] = 0x10;
        var snapshot = new EmulatorStateSnapshot(state);
        state.Registers[5] = 0xFF;
        snapshot.Registers[5].Should().Be(0x10);
    }

    [TestMethod]
    public void ChangingSregAfterSnapshotDoesNotMutateSnapshot()
    {
        var state = new AvrCpuState();
        state.SREG.Value = 0x00;
        var snapshot = new EmulatorStateSnapshot(state);
        state.SREG.Value = 0xFF;
        snapshot.SregRawValue.Should().Be(0x00);
    }

    [TestMethod]
    public void CallerCannotMutateSnapshotRegisterData()
    {
        var state = new AvrCpuState();
        state.Registers[0] = 0xAB;
        var snapshot = new EmulatorStateSnapshot(state);
        var regs = snapshot.Registers;

        regs.Should().BeOfType<System.Collections.Immutable.ImmutableArray<byte>>();
        regs[0].Should().Be(0xAB);

        Action act = () => ((byte[])(object)regs)[0] = 0xFF;
        act.Should().Throw<InvalidCastException>();
    }
}
