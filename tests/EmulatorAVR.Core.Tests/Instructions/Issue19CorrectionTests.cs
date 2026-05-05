using EmulatorAVR.Core.Cpu;
using EmulatorAVR.Core.Execution;
using EmulatorAVR.Core.Instructions;
using EmulatorAVR.Core.Memory;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmulatorAVR.Core.Tests.Instructions;

[TestClass]
public class Issue19CorrectionTests
{
    private readonly InstructionDecoder _decoder = new();
    private readonly InstructionExecutor _executor = new();

    [TestMethod]
    public void Bld_Bst_Sbrc_Sbrs_HaveDistinctOpcodes()
    {
        _decoder.Decode(0xF903).Kind.Should().Be(InstructionKind.Bld);
        _decoder.Decode(0xFB03).Kind.Should().Be(InstructionKind.Bst);
        _decoder.Decode(0xFD03).Kind.Should().Be(InstructionKind.Sbrc);
        _decoder.Decode(0xFF03).Kind.Should().Be(InstructionKind.Sbrs);
    }

    [TestMethod]
    public void Bld_LoadsTFlagIntoRegisterBit()
    {
        var state = new AvrCpuState();
        state.SREG.T = true;
        var instruction = _decoder.Decode(0xF903);

        _executor.Execute(state, instruction);

        (state.Registers[16] & 0x08).Should().Be(0x08);
    }

    [TestMethod]
    public void Bst_StoresRegisterBitIntoTFlag()
    {
        var state = new AvrCpuState();
        state.Registers[16] = 0x08;
        var instruction = _decoder.Decode(0xFB03);

        _executor.Execute(state, instruction);

        state.SREG.T.Should().BeTrue();
    }

    [TestMethod]
    public void Sbrc_SkipsTwoWordInstructionWhenBitIsClear()
    {
        var instruction = _decoder.Decode(0xFD03, 0x940C); // next instruction is JMP, two words
        var state = new AvrCpuState { ProgramCounter = 10 };
        state.Registers[16] = 0x00;

        _executor.Execute(state, instruction);

        state.ProgramCounter.Should().Be(13u);
    }

    [TestMethod]
    public void Sbrs_SkipsTwoWordInstructionWhenBitIsSet()
    {
        var instruction = _decoder.Decode(0xFF03, 0x9000); // next instruction is LDS, two words
        var state = new AvrCpuState { ProgramCounter = 10 };
        state.Registers[16] = 0x08;

        _executor.Execute(state, instruction);

        state.ProgramCounter.Should().Be(13u);
    }

    [TestMethod]
    public void Cpse_SkipsTwoWordInstructionWhenRegistersEqual()
    {
        var instruction = _decoder.Decode(0x1012, 0x9200); // next instruction is STS, two words
        var state = new AvrCpuState { ProgramCounter = 10 };
        state.Registers[1] = 0x44;
        state.Registers[2] = 0x44;

        _executor.Execute(state, instruction);

        state.ProgramCounter.Should().Be(13u);
    }

    [TestMethod]
    public void Sbic_SkipsTwoWordInstructionWhenIoBitIsClear()
    {
        var instruction = _decoder.Decode(0x992D, 0x940E); // next instruction is CALL, two words
        var state = new AvrCpuState { ProgramCounter = 10 };
        state.DataMemory[0x25] = 0x00;

        _executor.Execute(state, instruction);

        state.ProgramCounter.Should().Be(13u);
    }

    [TestMethod]
    public void Sbis_SkipsTwoWordInstructionWhenIoBitIsSet()
    {
        var instruction = _decoder.Decode(0x9B2D, 0x9000); // next instruction is LDS, two words
        var state = new AvrCpuState { ProgramCounter = 10 };
        state.DataMemory[0x25] = 0x20;

        _executor.Execute(state, instruction);

        state.ProgramCounter.Should().Be(13u);
    }

    [TestMethod]
    public void LpmRdZ_LoadsProgramByteWithoutIncrementingZ()
    {
        var state = new AvrCpuState();
        state.ProgramMemory = new ProgramMemory(1);
        state.ProgramMemory[0] = 0xAABB;
        state.Registers[30] = 0x01;
        state.Registers[31] = 0x00;

        var instruction = _decoder.Decode(0x9104); // LPM R16,Z
        _executor.Execute(state, instruction);

        state.Registers[16].Should().Be(0xAA);
        state.Registers[30].Should().Be(0x01);
        state.Registers[31].Should().Be(0x00);
    }

    [TestMethod]
    public void LpmRdZPlus_LoadsProgramByteAndIncrementsZ()
    {
        var state = new AvrCpuState();
        state.ProgramMemory = new ProgramMemory(1);
        state.ProgramMemory[0] = 0xAABB;
        state.Registers[30] = 0x00;
        state.Registers[31] = 0x00;

        var instruction = _decoder.Decode(0x9105); // LPM R16,Z+
        _executor.Execute(state, instruction);

        state.Registers[16].Should().Be(0xBB);
        state.Registers[30].Should().Be(0x01);
    }

    [TestMethod]
    public void Elpm_IsUnsupportedForAtmega328PProfile()
    {
        _decoder.Decode(0x95D8).Kind.Should().Be(InstructionKind.Unsupported);
        _decoder.Decode(0x9006).Kind.Should().Be(InstructionKind.Unsupported);
        _decoder.Decode(0x9007).Kind.Should().Be(InstructionKind.Unsupported);
    }

    [TestMethod]
    public void Spm_DecodesAndExecutesAsNoOpStub()
    {
        var state = new AvrCpuState { ProgramCounter = 10 };
        state.Registers[0] = 0x55;
        var instruction = _decoder.Decode(0x95E8);

        _executor.Execute(state, instruction);

        state.Registers[0].Should().Be(0x55);
        state.ProgramCounter.Should().Be(11u);
        state.CycleCount.Should().Be(1u);
    }
}
