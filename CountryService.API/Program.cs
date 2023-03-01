using Calzolari.Grpc.AspNetCore.Validation;
using CountryService.API.Interceptors;
using CountryService.API.Services;
using CountryService.API.Validators;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.MaxReceiveMessageSize= 6291456; 
    options.MaxSendMessageSize= 6291456;
    options.ResponseCompressionLevel = CompressionLevel.Optimal;
    options.EnableDetailedErrors= true;
    options.IgnoreUnknownServices= true;
    options.Interceptors.Add<ExceptionInterceptor>();
    options.EnableMessageValidation();
});

builder.Services.AddValidator<CountryCreateRequestValidator>();
builder.Services.AddValidators();
builder.Services.AddGrpcValidation();

builder.Services.AddGrpcReflection();        
builder.Services.AddSingleton<CountryManagementService>();
builder.Services.AddSingleton<ProtoService>();

var app = builder.Build();

app.MapGrpcReflectionService();
app.MapGrpcService<CountryGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
app.MapGet("/protos", (ProtoService protoService) =>
{
    return Results.Ok(protoService.GetAll());
});
app.MapGet("/protos/v{version:int}/{protoName}", (ProtoService protoService, int version, string protoName) =>
{
    var filePath = protoService.Get(version, protoName);
    if (filePath != null)
        return Results.File(filePath);
    return Results.NotFound();
});
app.MapGet("/protos/v{version:int}/{protoName}/view", async (ProtoService protoService, int version, string protoName) =>
{
    var text = await protoService.ViewAsync(version, protoName);
    if (!string.IsNullOrEmpty(text))
        return Results.Text(text);
    return Results.NotFound();
});
app.Run();
