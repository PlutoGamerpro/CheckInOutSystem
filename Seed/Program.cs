using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

const string defaultBase = "http://localhost:5169";
var baseUrl = ParseBaseUrl(args) ?? defaultBase;
Console.WriteLine($"[Seed] Base URL: {baseUrl}");

var http = new HttpClient { BaseAddress = new Uri(AppendSlash(baseUrl)) };
var jsonOpts = new JsonSerializerOptions(JsonSerializerDefaults.Web);

// Config do usuário seed
var phone = "12345678";
var userName = "Seed User";
var isAdmin = false;

try
{
    // 1. Usuário
    var user = await GetUserByPhone(phone);
    if (user is null)
    {
        Console.WriteLine("[Seed] Criando usuário...");
        await Post("/api/User", new { name = userName, phone, isAdmin });
        user = await GetUserByPhone(phone) ?? throw new Exception("Falha ao obter usuário recém-criado.");
    }
    Console.WriteLine($"[Seed] UserId={user.Id}");

    // 2. Verificar se já existem registros no ano
    var yearRegs = await GetYearRegistrationsCount();
    Console.WriteLine($"[Seed] Registros (ano) existentes: {yearRegs}");

    if (yearRegs > 0)
    {
        Console.WriteLine("[Seed] Já existem registros no período do ano. Nenhum seed criado.");
        return;
    }

    Console.WriteLine("[Seed] Nenhum registro do ano encontrado. Criando semana atrás e ontem...");

    // Base temporal (UTC)
    var baseDate = DateTime.UtcNow.Date;

    var sessions = new[] 
    {
        new SessionSeed(
            Start: baseDate.AddDays(-7).AddHours(8).AddMinutes(30),
            End:   baseDate.AddDays(-7).AddHours(17),
            Label: "WeekAgo"),
        new SessionSeed(
            Start: baseDate.AddDays(-1).AddHours(8).AddMinutes(30),
            End:   baseDate.AddDays(-1).AddHours(17),
            Label: "Yesterday")
    };

    // 3. Criar cada sessão
    foreach (var s in sessions)
    {
        Console.WriteLine($"\n[Seed] Sessão {s.Label}: {s.Start:o} -> {s.End:o}");

        var checkInObj = await Post("/api/CheckIn", new {
            timeStart = s.Start.ToString("o"),
            fkUserId = user.Id
        });
        int checkInId = checkInObj?.GetPropertyOrDefault<int>("id") ?? 0;
        Console.WriteLine($"[Seed] CheckIn Id={checkInId}");

        var checkOutObj = await Post("/api/CheckOut", new {
            timeEnd = s.End.ToString("o"),
            fkUserId = user.Id
        });
        int checkOutId = checkOutObj?.GetPropertyOrDefault<int>("id") ?? 0;
        Console.WriteLine($"[Seed] CheckOut Id={checkOutId}");

        var regObj = await Post("/api/Registration", new {
            fkCheckInId = checkInId,
            fkCheckOutId = checkOutId,
            fkUserId = user.Id,
            timeStart = s.Start.ToString("o")
        });
        int regId = regObj?.GetPropertyOrDefault<int>("id") ?? 0;
        Console.WriteLine($"[Seed] Registration Id={regId}");
    }

    Console.WriteLine("\n[Seed] Concluído.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[Seed][Erro] " + ex.Message);
    Console.ResetColor();
}

#region Métodos auxiliares

async Task<int> GetYearRegistrationsCount()
{
    try
    {
        var resp = await http.GetAsync("/api/admin/registrations/year");
        if (!resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"[Seed] Aviso: falha ao consultar /year ({(int)resp.StatusCode}). Assumindo 0.");
            return 0;
        }
        var txt = await resp.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(txt)) return 0;
        try
        {
            using var doc = JsonDocument.Parse(txt);
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
                return doc.RootElement.GetArrayLength();
        }
        catch { }
        return 0;
    }
    catch { return 0; }
}

async Task<UserDto?> GetUserByPhone(string ph)
{
    var resp = await http.GetAsync($"/api/User/byphone/{ph}");
    if (resp.StatusCode == HttpStatusCode.NotFound) return null;
    if (!resp.IsSuccessStatusCode)
        throw new Exception($"Falha GET User/byphone ({(int)resp.StatusCode})");
    return await resp.Content.ReadFromJsonAsync<UserDto>(jsonOpts);
}

async Task<JsonElement?> Post(string path, object body)
{
    using var resp = await http.PostAsJsonAsync(path, body, jsonOpts);
    var content = await resp.Content.ReadAsStringAsync();
    if (!resp.IsSuccessStatusCode)
        throw new Exception($"POST {path} falhou ({(int)resp.StatusCode}) => {content}");
    if (string.IsNullOrWhiteSpace(content)) return null;
    try { return JsonSerializer.Deserialize<JsonElement>(content, jsonOpts); }
    catch { return null; }
}

static string? ParseBaseUrl(string[] args)
{
    for (int i = 0; i < args.Length - 1; i++)
        if (args[i].Equals("--baseUrl", StringComparison.OrdinalIgnoreCase))
            return args[i + 1];
    return null;
}

static string AppendSlash(string url) => url.EndsWith("/") ? url : url + "/";

record SessionSeed(DateTime Start, DateTime End, string Label);

class UserDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public bool IsCheckedIn { get; set; }
    public bool IsAdmin { get; set; }
}

static class JsonElementExtensions
{
    public static T? GetPropertyOrDefault<T>(this JsonElement element, string name)
    {
        if (element.ValueKind != JsonValueKind.Object) return default;
        if (!element.TryGetProperty(name, out var prop)) return default;
        try { return JsonSerializer.Deserialize<T>(prop.GetRawText()); }
        catch { return default; }
    }
}

#endregion
