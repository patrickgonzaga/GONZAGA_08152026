using FileProcessing.Api.Middlewares;
using FileProcessing.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IFileValidator, FileValidator>();
builder.Services.AddScoped<IFileProcessingService, FileProcessingService>();
builder.Services.AddSingleton<IFileProcessingTrackerService, FileProcessingTrackerService>();
builder.Services.AddScoped<ApiKeyMiddleware>();

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyMiddleware>();

app.Run();
