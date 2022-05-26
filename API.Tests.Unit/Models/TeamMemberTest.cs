namespace API.Tests.Unit.Models;

[TestClass]
public class TeamMemberTest
{
    [TestMethod]
    public void HasRoleTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake1 = snowflakeService.Generate();
        Snowflake snowflake2 = snowflakeService.Generate();

        User user = new(snowflake1.ToString(), "Username");
        Team team = new(snowflake2.ToString(), "Name", null, false);
        TeamMember member = new(user, team, (int)TeamRole.Substitute);

        Assert.IsTrue(member.HasRole(TeamRole.Substitute));
        Assert.IsFalse(member.HasRole(TeamRole.Player));
    }

    [TestMethod]
    public void AddRoleTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake1 = snowflakeService.Generate();
        Snowflake snowflake2 = snowflakeService.Generate();

        User user = new(snowflake1.ToString(), "Username");
        Team team = new(snowflake2.ToString(), "Name", null, false);
        TeamMember member = new(user, team);

        Assert.IsTrue(member.HasRole(TeamRole.Player));

        member.AddRole(TeamRole.Captain);
        Assert.IsTrue(member.HasRole(TeamRole.Player));
        Assert.IsTrue(member.HasRole(TeamRole.Captain));
    }

    [TestMethod]
    public void RemoveRoleTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake1 = snowflakeService.Generate();
        Snowflake snowflake2 = snowflakeService.Generate();

        User user = new(snowflake1.ToString(), "Username");
        Team team = new(snowflake2.ToString(), "Name", null, false);
        TeamMember member = new(user, team);
        member.AddRole(TeamRole.Captain);

        Assert.IsTrue(member.HasRole(TeamRole.Player));
        Assert.IsTrue(member.HasRole(TeamRole.Captain));

        member.RemoveRole(TeamRole.Captain);
        Assert.IsFalse(member.HasRole(TeamRole.Captain));
    }

    [TestMethod]
    public void HasPermissionTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake1 = snowflakeService.Generate();
        Snowflake snowflake2 = snowflakeService.Generate();

        User user = new(snowflake1.ToString(), "Username");
        Team team = new(snowflake2.ToString(), "Name", null, false);

        TeamMember member = new(user, team);
        Assert.IsTrue(member.HasPermission(TeamRolePermission.Substitute));
        Assert.IsFalse(member.HasPermission(TeamRolePermission.ManageRegistration));
        Assert.IsFalse(member.HasPermission(TeamRolePermission.ManageTeam));

        member.AddRole(TeamRole.Captain);
        Assert.IsTrue(member.HasPermission(TeamRolePermission.Substitute));
        Assert.IsTrue(member.HasPermission(TeamRolePermission.ManageRegistration));
        Assert.IsFalse(member.HasPermission(TeamRolePermission.ManageTeam));

        member.AddRole(TeamRole.Owner);
        Assert.IsTrue(member.HasPermission(TeamRolePermission.Substitute));
        Assert.IsTrue(member.HasPermission(TeamRolePermission.ManageRegistration));
        Assert.IsTrue(member.HasPermission(TeamRolePermission.ManageTeam));
    }
}
