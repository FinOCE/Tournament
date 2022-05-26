namespace API.Tests.Unit.Models;

[TestClass]
public class UserTest
{
    [TestMethod]
    public void ConstructorTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();
        
        Assert.ThrowsException<ArgumentException>(() => {
            User user = new("", "");
        });
        Assert.ThrowsException<ArgumentException>(() => {
            User user = new("", "Username");
        });
        Assert.ThrowsException<ArgumentException>(() => {
            User user = new(snowflake.ToString(), "");
        });
        Assert.IsInstanceOfType(new User(snowflake.ToString(), "Username"), typeof(User));
    }

    [TestMethod]
    public void SetUsernameTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user = new(snowflake.ToString(), "Username");

        Assert.IsFalse(user.SetUsername(""));
        Assert.IsFalse(user.SetUsername("1"));
        Assert.IsFalse(user.SetUsername("     "));
        Assert.IsFalse(user.SetUsername(" Us "));
        Assert.IsFalse(user.SetUsername("User,"));
        Assert.IsTrue(user.SetUsername("U.s.e.r."));
        Assert.IsTrue(user.SetUsername("    User  "));
        Assert.IsTrue(user.SetUsername("12345"));
        Assert.IsTrue(user.SetUsername("User."));
    }

    [TestMethod]
    public void HasPermissionTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user1 = new(snowflake.ToString(), "Username", 0);
        Assert.IsFalse(user1.HasPermission(UserPermission.Administrator));

        User user2 = new(snowflake.ToString(), "Username", 1);
        Assert.IsTrue(user2.HasPermission(UserPermission.Administrator));

        User user3 = new(snowflake.ToString(), "Username", (int)UserPermission.Administrator);
        Assert.IsTrue(user3.HasPermission(UserPermission.Administrator));
    }

    [TestMethod]
    public void AddPermissionTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user = new(snowflake.ToString(), "Username", 0);
        Assert.IsFalse(user.HasPermission(UserPermission.Administrator));

        user.AddPermission(UserPermission.Administrator);
        Assert.IsTrue(user.HasPermission(UserPermission.Administrator));

        user.AddPermission(UserPermission.Administrator);
        Assert.IsTrue(user.HasPermission(UserPermission.Administrator));
    }

    [TestMethod]
    public void RemovePermissionTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user = new(snowflake.ToString(), "Username", (int)UserPermission.Administrator);
        Assert.IsTrue(user.HasPermission(UserPermission.Administrator));

        user.RemovePermission(UserPermission.Administrator);
        Assert.IsFalse(user.HasPermission(UserPermission.Administrator));

        user.RemovePermission(UserPermission.Administrator);
        Assert.IsFalse(user.HasPermission(UserPermission.Administrator));
    }
}
