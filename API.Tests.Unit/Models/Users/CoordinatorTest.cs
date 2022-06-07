namespace API.Tests.Unit.Models.Users;

[TestClass]
public class CoordinatorTest
{
    SnowflakeService SnowflakeService = null!;
    Tournament Tournament = null!;
    User User = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();
        Tournament = new(SnowflakeService.Generate().ToString(), "Tournament");
        User = new(SnowflakeService.Generate().ToString(), "User", 1234);
    }

    [TestMethod]
    public void HasRoleTest()
    {
        // Arrange
        Coordinator coordinator = new(User, Tournament);

        // Act
        bool isOwner = coordinator.HasRole(HostRole.Owner);
        bool isModerator = coordinator.HasRole(HostRole.Moderator);

        // Assert
        Assert.IsFalse(isOwner, "By default coordinators should not be owners");
        Assert.IsTrue(isModerator, "By default coordinators should be moderators");
    }

    [TestMethod]
    public void AddRoleTest()
    {
        // Arrange
        Coordinator coordinator = new(User, Tournament);

        // Act
        bool isOwnerInitially = coordinator.HasRole(HostRole.Owner);
        coordinator.AddRole(HostRole.Owner);
        bool isOwnerAfterCall = coordinator.HasRole(HostRole.Owner);
        coordinator.AddRole(HostRole.Owner);
        bool isOwnerAfterRecall = coordinator.HasRole(HostRole.Owner);

        // Assert
        Assert.IsFalse(isOwnerInitially, "By default coordinators should not be owners");
        Assert.IsTrue(isOwnerAfterCall, "The coordinator should be an owner after method call");
        Assert.IsTrue(isOwnerAfterCall, "The coordinator should still be an owner after method recall");
    }

    [TestMethod]
    public void RemoveRoleTest()
    {
        // Arrange
        Coordinator coordinator = new(User, Tournament);
        coordinator.AddRole(HostRole.Owner);

        // Act
        bool isOwnerInitially = coordinator.HasRole(HostRole.Owner);
        coordinator.RemoveRole(HostRole.Owner);
        bool isOwnerAfterCall = coordinator.HasRole(HostRole.Owner);
        coordinator.RemoveRole(HostRole.Owner);
        bool isOwnerAfterRecall = coordinator.HasRole(HostRole.Owner);

        // Assert
        Assert.IsTrue(isOwnerInitially, "The coordinator should start of as an owner");
        Assert.IsFalse(isOwnerAfterCall, "The coordinator should no longer be an owner on method call");
        Assert.IsFalse(isOwnerAfterRecall, "The coordinator should still not be owner on method recall");
    }

    [TestMethod]
    public void HasPermissionTest()
    {
        // Arrange
        Coordinator coordinator = new(User, Tournament);

        // Act
        bool canSpectate = coordinator.HasPermission(HostRolePermission.Spectate);
        bool canManageSettingsInitially = coordinator.HasPermission(HostRolePermission.ManageSettings);
        coordinator.AddRole(HostRole.Owner);
        bool canManageSettingsAfterCall = coordinator.HasPermission(HostRolePermission.ManageSettings);

        // Assert
        Assert.IsTrue(canSpectate, "The coordinator should be able to spectate games");
        Assert.IsFalse(canManageSettingsInitially, "The coordinator shouldn't be able to manage the settings initially");
        Assert.IsTrue(canManageSettingsAfterCall, "The coordinator should be able to manage settings after methdo call");
    }
}
