namespace EmulatorAVR.Core.Firmware;

public interface IFirmwareLoader
{
    FirmwareImage Load(byte[] data, uint baseAddress = 0);
}
