namespace FFS.Api;

public sealed class EmailOutbox
{
    private readonly List<SentEmail> _sent = [];

    public IReadOnlyList<EmailTemplate> Templates { get; } =
    [
        new("monthly-cash-flow", "Money in, money out, and net for the month.", ["moneyIn", "moneyOut", "net"]),
        new("goal-progress", "How close a goal is to its target.", ["name", "saved", "target"]),
        new("custom", "A subject and body for the mailer that still has to be built.", ["subject", "body"])
    ];

    public IReadOnlyList<SentEmail> Sent => _sent;

    public EmailPreview Preview(EmailRequest request)
    {
        var template = Templates.FirstOrDefault(t => t.Id.Equals(request.Template, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Unknown template '{request.Template}'.");

        var data = Fields(request);
        var (subject, body) = template.Id switch
        {
            "monthly-cash-flow" => (
                "Your month in F.F.S",
                $"Money in {Value(data, "moneyIn")}. Money out {Value(data, "moneyOut")}. Net {Value(data, "net")}."),
            "goal-progress" => (
                $"{Value(data, "name")} is moving",
                $"{Value(data, "saved")} saved of {Value(data, "target")}."),
            _ => (
                string.IsNullOrWhiteSpace(request.Subject) ? Value(data, "subject") : request.Subject!,
                string.IsNullOrWhiteSpace(request.Body) ? Value(data, "body") : request.Body!)
        };

        return new EmailPreview(template.Id, request.To ?? "you@example.com", subject, body);
    }

    public SentEmail Send(EmailRequest request)
    {
        var preview = Preview(request);
        var sent = new SentEmail(preview.Template, preview.To, preview.Subject, preview.Body, DateTime.UtcNow);
        _sent.Add(sent);
        return sent;
    }

    public void Clear() => _sent.Clear();

    private static Dictionary<string, string> Fields(EmailRequest request)
    {
        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (request.Data is not null)
        {
            foreach (var pair in request.Data)
                data[pair.Key] = pair.Value;
        }

        Put(data, "moneyIn", request.MoneyIn);
        Put(data, "moneyOut", request.MoneyOut);
        Put(data, "net", request.Net);
        Put(data, "name", request.Name);
        Put(data, "saved", request.Saved);
        Put(data, "target", request.Target);
        return data;
    }

    private static void Put(Dictionary<string, string> data, string key, decimal? value)
    {
        if (value is decimal amount)
            data[key] = amount.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private static void Put(Dictionary<string, string> data, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            data[key] = value;
    }

    private static string Value(IReadOnlyDictionary<string, string> data, string key) =>
        data.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : "—";
}

public record EmailTemplate(string Id, string Description, IReadOnlyList<string> Fields);

public record EmailRequest(
    string Template,
    string? To,
    Dictionary<string, string>? Data,
    string? Subject,
    string? Body,
    decimal? MoneyIn,
    decimal? MoneyOut,
    decimal? Net,
    string? Name,
    decimal? Saved,
    decimal? Target);

public record EmailPreview(string Template, string To, string Subject, string Body);

public record SentEmail(string Template, string To, string Subject, string Body, DateTime SentAtUtc);
