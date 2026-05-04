using EmulatorAVR.Core.Execution;
using EmulatorAVR.Core.Firmware;

using EmulatorAVR.Cli;

var parseResult = CliOptions.Parse(args);
if (parseResult.options == null)
{
    Console.Error.WriteLine(parseResult.error);
    Environment.Exit(1);
    return;
}

var cliOptions = parseResult.options;

if (!File.Exists(cliOptions.FirmwarePath))
{
    Console.Error.WriteLine($"File not found: {cliOptions.FirmwarePath}");
    Environment.Exit(1);
    return;
}

FirmwareImage firmware;

if (cliOptions.FirmwarePath.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
{
    var rawLoader = new RawBinaryLoader();
    firmware = rawLoader.Load(File.ReadAllBytes(cliOptions.FirmwarePath));
}
else if (cliOptions.FirmwarePath.EndsWith(".hex", StringComparison.OrdinalIgnoreCase))
{
    var hexLoader = new IntelHexLoader();
    firmware = hexLoader.Load(File.ReadAllBytes(cliOptions.FirmwarePath));
}
else
{
    Console.Error.WriteLine($"Unsupported firmware format: {Path.GetExtension(cliOptions.FirmwarePath)}");
    Environment.Exit(1);
    return;
}

var runOptions = new RunOptions(
    cliOptions.MCU!,
    cliOptions.MaxCycles,
    cliOptions.TraceRegisters,
    cliOptions.TracePorts,
    firmware);

var runLoop = new AvrRunLoop();
var result = runLoop.Run(runOptions);

Console.WriteLine($"Stop: {result.Reason}");
Console.WriteLine($"PC: {result.FinalPC}");
Console.WriteLine($"Cycles: {result.FinalCycleCount}");

if (result.ErrorMessage != null)
    Console.Error.WriteLine($"Error: {result.ErrorMessage}");

if (result.TraceFrames.Count > 0)
{
    Console.WriteLine("Trace:");
    foreach (var frame in result.TraceFrames)
    {
        Console.WriteLine($"  Cycle {frame.CycleCount} PC 0x{frame.PC:X4} Opcode 0x{frame.OpcodeWord:X4}");

        if (frame.ChangedRegisters.Count > 0)
        {
            foreach (var reg in frame.ChangedRegisters)
                Console.WriteLine($"    R{reg.RegisterIndex}: 0x{reg.OldValue:X2} -> 0x{reg.NewValue:X2}");
        }

        if (frame.ChangedPorts.Count > 0)
        {
            foreach (var port in frame.ChangedPorts)
                Console.WriteLine($"    {port.PortName}: 0x{port.OldValue:X2} -> 0x{port.NewValue:X2}");
        }
    }
}

if (result.Reason == StopReason.Error)
    Environment.Exit(1);
