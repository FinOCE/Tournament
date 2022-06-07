namespace API.Tests.Unit.Models.Teams;

[TestClass]
public class TeamTest
{
    [TestMethod]
    public void ConstructorTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        Assert.ThrowsException<ArgumentException>(() => {
            Team team = new("", "", null, false);
        });
        Assert.ThrowsException<ArgumentException>(() => {
            Team team = new("", "Name", null, false);
        });
        Assert.ThrowsException<ArgumentException>(() => {
            Team team = new(snowflake.ToString(), "", null, false);
        });
        Assert.IsInstanceOfType(new Team(snowflake.ToString(), "Name", null, false), typeof(Team));
    }

    [TestMethod]
    public void SetNameTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        Team team = new(snowflake.ToString(), "Team", null, false);

        Assert.IsFalse(team.SetName(""));
        Assert.IsFalse(team.SetName("    "));
        Assert.IsFalse(team.SetName(","));
        Assert.IsFalse(team.SetName("Test,"));
        Assert.IsTrue(team.SetName("V"));
        Assert.IsTrue(team.SetName("Team"));
        Assert.IsTrue(team.SetName("This is a team"));
        Assert.IsTrue(team.SetName("12345"));
        Assert.IsTrue(team.SetName("T.e.a.m."));
        Assert.IsTrue(team.SetName("    Team      "));
        Assert.IsTrue(team.SetName("._."));
    }

    [TestMethod]
    public void SetIconTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        Team team = new(snowflake.ToString(), "Team", null, false);

        Assert.AreEqual(Team.DefaultIcon, team.Icon);

        string validIconName = "1234567890123456";
        Assert.IsTrue(team.SetIcon(validIconName));
        Assert.AreEqual(validIconName, team.Icon);

        Assert.IsFalse(team.SetIcon(""));
        Assert.IsFalse(team.SetIcon("InvalidLength"));
        Assert.IsFalse(team.SetIcon("Has a space"));
        Assert.IsFalse(team.SetIcon("Some!invalid_chars"));
        Assert.AreEqual(validIconName, team.Icon);

        Assert.IsTrue(team.SetIcon(null));
        Assert.AreEqual(Team.DefaultIcon, team.Icon);
    }

    [TestMethod]
    public void AddMemberTest_TeamMember()
    {
        SnowflakeService snowflakeService = new();
        Team team = new(snowflakeService.Generate().ToString(), "Team", null, false);

        User user1 = new(snowflakeService.Generate().ToString(), "User 1", 1234);
        TeamMember member1 = new(user1, team, (int)TeamRole.Player);
        Assert.IsTrue(team.AddMember(member1));
        Assert.IsFalse(team.AddMember(member1));

        Assert.IsTrue(team.Members.Keys.Count == 1);
        Assert.IsTrue(team.Members.ContainsKey(member1.User.Id));
        Assert.IsTrue(team.Members[member1.User.Id].HasRole(TeamRole.Player));

        User user2 = new(snowflakeService.Generate().ToString(), "User 2", 1234);
        TeamMember member2 = new(user2, team, (int)TeamRole.Substitute);
        Assert.IsTrue(team.AddMember(member2));

        Assert.IsTrue(team.Members.Keys.Count == 2);
        Assert.IsTrue(team.Members.ContainsKey(member2.User.Id));
        Assert.IsTrue(team.Members[member2.User.Id].HasRole(TeamRole.Substitute));
    }

    [TestMethod]
    public void AddMemberTest_User()
    {
        SnowflakeService snowflakeService = new();
        Team team = new(snowflakeService.Generate().ToString(), "Team", null, false);

        User user1 = new(snowflakeService.Generate().ToString(), "User 1", 1234);
        Assert.IsTrue(team.AddMember(user1));
        Assert.IsFalse(team.AddMember(user1));

        Assert.IsTrue(team.Members.Keys.Count == 1);
        Assert.IsTrue(team.Members.ContainsKey(user1.Id));
        Assert.IsTrue(team.Members[user1.Id].HasRole(TeamRole.Player));

        User user2 = new(snowflakeService.Generate().ToString(), "User 2", 1234);
        Assert.IsTrue(team.AddMember(user2, (int)TeamRole.Substitute));
        Assert.IsFalse(team.AddMember(user2, (int)TeamRole.Substitute));

        Assert.IsTrue(team.Members.Keys.Count == 2);
        Assert.IsTrue(team.Members.ContainsKey(user2.Id));
        Assert.IsFalse(team.Members[user2.Id].HasRole(TeamRole.Player));
        Assert.IsTrue(team.Members[user2.Id].HasRole(TeamRole.Substitute));
    }
}
