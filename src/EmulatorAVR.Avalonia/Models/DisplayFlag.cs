namespace EmulatorAVR.Avalonia.Models;

public sealed record DisplayFlag(string Name, bool IsActive)
{
    public bool IsInactive => !IsActive;
}
