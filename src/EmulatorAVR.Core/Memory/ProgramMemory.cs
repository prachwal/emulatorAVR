namespace EmulatorAVR.Core.Memory;

public class ProgramMemory
{
    private readonly ushort[] _words;

    public int WordCapacity { get; }

    public ProgramMemory(int wordCapacity)
    {
        if (wordCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(wordCapacity), "Capacity must be positive.");
        WordCapacity = wordCapacity;
        _words = new ushort[wordCapacity];
    }

    public ushort this[int wordAddress]
    {
        get
        {
            if (wordAddress < 0 || wordAddress >= WordCapacity)
                throw new ArgumentOutOfRangeException(nameof(wordAddress));
            return _words[wordAddress];
        }
        set
        {
            if (wordAddress < 0 || wordAddress >= WordCapacity)
                throw new ArgumentOutOfRangeException(nameof(wordAddress));
            _words[wordAddress] = value;
        }
    }
}
