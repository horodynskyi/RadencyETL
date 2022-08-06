using Microsoft.Extensions.DependencyInjection;

namespace ETL;

public static class DependencyInjection
{
    public static IServiceCollection AddEtl(this IServiceCollection service)
    {
        return service
            .AddTransient(ser => new LogSaver(ser.GetRequiredService<AppSettings>().OutputFolderPath))
            .AddTransient<IDirectoryReader, DirectoryReader>()
            .AddTransient<IEtl, Etl>();
    }
}