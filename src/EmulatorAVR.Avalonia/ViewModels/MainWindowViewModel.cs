using System.Collections.ObjectModel;
using EmulatorAVR.Core.Cpu;
using EmulatorAVR.Core.Tracing;

namespace EmulatorAVR.Avalonia.ViewModels;

public class MainWindowViewModel
{
    public string PcText { get; }
    public string CycleCountText { get; }
    public string SregRawText { get; }
    public bool FlagI { get; }
    public bool FlagT { get; }
    public bool FlagH { get; }
    public bool FlagS { get; }
    public bool FlagV { get; }
    public bool FlagN { get; }
    public bool FlagZ { get; }
    public bool FlagC { get; }
    public ObservableCollection<RegisterItem> Registers { get; } = new();
    public ObservableCollection<PortPlaceholderItem> Ports { get; } = new();
    public string PortPlaceholderText { get; } = "Port behavior is not implemented yet in the UI viewer.";

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
        FlagI = snapshot.I;
        FlagT = snapshot.T;
        FlagH = snapshot.H;
        FlagS = snapshot.S;
        FlagV = snapshot.V;
        FlagN = snapshot.N;
        FlagZ = snapshot.Z;
        FlagC = snapshot.C;

        for (int i = 0; i < 32; i++)
            Registers.Add(new RegisterItem($"R{i}", snapshot.Registers[i]));

        var portNames = new[] { "PINB", "DDRB", "PORTB", "PINC", "DDRC", "PORTC", "PIND", "DDRD", "PORTD" };
        foreach (var name in portNames)
            Ports.Add(new PortPlaceholderItem(name));
    }
}

public class RegisterItem
{
    public string Name { get; }
    public string ValueText { get; }

    public RegisterItem(string name, byte value)
    {
        Name = name;
        ValueText = $"0x{value:X2}";
    }
}

public class PortPlaceholderItem
{
    public string Name { get; }

    public PortPlaceholderItem(string name)
    {
        Name = name;
    }
}
