using Application;
using Endpoint.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddScoped<LibraryService>();
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.MapGrpcService<LibraryGrpcService>();
app.MapGrpcReflectionService();

app.Run();