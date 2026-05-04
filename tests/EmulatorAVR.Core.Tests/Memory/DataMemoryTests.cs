using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Memory;

namespace EmulatorAVR.Core.Tests.Memory;

[TestClass]
public class DataMemoryTests
{
    [TestMethod]
    public void ConstantsMatchRegionLayout()
    {
        DataMemory.RegisterFileStart.Should().Be(0x0000);
        DataMemory.RegisterFileSize.Should().Be(32);
        DataMemory.IoRegistersStart.Should().Be(0x0020);
        DataMemory.IoRegistersSize.Should().Be(64);
        DataMemory.ExtendedIoStart.Should().Be(0x0060);
        DataMemory.ExtendedIoSize.Should().Be(160);
        DataMemory.SramStart.Should().Be(0x0100);
        DataMemory.SramSize.Should().Be(2048);
    }

    [TestMethod]
    public void SizeCovers0000to08FF_Inclusive()
    {
        DataMemory.Size.Should().Be(0x0900);
    }

    [TestMethod]
    public void DefaultByteIsZero()
    {
        var mem = new DataMemory();
        mem[0x0100].Should().Be(0);
    }

    [TestMethod]
    public void CanWriteAndReadFirstByte()
    {
        var mem = new DataMemory();
        mem[0] = 0xAB;
        mem[0].Should().Be(0xAB);
    }

    [TestMethod]
    public void CanWriteAndReadLastByte()
    {
        var mem = new DataMemory();
        mem[0x08FF] = 0xCD;
        mem[0x08FF].Should().Be(0xCD);
    }

    [TestMethod]
    public void AddressEqualToSizeThrows()
    {
        var mem = new DataMemory();
        Action act = () => { var x = mem[0x0900]; };
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void VeryLargeOutOfRangeAddressThrows()
    {
        var mem = new DataMemory();
        Action act = () => { var x = mem[99999]; };
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
