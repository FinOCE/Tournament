namespace API.Services;

public class DbService
{
    private readonly string? ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

    /// <summary>
    /// Run a stored procedure
    /// </summary>
    /// <typeparam name="T">The type of the result of the callback</typeparam>
    /// <param name="procedure">The name of the procedure</param>
    /// <param name="parameters">The parameters necessary for the procedure</param>
    /// <param name="callback">The callback to run for each row of the result</param>
    /// <returns>An array containing the results of all callback calls</returns>
    /// <exception cref="ApplicationException"></exception>
    public T[] RunProcedure<T>(
        string procedure,
        Dictionary<string, object> parameters,
        Func<SqlDataReader, T> callback)
    {
        if (ConnectionString is null)
            throw new ApplicationException("Database connection string secret env is not set");

        T[] results = Array.Empty<T>();

        using SqlConnection connection = new(ConnectionString);
        connection.Open();

        SqlCommand command = new(procedure, connection);
        command.CommandType = CommandType.StoredProcedure;

        foreach (KeyValuePair<string, object> param in parameters)
            command.Parameters.AddWithValue(param.Key, param.Value);

        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
            results = results.Append(callback(reader)).ToArray();

        return results;
    }
}
