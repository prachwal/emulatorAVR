using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Cpu;

namespace EmulatorAVR.Core.Tests.Cpu;

[TestClass]
public class AvrCpuStateTests
{
    [TestMethod]
    public void DefaultProgramCounterIsZero()
    {
        var state = new AvrCpuState();
        state.ProgramCounter.Should().Be(0u);
    }

    [TestMethod]
    public void DefaultCycleCountIsZero()
    {
        var state = new AvrCpuState();
        state.CycleCount.Should().Be(0u);
    }

    [TestMethod]
    public void DefaultRegistersAreInitialized()
    {
        var state = new AvrCpuState();
        state.Registers.Should().NotBeNull();
        state.Registers[0].Should().Be(0);
        state.Registers[31].Should().Be(0);
    }

    [TestMethod]
    public void DefaultSREGIsInitialized()
    {
        var state = new AvrCpuState();
        state.SREG.Should().NotBeNull();
        state.SREG.Value.Should().Be(0);
    }

    [TestMethod]
    public void AddCycles1_IncrementsCycleCount()
    {
        var state = new AvrCpuState();
        state.AddCycles(1);
        state.CycleCount.Should().Be(1u);
    }

    [TestMethod]
    public void AddCycles0_DoesNotChangeCycleCount()
    {
        var state = new AvrCpuState();
        state.AddCycles(0);
        state.CycleCount.Should().Be(0u);
    }
}
