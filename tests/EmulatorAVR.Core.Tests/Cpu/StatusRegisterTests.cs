using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Cpu;

namespace EmulatorAVR.Core.Tests.Cpu;

[TestClass]
public class StatusRegisterTests
{
    [TestMethod]
    public void DefaultRawValueIsZero()
    {
        var sreg = new StatusRegister();
        sreg.Value.Should().Be(0);
    }

    [TestMethod]
    public void CreateFromFF_SetsAllFlags()
    {
        var sreg = new StatusRegister(0xFF);
        sreg.I.Should().BeTrue();
        sreg.T.Should().BeTrue();
        sreg.H.Should().BeTrue();
        sreg.S.Should().BeTrue();
        sreg.V.Should().BeTrue();
        sreg.N.Should().BeTrue();
        sreg.Z.Should().BeTrue();
        sreg.C.Should().BeTrue();
    }

    [TestMethod]
    public void EachFlagMapsToCorrectBit()
    {
        var flags = new (string name, Action<StatusRegister, bool> setter, Func<StatusRegister, bool> getter, byte mask)[]
        {
            ("I", (s, v) => s.I = v, s => s.I, 0x80),
            ("T", (s, v) => s.T = v, s => s.T, 0x40),
            ("H", (s, v) => s.H = v, s => s.H, 0x20),
            ("S", (s, v) => s.S = v, s => s.S, 0x10),
            ("V", (s, v) => s.V = v, s => s.V, 0x08),
            ("N", (s, v) => s.N = v, s => s.N, 0x04),
            ("Z", (s, v) => s.Z = v, s => s.Z, 0x02),
            ("C", (s, v) => s.C = v, s => s.C, 0x01),
        };

        foreach (var (name, setter, getter, mask) in flags)
        {
            var sreg = new StatusRegister();
            setter(sreg, true);
            sreg.Value.Should().Be(mask, $"setting {name} should produce 0x{mask:X2}");
        }
    }

    [TestMethod]
    public void Z_SetsBit1Only()
    {
        var sreg = new StatusRegister();
        sreg.Z = true;
        sreg.Value.Should().Be(0x02);
        sreg.I.Should().BeFalse();
        sreg.T.Should().BeFalse();
        sreg.H.Should().BeFalse();
        sreg.S.Should().BeFalse();
        sreg.V.Should().BeFalse();
        sreg.N.Should().BeFalse();
        sreg.C.Should().BeFalse();
    }

    [TestMethod]
    public void C_SetsBit0Only()
    {
        var sreg = new StatusRegister();
        sreg.C = true;
        sreg.Value.Should().Be(0x01);
    }

    [TestMethod]
    public void ClearingFlagClearsOnlyThatFlag()
    {
        var sreg = new StatusRegister(0xFF);
        sreg.Z = false;
        sreg.Value.Should().Be(0xFD);
        sreg.I.Should().BeTrue();
        sreg.T.Should().BeTrue();
        sreg.H.Should().BeTrue();
        sreg.S.Should().BeTrue();
        sreg.V.Should().BeTrue();
        sreg.N.Should().BeTrue();
        sreg.Z.Should().BeFalse();
        sreg.C.Should().BeTrue();
    }
}
