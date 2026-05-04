using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Tracing;

namespace EmulatorAVR.Core.Tests.Tracing;

[TestClass]
public class TraceFrameTests
{
    [TestMethod]
    public void RegisterTraceEntryStoresValues()
    {
        var entry = new RegisterTraceEntry(5, 0x00, 0xFF);
        entry.RegisterIndex.Should().Be(5);
        entry.OldValue.Should().Be(0x00);
        entry.NewValue.Should().Be(0xFF);
    }

    [TestMethod]
    public void PortTraceEntryStoresValues()
    {
        var entry = new PortTraceEntry("PORTB", 0x00, 0x01);
        entry.PortName.Should().Be("PORTB");
        entry.OldValue.Should().Be(0x00);
        entry.NewValue.Should().Be(0x01);
    }

    [TestMethod]
    public void TraceFrameStoresCyclePcAndOpcode()
    {
        var frame = new TraceFrame(42, 0x100, 0xABCD, Array.Empty<RegisterTraceEntry>(), Array.Empty<PortTraceEntry>());
        frame.CycleCount.Should().Be(42u);
        frame.PC.Should().Be(0x100u);
        frame.OpcodeWord.Should().Be(0xABCD);
    }

    [TestMethod]
    public void TraceFrameChangedRegistersAreEmptyByDefault()
    {
        var frame = new TraceFrame(0, 0, 0, Array.Empty<RegisterTraceEntry>(), Array.Empty<PortTraceEntry>());
        frame.ChangedRegisters.Should().BeEmpty();
        frame.ChangedPorts.Should().BeEmpty();
    }
}
