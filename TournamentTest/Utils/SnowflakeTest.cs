using Tournament.Utils;

namespace Test;

[TestClass]
public class SnowflakeTest
{
    [TestMethod]
    public void ToStringTest()
    {
        BitArray workerIdBits = new(5);
        BitArray processIdBits = new(5);
        BitArray serialBits = new(Snowflake.SerialBitArrayLength);

        Snowflake snowflake1 = new(Snowflake.Epoch, workerIdBits, processIdBits, serialBits);
        Assert.AreEqual("0", snowflake1.ToString());

        serialBits.Set(0, true);

        Snowflake snowflake2 = new(Snowflake.Epoch.AddYears(1), workerIdBits, processIdBits, serialBits);
        Assert.AreEqual("132271570944000001", snowflake2.ToString());
    }

    [TestMethod]
    public void ConvertBitsToStringTest()
    {
        Assert.AreEqual("", Snowflake.ConvertBitsToString(new BitArray(0)));
        Assert.AreEqual("0", Snowflake.ConvertBitsToString(new BitArray(1)));
        Assert.AreEqual("00", Snowflake.ConvertBitsToString(new BitArray(2)));
        Assert.AreEqual("0101", Snowflake.ConvertBitsToString(new BitArray(new bool[] {true, false, true, false})));
    }
}
