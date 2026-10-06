using System.Text.Json;

namespace FFS.Web.Services;

public sealed class PaletteService
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public PaletteColors Colors { get; private set; } = new();
    public int Revision { get; private set; }
    public event Action? Changed;

    public void LoadJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return;
        try
        {
            var parsed = JsonSerializer.Deserialize<PaletteColors>(json, Json);
            if (parsed is null) return;
            Colors = parsed;
            Revision++;
        }
        catch (JsonException)
        {
            /* keep defaults */
        }
    }

    public void Publish() => Changed?.Invoke();

    public string ToJson() => JsonSerializer.Serialize(Colors, Json);

    public void Replace(PaletteColors colors)
    {
        Colors = colors;
        Revision++;
        Changed?.Invoke();
    }

    public void Reset() => Replace(new PaletteColors());
}
