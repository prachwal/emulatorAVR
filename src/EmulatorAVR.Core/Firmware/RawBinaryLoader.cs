namespace EmulatorAVR.Core.Firmware;

public class RawBinaryLoader : IFirmwareLoader
{
    public FirmwareImage Load(byte[] data, uint baseAddress = 0)
    {
        ArgumentNullException.ThrowIfNull(data);
        return new FirmwareImage(baseAddress, data);
    }
}
