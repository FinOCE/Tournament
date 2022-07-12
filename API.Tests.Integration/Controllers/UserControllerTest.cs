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

    [TestMethod]
    public async Task PostTest_Valid()
    {
        // Arrange
        UserController.UserPostBody validBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        UserController.UserPostBody newEmailBody = new()
        {
            Email = "new@example.com",
            Username = "User",
            Password = "Password"
        };

        // Act
        HttpResponseMessage valid = await _Client.PostAsJsonAsync("/users", validBody);
        HttpResponseMessage newEmail = await _Client.PostAsJsonAsync("/users", newEmailBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, valid.StatusCode, "A user should be successfully created");
        Assert.AreEqual(HttpStatusCode.Created, newEmail.StatusCode, "A user should be successfully created");
    }

    [TestMethod]
    public async Task PostTest_DuplicateEmail()
    {
        // Arrange
        UserController.UserPostBody validBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        // Act
        HttpResponseMessage valid = await _Client.PostAsJsonAsync("/users", validBody);
        HttpResponseMessage duplicate = await _Client.PostAsJsonAsync("/users", validBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, valid.StatusCode, "The first case of an email should work");
        Assert.AreEqual(HttpStatusCode.BadRequest, duplicate.StatusCode, "The duplicate email use should not work");
    }

    [TestMethod]
    public async Task PostTest_MissingBody()
    {
        // Arrange
        UserController.UserPostBody missingEmailBody = new()
        {
            Username = "User",
            Password = "Password"
        };

        // Act
        HttpResponseMessage missingEmail = await _Client.PostAsJsonAsync("/users", missingEmailBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, missingEmail.StatusCode, "Missing part of the request body should fail");
    }

    [TestMethod]
    public async Task PostTest_InvalidEmail()
    {
        // Arrange
        UserController.UserPostBody invalidEmailBody1 = new()
        {
            Email = "invalid",
            Username = "User",
            Password = "Password"
        };

        // Act
        HttpResponseMessage invalidEmail = await _Client.PostAsJsonAsync("/users", invalidEmailBody1);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, invalidEmail.StatusCode, "An invalid email should not work");
    }

    [TestMethod]
    [Ignore]
    public async Task PostTest_ProfaneUsername()
    {
        // Arrange


        // Act


        // Assert

    }

    [TestMethod]
    [Ignore]
    public async Task PatchTest()
    {
        // Arrange


        // Act


        // Assert
        
    }
}
