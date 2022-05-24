namespace Tournament.Models;

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

    private DateTime Timestamp { get; set; }
    private BitArray WorkerId { get; set; }
    private BitArray ProcessId { get; set; }
    private BitArray Serial { get; set; }

    public Snowflake(DateTime timestamp, BitArray workerIdBits, BitArray processIdBits, BitArray serialBits)
    {
        Timestamp = timestamp;

        WorkerId = workerIdBits;
        WorkerId.Length = WorkerIdBitArrayLength;

        ProcessId = processIdBits;
        ProcessId.Length = ProcessIdBitArrayLength;

        Serial = serialBits;
        Serial.Length = SerialBitArrayLength;
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
                + BitUtil.ConvertBitsToString(WorkerId)
                + BitUtil.ConvertBitsToString(ProcessId)
                + BitUtil.ConvertBitsToString(Serial),
            2
        ).ToString();
    }

    /// <summary>
    /// Get the timestamp from a snowflake string
    /// </summary>
    public static DateTime GetTimestamp(string snowflake)
    {
        StringBuilder snowflakeBits = new();

        foreach (byte b in BitConverter.GetBytes(Convert.ToUInt64(snowflake)))
            snowflakeBits.Insert(0, Convert.ToString(b, 2).PadLeft(8, '0'));

        ulong milliseconds = Convert.ToUInt64(snowflakeBits.ToString()[..TimestampBitArrayLength], 2);

        return Epoch.AddMilliseconds(milliseconds);
    }
}
