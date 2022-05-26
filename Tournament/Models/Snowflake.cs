namespace API.Models;

/// <summary>
/// A unique identifier as described by https://discord.com/developers/docs/reference#snowflakes
/// </summary>
public class Snowflake
{
    public static readonly DateTime Epoch = new DateTime(2022, 1, 1);
    public static readonly int TimestampBitArrayLength = 42;
    public static readonly int WorkerIdBitArrayLength = 5;
    public static readonly int ProcessIdBitArrayLength = 5;
    public static readonly int SerialBitArrayLength = 12;

    public DateTime Timestamp { get; init; }
    private BitArray _WorkerId { get; init; }
    private BitArray _ProcessId { get; init; }
    private BitArray _Serial { get; init; }

    public Snowflake(DateTime timestamp, BitArray workerIdBits, BitArray processIdBits, BitArray serialBits)
    {
        Timestamp = timestamp;

        _WorkerId = workerIdBits;
        _WorkerId.Length = WorkerIdBitArrayLength;

        _ProcessId = processIdBits;
        _ProcessId.Length = ProcessIdBitArrayLength;

        _Serial = serialBits;
        _Serial.Length = SerialBitArrayLength;
    }

    public override string ToString()
    {
        // Convert DateTime timestamp to bit array
        ulong milliseconds = (ulong)Timestamp.Subtract(Epoch).TotalMilliseconds;
        BitArray timestampBits = new(BitConverter.GetBytes(milliseconds));
        timestampBits.Length = TimestampBitArrayLength;

        // Return snowflake string
        return Convert.ToUInt64(
            BitUtil.ConvertBitsToString(timestampBits)
                + BitUtil.ConvertBitsToString(_WorkerId)
                + BitUtil.ConvertBitsToString(_ProcessId)
                + BitUtil.ConvertBitsToString(_Serial),
            2
        ).ToString();
    }

    /// <summary>
    /// Checks if a given snowflake string is valid
    /// </summary>
    public static bool Validate(string snowflake)
    {
        try
        {
            GetTimestamp(snowflake);
            // If it cannot get the timestamp then it's probably invalid (but this probably isn't perfect)
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get the timestamp from a snowflake string
    /// </summary>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="OverflowException"></exception>
    public static DateTime GetTimestamp(string snowflake)
    {
        StringBuilder snowflakeBits = new();

        foreach (byte b in BitConverter.GetBytes(Convert.ToUInt64(snowflake)))
            snowflakeBits.Insert(0, Convert.ToString(b, 2).PadLeft(8, '0'));

        ulong milliseconds = Convert.ToUInt64(snowflakeBits.ToString()[..TimestampBitArrayLength], 2);

        return Epoch.AddMilliseconds(milliseconds);
    }
}
