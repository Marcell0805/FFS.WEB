namespace FFS.Web.Services;

public sealed class DisplayPrefsService
{
    public bool ShowVsLast { get; private set; } = true;
    public bool Loaded { get; private set; }
    public event Action? Changed;

    public void SetShowVsLast(bool value, bool loaded = true)
    {
        Loaded = loaded;
        if (ShowVsLast == value && Loaded) return;
        ShowVsLast = value;
        Changed?.Invoke();
    }
}
