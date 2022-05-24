namespace Tournament.Utils;

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
            ConvertBitsToString(timestampBits)
                + ConvertBitsToString(WorkerId)
                + ConvertBitsToString(ProcessId)
                + ConvertBitsToString(Serial),
            2
        ).ToString();
    }

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