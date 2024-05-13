using Microsoft.Extensions.Configuration;
using UPB.BusinessLogic.Managers;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration.AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json"
, false, false).Build();


// Registramos el servicio de almacenamiento de pacientes
string patientsFilePath = configuration.GetSection("FileStorage").GetSection("PatientsFilePath").Value;
//Console.WriteLine(patientsFilePath);

builder.Services.AddSingleton<IFileStorageService, FileStorageService>(provider =>
    new FileStorageService(patientsFilePath));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<IPatientManager, PatientManager>();
// Agregar HttpClient como servicio
builder.Services.AddHttpClient();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseLoggingMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (app.Environment.EnvironmentName == "QA")
{
    Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs\\log.txt")
    .CreateLogger();
    Log.Information("Server initialized......!!!!!");


    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (app.Environment.EnvironmentName == "UAT")
{
    Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs\\log.txt")
    .CreateLogger();
    Log.Information("Server initialized......!!!!!");

    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.Run();
