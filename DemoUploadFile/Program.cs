using Azure.Identity;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

// Add services to the container.
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddSecretClient(vaultUri: new Uri("https://romik-kv.vault.azure.net/"));

    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        ManagedIdentityClientId = "9f49b200-a820-47bd-bf8f-709ecbc9b4c9",
    });

    clientBuilder.AddBlobServiceClient(new Uri("https://romikstorage.blob.core.windows.net/")).WithName("RoMikVersion");
        // .WithCredential(credential);


    clientBuilder.AddBlobServiceClient(serviceUri: new Uri("https://romikstorage.blob.core.windows.net/"));
    clientBuilder.UseCredential(credential);
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

app.Run();