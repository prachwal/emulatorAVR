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
        var (exitCode, _, stderr) = CliRunner.Run(new[] { "run", "--mcu", "atmega328p", "--firmware", "nonexistent.hex", "--max-cycles", "1000" });
        exitCode.Should().Be(1);
        stderr.Should().Contain("File not found");
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

    [TestMethod]
    public void UnsupportedExtensionIsRejected()
    {
        var path = Path.GetTempFileName() + ".xyz";
        try
        {
            File.WriteAllBytes(path, new byte[] { 0x00 });
            var (exitCode, _, stderr) = CliRunner.Run(new[] { "run", "--mcu", "atmega328p", "--firmware", path, "--max-cycles", "1000" });
            exitCode.Should().Be(1);
            stderr.Should().Contain("Unsupported firmware format");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void HexLoaderIsSelectedByHexExtension()
    {
        var path = Path.GetTempFileName() + ".hex";
        try
        {
            var hex = ":03000000010203F7\n:00000001FF\n";
            File.WriteAllText(path, hex);
            var (exitCode, stdout, _) = CliRunner.Run(new[] { "run", "--mcu", "atmega328p", "--firmware", path, "--max-cycles", "10" });
            exitCode.Should().Be(0);
            stdout.Should().Contain("Stop:");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void BinLoaderIsSelectedByBinExtension()
    {
        var path = Path.GetTempFileName() + ".bin";
        try
        {
            var bytes = new byte[] { 0x00, 0x00 };
            File.WriteAllBytes(path, bytes);
            var (exitCode, stdout, _) = CliRunner.Run(new[] { "run", "--mcu", "atmega328p", "--firmware", path, "--max-cycles", "10" });
            exitCode.Should().Be(0);
            stdout.Should().Contain("Stop:");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void InvalidHexLoaderErrorIsHandled()
    {
        var path = Path.GetTempFileName() + ".hex";
        try
        {
            File.WriteAllBytes(path, new byte[] { 0xFF, 0xFF, 0xFF });
            var (exitCode, _, stderr) = CliRunner.Run(new[] { "run", "--mcu", "atmega328p", "--firmware", path, "--max-cycles", "10" });
            exitCode.Should().Be(1);
            stderr.Should().Contain("Loader error");
        }
        finally
        {
            File.Delete(path);
        }
    }
}
