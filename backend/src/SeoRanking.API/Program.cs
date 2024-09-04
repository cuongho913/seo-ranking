using System.Reflection;
using SeoRanking.API.Infrastructure;
using SeoRanking.Infrastructure.Cache;
using SeoRanking.Infrastructure.Configuration;
using SeoRanking.Infrastructure.Swagger;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.LoadConfiguration();

builder.Services.AddControllers();
builder.Services.AddCoreServices();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("SeoRanking.Application")));
builder.Services.AddHttpClient();
builder.Services.AddSwaggerDoc();
builder.Services.AddCaching();
builder.Services.AddOptions();
builder.Services.AddSearchEngineOptions(configuration);

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials

app.UseSwaggerDoc();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();