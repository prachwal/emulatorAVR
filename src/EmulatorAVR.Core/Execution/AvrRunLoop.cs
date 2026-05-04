using EmulatorAVR.Core.Cpu;
using EmulatorAVR.Core.Firmware;
using EmulatorAVR.Core.Instructions;
using EmulatorAVR.Core.Memory;
using EmulatorAVR.Core.Tracing;

namespace EmulatorAVR.Core.Execution;

public class AvrRunLoop
{
    private readonly InstructionDecoder _decoder = new();
    private readonly InstructionExecutor _executor = new();
    private int _loadedWordCount;

    public RunResult Run(RunOptions options)
    {
        if (options.Firmware == null)
            return new RunResult(StopReason.Error, 0, 0, Array.Empty<TraceFrame>(), "No firmware loaded.");

        var state = new AvrCpuState();
        var programMemory = LoadFirmware(options.Firmware);
        state.ProgramCounter = options.Firmware.BaseAddress / 2;

        var traces = new List<TraceFrame>();
        var previousRegisterSnapshot = state.Registers.Snapshot();

        while (true)
        {
            if (state.CycleCount >= options.MaxCycles)
                return new RunResult(StopReason.MaxCycles, state.ProgramCounter, state.CycleCount, traces);

            if (state.ProgramCounter >= (uint)_loadedWordCount)
                return new RunResult(StopReason.ProgramEnd, state.ProgramCounter, state.CycleCount, traces);

            ushort opcode;
            try
            {
                opcode = programMemory[(int)state.ProgramCounter];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new RunResult(StopReason.ProgramEnd, state.ProgramCounter, state.CycleCount, traces);
            }

            var instruction = _decoder.Decode(opcode);

            if (instruction.Kind == InstructionKind.Unsupported)
                return new RunResult(StopReason.UnsupportedInstruction, state.ProgramCounter, state.CycleCount, traces);

            _executor.Execute(state, instruction);

            if (options.TraceRegisters || options.TracePorts)
            {
                var currentSnapshot = state.Registers.Snapshot();
                var changedRegisters = new List<RegisterTraceEntry>();

                for (int i = 0; i < 32; i++)
                {
                    if (previousRegisterSnapshot[i] != currentSnapshot[i])
                    {
                        changedRegisters.Add(new RegisterTraceEntry(i, previousRegisterSnapshot[i], currentSnapshot[i]));
                    }
                }

                var changedPorts = new List<PortTraceEntry>();

                var frame = new TraceFrame(
                    state.CycleCount,
                    state.ProgramCounter,
                    opcode,
                    changedRegisters,
                    changedPorts);

                traces.Add(frame);
                previousRegisterSnapshot = currentSnapshot;
            }
        }
    }

    private ProgramMemory LoadFirmware(FirmwareImage firmware)
    {
        int wordCapacity = 32768;
        var programMemory = new ProgramMemory(wordCapacity);
        var bytes = firmware.ToArray();
        int wordAddress = 0;

        for (int i = 0; i < bytes.Length; i += 2)
        {
            ushort opcode = (ushort)(bytes[i] | (i + 1 < bytes.Length ? bytes[i + 1] << 8 : 0));
            programMemory[wordAddress++] = opcode;
        }

        _loadedWordCount = wordAddress;
        return programMemory;
    }
}
