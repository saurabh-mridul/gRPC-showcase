using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using static CountryService.API.v1.CountryService;

Console.WriteLine("Starting gRPC Client...");

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var handler = new SocketsHttpHandler
{
    KeepAlivePingDelay = TimeSpan.FromSeconds(15),
    KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
    EnableMultipleHttp2Connections = true
};

var channel = GrpcChannel.ForAddress("https://localhost:7098", new GrpcChannelOptions
{
    LoggerFactory = loggerFactory,
    HttpHandler = handler
});
var countryClient = new CountryServiceClient(channel);
using var serverStreamingCall = countryClient.GetAll(new Empty());
await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"NAME- {response.Name}: DESC-{response.Description}");
}

Console.ReadKey();