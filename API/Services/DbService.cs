namespace API.Services;

public class DbService
{
    private readonly IConfiguration _Configuration;

    public DbService(IConfiguration configuration)
    {
        _Configuration = configuration;
    }

    /// <summary>
    /// Run a stored procedure
    /// </summary>
    /// <typeparam name="T">The type of the result of the callback</typeparam>
    /// <param name="procedure">The name of the procedure</param>
    /// <param name="parameters">The parameters necessary for the procedure</param>
    /// <param name="callback">The callback to run for each row of the result</param>
    /// <returns>An array containing the results of all callback calls</returns>
    /// <exception cref="ApplicationException"></exception>
    public async Task<T[]> RunProcedure<T>(
        string procedure,
        Dictionary<string, object> parameters,
        Func<SqlDataReader, T> callback)
    {
        if (_Configuration["DB_CONNECTION_STRING"] is null)
            throw new ApplicationException("Database connection string secret env is not set");

        T[] results = Array.Empty<T>();

        using SqlConnection connection = new(_Configuration["DB_CONNECTION_STRING"]);
        await connection.OpenAsync();

        SqlCommand command = new(procedure, connection)
        {
          CommandType = CommandType.StoredProcedure
        };
        
        foreach (KeyValuePair<string, object> param in parameters)
            command.Parameters.AddWithValue(param.Key, param.Value);

        using SqlDataReader reader = await command.ExecuteReaderAsync();
        while (reader.Read())
            results = results.Append(callback(reader)).ToArray();

        return results;
    }

    /// <summary>
    /// Run a SQL query
    /// </summary>
    /// <typeparam name="T">The type of the result of the callback</typeparam>
    /// <param name="query">The SQL query to be run</param>
    /// <param name="callback">The callback to run for each row of the result</param>
    /// <returns>An array containing the results of all callback calls</returns>
    /// <exception cref="ApplicationException"></exception>
    public async Task<T[]> RunQueryAsync<T>(
        string query,
        Func<SqlDataReader, T>? callback = null)
    {
        if (_Configuration["DB_CONNECTION_STRING"] is null)
            throw new ApplicationException("Database connection string secret env is not set");

        T[] results = Array.Empty<T>();

        using SqlConnection connection = new(_Configuration["DB_CONNECTION_STRING"]);
        await connection.OpenAsync();

        SqlCommand command = new(query.Trim(), connection);
        using SqlDataReader reader = await command.ExecuteReaderAsync();

        if (callback is not null)
            while (reader.Read())
                results = results.Append(callback(reader)).ToArray();
        
        return results;
    }
}
