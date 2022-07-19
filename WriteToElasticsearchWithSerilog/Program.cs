using Audit.Elasticsearch.Providers;
using Elasticsearch.Net;
using Nest;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
    loggerConfiguration
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://elastic:password@es01:9200"))
        {
            ModifyConnectionSettings = c => c
                .ConnectionLimit(-1)
                .ServerCertificateValidationCallback((o, certificate, arg3, arg4) => true),
            TypeName = null,
            AutoRegisterTemplate = true,
            IndexFormat = "WriteToElasticsearchWithSerilog-{0:yyyy-MM-dd}",
        }));
SelfLog.Enable(Console.Error);
// Add services to the container.

Audit.Core.Configuration.DataProvider = new ElasticsearchDataProvider()
{
    ConnectionSettings = new ConnectionSettings(new SingleNodeConnectionPool(new Uri("https://elastic:password@es01:9200")))
        .ConnectionLimit(-1)
        .ServerCertificateValidationCallback((o, certificate, arg3, arg4) => true),
    IndexBuilder = ev => ev.EventType.ToLower(),
    IdBuilder = ev => Guid.NewGuid()
};

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

app.UseAuthorization();

app.MapControllers();

app.Run();

