namespace API.Tests.Unit.Services;

[TestClass]
public class SnowflakeServiceTest
{
    [TestMethod]
    public void GenerateTest()
    {
        SnowflakeService snowflakeService = new();

        Dictionary<string, Snowflake> snowflakes = new();
        for (int i = 0; i < 10; i++)
        {
            Snowflake snowflake = snowflakeService.Generate();
            snowflakes.Add(snowflake.ToString(), snowflake);
        }

        Assert.IsTrue(snowflakes.Keys.Count == 10); // If there are any duplicates they would overwrite the previous hash
    }
}
