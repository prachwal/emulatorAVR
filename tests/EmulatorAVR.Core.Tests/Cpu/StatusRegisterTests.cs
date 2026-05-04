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
