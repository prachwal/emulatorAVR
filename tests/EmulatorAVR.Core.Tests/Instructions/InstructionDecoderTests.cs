using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Instructions;

namespace EmulatorAVR.Core.Tests.Instructions;

[TestClass]
public class InstructionDecoderTests
{
    private readonly InstructionDecoder _decoder = new();

    [TestMethod]
    public void Opcode0000_DecodesAsNop()
    {
        var instruction = _decoder.Decode(0x0000);
        instruction.Kind.Should().Be(InstructionKind.Nop);
    }

    [TestMethod]
    public void LdiOpcode_DecodesDestinationRegisterAndImmediate()
    {
        var instruction = _decoder.Decode(0xE402);
        instruction.Kind.Should().Be(InstructionKind.Ldi);
        instruction.Rd.Should().Be(16);
        instruction.Immediate.Should().Be(0x42);
    }

    [TestMethod]
    public void LdiOpcode_R31DecodesCorrectly()
    {
        var instruction = _decoder.Decode(0xEFFF);
        instruction.Kind.Should().Be(InstructionKind.Ldi);
        instruction.Rd.Should().Be(31);
        instruction.Immediate.Should().Be(0xFF);
    }

    [TestMethod]
    public void MovOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x2C12);
        instruction.Kind.Should().Be(InstructionKind.Mov);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void MovOpcode_DecodesR17R2()
    {
        var instruction = _decoder.Decode(0x2D12);
        instruction.Kind.Should().Be(InstructionKind.Mov);
        instruction.Rd.Should().Be(17);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void MovOpcode_DecodesR1R18()
    {
        var instruction = _decoder.Decode(0x2E12);
        instruction.Kind.Should().Be(InstructionKind.Mov);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(18);
    }

    [TestMethod]
    public void MovOpcode_DecodesR17R18()
    {
        var instruction = _decoder.Decode(0x2F12);
        instruction.Kind.Should().Be(InstructionKind.Mov);
        instruction.Rd.Should().Be(17);
        instruction.Rr.Should().Be(18);
    }

    [TestMethod]
    public void AddOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x0C12);
        instruction.Kind.Should().Be(InstructionKind.Add);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void AddOpcode_DecodesR17R2()
    {
        var instruction = _decoder.Decode(0x0D12);
        instruction.Kind.Should().Be(InstructionKind.Add);
        instruction.Rd.Should().Be(17);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void AddOpcode_DecodesR1R18()
    {
        var instruction = _decoder.Decode(0x0E12);
        instruction.Kind.Should().Be(InstructionKind.Add);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(18);
    }

    [TestMethod]
    public void AddOpcode_DecodesR17R18()
    {
        var instruction = _decoder.Decode(0x0F12);
        instruction.Kind.Should().Be(InstructionKind.Add);
        instruction.Rd.Should().Be(17);
        instruction.Rr.Should().Be(18);
    }

    [TestMethod]
    public void UnsupportedOpcode_DecodesAsUnsupported()
    {
        var instruction = _decoder.Decode(0xFFFF);
        instruction.Kind.Should().Be(InstructionKind.Unsupported);
    }

    [TestMethod]
    public void AdcOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x1C12);
        instruction.Kind.Should().Be(InstructionKind.Adc);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void SubOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x1812);
        instruction.Kind.Should().Be(InstructionKind.Sub);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void CpOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x1412);
        instruction.Kind.Should().Be(InstructionKind.Cp);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void CpcOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x0412);
        instruction.Kind.Should().Be(InstructionKind.Cpc);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void SbcOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x0812);
        instruction.Kind.Should().Be(InstructionKind.Sbc);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void SubiOpcode_DecodesR16Immediate()
    {
        // SUBI R16, 0x42
        var instruction = _decoder.Decode(0x5402);
        instruction.Kind.Should().Be(InstructionKind.Subi);
        instruction.Rd.Should().Be(16);
        instruction.Immediate.Should().Be(0x42);
    }

    [TestMethod]
    public void SbciOpcode_DecodesR16Immediate()
    {
        // SBCI R16, 0x42
        var instruction = _decoder.Decode(0x4402);
        instruction.Kind.Should().Be(InstructionKind.Sbci);
        instruction.Rd.Should().Be(16);
        instruction.Immediate.Should().Be(0x42);
    }

    [TestMethod]
    public void CpiOpcode_DecodesR16Immediate()
    {
        // CPI R16, 0x42
        var instruction = _decoder.Decode(0x3402);
        instruction.Kind.Should().Be(InstructionKind.Cpi);
        instruction.Rd.Should().Be(16);
        instruction.Immediate.Should().Be(0x42);
    }

    [TestMethod]
    public void IncOpcode_DecodesR16()
    {
        var instruction = _decoder.Decode(0x9503);
        instruction.Kind.Should().Be(InstructionKind.Inc);
        instruction.Rd.Should().Be(16);
    }

    [TestMethod]
    public void DecOpcode_DecodesR16()
    {
        var instruction = _decoder.Decode(0x950A);
        instruction.Kind.Should().Be(InstructionKind.Dec);
        instruction.Rd.Should().Be(16);
    }

    [TestMethod]
    public void NegOpcode_DecodesR16()
    {
        var instruction = _decoder.Decode(0x9501);
        instruction.Kind.Should().Be(InstructionKind.Neg);
        instruction.Rd.Should().Be(16);
    }

    [TestMethod]
    public void AdiwOpcode_DecodesR24WithImmediate()
    {
        var instruction = _decoder.Decode(0x9600);
        instruction.Kind.Should().Be(InstructionKind.Adiw);
        instruction.WordRegisterPair.Should().Be(24);
        instruction.Immediate.Should().Be(0);
    }

    [TestMethod]
    public void AdiwOpcode_DecodesR28WithImmediate()
    {
        var instruction = _decoder.Decode(0x9620);
        instruction.Kind.Should().Be(InstructionKind.Adiw);
        instruction.WordRegisterPair.Should().Be(28);
    }

    [TestMethod]
    public void SbiwOpcode_DecodesR24WithImmediate()
    {
        var instruction = _decoder.Decode(0x9700);
        instruction.Kind.Should().Be(InstructionKind.Sbiw);
        instruction.WordRegisterPair.Should().Be(24);
        instruction.Immediate.Should().Be(0);
    }
}
