namespace Test.Models;

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
}
