using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Memory;

namespace EmulatorAVR.Core.Tests.Memory;

[TestClass]
public class ProgramMemoryTests
{
    [TestMethod]
    public void ConstructorSetsWordCapacity()
    {
        var mem = new ProgramMemory(1024);
        mem.WordCapacity.Should().Be(1024);
    }

    [TestMethod]
    public void DefaultWordIsZero()
    {
        var mem = new ProgramMemory(256);
        mem[0].Should().Be(0);
    }

    [TestMethod]
    public void CanWriteAndReadFirstWord()
    {
        var mem = new ProgramMemory(256);
        mem[0] = 0xABCD;
        mem[0].Should().Be(0xABCD);
    }

    [TestMethod]
    public void CanWriteAndReadLastWord()
    {
        var mem = new ProgramMemory(256);
        mem[255] = 0x1234;
        mem[255].Should().Be(0x1234);
    }

    [TestMethod]
    public void AddressEqualToCapacityThrows()
    {
        var mem = new ProgramMemory(256);
        Action act = () => { var x = mem[256]; };
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void VeryLargeOutOfRangeAddressThrows()
    {
        var mem = new ProgramMemory(256);
        Action act = () => { var x = mem[99999]; };
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
