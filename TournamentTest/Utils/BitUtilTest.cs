namespace Test.Utils;

[TestClass]
public class BitUtilTest
{
    [TestMethod]
    public void ConvertBitsToStringTest()
    {
        Assert.AreEqual("", BitUtil.ConvertBitsToString(new BitArray(0)));
        Assert.AreEqual("0", BitUtil.ConvertBitsToString(new BitArray(1)));
        Assert.AreEqual("00", BitUtil.ConvertBitsToString(new BitArray(2)));
        Assert.AreEqual("0101", BitUtil.ConvertBitsToString(new BitArray(new bool[] { true, false, true, false })));
    }
}
