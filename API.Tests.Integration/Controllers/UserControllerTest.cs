namespace API.Tests.Integration.Controllers;

[TestClass]
public class UserControllerTest : Test
{
    [TestCleanup]
    public async Task TestCleaup()
    {
        await _Database.RunQueryAsync<int>("DELETE FROM [dbo].[User]");
    }
    
    [TestMethod]
    public async Task GetTest_NotFound()
    {
        // Arrange
        string invalidId = "123";

        // Act
        HttpResponseMessage response = await _Client.GetAsync($"/users/{invalidId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "An ID that doesn't match a user should not find anything");
    }

    [TestMethod]
    public async Task GetTest_NotSnowflakeConstraint()
    {
        // Arrange
        string invalidConstraint = "ThisIsNotASnowflake";

        // Act
        HttpResponseMessage response = await _Client.GetAsync($"/users/{invalidConstraint}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "An ID that is not a snowflake should not work");
    }

    [TestMethod]
    public async Task GetTest_Existing()
    {
        // Arrange
        UserController.UserPostBody body = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        HttpResponseMessage userResponse = await _Client.PostAsJsonAsync("/users", body);
        User userCreated = await userResponse.Content.ReadAsAsync<User>();

        string existingUserId = userCreated.Id;

        // Act
        HttpResponseMessage response = await _Client.GetAsync($"/users/{existingUserId}");
        User user = await response.Content.ReadAsAsync<User>();

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, userResponse.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The user should be successfully fetched");
        Assert.AreEqual(userCreated, user, "The created and fetched users should be the same");
    }
}
