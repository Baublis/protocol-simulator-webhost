using NLog;
using NLog.Web;
using ProtocolSimulator.WebHost.Components;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка NLog
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var logger = LogManager.Setup().GetCurrentClassLogger();

// 2. Поддержка запуска как Службы Windows (Windows Service)
builder.Host.UseWindowsService();

// 3. Добавляем сервисы Blazor (Interactive Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 4. Добавляем сервисы Radzen (UI компоненты)
builder.Services.AddRadzenComponents();

// 5. Регистрация HttpClient (пригодится для запросов к API)
builder.Services.AddHttpClient();

var app = builder.Build();

// 6. Конфигурация HTTP пайплайна
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

logger.Info($"Запуск сервиса {AppDomain.CurrentDomain.FriendlyName}");

app.Run();