namespace FFS.Web.Services;

public sealed class UserSessionService
{
    public string? Username { get; private set; }

    public bool IsSignedIn => !string.IsNullOrWhiteSpace(Username);

    public bool PromptOpen { get; private set; }

    public string Initial =>
        IsSignedIn ? char.ToUpperInvariant(Username!.Trim()[0]).ToString() : "";

    public event Action? Changed;

    public void OpenPrompt()
    {
        if (PromptOpen) return;
        PromptOpen = true;
        Changed?.Invoke();
    }

    public void ClosePrompt()
    {
        if (!PromptOpen) return;
        PromptOpen = false;
        Changed?.Invoke();
    }

    public void SignIn(string username)
    {
        var name = username.Trim();
        if (name.Length == 0 || name == Username) return;
        Username = name;
        Changed?.Invoke();
    }

    public void SignOut()
    {
        if (!IsSignedIn) return;
        Username = null;
        Changed?.Invoke();
    }
}
