namespace API.Tests.Integration.Controllers;

[TestClass]
public class AuthControllerTest : Test
{
    [TestMethod]
    public async Task PostTest_Valid()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "ValidPassword"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);
        User user = await post.Content.ReadAsAsync<User>();

        AuthController.AuthPostBody requestBody = new()
        {
            Email = "valid@example.com",
            Password = "ValidPassword"
        };

        // Act
        HttpResponseMessage request = await _Client.PostAsJsonAsync("/login", requestBody);
        string jwt = await request.Content.ReadAsStringAsync();

        Token token = Token.Deserialize(jwt);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.OK, request.StatusCode, "A token should be successfully generated");
        Assert.AreEqual(user.Id, token.Payload.Subject, "The token should be for the user");
        Assert.IsTrue(
            token.Payload.IssuedAt <= DateTime.Now &&
            token.Payload.IssuedAt > DateTime.Now.AddMinutes(-1),
            "The token should have been recently issued");
    }

    [TestMethod]
    public async Task PostTest_InvalidEmail()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "ValidPassword"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);

        AuthController.AuthPostBody requestBody = new()
        {
            Email = "invalid",
            Password = "ValidPassword"
        };

        // Act
        HttpResponseMessage request = await _Client.PostAsJsonAsync("/login", requestBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode, "An invalid email should be rejected");
    }

    [TestMethod]
    public async Task PostTest_IncorrectEmail()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "ValidPassword"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);

        AuthController.AuthPostBody requestBody = new()
        {
            Email = "incorrect@example.com",
            Password = "ValidPassword"
        };

        // Act
        HttpResponseMessage request = await _Client.PostAsJsonAsync("/login", requestBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.NotFound, request.StatusCode, "An email without a user should not be found");
    }

    [TestMethod]
    public async Task PostTest_InvalidPassword()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "ValidPassword"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);

        AuthController.AuthPostBody requestBody = new()
        {
            Email = "valid@example.com",
            Password = "invalid"
        };

        // Act
        HttpResponseMessage request = await _Client.PostAsJsonAsync("/login", requestBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode, "An invalid email should be rejected");
    }

    [TestMethod]
    public async Task PostTest_IncorrectPassword()
    {
        // Arrange
        UserController.UserPostBody postBody = new()
        {
            Email = "valid@example.com",
            Username = "Valid",
            Password = "ValidPassword"
        };

        HttpResponseMessage post = await _Client.PostAsJsonAsync("/users", postBody);

        AuthController.AuthPostBody requestBody = new()
        {
            Email = "valid@example.com",
            Password = "IncorrectPassword"
        };

        // Act
        HttpResponseMessage request = await _Client.PostAsJsonAsync("/login", requestBody);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, post.StatusCode, "The user should be successfully created");
        Assert.AreEqual(HttpStatusCode.NotFound, request.StatusCode, "An incorrect password should not find the user");
    }
}
