namespace EmulatorAVR.Core.Instructions;

public enum InstructionKind
{
    Unsupported,
    Nop,
    Ldi,
    Mov,
    Add,
    // Group B — arithmetic and compare
    Adc,
    Adiw,
    Sub,
    Subi,
    Sbc,
    Sbci,
    Sbiw,
    Cp,
    Cpc,
    Cpi,
    Inc,
    Dec,
    Neg
}
