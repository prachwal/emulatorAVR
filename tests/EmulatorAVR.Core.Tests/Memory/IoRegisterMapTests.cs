using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Memory;
using EmulatorAVR.Core.Ports;

namespace EmulatorAVR.Core.Tests.Memory;

[TestClass]
public class IoRegisterMapTests
{
    private static IoRegisterMap CreateMap()
    {
        var portMap = new ArduinoUnoPortMap();
        return new IoRegisterMap(portMap);
    }

    [TestMethod]
    public void AllNineNamedAddressesExist()
    {
        var map = CreateMap();
        map.ContainsAddress(0x23).Should().BeTrue();
        map.ContainsAddress(0x24).Should().BeTrue();
        map.ContainsAddress(0x25).Should().BeTrue();
        map.ContainsAddress(0x26).Should().BeTrue();
        map.ContainsAddress(0x27).Should().BeTrue();
        map.ContainsAddress(0x28).Should().BeTrue();
        map.ContainsAddress(0x29).Should().BeTrue();
        map.ContainsAddress(0x2A).Should().BeTrue();
        map.ContainsAddress(0x2B).Should().BeTrue();
    }

    [TestMethod]
    public void DefaultValuesAreZero()
    {
        var map = CreateMap();
        map.Read(0x23).Should().Be(0);
        map.Read(0x24).Should().Be(0);
        map.Read(0x25).Should().Be(0);
        map.Read(0x26).Should().Be(0);
        map.Read(0x27).Should().Be(0);
        map.Read(0x28).Should().Be(0);
        map.Read(0x29).Should().Be(0);
        map.Read(0x2A).Should().Be(0);
        map.Read(0x2B).Should().Be(0);
    }

    [TestMethod]
    public void WriteReadDDRB_Stores0xFF()
    {
        var map = CreateMap();
        map.Write(0x24, 0xFF);
        map.Read(0x24).Should().Be(0xFF);
        map.GetRegister("DDRB").Value.Should().Be(0xFF);
    }

    [TestMethod]
    public void WriteReadPORTB_Stores0xA5()
    {
        var map = CreateMap();
        map.Write(0x25, 0xA5);
        map.Read(0x25).Should().Be(0xA5);
    }

    [TestMethod]
    public void WriteReadDDRD_Stores0x55()
    {
        var map = CreateMap();
        map.Write(0x2A, 0x55);
        map.Read(0x2A).Should().Be(0x55);
    }

    [TestMethod]
    public void WriteReadPORTD_Stores0x80()
    {
        var map = CreateMap();
        map.Write(0x2B, 0x80);
        map.Read(0x2B).Should().Be(0x80);
    }

    [TestMethod]
    public void WritingOneRegisterDoesNotMutateAnother()
    {
        var map = CreateMap();
        map.Write(0x24, 0xFF);
        map.Read(0x25).Should().Be(0);
        map.Read(0x23).Should().Be(0);
    }

    [TestMethod]
    public void ContainsAddress0x23_IsTrue()
    {
        var map = CreateMap();
        map.ContainsAddress(0x23).Should().BeTrue();
    }

    [TestMethod]
    public void ContainsAddress0x22_IsFalse()
    {
        var map = CreateMap();
        map.ContainsAddress(0x22).Should().BeFalse();
    }

    [TestMethod]
    public void ContainsAddress_ReducedIoOffsetsAreRejected()
    {
        var map = CreateMap();
        for (ushort addr = 0x03; addr <= 0x0B; addr++)
            map.ContainsAddress(addr).Should().BeFalse($"address 0x{addr:X2} is a reduced I/O offset, not data memory");
    }

    [TestMethod]
    public void GetRegisters_ReturnsStableList()
    {
        var map = CreateMap();
        map.Registers.Should().HaveCount(9);
    }

    [TestMethod]
    public void ReadingUnknownAddressIsRejected()
    {
        var map = CreateMap();
        Action act = () => map.Read(0x22);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void WritingUnknownAddressIsRejected()
    {
        var map = CreateMap();
        Action act = () => map.Write(0x22, 0x00);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void UnknownRegisterNameIsRejected()
    {
        var map = CreateMap();
        Action act = () => map.GetRegister("TIMSK0");
        act.Should().Throw<KeyNotFoundException>();
    }
}
