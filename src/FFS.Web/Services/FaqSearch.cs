namespace FFS.Web.Services;

public sealed record FaqEntry(string Question, string Answer);
public sealed record FaqHit(FaqEntry Entry, int Score);

public static class FaqSearch
{
    private static readonly HashSet<string> Stop = new(StringComparer.OrdinalIgnoreCase)
    {
        "how", "do", "i", "my", "a", "an", "the", "to", "is", "of", "for", "does", "can",
        "what", "where", "and", "it", "in", "on", "you", "your", "me", "we", "this", "that",
        "with", "from", "or", "be", "if", "are", "am", "was", "were", "have", "has"
    };

    private static readonly string[][] Synonyms =
    [
        ["import", "upload", "csv", "pdf", "statement"],
        ["export", "download", "backup"],
        ["lock", "fingerprint", "biometric", "pin"],
        ["delete", "wipe", "erase"],
        ["scan", "receipt", "ocr"],
        ["notification", "alert", "alerts", "notifications"],
        ["sync", "website"],
        ["update", "apk"],
        ["store", "stored", "storage", "database"]
    ];

    public static List<FaqEntry> Parse(string text)
    {
        var items = new List<FaqEntry>();
        string? question = null;
        var body = new List<string>();

        void Flush()
        {
            if (string.IsNullOrWhiteSpace(question)) return;
            items.Add(new FaqEntry(question.Trim(), string.Join("\n", body).Trim()));
            body.Clear();
        }

        foreach (var line in text.Replace("\r\n", "\n").Split('\n'))
        {
            if (line.StartsWith("# ", StringComparison.Ordinal))
            {
                Flush();
                question = line[2..];
                continue;
            }

            if (question is not null)
                body.Add(line);
        }

        Flush();
        return items;
    }

    public static List<string> Tokens(string text)
    {
        var raw = new string(text.Select(c => char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : ' ').ToArray());
        return raw.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length > 1 && !Stop.Contains(t))
            .Distinct()
            .ToList();
    }

    public static IReadOnlyList<FaqHit> Rank(IReadOnlyList<FaqEntry> items, string query)
    {
        var tokens = Tokens(query);
        if (tokens.Count == 0)
            return items.Select(i => new FaqHit(i, 0)).ToList();

        var hits = new List<FaqHit>();
        foreach (var item in items)
        {
            var question = Tokens(item.Question);
            var answer = Tokens(item.Answer);
            var score = tokens.Sum(token => ScoreToken(token, question, answer));
            if (score > 0)
                hits.Add(new FaqHit(item, score));
        }

        hits.Sort((a, b) => b.Score.CompareTo(a.Score));
        return hits;
    }

    private static int ScoreToken(string token, List<string> question, List<string> answer)
    {
        var questionScore = Match(token, question, inQuestion: true);
        var answerScore = Match(token, answer, inQuestion: false);
        return Math.Max(questionScore, answerScore);
    }

    private static int Match(string token, List<string> words, bool inQuestion)
    {
        var exact = inQuestion ? 8 : 3;
        var synonym = inQuestion ? 6 : 2;
        var stem = inQuestion ? 5 : 2;
        var fuzzy = inQuestion ? 4 : 1;

        foreach (var word in words)
        {
            if (word.Equals(token, StringComparison.Ordinal)) return exact;
        }

        var group = Group(token);
        if (group is not null)
        {
            foreach (var word in words)
            {
                if (group.Contains(word) || Group(word) == group) return synonym;
            }
        }

        foreach (var word in words)
        {
            if (SharesStem(token, word)) return stem;
        }

        if (token.Length >= 5)
        {
            foreach (var word in words)
            {
                if (word.Length >= 5 && EditDistance(token, word) <= 1) return fuzzy;
            }
        }

        return 0;
    }

    private static HashSet<string>? Group(string token)
    {
        foreach (var group in Synonyms)
        {
            if (group.Contains(token, StringComparer.Ordinal))
                return new HashSet<string>(group, StringComparer.Ordinal);
        }

        return null;
    }

    private static bool SharesStem(string a, string b)
    {
        var shortOne = a.Length <= b.Length ? a : b;
        var longOne = a.Length <= b.Length ? b : a;
        return shortOne.Length >= 4 && longOne.StartsWith(shortOne, StringComparison.Ordinal);
    }

    private static int EditDistance(string a, string b)
    {
        if (Math.Abs(a.Length - b.Length) > 1) return 2;
        var i = 0;
        var j = 0;
        var edits = 0;
        while (i < a.Length && j < b.Length)
        {
            if (a[i] == b[j])
            {
                i++;
                j++;
                continue;
            }

            edits++;
            if (edits > 1) return edits;
            if (i + 1 < a.Length && j + 1 < b.Length && a[i] == b[j + 1] && a[i + 1] == b[j])
            {
                i += 2;
                j += 2;
                continue;
            }

            if (a.Length > b.Length) i++;
            else if (b.Length > a.Length) j++;
            else { i++; j++; }
        }

        if (i < a.Length || j < b.Length) edits++;
        return edits;
    }
}
