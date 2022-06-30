namespace API.Contraints;

/// <summary>
/// HTTP route constraint to ensure the provided value is a snowflake
/// </summary>
public class SnowflakeConstraint : IHttpRouteConstraint, IParameterPolicy
{
    public bool Match(
        HttpRequestMessage request,
        IHttpRoute route,
        string parameterName,
        IDictionary<string, object> values,
        HttpRouteDirection routeDirection)
    {
        if (values.TryGetValue(parameterName, out object? value) && value != null)
        {
            string? valueString = Convert.ToString(value);

            if (valueString is null)
                return false;

            return Snowflake.Validate(valueString);
        }
        
        return false;
    }
}
