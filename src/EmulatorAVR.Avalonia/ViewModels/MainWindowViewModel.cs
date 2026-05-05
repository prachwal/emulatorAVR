using System.Collections.ObjectModel;
using EmulatorAVR.Avalonia.Models;
using EmulatorAVR.Core.Cpu;
using EmulatorAVR.Core.Ports;
using EmulatorAVR.Core.Tracing;

namespace EmulatorAVR.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string AppTitle { get; } = "emulatorAVR";
    public string AppSubtitle { get; } = "ATmega328P state viewer — static/demo snapshot";
    public string PcText { get; }
    public string CycleCountText { get; }
    public string SregRawText { get; }
    public string PortStatusText { get; } = "Static/demo port values — no live execution";
    public ObservableCollection<DisplayFlag> Flags { get; } = new();
    public ObservableCollection<DisplayRegister> Registers { get; } = new();
    public ObservableCollection<DisplayPortRegister> Ports { get; } = new();
    public ObservableCollection<DisplayPinMapping> Pins { get; } = new();

    public MainWindowViewModel()
    {
        var state = new AvrCpuState();
        state.ProgramCounter = 0x0100;
        state.AddCycles(42);
        state.SREG.Value = 0xA5;

        var snapshot = new EmulatorStateSnapshot(state);

        PcText = $"0x{snapshot.ProgramCounter:X4}";
        CycleCountText = snapshot.CycleCount.ToString();
        SregRawText = $"0x{snapshot.SregRawValue:X2}";

        Flags.Add(new DisplayFlag("I", snapshot.I));
        Flags.Add(new DisplayFlag("T", snapshot.T));
        Flags.Add(new DisplayFlag("H", snapshot.H));
        Flags.Add(new DisplayFlag("S", snapshot.S));
        Flags.Add(new DisplayFlag("V", snapshot.V));
        Flags.Add(new DisplayFlag("N", snapshot.N));
        Flags.Add(new DisplayFlag("Z", snapshot.Z));
        Flags.Add(new DisplayFlag("C", snapshot.C));

        for (int i = 0; i < 32; i++)
            Registers.Add(new DisplayRegister($"R{i}", $"0x{snapshot.Registers[i]:X2}"));

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
