using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Cpu;
using EmulatorAVR.Core.Execution;
using EmulatorAVR.Core.Firmware;
using EmulatorAVR.Core.Memory;
using EmulatorAVR.Core.Tracing;

namespace EmulatorAVR.Core.Tests.Execution;

[TestClass]
public class AvrRunLoopTests
{
    private static ProgramMemory CreateProgramMemory(params ushort[] opcodes)
    {
        var mem = new ProgramMemory(32768);
        for (int i = 0; i < opcodes.Length; i++)
            mem[i] = opcodes[i];
        return mem;
    }

    private static FirmwareImage FirmwareFromOpcodes(params ushort[] opcodes)
    {
        var bytes = new byte[opcodes.Length * 2];
        for (int i = 0; i < opcodes.Length; i++)
        {
            bytes[i * 2] = (byte)(opcodes[i] & 0xFF);
            bytes[i * 2 + 1] = (byte)((opcodes[i] >> 8) & 0xFF);
        }
        return new FirmwareImage(0, bytes);
    }

    [TestMethod]
    public void StopsAtMaxCycles()
    {
        var firmware = FirmwareFromOpcodes(0x0000, 0x0000, 0x0000, 0x0000, 0x0000);
        var options = new RunOptions("atmega328p", 3, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.MaxCycles);
        result.FinalCycleCount.Should().Be(3);
    }

    [TestMethod]
    public void StopsOnUnsupportedOpcode()
    {
        var firmware = FirmwareFromOpcodes(0xFFFF);
        var options = new RunOptions("atmega328p", 100, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.UnsupportedInstruction);
        result.FinalCycleCount.Should().Be(0);
    }

    [TestMethod]
    public void StopsAtProgramEnd()
    {
        var firmware = FirmwareFromOpcodes(0x0000, 0x0000);
        var options = new RunOptions("atmega328p", 100, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.ProgramEnd);
        result.FinalCycleCount.Should().Be(2);
        result.FinalPC.Should().Be(2u);
    }

    [TestMethod]
    public void RegisterDiffTraceAfterLdi()
    {
        var firmware = FirmwareFromOpcodes(0xE402);
        var options = new RunOptions("atmega328p", 10, true, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.TraceFrames.Should().NotBeEmpty();
        var frame = result.TraceFrames[0];
        frame.ChangedRegisters.Should().ContainSingle(r => r.RegisterIndex == 16);
        frame.ChangedRegisters[0].OldValue.Should().Be(0);
        frame.ChangedRegisters[0].NewValue.Should().Be(0x42);
    }

    [TestMethod]
    public void PortTraceIsEmptyPlaceholder()
    {
        var firmware = FirmwareFromOpcodes(0x0000);
        var options = new RunOptions("atmega328p", 1, true, true, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.TraceFrames.Should().NotBeEmpty();
        foreach (var frame in result.TraceFrames)
            frame.ChangedPorts.Should().BeEmpty();
    }

    [TestMethod]
    public void NoWallClockDependency()
    {
        var firmware = FirmwareFromOpcodes(0x0000, 0x0000, 0x0000, 0x0000);
        var options = new RunOptions("atmega328p", 4, false, false, firmware);
        var loop = new AvrRunLoop();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = loop.Run(options);
        sw.Stop();
        result.Reason.Should().Be(StopReason.MaxCycles);
        result.FinalCycleCount.Should().Be(4);
    }
}
