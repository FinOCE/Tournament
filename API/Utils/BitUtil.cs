namespace API.Utils;

/// <summary>
/// Helper utility for bit-related tasks
/// </summary>
public static class BitUtil
{
    /// <summary>
    /// Convert a BitArray into a string
    /// </summary>
    public static string ConvertBitsToString(BitArray bits)
    {
        StringBuilder result = new();

        for (int i = 0; i < bits.Length; i++)
            result.Insert(0, bits.Get(i) ? "1" : "0");

        return result.ToString();
    }

    /// <summary>
    /// Add one to a BitArray
    /// </summary>
    public static BitArray AddOne(BitArray bitArray)
    {
        bool finished = false;

        for (int i = 0; i < bitArray.Length; i++)
        {
            if (!bitArray[i])
            {
                bitArray.Set(i, true);
                finished = true;
                break;
            }
            
            bitArray[i] = false;
        }

        if (!finished)
            bitArray = new(bitArray.Length);

        return bitArray;
    }
}