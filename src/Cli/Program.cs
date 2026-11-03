using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Reports;
using Core.Services;
using Core.Storage;
using System.Reflection;
using Core.Configuration;
using Microsoft.Extensions.Configuration;

var envName = Environment.GetEnvironmentVariable("CROSSAPP_ENVIRONMENT") ?? "Development";

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{envName}.json", optional: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables(prefix: "CROSSAPP_")
    .AddCommandLine(args)
    .Build();

// Біндінг у міцно типізований клас AppOptions
var options = config.Get<AppOptions>() ?? new AppOptions();

// Окрема перевірка простішої змінної середовища CROSSAPP_DATA_PATH
var legacy = Environment.GetEnvironmentVariable("CROSSAPP_DATA_PATH");
var storage = new AppOptions.StorageOptions
{
    DataPath = string.IsNullOrWhiteSpace(legacy) ? options.Storage.DataPath : legacy,
    FileName = options.Storage.FileName
};

var apiKey = config["Integration:ApiKey"];

Console.WriteLine($"Файл даних: {storage.ResolveFullPath(AppContext.BaseDirectory)}");
Console.WriteLine(apiKey is null ? "ApiKey: не заданий" : $"ApiKey: присутній, довжина {apiKey.Length}");
Console.WriteLine($"Валюта замовлень: {options.Orders.Currency}");