namespace EmulatorAVR.Core.Ports;

public sealed record ArduinoPinMapping(int DigitalPin, string PortName, int BitIndex);
