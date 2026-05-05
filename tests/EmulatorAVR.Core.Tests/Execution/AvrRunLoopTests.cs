using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Cpu;
using EmulatorAVR.Core.Execution;
using EmulatorAVR.Core.Firmware;
using EmulatorAVR.Core.Memory;

namespace EmulatorAVR.Core.Tests.Execution;

[TestClass]
public class AvrRunLoopTests
{
    private static FirmwareImage FirmwareFromOpcodes(ushort baseAddressWords, params ushort[] opcodes)
    {
        uint baseAddress = (uint)(baseAddressWords * 2);
        var bytes = new byte[opcodes.Length * 2];
        for (int i = 0; i < opcodes.Length; i++)
        {
            bytes[i * 2] = (byte)(opcodes[i] & 0xFF);
            bytes[i * 2 + 1] = (byte)((opcodes[i] >> 8) & 0xFF);
        }
        return new FirmwareImage(baseAddress, bytes);
    }

    [TestMethod]
    public void StopsAtMaxCycles()
    {
        var firmware = FirmwareFromOpcodes(0, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000);
        var options = new RunOptions("atmega328p", 3, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.MaxCycles);
        result.FinalCycleCount.Should().Be(3);
    }

    [TestMethod]
    public void StopsOnUnsupportedOpcode()
    {
        var firmware = FirmwareFromOpcodes(0, 0xFFFF);
        var options = new RunOptions("atmega328p", 100, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.UnsupportedInstruction);
        result.FinalCycleCount.Should().Be(0);
    }

    [TestMethod]
    public void UnsupportedOpcodeDoesNotMutateState()
    {
        var firmware = FirmwareFromOpcodes(0, 0xFFFF);
        var options = new RunOptions("atmega328p", 100, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);

        result.Reason.Should().Be(StopReason.UnsupportedInstruction);
        result.FinalPC.Should().Be(0u);
        result.FinalCycleCount.Should().Be(0u);
        result.State.Should().NotBeNull();
        result.State!.Registers[0].Should().Be(0);
        result.State.Registers[31].Should().Be(0);
        result.State.SREG.Value.Should().Be(0);
    }

    [TestMethod]
    public void StopsAtProgramEnd()
    {
        var firmware = FirmwareFromOpcodes(0, 0x0000, 0x0000);
        var options = new RunOptions("atmega328p", 100, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.ProgramEnd);
        result.FinalCycleCount.Should().Be(2);
        result.FinalPC.Should().Be(2u);
    }

    [TestMethod]
    public void NonZeroFirmwareBaseAddressLoadsCorrectly()
    {
        var firmware = FirmwareFromOpcodes(4, 0x0000, 0x0000);
        var options = new RunOptions("atmega328p", 100, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.ProgramEnd);
        result.FinalCycleCount.Should().Be(2);
        result.FinalPC.Should().Be(6u);
    }

    [TestMethod]
    public void OddFirmwareByteCountIsPadded()
    {
        var bytes = new byte[] { 0x02, 0xE4, 0x00 };
        var firmware = new FirmwareImage(0, bytes);
        var options = new RunOptions("atmega328p", 2, true, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.MaxCycles);
        result.FinalCycleCount.Should().Be(2);
        result.FinalPC.Should().Be(2u);
        result.TraceFrames.Should().HaveCount(2);
        result.TraceFrames[0].OpcodeWord.Should().Be(0xE402);
        result.TraceFrames[1].OpcodeWord.Should().Be(0x0000);
        result.TraceFrames[1].ChangedRegisters.Should().BeEmpty();
    }

    [TestMethod]
    public void RegisterDiffTraceAfterLdi()
    {
        var firmware = FirmwareFromOpcodes(0, 0xE402);
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
    public void RegisterTraceDisabled_ProducesNoRegisterEntries()
    {
        var firmware = FirmwareFromOpcodes(0, 0xE402);
        var options = new RunOptions("atmega328p", 10, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.TraceFrames.Should().BeEmpty();
    }

    [TestMethod]
    public void PortTraceIsEmptyPlaceholder()
    {
        var firmware = FirmwareFromOpcodes(0, 0x0000);
        var options = new RunOptions("atmega328p", 1, true, true, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.TraceFrames.Should().NotBeEmpty();
        foreach (var frame in result.TraceFrames)
            frame.ChangedPorts.Should().BeEmpty();
    }

    [TestMethod]
    public void BlinkLikeFirmware_SetsDdrbAndTogglesPortb()
    {
        // LDI R16,0x20 | OUT DDRB,R16 | SBI PORTB,5 | CBI PORTB,5 | RJMP -3
        var firmware = FirmwareFromOpcodes(0, 0xE200, 0xB904, 0x9A2D, 0x982D, 0xCFFD);
        var options = new RunOptions("atmega328p", 15, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.MaxCycles);
        // After execution, R16 should be 0x20 (LDI), DDRB (0x24) should be 0x20
        result.State.Should().NotBeNull();
        result.State!.DataMemory[0x24].Should().Be(0x20);
        // After SBI then CBI, PORTB bit 5 is set (SBI was last at cycle 15)
        result.State.DataMemory[0x25].Should().Be(0x20);
    }

    [TestMethod]
    public void NoWallClockDependency()
    {
        var firmware = FirmwareFromOpcodes(0, 0x0000, 0x0000, 0x0000, 0x0000);
        var options = new RunOptions("atmega328p", 4, false, false, firmware);
        var loop = new AvrRunLoop();
        var result = loop.Run(options);
        result.Reason.Should().Be(StopReason.MaxCycles);
        result.FinalCycleCount.Should().Be(4);
    }
}
