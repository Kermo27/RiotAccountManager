using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.Logging;
using RiotAccountManager.MAUI.Extensions;
using Serilog;

namespace RiotAccountManager.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        builder.Services.AddApplicationServices();

        builder.Logging.AddSerilog();

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("RiotAccountManager.MAUI.appsettings.json");
        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
        builder.Configuration.AddConfiguration(config);

        return builder.Build();
    }
}