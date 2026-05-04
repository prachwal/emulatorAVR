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

    public RunResult Run(RunOptions options)
    {
        if (options.Firmware == null)
            return new RunResult(StopReason.Error, 0, 0, Array.Empty<TraceFrame>(), "No firmware loaded.");

        var state = new AvrCpuState();
        var (programMemory, startWordAddress, loadedWordCount) = LoadFirmware(options.Firmware);
        state.ProgramCounter = (uint)startWordAddress;

        var traces = new List<TraceFrame>();
        var previousRegisterSnapshot = state.Registers.Snapshot();

        while (true)
        {
            if (state.CycleCount >= options.MaxCycles)
                return new RunResult(StopReason.MaxCycles, state.ProgramCounter, state.CycleCount, traces);

            if (state.ProgramCounter >= (uint)(startWordAddress + loadedWordCount))
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

            var recordRegisterTrace = options.TraceRegisters;
            var recordPortTrace = options.TracePorts;

            if (recordRegisterTrace || recordPortTrace)
            {
                var currentSnapshot = state.Registers.Snapshot();
                var changedRegisters = new List<RegisterTraceEntry>();

                if (recordRegisterTrace)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if (previousRegisterSnapshot[i] != currentSnapshot[i])
                        {
                            changedRegisters.Add(new RegisterTraceEntry(i, previousRegisterSnapshot[i], currentSnapshot[i]));
                        }
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

                if (recordRegisterTrace)
                    previousRegisterSnapshot = currentSnapshot;
            }
        }
    }

    private static (ProgramMemory memory, int startWordAddress, int loadedWordCount) LoadFirmware(FirmwareImage firmware)
    {
        int wordCapacity = 32768;
        var programMemory = new ProgramMemory(wordCapacity);
        var bytes = firmware.ToArray();
        int startWordAddress = (int)(firmware.BaseAddress / 2);
        int wordAddress = startWordAddress;

        for (int i = 0; i < bytes.Length; i += 2)
        {
            byte low = bytes[i];
            byte high = (byte)(i + 1 < bytes.Length ? bytes[i + 1] : 0);
            ushort opcode = (ushort)(low | (high << 8));
            programMemory[wordAddress++] = opcode;
        }

        int loadedWordCount = wordAddress - startWordAddress;
        return (programMemory, startWordAddress, loadedWordCount);
    }
}
