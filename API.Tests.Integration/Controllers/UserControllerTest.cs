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
        UserController.UserPostBody invalidEmailBody = new()
        {
            Email = "invalid",
            Username = "User",
            Password = "Password"
        };

        // Act
        HttpResponseMessage invalidEmail = await _Client.PostAsJsonAsync("/users", invalidEmailBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, invalidEmail.StatusCode, "An invalid email should not work");
    }

    [TestMethod]
    public async Task PostTest_InvalidUsername()
    {
        // Arrange
        UserController.UserPostBody invalidUsernameBody = new()
        {
            Email = "user@example.com",
            Username = "",
            Password = "Password"
        };

        // Act
        HttpResponseMessage invalidUsername = await _Client.PostAsJsonAsync("/users", invalidUsernameBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, invalidUsername.StatusCode, "An invalid username should not work");
    }

    [TestMethod]
    public async Task PostTest_InvalidPassword()
    {
        // Arrange
        UserController.UserPostBody invalidPasswordBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "short"
        };

        // Act
        HttpResponseMessage invalidPassword = await _Client.PostAsJsonAsync("/users", invalidPasswordBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, invalidPassword.StatusCode, "An invalid password should not work");
    }

    [TestMethod]
    public async Task PatchTest_Valid()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };
        
        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);
        User user = await post.Content.ReadAsAsync<User>();

        UserController.UserPatchBody validBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "ValidPassword",
            Discriminator = 1234,
            Icon = "2345678901234567",
            Permissions = 1,
            Verified = true
        };

        User expected = new(
            user.Id,
            "Valid",
            1234,
            "2345678901234567",
            true,
            permissions: 1);

        using HMACSHA256 hash = new(Encoding.UTF8.GetBytes(_Configuration["PASSWORD_HASHING_SECRET"]));
        byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(validBody.Password));
        string expectedHashedPassword = Convert.ToBase64String(bytes);

        // Act
        StringContent content = new(JsonSerializer.Serialize(validBody), Encoding.UTF8, "application/json");
        HttpResponseMessage patch = await _Client.PatchAsync($"/users/{user.Id}", content);
        User result = await patch.Content.ReadAsAsync<User>();

        string resultHashedPassword = (await _Database
            .RunQueryAsync(
                $"SELECT * FROM [dbo].[User] WHERE [Id] = '{user.Id}'",
                reader => (string)reader["Password"]))
            .First();

        string resultEmail = (await _Database
            .RunQueryAsync(
                $"SELECT * FROM [dbo].[User] WHERE [Id] = '{user.Id}'",
                reader => (string)reader["Email"]))
            .First();

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.OK, patch.StatusCode, "The user should be successfully updated");
        Assert.AreEqual(expected.Id, result.Id, "The ID should have updated");
        Assert.AreEqual(expected.Username, result.Username, "The username should have updated");
        Assert.AreEqual(expected.Discriminator, result.Discriminator, "The discriminator should have updated");
        Assert.AreEqual(expected.Icon, result.Icon, "The icon should have updated");
        Assert.AreEqual(
            expected.HasPermission(UserPermission.Administrator),
            result.HasPermission(UserPermission.Administrator),
            "The permissions should have updated");
        Assert.AreEqual(expected.Verified, result.Verified, "The verification should have updated");
        Assert.AreEqual(expectedHashedPassword, resultHashedPassword, "The password should have updated");
        Assert.AreEqual(validBody.Email, resultEmail, "The email should have updated");
    }

    [TestMethod]
    public async Task PatchTest_InvalidEmail()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);
        User user = await post.Content.ReadAsAsync<User>();

        UserController.UserPatchBody validBody = new()
        {
            Email = "invalid",
            Username = "Valid",
            Password = "ValidPassword",
            Discriminator = 1234,
            Icon = "2345678901234567",
            Permissions = 1,
            Verified = true
        };

        // Act
        StringContent content = new(JsonSerializer.Serialize(validBody), Encoding.UTF8, "application/json");
        HttpResponseMessage patch = await _Client.PatchAsync($"/users/{user.Id}", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.BadRequest, patch.StatusCode, "An invalid email should not work");
    }

    [TestMethod]
    public async Task PatchTest_InvalidUsername()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);
        User user = await post.Content.ReadAsAsync<User>();

        UserController.UserPatchBody validBody = new()
        {
            Email = "valid@example.com",
            Username = "",
            Password = "ValidPassword",
            Discriminator = 1234,
            Icon = "2345678901234567",
            Permissions = 1,
            Verified = true
        };

        // Act
        StringContent content = new(JsonSerializer.Serialize(validBody), Encoding.UTF8, "application/json");
        HttpResponseMessage patch = await _Client.PatchAsync($"/users/{user.Id}", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.BadRequest, patch.StatusCode, "An invalid username should not work");
    }

    [TestMethod]
    public async Task PatchTest_InvalidPassword()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);
        User user = await post.Content.ReadAsAsync<User>();

        UserController.UserPatchBody validBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "invalid",
            Discriminator = 1234,
            Icon = "2345678901234567",
            Permissions = 1,
            Verified = true
        };

        // Act
        StringContent content = new(JsonSerializer.Serialize(validBody), Encoding.UTF8, "application/json");
        HttpResponseMessage patch = await _Client.PatchAsync($"/users/{user.Id}", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.BadRequest, patch.StatusCode, "An invalid password should not work");
    }

    [TestMethod]
    public async Task PatchTest_InvalidDiscriminator()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);
        User user = await post.Content.ReadAsAsync<User>();

        UserController.UserPatchBody validBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "ValidPassword",
            Discriminator = 0,
            Icon = "2345678901234567",
            Permissions = 1,
            Verified = true
        };

        // Act
        StringContent content = new(JsonSerializer.Serialize(validBody), Encoding.UTF8, "application/json");
        HttpResponseMessage patch = await _Client.PatchAsync($"/users/{user.Id}", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.BadRequest, patch.StatusCode, "An invalid discriminator should not work");
    }

    [TestMethod]
    public async Task PatchTest_InvalidIcon()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);
        User user = await post.Content.ReadAsAsync<User>();

        UserController.UserPatchBody validBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "ValidPassword",
            Discriminator = 1234,
            Icon = "invalid",
            Permissions = 1,
            Verified = true
        };

        // Act
        StringContent content = new(JsonSerializer.Serialize(validBody), Encoding.UTF8, "application/json");
        HttpResponseMessage patch = await _Client.PatchAsync($"/users/{user.Id}", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.BadRequest, patch.StatusCode, "An invalid icon should not work");
    }

    [TestMethod]
    public async Task PatchTest_InvalidPermissions()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);
        User user = await post.Content.ReadAsAsync<User>();

        UserController.UserPatchBody validBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "ValidPassword",
            Discriminator = 1234,
            Icon = "2345678901234567",
            Permissions = -1,
            Verified = true
        };

        // Act
        StringContent content = new(JsonSerializer.Serialize(validBody), Encoding.UTF8, "application/json");
        HttpResponseMessage patch = await _Client.PatchAsync($"/users/{user.Id}", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.BadRequest, patch.StatusCode, "Invalid permissions should not work");
    }

    [TestMethod]
    public async Task PatchTest_Partial()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);
        User user = await post.Content.ReadAsAsync<User>();

        UserController.UserPatchBody validBody = new()
        {
            Username = "Valid",
            Discriminator = 1234
        };

        User expected = new(
            user.Id,
            "Valid",
            1234,
            "1234567890123456",
            false,
            permissions: 0);

        // Act
        StringContent content = new(JsonSerializer.Serialize(validBody), Encoding.UTF8, "application/json");
        HttpResponseMessage patch = await _Client.PatchAsync($"/users/{user.Id}", content);
        User result = await patch.Content.ReadAsAsync<User>();

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.OK, patch.StatusCode, "The user should be successfully updated");
        Assert.AreEqual(expected.Id, result.Id, "The ID should remain the same");
        Assert.AreEqual(expected.Username, result.Username, "The username should have updated");
        Assert.AreEqual(expected.Discriminator, result.Discriminator, "The discriminator should have updated");
        Assert.AreEqual(expected.Icon, result.Icon, "The icon should remain the same");
        Assert.AreEqual(
            expected.HasPermission(UserPermission.Administrator),
            result.HasPermission(UserPermission.Administrator),
            "The permissions should remain the same");
        Assert.AreEqual(expected.Verified, result.Verified, "The verification should remain the same");
    }
}
