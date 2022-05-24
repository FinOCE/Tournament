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

    [TestMethod]
    public void AddOneTest()
    {
        BitArray bitArray = new BitArray(new bool[] {false, false, false});
        Assert.AreEqual("001", BitUtil.ConvertBitsToString(BitUtil.AddOne(bitArray)));
        Assert.AreEqual("010", BitUtil.ConvertBitsToString(BitUtil.AddOne(bitArray)));
        Assert.AreEqual("011", BitUtil.ConvertBitsToString(BitUtil.AddOne(bitArray)));
        Assert.AreEqual("100", BitUtil.ConvertBitsToString(BitUtil.AddOne(bitArray)));
        Assert.AreEqual("101", BitUtil.ConvertBitsToString(BitUtil.AddOne(bitArray)));
        Assert.AreEqual("110", BitUtil.ConvertBitsToString(BitUtil.AddOne(bitArray)));
        Assert.AreEqual("111", BitUtil.ConvertBitsToString(BitUtil.AddOne(bitArray)));
        Assert.AreEqual("000", BitUtil.ConvertBitsToString(BitUtil.AddOne(bitArray)));
    }
}
