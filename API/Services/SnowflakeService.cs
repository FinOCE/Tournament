namespace API.Services;

public class SnowflakeService
{
    public BitArray WorkerId { get; init; }
    public BitArray ProcessId { get; init; }
    private DateTime _CurrentTimestamp { get; set; } = Snowflake.Epoch;
    private BitArray _Serial { get; set; } = new(12);

    public SnowflakeService()
    {
        WorkerId = new BitArray(5); // Set to "00000" due to this being run on a single server
        ProcessId = new BitArray(5); // Set to "00000" due to this being run on a single server
    }

    /// <summary>
    /// Generate a new snowflake
    /// </summary>
    public Snowflake Generate()
    {
        // Add one bit to the serial
        DateTime currentTime = DateTime.UtcNow;
        BitUtil.AddOne(_Serial);

        // Reset serial if new DateTime
        if (currentTime.Millisecond != _CurrentTimestamp.Millisecond)
        {
            _CurrentTimestamp = currentTime;
            _Serial = new(12);
        }

        // Generate and return with a snowflake
        return new Snowflake(_CurrentTimestamp, WorkerId, ProcessId, _Serial);
    }
}
