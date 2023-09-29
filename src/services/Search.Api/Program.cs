var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<SyncAuctionService>()
    .AddPolicyHandler(policy =>
    {
        return HttpPolicyExtensions
             .HandleTransientHttpError()
             .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
             .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await app.InitializeDatabaseAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Search Api - An error occurred while applying pending migrations.");
    }
}); 

app.Run();
