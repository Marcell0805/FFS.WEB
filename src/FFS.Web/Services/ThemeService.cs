namespace FFS.Web.Services;

public sealed class ThemeService
{
    public bool IsDark { get; private set; } = true;

    public event Action? Changed;

    public void SetDark(bool isDark)
    {
        if (IsDark == isDark) return;
        IsDark = isDark;
        Changed?.Invoke();
    }

    public void Toggle() => SetDark(!IsDark);
}
