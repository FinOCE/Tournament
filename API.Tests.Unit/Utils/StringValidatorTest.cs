namespace API.Tests.Unit.Utils;

[TestClass]
public class StringValidatorTest
{
    [TestMethod]
    public void TestTest()
    {
        // Arrange
        int successfulTests = 0;
        int failedTests = 0;

        int minimumLength = 2;
        int maximumLength = 5;
        Regex invalidRegex = new("!");

        StringValidator validator = new StringValidator()
            .SetMinimumLength(minimumLength)
            .SetMaximumLength(maximumLength)
            .SetInvalidRegex(invalidRegex)
            .AllowNull()
            .Trim()
            .OnSuccess((string? str) => successfulTests++)
            .OnFailure((string? str) => failedTests++);

        string shortStr = "1";
        string longStr = "123456";
        string invalidStr = "Mmm!";
        string goodStr = "1234";
        string atMinStr = "12";
        string atMaxStr = "12345";
        string trimStr = "     12345     ";

        // Act
        bool shortStrValid = validator.Test(shortStr);
        bool longStrValid = validator.Test(longStr);
        bool invalidStrValid = validator.Test(invalidStr);
        bool goodStrValid = validator.Test(goodStr);
        bool atMinStrValid = validator.Test(atMinStr);
        bool atMaxStrvalid = validator.Test(atMaxStr);
        bool nullStrValid = validator.Test(null);
        bool trimStrValid = validator.Test(trimStr);

        // Assert
        Assert.IsFalse(shortStrValid);
        Assert.IsFalse(longStrValid);
        Assert.IsFalse(invalidStrValid);
        Assert.IsTrue(goodStrValid);
        Assert.IsTrue(atMinStrValid);
        Assert.IsTrue(atMaxStrvalid);
        Assert.IsTrue(nullStrValid);
        Assert.IsTrue(trimStrValid);
        Assert.AreEqual(5, successfulTests);
        Assert.AreEqual(3, failedTests);
    }

    [TestMethod]
    public void SetMinimumLengthTest()
    {
        // Arrange
        StringValidator validator = new();
        int minimumLength = 5;

        // Act
        validator.SetMinimumLength(minimumLength);

        // Assert
        Assert.AreEqual(minimumLength, validator.MinimumLength, "The minimum length should have been updated");
    }

    [TestMethod]
    public void SetMaximumLengthTest()
    {
        // Arrange
        StringValidator validator = new();
        int maximumLength = 5;

        // Act
        validator.SetMaximumLength(maximumLength);

        // Assert
        Assert.AreEqual(maximumLength, validator.MaximumLength, "The maximum length should have been updated");
    }

    [TestMethod]
    public void SetInvalidRegexTest()
    {
        // Arrange
        StringValidator validator = new();
        Regex invalidRegex = new(@"\w");

        // Act
        validator.SetInvalidRegex(invalidRegex);

        // Assert
        Assert.AreEqual(invalidRegex, validator.InvalidRegex, "The invalid regex should have been updated");
    }

    [TestMethod]
    public void AllowNullTest()
    {
        // Arrange
        StringValidator validator = new();

        // Act
        validator.AllowNull();

        // Assert
        Assert.IsTrue(validator.Nullable, "The validator should now allow null values");
    }

    [TestMethod]
    public void TrimTest()
    {
        // Arrange
        StringValidator validator = new();

        // Act
        validator.Trim();

        // Assert
        Assert.IsTrue(validator.UseTrim, "The validator should trim string before testing");
    }

    [TestMethod]
    public void OnSuccessTest()
    {
        // Arrange
        StringValidator validator = new();
        Action<string?> successAction = (string? str) => { };

        // Act
        validator.OnSuccess(successAction);

        // Assert
        Assert.AreEqual(successAction, validator.SuccessAction, "The onSuccess action should have been updated");
    }

    [TestMethod]
    public void OnFailureTest()
    {
        // Arrange
        StringValidator validator = new();
        Action<string?> failureAction = (string? str) => { };

        // Act
        validator.OnFailure(failureAction);

        // Assert
        Assert.AreEqual(failureAction, validator.FailureAction, "The onFailure action should have been updated");
    }
}
