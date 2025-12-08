using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FISEI.ServiceDesk.Web.Services;

public class ToastService
{
    public event Func<ToastMessage, Task>? OnShow;

    public async Task ShowAsync(string message, ToastType type = ToastType.Info, string? title = null)
    {
        var toast = new ToastMessage
        {
            Type = type,
            Title = title ?? GetDefaultTitle(type),
            Message = message,
            HelpText = DateTime.Now.ToString("HH:mm:ss"),
            AutoHide = !(type == ToastType.Danger || type == ToastType.Warning)
        };

        if (OnShow != null)
        {
            await OnShow.Invoke(toast);
        }
    }

    public async Task ShowSuccessAsync(string message, string? title = null)
        => await ShowAsync(message, ToastType.Success, title ?? "Éxito");

    public async Task ShowErrorAsync(string message, string? title = null)
        => await ShowAsync(message, ToastType.Danger, title ?? "Error");

    public async Task ShowWarningAsync(string message, string? title = null)
        => await ShowAsync(message, ToastType.Warning, title ?? "Advertencia");

    public async Task ShowInfoAsync(string message, string? title = null)
        => await ShowAsync(message, ToastType.Info, title ?? "Información");

    private string GetDefaultTitle(ToastType type) => type switch
    {
        ToastType.Success => "Éxito",
        ToastType.Danger => "Error",
        ToastType.Warning => "Advertencia",
        ToastType.Info => "Información",
        _ => "Notificación"
    };
}

public class ToastMessage
{
    public ToastType Type { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? HelpText { get; set; }
    public bool AutoHide { get; set; } = true;
}

public enum ToastType
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Dark
}
