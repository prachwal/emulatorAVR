namespace EmulatorAVR.Core.Firmware;

public class IntelHexLoader : IFirmwareLoader
{
    public FirmwareImage Load(byte[] data, uint baseAddress = 0)
    {
        var text = System.Text.Encoding.ASCII.GetString(data);
        return LoadText(text);
    }

    public FirmwareImage LoadText(string hexText)
    {
        ArgumentNullException.ThrowIfNull(hexText);

        var lines = hexText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
            throw new FormatException("HEX file is empty.");

        uint baseAddr = 0;
        var bytes = new List<byte>();
        bool eofFound = false;

        foreach (var line in lines)
        {
            if (line.Length < 11)
                throw new FormatException($"Line too short: {line}");

            if (line[0] != ':')
                throw new FormatException($"Line does not start with ':': {line}");

            int byteCount = ParseHexByte(line, 1);
            int address = (ParseHexByte(line, 3) << 8) | ParseHexByte(line, 5);
            int recordType = ParseHexByte(line, 7);

            int dataLength = byteCount * 2;
            int expectedLineLength = 1 + 2 + 4 + 2 + dataLength + 2;
            if (line.Length != expectedLineLength)
                throw new FormatException($"Invalid line length for record type {recordType}: {line}");

            if (recordType == 0x01)
            {
                eofFound = true;
                continue;
            }

            if (recordType != 0x00)
                throw new NotSupportedException($"Unsupported record type {recordType}: {line}");

            var dataBytes = new List<byte>();
            int sum = byteCount + (address >> 8) + (address & 0xFF) + recordType;
            for (int i = 0; i < byteCount; i++)
            {
                int b = ParseHexByte(line, 9 + i * 2);
                sum += b;
                dataBytes.Add((byte)b);
            }

            int checksum = ParseHexByte(line, 9 + dataLength);
            sum = (sum + checksum) & 0xFF;
            if (sum != 0)
                throw new FormatException($"Invalid checksum at line: {line}");

            if (bytes.Count == 0)
                baseAddr = (uint)address;

            bytes.AddRange(dataBytes);
        }

        if (!eofFound)
            throw new FormatException("Missing EOF record (type 01).");

        return new FirmwareImage(baseAddr, bytes.ToArray());
    }

    private static int ParseHexByte(string line, int startIndex)
    {
        if (startIndex + 2 > line.Length)
            throw new FormatException($"Unexpected end of line at position {startIndex}.");

        int high = HexCharToNibble(line[startIndex]);
        int low = HexCharToNibble(line[startIndex + 1]);
        return (high << 4) | low;
    }

    private static int HexCharToNibble(char c)
    {
        if (c >= '0' && c <= '9') return c - '0';
        if (c >= 'A' && c <= 'F') return c - 'A' + 10;
        if (c >= 'a' && c <= 'f') return c - 'a' + 10;
        throw new FormatException($"Invalid hex character: {c}");
    }
}
