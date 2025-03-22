using RiotAccountManager.MAUI.Data.Repositories;
using RiotAccountManager.MAUI.Services.EncryptionService;
using RiotAccountManager.MAUI.Services.RiotApiService;
using RiotAccountManager.MAUI.Services.RiotClientService;
using WindowsInput;

namespace RiotAccountManager.MAUI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<AccountRepository>();
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddScoped<IRiotClientService, RiotClientService>();
        services.AddScoped<IRiotClientProcessService, RiotClientProcessService>();
        services.AddScoped<IRiotClientLockfileService, RiotClientLockfileService>();
        services.AddScoped<IRiotClientUiAutomationService, RiotClientUiAutomationService>();
        services.AddScoped<IRiotApiService, RiotApiService>();

        services.AddSingleton<IInputSimulator, InputSimulator>();
        
        services.AddMemoryCache();

        return services;
    }
}