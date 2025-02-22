using Azure.Identity;
using Omni.Copilot.Options;
using Omni.Copilot.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.RegisterConfiguration();

// Remove Razor Pages and Blazor
// builder.Services.AddRazorPages();
// builder.Services.AddServerSideBlazor();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Azure Cosmos DB Aspire integration
var cosmosEndpoint = builder.Configuration.GetSection(nameof(CosmosDb)).GetValue<string>("Endpoint");
if (cosmosEndpoint is null)
{
    throw new ArgumentException($"{nameof(IOptions<CosmosDb>)} was not resolved through dependency injection.");
}
builder.AddAzureCosmosClient(
    "Omni-Copilot",
    settings =>
    {
        settings.AccountEndpoint = new Uri(cosmosEndpoint);
        settings.Credential = new DefaultAzureCredential();
        settings.DisableTracing = false;
    },
    clientOptions => {
        clientOptions.ApplicationName = "Omni-Copilot";
        clientOptions.UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        clientOptions.CosmosClientTelemetryOptions = new()
        {
            CosmosThresholdOptions = new()
            {
                PointOperationLatencyThreshold = TimeSpan.FromMilliseconds(10),
                NonPointOperationLatencyThreshold = TimeSpan.FromMilliseconds(20)
            }
        };
    });

// Configure OpenAI Aspire integration
var openAIEndpoint = builder.Configuration.GetSection(nameof(OpenAi)).GetValue<string>("Endpoint");
if (openAIEndpoint is null)
{
    throw new ArgumentException($"{nameof(IOptions<OpenAi>)} was not resolved through dependency injection.");
}
builder.AddAzureOpenAIClient("openAiConnectionName",
    configureSettings: settings =>
    {
        settings.Endpoint = new Uri(openAIEndpoint);
        settings.Credential = new DefaultAzureCredential();
    });

builder.Services.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

static class ProgramExtensions
{
    public static void RegisterConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<CosmosDb>()
            .Bind(builder.Configuration.GetSection(nameof(CosmosDb)));

        builder.Services.AddOptions<OpenAi>()
            .Bind(builder.Configuration.GetSection(nameof(OpenAi)));

        builder.Services.AddOptions<Chat>()
            .Bind(builder.Configuration.GetSection(nameof(Chat)));
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<CosmosDbService, CosmosDbService>();
        // services.AddSingleton<OpenAiService, OpenAiService>();
        services.AddSingleton<SemanticKernelService, SemanticKernelService>();
        services.AddSingleton<ChatService, ChatService>();
    }
}