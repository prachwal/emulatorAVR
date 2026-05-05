using System.Collections.ObjectModel;
using EmulatorAVR.Avalonia.Models;
using EmulatorAVR.Core.Ports;

namespace EmulatorAVR.Avalonia.ViewModels;

public class DesignTimeMainWindowViewModel
{
    public string AppTitle { get; } = "emulatorAVR";
    public string AppSubtitle { get; } = "ATmega328P state viewer — static/demo snapshot";
    public string PcText { get; } = "0x0100";
    public string CycleCountText { get; } = "42";
    public string SregRawText { get; } = "0xA5";
    public string PortStatusText { get; } = "Static/demo port values — no live execution";
    public ObservableCollection<DisplayFlag> Flags { get; } = new()
    {
        new DisplayFlag("I", true),
        new DisplayFlag("T", false),
        new DisplayFlag("H", true),
        new DisplayFlag("S", false),
        new DisplayFlag("V", true),
        new DisplayFlag("N", false),
        new DisplayFlag("Z", false),
        new DisplayFlag("C", true),
    };
    public ObservableCollection<DisplayRegister> Registers { get; } = new();
    public ObservableCollection<DisplayPortRegister> Ports { get; } = new();
    public ObservableCollection<DisplayPinMapping> Pins { get; } = new();

    public DesignTimeMainWindowViewModel()
    {
        for (int i = 0; i < 32; i++)
            Registers.Add(new DisplayRegister($"R{i}", $"0x{i:X2}"));

        var portMap = new ArduinoUnoPortMap();

        Ports.Add(new DisplayPortRegister("PINB", $"0x{portMap.GetRegister("PINB").Read():X2}", "0x23"));
        Ports.Add(new DisplayPortRegister("DDRB", $"0x{portMap.GetRegister("DDRB").Read():X2}", "0x24"));
        Ports.Add(new DisplayPortRegister("PORTB", $"0x{portMap.GetRegister("PORTB").Read():X2}", "0x25"));
        Ports.Add(new DisplayPortRegister("PINC", $"0x{portMap.GetRegister("PINC").Read():X2}", "0x26"));
        Ports.Add(new DisplayPortRegister("DDRC", $"0x{portMap.GetRegister("DDRC").Read():X2}", "0x27"));
        Ports.Add(new DisplayPortRegister("PORTC", $"0x{portMap.GetRegister("PORTC").Read():X2}", "0x28"));
        Ports.Add(new DisplayPortRegister("PIND", $"0x{portMap.GetRegister("PIND").Read():X2}", "0x29"));
        Ports.Add(new DisplayPortRegister("DDRD", $"0x{portMap.GetRegister("DDRD").Read():X2}", "0x2A"));
        Ports.Add(new DisplayPortRegister("PORTD", $"0x{portMap.GetRegister("PORTD").Read():X2}", "0x2B"));

        for (int pin = 0; pin <= 13; pin++)
        {
            var mapping = portMap.GetPin(pin);
            Pins.Add(new DisplayPinMapping($"D{pin}", mapping.PortName, $"bit {mapping.BitIndex}"));
        }
    }
}
