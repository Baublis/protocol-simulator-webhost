using NLog.Web;
using NLog;
using ProtocolSimulator.WebHost.Components;
using ProtocolSimulator.WebHost.Services.RadzenServices;
using ProtocolSimulator.WebHost;
using Radzen;

var logger = NLog.LogManager.Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

try
{
    logger.Info($"Запуск сервиса {AppDomain.CurrentDomain.FriendlyName}, версия {AppCommon.AppVersion}");

    var builder = WebApplication.CreateBuilder(args);

    // 1. NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // 2. Windows Service
    builder.Host.UseWindowsService();

    // 3. Blazor (Interactive Server)
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // 4. Radzen UI
    builder.Services.AddRadzenComponents();

    // 5. Radzen Services — обязательны для ваших компонентов
    builder.Services.AddScoped<DialogService>();
    builder.Services.AddScoped<NotificationService>();
    builder.Services.AddScoped<TooltipService>();
    builder.Services.AddScoped<ContextMenuService>();
    builder.Services.AddScoped<ClipboardService>();

    // 6. HttpClient
    builder.Services.AddHttpClient();

    // 7. Ваши бэкенд-сервисы (пока не регистрируем — создадим на следующем шаге)
    // builder.Services.AddSingleton<ServerService>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAntiforgery();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    logger.Info("ProtocolSimulator.WebHost успешно настроен");
    logger.Info($"Режим: {(app.Environment.IsDevelopment() ? "Разработка" : "Продакшн")}");

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Программа остановлена из-за необработанного исключения");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}