namespace EmulatorAVR.Cli;

public class CliOptions
{
    public string? MCU { get; set; }
    public ulong MaxCycles { get; set; }
    public bool TraceRegisters { get; set; }
    public bool TracePorts { get; set; }
    public string? FirmwarePath { get; set; }

    public static (CliOptions? options, string? error) Parse(string[] args)
    {
        if (args.Length < 1 || args[0] != "run")
            return (null, "Expected 'run' command.");

        var options = new CliOptions();
        bool maxCyclesProvided = false;

        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--mcu":
                    if (++i >= args.Length)
                        return (null, "Missing value for --mcu.");
                    if (args[i] != "atmega328p")
                        return (null, $"Unsupported MCU: {args[i]}");
                    options.MCU = args[i];
                    break;

                case "--firmware":
                    if (++i >= args.Length)
                        return (null, "Missing value for --firmware.");
                    options.FirmwarePath = args[i];
                    break;

                case "--max-cycles":
                    if (++i >= args.Length)
                        return (null, "Missing value for --max-cycles.");
                    if (!ulong.TryParse(args[i], out ulong cycles) || cycles == 0)
                        return (null, $"Invalid --max-cycles: {args[i]}");
                    options.MaxCycles = cycles;
                    maxCyclesProvided = true;
                    break;

                case "--trace":
                    if (++i >= args.Length)
                        return (null, "Missing value for --trace.");
                    var flags = args[i].Split(',');
                    foreach (var flag in flags)
                    {
                        if (flag == "registers") options.TraceRegisters = true;
                        else if (flag == "ports") options.TracePorts = true;
                        else return (null, $"Unknown trace flag: {flag}");
                    }
                    break;

                default:
                    return (null, $"Unknown argument: {args[i]}");
            }
        }

        if (!maxCyclesProvided)
            return (null, "--max-cycles is required.");

        if (string.IsNullOrEmpty(options.FirmwarePath))
            return (null, "--firmware is required.");

        if (string.IsNullOrEmpty(options.MCU))
            options.MCU = "atmega328p";

        return (options, null);
    }
}
