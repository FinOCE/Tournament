namespace Tournament.Utils;

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
}