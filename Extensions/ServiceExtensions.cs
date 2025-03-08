using RiotAccountManager.MAUI.Data.Repositories;
using RiotAccountManager.MAUI.Services.EncryptionService;
using RiotAccountManager.MAUI.Services.RiotClientService;

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
        
        return services;
    }
}