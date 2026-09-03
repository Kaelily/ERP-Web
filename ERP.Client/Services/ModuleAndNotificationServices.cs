using ERP.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ERP.Client.Services;

public class ModuleStateService
{
    public ModuloSistema ActiveModule { get; private set; } = ModuloSistema.Comercial;

    public event Action? OnModuleChanged;

    public void SetActiveModule(ModuloSistema module)
    {
        if (ActiveModule != module)
        {
            ActiveModule = module;
            OnModuleChanged?.Invoke();
        }
    }
}

public class SignalRNotificationService : IAsyncDisposable
{
    private readonly NavigationManager _nav;
    private HubConnection? _hubConnection;
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public event Action<string, string, string>? OnNotificationReceived;
    public event Action<int, string>? OnMailingUpdated;
    public event Action? OnConnectionStatusChanged;

    public SignalRNotificationService(NavigationManager nav)
    {
        _nav = nav;
    }

    public async Task StartAsync()
    {
        try
        {
            if (_hubConnection != null) return;

            var hubUrl = _nav.ToAbsoluteUri("hub/notifications");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, string, string, DateTime>("ReceiveNotification", (user, msg, type, date) =>
            {
                OnNotificationReceived?.Invoke(user, msg, type);
            });

            _hubConnection.On<int, string, DateTime>("MailingUpdated", (id, action, date) =>
            {
                OnMailingUpdated?.Invoke(id, action);
            });

            _hubConnection.Reconnecting += _ =>
            {
                OnConnectionStatusChanged?.Invoke();
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += _ =>
            {
                OnConnectionStatusChanged?.Invoke();
                return Task.CompletedTask;
            };

            _hubConnection.Closed += _ =>
            {
                OnConnectionStatusChanged?.Invoke();
                return Task.CompletedTask;
            };

            await _hubConnection.StartAsync();
            OnConnectionStatusChanged?.Invoke();
        }
        catch
        {
            // Silently swallow SignalR connection failures (e.g. static GitHub Pages hosting)
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
