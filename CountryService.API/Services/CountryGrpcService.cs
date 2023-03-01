using CountryService.API.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static CountryService.API.v1.CountryService;

namespace CountryService.API.Services;

public class CountryGrpcService : CountryServiceBase
{
    private readonly ILogger<CountryGrpcService> logger;
    private readonly CountryManagementService countryManagementService;

    public CountryGrpcService(ILogger<CountryGrpcService> logger,
        CountryManagementService countryMangementService)
    {
        this.logger = logger;
        this.countryManagementService = countryMangementService;
    }

    public override async Task GetAll(Empty request, IServerStreamWriter<CountryReply> responseStream, ServerCallContext context)
    {
        var countries = await countryManagementService.GetAllAsync();
        foreach (var country in countries)
        {
            await responseStream.WriteAsync(country);
        }
        await Task.CompletedTask;
    }

    public override async Task<CountryReply> Get(CountryIdRequest request, ServerCallContext context)
    {
        return await countryManagementService.GetAsync(request);
    }

    public override async Task Create(IAsyncStreamReader<CountryCreationRequest> requestStream, IServerStreamWriter<CountryCreationReply> responseStream, ServerCallContext context)
    {
        var countryCreationRequestList = new List<CountryCreationRequest>();
        await foreach (var countryCreationRequest in requestStream.ReadAllAsync())
        {
            countryCreationRequestList.Add(countryCreationRequest);
        }
        var createdCountries = await countryManagementService.CreateAsync(countryCreationRequestList);
        foreach (var country in createdCountries)
        {
            await responseStream.WriteAsync(country);
        }
    }

    public override async Task<Empty> Update(CountryUpdateRequest request, ServerCallContext context)
    {
        await countryManagementService.UpdateAsync(request);
        return new Empty();
    }

    public override async Task<Empty> Delete(IAsyncStreamReader<CountryIdRequest> requestStream, ServerCallContext context)
    {
        var countryIdRequestList = new List<CountryIdRequest>();
        await foreach (var countryIdRequest in requestStream.ReadAllAsync())
        {
            countryIdRequestList.Add(countryIdRequest);
        }
        await countryManagementService.DeleteAsync(countryIdRequestList);
        return new Empty();
    }
}