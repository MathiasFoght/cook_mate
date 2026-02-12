using CookMate_project.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ServiceCollection(builder.Configuration);

var app = builder.Build();

app.UsePipeline();
await app.RunStartupAsync();

app.Run();
