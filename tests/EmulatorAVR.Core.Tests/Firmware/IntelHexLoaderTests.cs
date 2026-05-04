using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using EmulatorAVR.Core.Firmware;

namespace EmulatorAVR.Core.Tests.Firmware;

[TestClass]
public class IntelHexLoaderTests
{
    private readonly IntelHexLoader _loader = new();

    [TestMethod]
    public void ValidHexWithOneDataRecordAndEOF()
    {
        var hex = ":03000000010203F7\n"
                + ":00000001FF\n";
        var image = _loader.LoadText(hex);
        image.Length.Should().Be(3);
        var bytes = image.ToArray();
        bytes[0].Should().Be(0x01);
        bytes[1].Should().Be(0x02);
        bytes[2].Should().Be(0x03);
    }

    [TestMethod]
    public void LoadedBytesMatchExpectedBytes()
    {
        var hex = ":06001000AABBCCDDEEFFEF\n"
                + ":00000001FF\n";
        var image = _loader.LoadText(hex);
        image.Length.Should().Be(6);
        var bytes = image.ToArray();
        bytes[0].Should().Be(0xAA);
        bytes[1].Should().Be(0xBB);
        bytes[2].Should().Be(0xCC);
        bytes[3].Should().Be(0xDD);
        bytes[4].Should().Be(0xEE);
        bytes[5].Should().Be(0xFF);
    }

    [TestMethod]
    public void BaseAddressIsCorrect()
    {
        var hex = ":0312340002065E51\n"
                + ":00000001FF\n";
        var image = _loader.LoadText(hex);
        image.BaseAddress.Should().Be(0x1234u);
        image.Length.Should().Be(3);
    }

    [TestMethod]
    public void InvalidChecksumIsRejected()
    {
        var hex = ":0300000001020300\n"
                + ":00000001FF\n";
        Action act = () => _loader.LoadText(hex);
        act.Should().Throw<FormatException>();
    }

    [TestMethod]
    public void MalformedLineIsRejected()
    {
        var hex = "not a hex line\n"
                + ":00000001FF\n";
        Action act = () => _loader.LoadText(hex);
        act.Should().Throw<FormatException>();
    }

    [TestMethod]
    public void InvalidHexCharactersAreRejected()
    {
        var hex = ":03000000ZZ06ZZZZ\n"
                + ":00000001FF\n";
        Action act = () => _loader.LoadText(hex);
        act.Should().Throw<FormatException>();
    }

    [TestMethod]
    public void UnsupportedRecordTypeIsRejected()
    {
        var hex = ":00000002FE\n"
                + ":00000001FF\n";
        Action act = () => _loader.LoadText(hex);
        act.Should().Throw<NotSupportedException>();
    }

    [TestMethod]
    public void MissingEOFIsRejected()
    {
        var hex = ":03000000010203F7\n";
        Action act = () => _loader.LoadText(hex);
        act.Should().Throw<FormatException>();
    }
}
