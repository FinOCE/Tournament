namespace API.Utils;

public class StringValidator
{
    public int? MinimumLength { get; private set; } = null;
    public int? MaximumLength { get; private set; } = null;
    public Regex? InvalidRegex { get; private set; } = null;
    public Regex? ValidRegex { get; private set; } = null;
    public bool Nullable { get; private set; } = false;
    public bool UseTrim { get; private set; } = false;
    public Action<string?>? SuccessAction { get; private set; } = null;
    public Action<string?>? FailureAction { get; private set; } = null;

    /// <summary>
    /// Test to see if a string passes the validation
    /// </summary>
    public bool Test(string? str)
    {
        bool success = true;

        // Test if the string passes applied tests
        if (str is not null)
        {
            if (UseTrim)
                str = str.Trim();

            if (MinimumLength is not null && str.Length < MinimumLength)
                success = false;

            if (MaximumLength is not null && str.Length > MaximumLength)
                success = false;

            if (InvalidRegex is not null && InvalidRegex.IsMatch(str))
                success = false;

            if (ValidRegex is not null && !ValidRegex.IsMatch(str))
                success = false;
        }
        else if (!Nullable)
            success = false;

        // Run callbacks if provided
        if (success && SuccessAction is not null)
            SuccessAction(str);
        else if (!success && FailureAction is not null)
            FailureAction(str);

        // Return result of the test
        return success;
    }

    /// <summary>
    /// Add a minimum length 
    /// </summary>
    public StringValidator SetMinimumLength(int? length)
    {
        MinimumLength = length;
        return this;
    }

    /// <summary>
    /// Add a maximum length
    /// </summary>
    public StringValidator SetMaximumLength(int? length)
    {
        MaximumLength = length;
        return this;
    }

    /// <summary>
    /// Add a regex to test for invalid characters
    /// </summary>
    public StringValidator SetInvalidRegex(Regex? regex)
    {
        InvalidRegex = regex;
        return this;
    }

    /// <summary>
    /// Add a regex to test for valid strings
    /// </summary>
    public StringValidator SetValidRegex(Regex? regex)
    {
        ValidRegex = regex;
        return this;
    }

    /// <summary>
    /// Choose whether or not the value can be null
    /// </summary>
    public StringValidator AllowNull(bool allow = true)
    {
        Nullable = allow;
        return this;
    }

    /// <summary>
    /// Choose whether or not the string is trimmed before testing
    /// </summary>
    public StringValidator Trim(bool allow = true)
    {
        UseTrim = allow;
        return this;
    }

    /// <summary>
    /// Provide a callback to run on a success
    /// </summary>
    public StringValidator OnSuccess(Action<string?>? successAction)
    {
        SuccessAction = successAction;
        return this;
    }

    /// <summary>
    /// Provide a callback to run on a failure
    /// </summary>
    public StringValidator OnFailure(Action<string?>? failureAction)
    {
        FailureAction = failureAction;
        return this;
    }
}
