namespace API.Tests.Integration.Controllers;

public class UserControllerTest : TestClass
{
    // TODO: Run initialise and clean-up for tests https://hamidmosalla.com/2018/08/30/xunit-beforeaftertestattribute-how-to-run-code-before-and-after-test/
    
    [Fact]
    public async void GetTest()
    {
        // Arrange
        string invalidId = "123";

        // Act
        HttpResponseMessage invalid = await Client.GetAsync($"/users/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, invalid.StatusCode);

        // TODO: Test fetching existing user
    }
    
    [Fact]
    public async void PostTest()
    {
        // Arrange
        UserController.UserPostBody validBody = new()
        {
            Email = "user@example.com",
            Username = "User",
            Password = "Password"
        };

        UserController.UserPostBody validSameNameBody = new()
        {
            Email = "new@example.com",
            Username = "User",
            Password = "Password"
        };

        // Act
        HttpResponseMessage valid = await Client.PostAsJsonAsync("/users", validBody);
        HttpResponseMessage validSameName = await Client.PostAsJsonAsync("/users", validSameNameBody);
        HttpResponseMessage duplicateEmail = await Client.PostAsJsonAsync("/users", validBody);

        // Assert
        Assert.Equal(HttpStatusCode.Created, valid.StatusCode);
        Assert.Equal(HttpStatusCode.Created, validSameName.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateEmail.StatusCode);
    }

    [Fact(Skip = "Not implemented")]
    public async void PatchTest()
    {
        // Arrange


        // Act


        // Assert

    }
}
