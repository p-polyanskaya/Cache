using Application;
using Endpoint.GrpcServices;
using RedisOperations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddScoped<LibraryService>();
builder.Services.AddScoped<RedisRepository>();
builder.Services.AddGrpcReflection();

var app = builder.Build();

app.MapGrpcService<LibraryGrpcService>();
app.MapGrpcReflectionService();

app.Run();