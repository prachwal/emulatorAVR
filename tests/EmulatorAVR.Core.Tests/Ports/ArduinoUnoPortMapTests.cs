using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Ports;

namespace EmulatorAVR.Core.Tests.Ports;

[TestClass]
public class ArduinoUnoPortMapTests
{
    [TestMethod]
    public void AllNineRegistersExistByName()
    {
        var map = new ArduinoUnoPortMap();
        map.GetRegister("PINB").Should().NotBeNull();
        map.GetRegister("DDRB").Should().NotBeNull();
        map.GetRegister("PORTB").Should().NotBeNull();
        map.GetRegister("PINC").Should().NotBeNull();
        map.GetRegister("DDRC").Should().NotBeNull();
        map.GetRegister("PORTC").Should().NotBeNull();
        map.GetRegister("PIND").Should().NotBeNull();
        map.GetRegister("DDRD").Should().NotBeNull();
        map.GetRegister("PORTD").Should().NotBeNull();
    }

    [TestMethod]
    public void AllPinsD0ToD13Exist()
    {
        var map = new ArduinoUnoPortMap();
        for (int i = 0; i <= 13; i++)
        {
            var pin = map.GetPin(i);
            pin.DigitalPin.Should().Be(i);
        }
    }

    [TestMethod]
    public void D0_MapsToPORTDBit0()
    {
        var map = new ArduinoUnoPortMap();
        var pin = map.GetPin(0);
        pin.PortName.Should().Be("PORTD");
        pin.BitIndex.Should().Be(0);
    }

    [TestMethod]
    public void D7_MapsToPORTDBit7()
    {
        var map = new ArduinoUnoPortMap();
        var pin = map.GetPin(7);
        pin.PortName.Should().Be("PORTD");
        pin.BitIndex.Should().Be(7);
    }

    [TestMethod]
    public void D8_MapsToPORTBBit0()
    {
        var map = new ArduinoUnoPortMap();
        var pin = map.GetPin(8);
        pin.PortName.Should().Be("PORTB");
        pin.BitIndex.Should().Be(0);
    }

    [TestMethod]
    public void D13_MapsToPORTBBit5()
    {
        var map = new ArduinoUnoPortMap();
        var pin = map.GetPin(13);
        pin.PortName.Should().Be("PORTB");
        pin.BitIndex.Should().Be(5);
    }

    [TestMethod]
    public void PinNegative1IsRejected()
    {
        var map = new ArduinoUnoPortMap();
        Action act = () => map.GetPin(-1);
        act.Should().Throw<KeyNotFoundException>();
    }

    [TestMethod]
    public void Pin14IsRejected()
    {
        var map = new ArduinoUnoPortMap();
        Action act = () => map.GetPin(14);
        act.Should().Throw<KeyNotFoundException>();
    }
}
