namespace API.Tests.Unit.Models.Brackets.Structures;

[TestClass]
public class FinaleTest
{
    SnowflakeService SnowflakeService = null!;
    Finale Structure = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();
        Series series = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure = new(series, new(1), new(2));
    }

    [TestMethod]
    public void ConstructorTest()
    {
        // Assert
        Assert.IsInstanceOfType(
            Structure.Series.WinnerProgression,
            typeof(ExactPlacement),
            "The winner progression should be an exact placement");

        Assert.AreEqual(
            1,
            ((ExactPlacement)Structure.Series.WinnerProgression!).Position,
            "The winner progression placement should be 1");

        Assert.IsInstanceOfType(
            Structure.Series.LoserProgression,
            typeof(ExactPlacement),
            "The loser progression should be an exact placement");

        Assert.AreEqual(
            2,
            ((ExactPlacement)Structure.Series.LoserProgression!).Position,
            "The loser progression placement should be 2");
    }

    [TestMethod]
    public void SetWinnerProgressionTest()
    {
        // Arrange
        ExactPlacement placement = new(4);

        // Act
        Structure.SetWinnerProgression(placement);

        // Assert
        Assert.AreNotEqual(
            4,
            ((ExactPlacement)Structure.Series.WinnerProgression!).Position,
            "The winner progression should not have updated to 4");

        Assert.AreEqual(
            1,
            ((ExactPlacement)Structure.Series.WinnerProgression!).Position,
            "The winner progression should still be 1");
    }

    [TestMethod]
    public void SetLoserProgressionTest()
    {
        // Arrange
        ExactPlacement placement = new(4);

        // Act
        Structure.SetLoserProgression(placement);

        // Assert
        Assert.AreNotEqual(
            4,
            ((ExactPlacement)Structure.Series.LoserProgression!).Position,
            "The loser progression should not have updated to 4");

        Assert.AreEqual(
            2,
            ((ExactPlacement)Structure.Series.LoserProgression!).Position,
            "The loser progression should still be 2");
    }
}
