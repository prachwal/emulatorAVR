using System.Collections.Immutable;

namespace EmulatorAVR.Core.Ports;

public sealed class ArduinoUnoPortMap
{
    private readonly Dictionary<string, PortRegister> _registers = new();
    private readonly Dictionary<int, ArduinoPinMapping> _pins = new();

    public IReadOnlyDictionary<string, PortRegister> Registers => _registers;

    public ArduinoUnoPortMap()
    {
        AddRegister("PINB", 0x23);
        AddRegister("DDRB", 0x24);
        AddRegister("PORTB", 0x25);
        AddRegister("PINC", 0x26);
        AddRegister("DDRC", 0x27);
        AddRegister("PORTC", 0x28);
        AddRegister("PIND", 0x29);
        AddRegister("DDRD", 0x2A);
        AddRegister("PORTD", 0x2B);

        AddPin(0, "PORTD", 0);
        AddPin(1, "PORTD", 1);
        AddPin(2, "PORTD", 2);
        AddPin(3, "PORTD", 3);
        AddPin(4, "PORTD", 4);
        AddPin(5, "PORTD", 5);
        AddPin(6, "PORTD", 6);
        AddPin(7, "PORTD", 7);
        AddPin(8, "PORTB", 0);
        AddPin(9, "PORTB", 1);
        AddPin(10, "PORTB", 2);
        AddPin(11, "PORTB", 3);
        AddPin(12, "PORTB", 4);
        AddPin(13, "PORTB", 5);
    }

    private void AddRegister(string name, ushort address)
    {
        _registers[name] = new PortRegister(name, address);
    }

    private void AddPin(int digitalPin, string portName, int bitIndex)
    {
        _pins[digitalPin] = new ArduinoPinMapping(digitalPin, portName, bitIndex);
    }

    public PortRegister GetRegister(string name)
    {
        if (!_registers.TryGetValue(name, out var register))
            throw new KeyNotFoundException($"Unknown register: {name}");
        return register;
    }

    public ArduinoPinMapping GetPin(int digitalPin)
    {
        if (!_pins.TryGetValue(digitalPin, out var pin))
            throw new KeyNotFoundException($"Unsupported pin: {digitalPin}");
        return pin;
    }

    public PortSnapshot CreateSnapshot()
    {
        var builder = ImmutableDictionary.CreateBuilder<string, byte>();
        foreach (var kvp in _registers)
            builder[kvp.Key] = kvp.Value.Value;
        return new PortSnapshot(builder.ToImmutable());
    }
}
