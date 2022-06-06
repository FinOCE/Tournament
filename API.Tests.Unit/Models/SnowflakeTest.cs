namespace API.Tests.Unit.Models;

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
    public void ValidateTest()
    {
        Assert.IsTrue(Snowflake.Validate("0"));
        Assert.IsTrue(Snowflake.Validate("132271570944000001"));
        Assert.IsFalse(Snowflake.Validate("99999999999999999999999"));
        Assert.IsFalse(Snowflake.Validate("a"));
        Assert.IsFalse(Snowflake.Validate(""));
    }

    [TestMethod]
    public void GetTimestampTest()
    {
        DateTime dateTime = Snowflake.Epoch.AddYears(1);

        BitArray workerIdBits = new(5);
        BitArray processIdBits = new(5);
        BitArray serialBits = new(Snowflake.SerialBitArrayLength);
        Snowflake snowflake = new(dateTime, workerIdBits, processIdBits, serialBits);

        Assert.AreEqual(dateTime, Snowflake.GetTimestamp(snowflake.ToString()));

        Assert.ThrowsException<OverflowException>(() => Snowflake.GetTimestamp("99999999999999999999999"));
        Assert.ThrowsException<FormatException>(() => Snowflake.GetTimestamp("a"));
        Assert.ThrowsException<FormatException>(() => Snowflake.GetTimestamp(""));
    }
}
