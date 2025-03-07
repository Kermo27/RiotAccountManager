using System.Reflection;
using Kunc.RiotGames.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiotAccountManager.MAUI.Data.Repositories;
using RiotAccountManager.MAUI.Services.EncryptionService;
using RiotAccountManager.MAUI.Services.LcuService;
using RiotAccountManager.MAUI.Services.MatchHistoryService;
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
        builder.Services.AddSingleton<AccountRepository>();
        builder.Services.AddSingleton<IPreferences>(Preferences.Default);

        builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
        builder.Services.AddScoped<IRiotClientService, RiotClientService>();
        builder.Services.AddScoped<IRiotClientProcessService, RiotClientProcessService>();
        builder.Services.AddScoped<IRiotClientLockfileService, RiotClientLockfileService>();
        builder.Services.AddScoped<IRiotClientUIAutomationService, RiotClientUIAutomationService>();
        builder.Services.AddScoped<ILcuService, LcuService>();
        builder.Services.AddSingleton<IMatchHistoryService, MatchHistoryService>();

        return builder.Build();
    }
}
