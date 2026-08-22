using FluentValidation;
using Gonzaga.SalesDataProcessor.Api.Common.Behavior;
using Gonzaga.SalesDataProcessor.Api.Common.Endpoints;
using Gonzaga.SalesDataProcessor.Api.Common.Middlewares;
using Gonzaga.SalesDataProcessor.Api.Features.Sales.ListSalesReports;
using Gonzaga.SalesDataProcessor.Api.Features.Sales.ProcessSalesReport;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApiVersioning();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped<ApiKeyMiddleware>();
builder.Services.AddScoped<ExceptionHandlingMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

var versionSet = ApiVersioning.CreateVersionSet(app);
app.MapProcessSalesReport(versionSet);
app.MapListSalesReports(versionSet);

app.UseHttpsRedirection();

app.Run();
