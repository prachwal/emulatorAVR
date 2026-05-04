using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Cli;

namespace EmulatorAVR.Cli.Tests;

[TestClass]
public class CliRunCommandTests
{
    [TestMethod]
    public void MissingFirmwareIsRejected()
    {
        var (options, error) = CliOptions.Parse(new[] { "run", "--mcu", "atmega328p", "--max-cycles", "1000" });
        options.Should().BeNull();
        error.Should().Contain("--firmware");
    }

    [TestMethod]
    public void NonExistingFirmwareIsRejected()
    {
        var (options, _) = CliOptions.Parse(new[] { "run", "--mcu", "atmega328p", "--firmware", "nonexistent.hex", "--max-cycles", "1000" });
        options.Should().NotBeNull();
        options!.FirmwarePath.Should().Be("nonexistent.hex");
    }

    [TestMethod]
    public void UnsupportedMcuIsRejected()
    {
        var (options, error) = CliOptions.Parse(new[] { "run", "--mcu", "attiny85", "--firmware", "test.hex", "--max-cycles", "1000" });
        options.Should().BeNull();
        error.Should().Contain("attiny85");
    }

    [TestMethod]
    public void ValidOptionShapeIsAccepted()
    {
        var (options, error) = CliOptions.Parse(new[] { "run", "--mcu", "atmega328p", "--firmware", "blink.hex", "--max-cycles", "100000", "--trace", "registers,ports" });
        error.Should().BeNull();
        options.Should().NotBeNull();
        options!.MCU.Should().Be("atmega328p");
        options.FirmwarePath.Should().Be("blink.hex");
        options.MaxCycles.Should().Be(100000u);
        options.TraceRegisters.Should().BeTrue();
        options.TracePorts.Should().BeTrue();
    }

    [TestMethod]
    public void MissingMaxCyclesIsRejected()
    {
        var (options, error) = CliOptions.Parse(new[] { "run", "--mcu", "atmega328p", "--firmware", "test.hex" });
        options.Should().BeNull();
        error.Should().Contain("--max-cycles");
    }

    [TestMethod]
    public void InvalidMaxCyclesIsRejected()
    {
        var (options, error) = CliOptions.Parse(new[] { "run", "--mcu", "atmega328p", "--firmware", "test.hex", "--max-cycles", "abc" });
        options.Should().BeNull();
        error.Should().Contain("--max-cycles");
    }

    [TestMethod]
    public void ZeroMaxCyclesIsRejected()
    {
        var (options, error) = CliOptions.Parse(new[] { "run", "--mcu", "atmega328p", "--firmware", "test.hex", "--max-cycles", "0" });
        options.Should().BeNull();
        error.Should().Contain("--max-cycles");
    }

    [TestMethod]
    public void UnknownTraceFlagIsRejected()
    {
        var (options, error) = CliOptions.Parse(new[] { "run", "--mcu", "atmega328p", "--firmware", "test.hex", "--max-cycles", "1000", "--trace", "abc" });
        options.Should().BeNull();
        error.Should().Contain("abc");
    }
}
