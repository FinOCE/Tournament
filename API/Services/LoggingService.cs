namespace API.Services;

public class LoggingService
{
    /// <summary>
    /// Make a log containing the given content
    /// </summary>
    /// <param name="source">The name of the class the log originates from</param>
    /// <param name="message">The content to log</param>
    public void Log(string source, string message)
    {
        Console.WriteLine(
            string.Format(
                "[{0}] {1}: {2}",
                DateTime.UtcNow.ToString(),
                source,
                message));
    }
}
