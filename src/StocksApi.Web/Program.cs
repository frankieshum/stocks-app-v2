using StocksApi.Core.Configuration;
using StocksApi.Core.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ILogger, Logger<StocksService>>();
builder.Services.AddScoped<IStocksService, StocksService>();
builder.Services.AddHttpClient<IStocksService, StocksService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.Get<AppSettings>().IexApiBaseUrl);
});
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TODO use exception handler

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
