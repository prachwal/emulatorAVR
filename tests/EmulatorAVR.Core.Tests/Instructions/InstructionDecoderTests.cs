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
    public void MovOpcode_DecodesDestinationAndSourceRegisters()
    {
        var instruction = _decoder.Decode(0x2C12);
        instruction.Kind.Should().Be(InstructionKind.Mov);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void AddOpcode_DecodesDestinationAndSourceRegisters()
    {
        var instruction = _decoder.Decode(0x0C12);
        instruction.Kind.Should().Be(InstructionKind.Add);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void UnsupportedOpcode_DecodesAsUnsupported()
    {
        var instruction = _decoder.Decode(0xFFFF);
        instruction.Kind.Should().Be(InstructionKind.Unsupported);
    }
}
