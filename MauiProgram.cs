using System.Reflection;
using Kunc.RiotGames.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiotAccountManager.MAUI.Data.Repositories;
using RiotAccountManager.MAUI.Services.EncryptionService;
using RiotAccountManager.MAUI.Services.LcuService;
using RiotAccountManager.MAUI.Services.RiotClientService;

namespace RiotAccountManager.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        var assembly = Assembly.GetExecutingAssembly();
        using var stream =
            assembly.GetManifestResourceStream(
                "RiotAccountManager.MAUI.Resources.Raw.appsettings.json"
            ) ?? throw new FileNotFoundException("Can't find appsettings.json.");

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .Build();

        builder.Configuration.AddConfiguration(config);

        // Rejestracja serwisów Riot Games API
        builder.Services.AddRiotGamesApi(options =>
        {
            options.ApiKey = config["RiotAPI:ApiKey"];
        });

        builder.Services.AddSingleton<AccountRepository>();

        builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
        builder.Services.AddScoped<IRiotClientService, RiotClientService>();
        builder.Services.AddScoped<IRiotClientProcessService, RiotClientProcessService>();
        builder.Services.AddScoped<IRiotClientLockfileService, RiotClientLockfileService>();
        builder.Services.AddScoped<IRiotClientUIAutomationService, RiotClientUIAutomationService>();
        builder.Services.AddScoped<ILcuService, LcuService>();

        return builder.Build();
    }
}
