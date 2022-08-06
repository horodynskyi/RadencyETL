// See https://aka.ms/new-console-template for more information


using ETL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Radency.ConsoleApp;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((hostContext,services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));
        services.AddTransient(ser =>
        {
            var appSettings = ser.GetRequiredService<IOptions<AppSettings>>().Value;
            if (appSettings is null || string.IsNullOrEmpty(appSettings.InputFolderPath) || string.IsNullOrEmpty(appSettings.OutputFolderPath))
                throw new ConfigSettingsIsEmptyException();
            return appSettings;
        });
        
        services.AddEtl();
    })
    .Build();
await host.RunAsync();
