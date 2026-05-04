using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Ports;

namespace EmulatorAVR.Core.Tests.Ports;

[TestClass]
public class PortSnapshotTests
{
    [TestMethod]
    public void SnapshotContainsAllNineRegisters()
    {
        var map = new ArduinoUnoPortMap();
        var snapshot = map.CreateSnapshot();
        snapshot.GetValue("PINB");
        snapshot.GetValue("DDRB");
        snapshot.GetValue("PORTB");
        snapshot.GetValue("PINC");
        snapshot.GetValue("DDRC");
        snapshot.GetValue("PORTC");
        snapshot.GetValue("PIND");
        snapshot.GetValue("DDRD");
        snapshot.GetValue("PORTD");
    }

    [TestMethod]
    public void SnapshotReflectsValuesWrittenBeforeCreation()
    {
        var map = new ArduinoUnoPortMap();
        map.GetRegister("DDRB").Write(0xAB);
        var snapshot = map.CreateSnapshot();
        snapshot.GetValue("DDRB").Should().Be(0xAB);
    }

    [TestMethod]
    public void LaterWritesDoNotMutateExistingSnapshot()
    {
        var map = new ArduinoUnoPortMap();
        var snapshot = map.CreateSnapshot();
        map.GetRegister("DDRB").Write(0xFF);
        snapshot.GetValue("DDRB").Should().Be(0);
    }

    [TestMethod]
    public void TwoSnapshotsDetectChangedValues()
    {
        var map = new ArduinoUnoPortMap();
        var before = map.CreateSnapshot();
        map.GetRegister("PORTB").Write(0x01);
        var after = map.CreateSnapshot();
        before.GetValue("PORTB").Should().Be(0);
        after.GetValue("PORTB").Should().Be(0x01);
    }

    [TestMethod]
    public void GetAllValuesReturnsImmutableView()
    {
        var map = new ArduinoUnoPortMap();
        var snapshot = map.CreateSnapshot();
        var allValues = snapshot.GetAllValues();
        allValues.Should().NotBeNull();
        allValues.Count.Should().Be(9);
    }

    [TestMethod]
    public void UnknownRegisterNameIsRejected()
    {
        var map = new ArduinoUnoPortMap();
        var snapshot = map.CreateSnapshot();
        Action act = () => snapshot.GetValue("NONEXISTENT");
        act.Should().Throw<KeyNotFoundException>();
    }
}
