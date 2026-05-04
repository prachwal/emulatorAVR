using System.Collections.Immutable;
using EmulatorAVR.Core.Ports;

namespace EmulatorAVR.Core.Memory;

public class IoRegisterMap
{
    private readonly Dictionary<ushort, PortRegister> _byAddress = new();
    private readonly Dictionary<string, PortRegister> _byName = new();

    public IReadOnlyCollection<PortRegister> Registers { get; }

    public IoRegisterMap(ArduinoUnoPortMap portMap)
    {
        var list = new List<PortRegister>();
        foreach (var kvp in portMap.Registers)
        {
            var reg = kvp.Value;
            _byAddress[reg.Address] = reg;
            _byName[reg.Name] = reg;
            list.Add(reg);
        }
        Registers = list.ToImmutableArray();
    }

    public byte Read(ushort address)
    {
        if (!_byAddress.TryGetValue(address, out var reg))
            throw new ArgumentOutOfRangeException(nameof(address), $"Unknown address: 0x{address:X4}");
        return reg.Read();
    }

    public void Write(ushort address, byte value)
    {
        if (!_byAddress.TryGetValue(address, out var reg))
            throw new ArgumentOutOfRangeException(nameof(address), $"Unknown address: 0x{address:X4}");
        reg.Write(value);
    }

    public PortRegister GetRegister(string name)
    {
        if (!_byName.TryGetValue(name, out var reg))
            throw new KeyNotFoundException($"Unknown register: {name}");
        return reg;
    }

    public bool ContainsAddress(ushort address) => _byAddress.ContainsKey(address);
}
