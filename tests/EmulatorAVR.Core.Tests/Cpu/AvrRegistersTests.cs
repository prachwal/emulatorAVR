using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Cpu;

namespace EmulatorAVR.Core.Tests.Cpu;

[TestClass]
public class AvrRegistersTests
{
    [TestMethod]
    public void Count_Is32()
    {
        AvrRegisters.Count.Should().Be(32);
    }

    [TestMethod]
    public void DefaultAllRegistersAreZero()
    {
        var regs = new AvrRegisters();
        for (int i = 0; i < AvrRegisters.Count; i++)
            regs[i].Should().Be(0);
    }

    [TestMethod]
    public void CanWriteAndReadRegister0()
    {
        var regs = new AvrRegisters();
        regs[0] = 0xAB;
        regs[0].Should().Be(0xAB);
    }

    [TestMethod]
    public void CanWriteAndReadRegister31()
    {
        var regs = new AvrRegisters();
        regs[31] = 0xCD;
        regs[31].Should().Be(0xCD);
    }

    [TestMethod]
    public void IndexNegativeOneThrows()
    {
        var regs = new AvrRegisters();
        Action act = () => { var x = regs[-1]; };
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void Index32Throws()
    {
        var regs = new AvrRegisters();
        Action act = () => { var x = regs[32]; };
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void SnapshotCannotMutateInternalStorage()
    {
        var regs = new AvrRegisters();
        regs[0] = 0x42;
        var snapshot = regs.Snapshot();
        snapshot[0] = 0xFF;
        regs[0].Should().Be(0x42);
    }
}
