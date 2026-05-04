using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Cpu;
using EmulatorAVR.Core.Instructions;
using EmulatorAVR.Core.Execution;

namespace EmulatorAVR.Core.Tests.Execution;

[TestClass]
public class InstructionExecutorTests
{
    private readonly InstructionDecoder _decoder = new();
    private readonly InstructionExecutor _executor = new();

    private AvrCpuState CreateState()
    {
        return new AvrCpuState();
    }

    [TestMethod]
    public void Nop_IncrementsPcAndCycles()
    {
        var state = CreateState();
        var instruction = _decoder.Decode(0x0000);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(1u);
        state.CycleCount.Should().Be(1u);
    }

    [TestMethod]
    public void Ldi_WritesImmediateToTargetRegister()
    {
        var state = CreateState();
        var instruction = _decoder.Decode(0xE402);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x42);
        state.ProgramCounter.Should().Be(1u);
        state.CycleCount.Should().Be(1u);
    }

    [TestMethod]
    public void Mov_CopiesSourceRegisterToDestination()
    {
        var state = CreateState();
        state.Registers[2] = 0xAB;
        var instruction = _decoder.Decode(0x2C12);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0xAB);
        state.ProgramCounter.Should().Be(1u);
        state.CycleCount.Should().Be(1u);
    }

    [TestMethod]
    public void Add_WritesResultAndIncrementsPcAndCycles()
    {
        var state = CreateState();
        state.Registers[1] = 0x05;
        state.Registers[2] = 0x03;
        var instruction = _decoder.Decode(0x0C12);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0x08);
        state.ProgramCounter.Should().Be(1u);
        state.CycleCount.Should().Be(1u);
    }

    [TestMethod]
    public void Add_SetsZeroFlagForZeroResult()
    {
        var state = CreateState();
        state.Registers[1] = 0x00;
        state.Registers[2] = 0x00;
        var instruction = _decoder.Decode(0x0C12);
        _executor.Execute(state, instruction);
        state.SREG.Z.Should().BeTrue();
    }

    [TestMethod]
    public void Add_SetsCarryFlag()
    {
        var state = CreateState();
        state.Registers[1] = 0xFF;
        state.Registers[2] = 0x01;
        var instruction = _decoder.Decode(0x0C12);
        _executor.Execute(state, instruction);
        state.SREG.C.Should().BeTrue();
        state.Registers[1].Should().Be(0x00);
    }

    [TestMethod]
    public void Add_SetsNegativeFlag()
    {
        var state = CreateState();
        state.Registers[1] = 0x01;
        state.Registers[2] = 0x7F;
        var instruction = _decoder.Decode(0x0C12);
        _executor.Execute(state, instruction);
        state.SREG.N.Should().BeTrue();
        state.Registers[1].Should().Be(0x80);
    }

    [TestMethod]
    public void Add_SetsOverflowFlag()
    {
        var state = CreateState();
        state.Registers[1] = 0x7F;
        state.Registers[2] = 0x01;
        var instruction = _decoder.Decode(0x0C12);
        _executor.Execute(state, instruction);
        state.SREG.V.Should().BeTrue();
        state.Registers[1].Should().Be(0x80);
    }

    [TestMethod]
    public void UnsupportedInstruction_DoesNotMutateState()
    {
        var state = CreateState();
        state.Registers[0] = 0xAA;
        state.ProgramCounter = 100;
        uint pcBefore = state.ProgramCounter;
        ulong cyclesBefore = state.CycleCount;
        byte sregBefore = state.SREG.Value;

        var instruction = _decoder.Decode(0xFFFF);
        var result = _executor.Execute(state, instruction);

        result.Should().Be(ExecutionResult.Unsupported);
        state.Registers[0].Should().Be(0xAA);
        state.ProgramCounter.Should().Be(pcBefore);
        state.CycleCount.Should().Be(cyclesBefore);
        state.SREG.Value.Should().Be(sregBefore);
    }
}
