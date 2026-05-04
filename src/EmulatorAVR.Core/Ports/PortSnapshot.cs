using System.Collections.Immutable;

namespace EmulatorAVR.Core.Ports;

public sealed class PortSnapshot
{
    private readonly ImmutableDictionary<string, byte> _values;

    internal PortSnapshot(ImmutableDictionary<string, byte> values)
    {
        _values = values;
    }

    public byte GetValue(string registerName)
    {
        if (!_values.TryGetValue(registerName, out byte value))
            throw new KeyNotFoundException($"Unknown register: {registerName}");
        return value;
    }

    public IReadOnlyDictionary<string, byte> GetAllValues() => _values;
}
