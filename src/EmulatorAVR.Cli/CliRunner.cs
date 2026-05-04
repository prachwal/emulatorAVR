using System.Text;
using EmulatorAVR.Core.Execution;
using EmulatorAVR.Core.Firmware;

namespace EmulatorAVR.Cli;

public class CliRunner
{
    public static (int exitCode, string stdout, string stderr) Run(string[] args)
    {
        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        int RunInner()
        {
            var parseResult = CliOptions.Parse(args);
            if (parseResult.options == null)
            {
                stderr.AppendLine(parseResult.error);
                return 1;
            }

            var cliOptions = parseResult.options;

            if (!File.Exists(cliOptions.FirmwarePath))
            {
                stderr.AppendLine($"File not found: {cliOptions.FirmwarePath}");
                return 1;
            }

            FirmwareImage firmware;

            try
            {
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
                    stderr.AppendLine($"Unsupported firmware format: {Path.GetExtension(cliOptions.FirmwarePath)}");
                    return 1;
                }
            }
            catch (Exception ex) when (ex is FormatException or InvalidOperationException or IOException or NotSupportedException)
            {
                stderr.AppendLine($"Loader error: {ex.Message}");
                return 1;
            }

            var runOptions = new RunOptions(
                cliOptions.MCU!,
                cliOptions.MaxCycles,
                cliOptions.TraceRegisters,
                cliOptions.TracePorts,
                firmware);

            var runLoop = new AvrRunLoop();
            var result = runLoop.Run(runOptions);

            stdout.AppendLine($"Stop: {result.Reason}");
            stdout.AppendLine($"PC: {result.FinalPC}");
            stdout.AppendLine($"Cycles: {result.FinalCycleCount}");

            if (result.ErrorMessage != null)
                stderr.AppendLine($"Error: {result.ErrorMessage}");

            if (result.TraceFrames.Count > 0)
            {
                stdout.AppendLine("Trace:");
                foreach (var frame in result.TraceFrames)
                {
                    stdout.AppendLine($"  Cycle {frame.CycleCount} PC 0x{frame.PC:X4} Opcode 0x{frame.OpcodeWord:X4}");

                    if (frame.ChangedRegisters.Count > 0)
                    {
                        foreach (var reg in frame.ChangedRegisters)
                            stdout.AppendLine($"    R{reg.RegisterIndex}: 0x{reg.OldValue:X2} -> 0x{reg.NewValue:X2}");
                    }

                    if (frame.ChangedPorts.Count > 0)
                    {
                        foreach (var port in frame.ChangedPorts)
                            stdout.AppendLine($"    {port.PortName}: 0x{port.OldValue:X2} -> 0x{port.NewValue:X2}");
                    }
                }
            }

            return result.Reason == StopReason.Error ? 1 : 0;
        }

        int exitCode = RunInner();
        return (exitCode, stdout.ToString().TrimEnd(), stderr.ToString().TrimEnd());
    }
}
