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
        instruction.Kind.Should().Be(InstructionKind.Ser);
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

    [TestMethod]
    public void AndOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x2012);
        instruction.Kind.Should().Be(InstructionKind.And);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void TstOpcode_DecodesAsTstWhenRdEqualsRr()
    {
        var instruction = _decoder.Decode(0x2011);
        instruction.Kind.Should().Be(InstructionKind.Tst);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(1);
    }

    [TestMethod]
    public void EorOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x2412);
        instruction.Kind.Should().Be(InstructionKind.Eor);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void ClrOpcode_DecodesAsClrWhenRdEqualsRr()
    {
        var instruction = _decoder.Decode(0x2411);
        instruction.Kind.Should().Be(InstructionKind.Clr);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(1);
    }

    [TestMethod]
    public void OrOpcode_DecodesR1R2()
    {
        var instruction = _decoder.Decode(0x2812);
        instruction.Kind.Should().Be(InstructionKind.Or);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(2);
    }

    [TestMethod]
    public void AndiOpcode_DecodesR16Immediate()
    {
        var instruction = _decoder.Decode(0x7402);
        instruction.Kind.Should().Be(InstructionKind.Andi);
        instruction.Rd.Should().Be(16);
        instruction.Immediate.Should().Be(0x42);
    }

    [TestMethod]
    public void OriOpcode_DecodesR16Immediate()
    {
        var instruction = _decoder.Decode(0x6402);
        instruction.Kind.Should().Be(InstructionKind.Ori);
        instruction.Rd.Should().Be(16);
        instruction.Immediate.Should().Be(0x42);
    }

    [TestMethod]
    public void ComOpcode_DecodesR16()
    {
        var instruction = _decoder.Decode(0x9500);
        instruction.Kind.Should().Be(InstructionKind.Com);
        instruction.Rd.Should().Be(16);
    }

    [TestMethod]
    public void SerOpcode_DecodesAsSerForLdiWithFF()
    {
        var instruction = _decoder.Decode(0xEFFF);
        instruction.Kind.Should().Be(InstructionKind.Ser);
        instruction.Rd.Should().Be(31);
        instruction.Immediate.Should().Be(0xFF);
    }

    [TestMethod]
    public void LslOpcode_DecodesAsLslForAddWithRdEqRr()
    {
        var instruction = _decoder.Decode(0x0C11);
        instruction.Kind.Should().Be(InstructionKind.Lsl);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(1);
    }

    [TestMethod]
    public void RolOpcode_DecodesAsRolForAdcWithRdEqRr()
    {
        var instruction = _decoder.Decode(0x1C11);
        instruction.Kind.Should().Be(InstructionKind.Rol);
        instruction.Rd.Should().Be(1);
        instruction.Rr.Should().Be(1);
    }

    [TestMethod]
    public void LsrOpcode_DecodesR16()
    {
        var instruction = _decoder.Decode(0x9506);
        instruction.Kind.Should().Be(InstructionKind.Lsr);
        instruction.Rd.Should().Be(16);
    }

    [TestMethod]
    public void RorOpcode_DecodesR16()
    {
        var instruction = _decoder.Decode(0x9507);
        instruction.Kind.Should().Be(InstructionKind.Ror);
        instruction.Rd.Should().Be(16);
    }

    [TestMethod]
    public void AsrOpcode_DecodesR16()
    {
        var instruction = _decoder.Decode(0x9505);
        instruction.Kind.Should().Be(InstructionKind.Asr);
        instruction.Rd.Should().Be(16);
    }

    [TestMethod]
    public void SwapOpcode_DecodesR16()
    {
        var instruction = _decoder.Decode(0x9502);
        instruction.Kind.Should().Be(InstructionKind.Swap);
        instruction.Rd.Should().Be(16);
    }

    [TestMethod]
    public void RjmpOpcode_DecodesWithOffset()
    {
        var instruction = _decoder.Decode(0xC000);
        instruction.Kind.Should().Be(InstructionKind.Rjmp);
        instruction.Offset.Should().Be(0);
    }

    [TestMethod]
    public void RjmpOpcode_DecodesForwardOffset()
    {
        var instruction = _decoder.Decode(0xC003);
        instruction.Kind.Should().Be(InstructionKind.Rjmp);
        instruction.Offset.Should().Be(3);
    }

    [TestMethod]
    public void RjmpOpcode_DecodesBackwardOffset()
    {
        var instruction = _decoder.Decode(0xCFFF);
        instruction.Kind.Should().Be(InstructionKind.Rjmp);
        instruction.Offset.Should().Be(-1);
    }

    [TestMethod]
    public void BreqOpcode_DecodesAsBreq()
    {
        var instruction = _decoder.Decode(0xF009);
        instruction.Kind.Should().Be(InstructionKind.Breq);
        instruction.Rd.Should().Be(1);
        instruction.Offset.Should().Be(1);
    }

    [TestMethod]
    public void BrneOpcode_DecodesAsBrne()
    {
        var instruction = _decoder.Decode(0xF409);
        instruction.Kind.Should().Be(InstructionKind.Brne);
        instruction.Rd.Should().Be(1);
        instruction.Offset.Should().Be(1);
    }

    [TestMethod]
    public void BrcsOpcode_DecodesAsBrcs()
    {
        var instruction = _decoder.Decode(0xF000);
        instruction.Kind.Should().Be(InstructionKind.Brcs);
        instruction.Rd.Should().Be(0);
    }

    [TestMethod]
    public void BrccOpcode_DecodesAsBrcc()
    {
        var instruction = _decoder.Decode(0xF400);
        instruction.Kind.Should().Be(InstructionKind.Brcc);
        instruction.Rd.Should().Be(0);
    }
}
