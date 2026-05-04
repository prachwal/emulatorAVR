using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Firmware;

namespace EmulatorAVR.Core.Tests.Firmware;

[TestClass]
public class RawBinaryLoaderTests
{
    private readonly RawBinaryLoader _loader = new();

    [TestMethod]
    public void MapsBytesToRequestedBaseAddress()
    {
        var input = new byte[] { 0x01, 0x02, 0x03 };
        var image = _loader.Load(input, 0x8000);
        image.BaseAddress.Should().Be(0x8000u);
    }

    [TestMethod]
    public void DefensivelyCopiesInputByteArray()
    {
        var input = new byte[] { 0xAB, 0xCD };
        var image = _loader.Load(input, 0);
        input[0] = 0xFF;
        image.ToArray()[0].Should().Be(0xAB);
    }

    [TestMethod]
    public void RejectsNullInput()
    {
        Action act = () => _loader.Load(null!, 0);
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void PreservesLengthAndData()
    {
        var input = new byte[] { 0x10, 0x20, 0x30, 0x40 };
        var image = _loader.Load(input, 0);
        image.Length.Should().Be(4);
        var bytes = image.ToArray();
        bytes[0].Should().Be(0x10);
        bytes[1].Should().Be(0x20);
        bytes[2].Should().Be(0x30);
        bytes[3].Should().Be(0x40);
    }

    [TestMethod]
    public void HandlesEmptyInput()
    {
        var input = Array.Empty<byte>();
        var image = _loader.Load(input, 0);
        image.Length.Should().Be(0);
        image.ToArray().Should().BeEmpty();
    }
}
