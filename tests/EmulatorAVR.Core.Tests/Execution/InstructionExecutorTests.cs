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
    public void Add_SetsHFlag()
    {
        var state = CreateState();
        state.Registers[1] = 0x0F;
        state.Registers[2] = 0x01;
        var instruction = _decoder.Decode(0x0C12);
        _executor.Execute(state, instruction);
        state.SREG.H.Should().BeTrue();
        state.Registers[1].Should().Be(0x10);
    }

    [TestMethod]
    public void Add_SetsSFlag()
    {
        var state = CreateState();
        state.Registers[1] = 0x80;
        state.Registers[2] = 0x80;
        var instruction = _decoder.Decode(0x0C12);
        _executor.Execute(state, instruction);
        state.SREG.S.Should().BeTrue();
        state.Registers[1].Should().Be(0x00);
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

    [TestMethod]
    public void Adc_AddsWithCarry()
    {
        var state = CreateState();
        state.Registers[1] = 0x01;
        state.Registers[2] = 0x01;
        state.SREG.C = true;
        var instruction = _decoder.Decode(0x1C12);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0x03);
        state.ProgramCounter.Should().Be(1u);
        state.CycleCount.Should().Be(1u);
    }

    [TestMethod]
    public void Sub_SubtractsAndSetsFlags()
    {
        var state = CreateState();
        state.Registers[1] = 0x05;
        state.Registers[2] = 0x03;
        var instruction = _decoder.Decode(0x1812);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0x02);
        state.SREG.Z.Should().BeFalse();
        state.SREG.C.Should().BeFalse();
    }

    [TestMethod]
    public void Sub_SetsCarryOnBorrow()
    {
        var state = CreateState();
        state.Registers[1] = 0x01;
        state.Registers[2] = 0x02;
        var instruction = _decoder.Decode(0x1812);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0xFF);
        state.SREG.C.Should().BeTrue();
    }

    [TestMethod]
    public void Subi_SubtractsImmediate()
    {
        var state = CreateState();
        state.Registers[16] = 0x05;
        var instruction = _decoder.Decode(0x5003);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x02);
    }

    [TestMethod]
    public void Cp_DoesNotMutateRegisters()
    {
        var state = CreateState();
        state.Registers[1] = 0x05;
        state.Registers[2] = 0x03;
        var instruction = _decoder.Decode(0x1412);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0x05);
        state.Registers[2].Should().Be(0x03);
        state.SREG.C.Should().BeFalse();
    }

    [TestMethod]
    public void Cp_DetectsEquality()
    {
        var state = CreateState();
        state.Registers[1] = 0x05;
        state.Registers[2] = 0x05;
        var instruction = _decoder.Decode(0x1412);
        _executor.Execute(state, instruction);
        state.SREG.Z.Should().BeTrue();
    }

    [TestMethod]
    public void Cpi_ComparesWithImmediate()
    {
        var state = CreateState();
        state.Registers[16] = 0x03;
        var instruction = _decoder.Decode(0x3404);
        _executor.Execute(state, instruction);
        state.SREG.Z.Should().BeFalse();
        state.Registers[16].Should().Be(0x03);
    }

    [TestMethod]
    public void Inc_IncrementsRegister()
    {
        var state = CreateState();
        state.Registers[16] = 0x05;
        var instruction = _decoder.Decode(0x9503);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x06);
    }

    [TestMethod]
    public void Dec_DecrementsRegister()
    {
        var state = CreateState();
        state.Registers[16] = 0x05;
        var instruction = _decoder.Decode(0x950A);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x04);
    }

    [TestMethod]
    public void Dec_SetsZeroFlag()
    {
        var state = CreateState();
        state.Registers[16] = 0x01;
        var instruction = _decoder.Decode(0x950A);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x00);
        state.SREG.Z.Should().BeTrue();
    }

    [TestMethod]
    public void Neg_NegatesRegister()
    {
        var state = CreateState();
        state.Registers[16] = 0x01;
        var instruction = _decoder.Decode(0x9501);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0xFF);
    }

    [TestMethod]
    public void Neg_NegatesZeroToZero()
    {
        var state = CreateState();
        state.Registers[16] = 0x00;
        var instruction = _decoder.Decode(0x9501);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x00);
        state.SREG.Z.Should().BeTrue();
    }

    [TestMethod]
    public void Adiw_AddsImmediateToWordPair()
    {
        var state = CreateState();
        state.Registers[24] = 0x01;
        state.Registers[25] = 0x00;
        var instruction = _decoder.Decode(0x9602);
        _executor.Execute(state, instruction);
        state.Registers[24].Should().Be(0x03);
        state.Registers[25].Should().Be(0x00);
    }

    [TestMethod]
    public void Sbiw_SubtractsImmediateFromWordPair()
    {
        var state = CreateState();
        state.Registers[24] = 0xFE;
        state.Registers[25] = 0x01;
        var instruction = _decoder.Decode(0x9702);
        _executor.Execute(state, instruction);
        state.Registers[24].Should().Be(0xFC);
        state.Registers[25].Should().Be(0x01);
    }

    [TestMethod]
    public void Sbc_SubtractsWithCarryBorrow()
    {
        var state = CreateState();
        state.Registers[1] = 0x05;
        state.Registers[2] = 0x02;
        state.SREG.C = true;
        var instruction = _decoder.Decode(0x0812);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0x02);
    }

    [TestMethod]
    public void Sbci_SubtractsImmediateWithCarryBorrow()
    {
        var state = CreateState();
        state.Registers[16] = 0x05;
        state.SREG.C = true;
        var instruction = _decoder.Decode(0x4002);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x02);
    }

    [TestMethod]
    public void Cpc_CompareWithCarryDoesNotMutate()
    {
        var state = CreateState();
        state.Registers[1] = 0x05;
        state.Registers[2] = 0x02;
        state.SREG.C = true;
        var instruction = _decoder.Decode(0x0412);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0x05);
        state.Registers[2].Should().Be(0x02);
    }

    [TestMethod]
    public void And_PerformsBitwiseAnd()
    {
        var state = CreateState();
        state.Registers[1] = 0x0F;
        state.Registers[2] = 0x03;
        var instruction = _decoder.Decode(0x2012);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0x03);
        state.SREG.Z.Should().BeFalse();
    }

    [TestMethod]
    public void Andi_PerformsBitwiseAndWithImmediate()
    {
        var state = CreateState();
        state.Registers[16] = 0x0F;
        var instruction = _decoder.Decode(0x7003);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x03);
    }

    [TestMethod]
    public void Or_PerformsBitwiseOr()
    {
        var state = CreateState();
        state.Registers[1] = 0x0F;
        state.Registers[2] = 0xF0;
        var instruction = _decoder.Decode(0x2812);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0xFF);
    }

    [TestMethod]
    public void Ori_PerformsBitwiseOrWithImmediate()
    {
        var state = CreateState();
        state.Registers[16] = 0x0F;
        var instruction = _decoder.Decode(0x6F00);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0xFF);
    }

    [TestMethod]
    public void Eor_PerformsBitwiseXor()
    {
        var state = CreateState();
        state.Registers[1] = 0xFF;
        state.Registers[2] = 0x0F;
        var instruction = _decoder.Decode(0x2412);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0xF0);
    }

    [TestMethod]
    public void Com_PerformsOnesComplement()
    {
        var state = CreateState();
        state.Registers[16] = 0xF0;
        var instruction = _decoder.Decode(0x9500);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x0F);
        state.SREG.C.Should().BeTrue();
    }

    [TestMethod]
    public void Tst_SetsZeroFlagForZeroRegister()
    {
        var state = CreateState();
        state.Registers[1] = 0x00;
        state.Registers[0] = 0x00;
        var instruction = _decoder.Decode(0x2011);
        _executor.Execute(state, instruction);
        instruction.Kind.Should().Be(InstructionKind.Tst);
        state.SREG.Z.Should().BeTrue();
        state.Registers[1].Should().Be(0x00);
    }

    [TestMethod]
    public void Clr_ClearsRegister()
    {
        var state = CreateState();
        state.Registers[1] = 0xFF;
        var instruction = _decoder.Decode(0x2411);
        _executor.Execute(state, instruction);
        state.Registers[1].Should().Be(0x00);
        state.SREG.Z.Should().BeTrue();
    }

    [TestMethod]
    public void Ser_SetsRegisterToFF()
    {
        var state = CreateState();
        state.Registers[31] = 0x00;
        var instruction = _decoder.Decode(0xEFFF);
        _executor.Execute(state, instruction);
        state.Registers[31].Should().Be(0xFF);
    }

    [TestMethod]
    public void Lsl_ShiftsLeft()
    {
        var state = CreateState();
        state.Registers[1] = 0x0F;
        var instruction = _decoder.Decode(0x0C11);
        _executor.Execute(state, instruction);
        instruction.Kind.Should().Be(InstructionKind.Lsl);
        state.Registers[1].Should().Be(0x1E);
    }

    [TestMethod]
    public void Lsr_ShiftsRight()
    {
        var state = CreateState();
        state.Registers[16] = 0x0F;
        var instruction = _decoder.Decode(0x9506);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x07);
        state.SREG.C.Should().BeTrue();
    }

    [TestMethod]
    public void Ror_RotatesRightThroughCarry()
    {
        var state = CreateState();
        state.Registers[16] = 0x01;
        state.SREG.C = true;
        var instruction = _decoder.Decode(0x9507);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x80);
        state.SREG.C.Should().BeTrue();
    }

    [TestMethod]
    public void Asr_ArithmeticShiftRightPreservesSign()
    {
        var state = CreateState();
        state.Registers[16] = 0xF0;
        var instruction = _decoder.Decode(0x9505);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0xF8);
        state.SREG.N.Should().BeTrue();
    }

    [TestMethod]
    public void Swap_SwapsNibbles()
    {
        var state = CreateState();
        state.Registers[16] = 0xF0;
        var instruction = _decoder.Decode(0x9502);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0x0F);
    }

    [TestMethod]
    public void Rjmp_JumpsForward()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        var instruction = _decoder.Decode(0xC005);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(16u);
    }

    [TestMethod]
    public void Rjmp_JumpsBackward()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        var instruction = _decoder.Decode(0xCFFF);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(10u);
    }

    [TestMethod]
    public void Breq_BranchesWhenZeroFlagSet()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.SREG.Z = true;
        var instruction = _decoder.Decode(0xF009);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(12u);
    }

    [TestMethod]
    public void Breq_DoesNotBranchWhenZeroFlagClear()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.SREG.Z = false;
        var instruction = _decoder.Decode(0xF009);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(11u);
    }

    [TestMethod]
    public void Brne_BranchesWhenZeroFlagClear()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.SREG.Z = false;
        var instruction = _decoder.Decode(0xF409);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(12u);
    }

    [TestMethod]
    public void Brne_DoesNotBranchWhenZeroFlagSet()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.SREG.Z = true;
        var instruction = _decoder.Decode(0xF409);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(11u);
    }

    [TestMethod]
    public void Sbrc_SkipsWhenRegisterBitClear()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.Registers[16] = 0x00;
        var instruction = _decoder.Decode(0xFA03);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(12u);
    }

    [TestMethod]
    public void Sbrc_DoesNotSkipWhenRegisterBitSet()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.Registers[16] = 0x08;
        var instruction = _decoder.Decode(0xFA03);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(11u);
    }

    [TestMethod]
    public void Sbrs_SkipsWhenRegisterBitSet()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.Registers[16] = 0x08;
        var instruction = _decoder.Decode(0xFE03);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(12u);
    }

    [TestMethod]
    public void Sbrs_DoesNotSkipWhenRegisterBitClear()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.Registers[16] = 0x00;
        var instruction = _decoder.Decode(0xFE03);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(11u);
    }

    [TestMethod]
    public void Sec_SetsCarryFlag()
    {
        var state = CreateState();
        state.SREG.C = false;
        var instruction = _decoder.Decode(0x9408);
        _executor.Execute(state, instruction);
        state.SREG.C.Should().BeTrue();
    }

    [TestMethod]
    public void Clc_ClearsCarryFlag()
    {
        var state = CreateState();
        state.SREG.C = true;
        var instruction = _decoder.Decode(0x9488);
        _executor.Execute(state, instruction);
        state.SREG.C.Should().BeFalse();
    }

    [TestMethod]
    public void Sei_SetsInterruptFlag()
    {
        var state = CreateState();
        state.SREG.I = false;
        var instruction = _decoder.Decode(0x9478);
        _executor.Execute(state, instruction);
        state.SREG.I.Should().BeTrue();
    }

    [TestMethod]
    public void Cli_ClearsInterruptFlag()
    {
        var state = CreateState();
        state.SREG.I = true;
        var instruction = _decoder.Decode(0x94F8);
        _executor.Execute(state, instruction);
        state.SREG.I.Should().BeFalse();
    }

    [TestMethod]
    public void Out_WritesRegisterToDataMemory()
    {
        var state = CreateState();
        state.Registers[16] = 0x55;
        var instruction = _decoder.Decode(0xB90B);
        _executor.Execute(state, instruction);
        state.DataMemory[0x2B].Should().Be(0x55);
    }

    [TestMethod]
    public void In_ReadsDataMemoryToRegister()
    {
        var state = CreateState();
        state.DataMemory[0x2B] = 0xAA;
        var instruction = _decoder.Decode(0xB10B);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0xAA);
    }

    [TestMethod]
    public void Sbi_SetsBitInIoRegister()
    {
        var state = CreateState();
        state.DataMemory[0x25] = 0x00;
        var instruction = _decoder.Decode(0x9A2D);
        _executor.Execute(state, instruction);
        (state.DataMemory[0x25] & 0x20).Should().Be(0x20);
    }

    [TestMethod]
    public void Cbi_ClearsBitInIoRegister()
    {
        var state = CreateState();
        state.DataMemory[0x25] = 0xFF;
        var instruction = _decoder.Decode(0x982D);
        _executor.Execute(state, instruction);
        (state.DataMemory[0x25] & 0x20).Should().Be(0);
    }

    [TestMethod]
    public void Push_PushesRegisterOntoStack()
    {
        var state = CreateState();
        state.Registers[16] = 0xAB;
        state.DataMemory[0x5D] = 0xFF;
        state.DataMemory[0x5E] = 0x08;
        var instruction = _decoder.Decode(0x930F);
        _executor.Execute(state, instruction);
        state.DataMemory[0x08FE].Should().Be(0xAB);
        state.DataMemory[0x5D].Should().Be(0xFE);
        state.DataMemory[0x5E].Should().Be(0x08);
    }

    [TestMethod]
    public void Pop_PopsRegisterFromStack()
    {
        var state = CreateState();
        state.DataMemory[0x5D] = 0xFE;
        state.DataMemory[0x5E] = 0x08;
        state.DataMemory[0x08FE] = 0xCD;
        var instruction = _decoder.Decode(0x910F);
        _executor.Execute(state, instruction);
        state.Registers[16].Should().Be(0xCD);
        state.DataMemory[0x5D].Should().Be(0xFF);
        state.DataMemory[0x5E].Should().Be(0x08);
    }

    [TestMethod]
    public void PushPop_RoundtripPreservesValue()
    {
        var state = CreateState();
        state.Registers[16] = 0x42;
        state.DataMemory[0x5D] = 0xFF;
        state.DataMemory[0x5E] = 0x08;
        var push = _decoder.Decode(0x930F);
        var pop = _decoder.Decode(0x910F);
        _executor.Execute(state, push);
        state.Registers[16] = 0x00;
        _executor.Execute(state, pop);
        state.Registers[16].Should().Be(0x42);
    }

    [TestMethod]
    public void Cpse_SkipsWhenRegistersEqual()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.Registers[1] = 0x05;
        state.Registers[2] = 0x05;
        var instruction = _decoder.Decode(0x1012);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(12u);
    }

    [TestMethod]
    public void Cpse_DoesNotSkipWhenRegistersDiffer()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        state.Registers[1] = 0x05;
        state.Registers[2] = 0x03;
        var instruction = _decoder.Decode(0x1012);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(11u);
    }

    [TestMethod]
    public void Sleep_DoesNotChangeState()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        var instruction = _decoder.Decode(0x9588);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(11u);
    }

    [TestMethod]
    public void Wdr_DoesNotChangeState()
    {
        var state = CreateState();
        state.ProgramCounter = 10;
        var instruction = _decoder.Decode(0x95A8);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(11u);
    }

    [TestMethod]
    public void Mul_MultipliesTwoRegisters()
    {
        var state = CreateState();
        state.Registers[16] = 0x0A;
        state.Registers[17] = 0x03;
        var instruction = _decoder.Decode(0x9F01);
        _executor.Execute(state, instruction);
        state.Registers[0].Should().Be(0x1E);
        state.Registers[1].Should().Be(0x00);
    }

    [TestMethod]
    public void Mul_SetsCarryFlagForLargeResult()
    {
        var state = CreateState();
        state.Registers[16] = 0xFF;
        state.Registers[17] = 0xFF;
        var instruction = _decoder.Decode(0x9F01);
        _executor.Execute(state, instruction);
        state.SREG.C.Should().BeTrue();
    }

    [TestMethod]
    public void Mul_SetsZeroFlagForResultZero()
    {
        var state = CreateState();
        state.Registers[16] = 0x00;
        state.Registers[17] = 0xFF;
        var instruction = _decoder.Decode(0x9F01);
        _executor.Execute(state, instruction);
        state.SREG.Z.Should().BeTrue();
    }

    [TestMethod]
    public void Ijmp_JumpsToZRegister()
    {
        var state = CreateState();
        state.Registers[30] = 0x42;
        state.Registers[31] = 0x01;
        var instruction = _decoder.Decode(0x9409);
        _executor.Execute(state, instruction);
        state.ProgramCounter.Should().Be(0x0142u);
    }

    [TestMethod]
    public void Ret_RoundtripThroughStack()
    {
        var state = CreateState();
        state.DataMemory[0x5D] = 0xFF;
        state.DataMemory[0x5E] = 0x08;
        state.Registers[30] = 0x10;
        state.Registers[31] = 0x00;
        state.ProgramCounter = 10;
        var icall = _decoder.Decode(0x9509);
        _executor.Execute(state, icall);
        var ret = _decoder.Decode(0x9508);
        _executor.Execute(state, ret);
        state.ProgramCounter.Should().Be(0x000Bu);
    }

    [TestMethod]
    public void Rcall_CallsAndReturns()
    {
        var state = CreateState();
        state.DataMemory[0x5D] = 0xFF;
        state.DataMemory[0x5E] = 0x08;
        state.ProgramCounter = 10;
        var rcall = _decoder.Decode(0xD005);
        _executor.Execute(state, rcall);
        state.ProgramCounter.Should().Be(0x0010u);
        var ret = _decoder.Decode(0x9508);
        _executor.Execute(state, ret);
        state.ProgramCounter.Should().Be(0x000Bu);
    }
}
