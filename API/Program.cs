var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Set basic info about API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tournament Generator API",
        Version = "v1",
        Description = "An API for generating tournaments"
    });

    // Set the comments path for the Swagger JSON and UI
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddSingleton<SnowflakeService>();
builder.Services.AddSingleton<DbService>();
builder.Services.AddSingleton<LoggingService>();
builder.Services.AddHttpClient<CaptchaService>();

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("snowflake", typeof(SnowflakeConstraint));
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
