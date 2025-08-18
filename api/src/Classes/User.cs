using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TimeRegistration.Classes;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    // ÚNICO campo de telefone exposto via API.
    // A coluna física ainda se chama "Tlf" no banco (legado). Faça migration de rename se quiser unificar.
    [JsonPropertyName("phone")]
    [Column("Tlf")] // Coluna física permanece Tlf
    public string? Phone
    {
        get => _phone;
        set => _phone = NormalizePhone(value);
    }
    private string? _phone;

    [JsonPropertyName("isCheckedIn")]
    public bool IsCheckedIn { get; set; }

    [JsonPropertyName("isAdmin")]
    public bool IsAdmin { get; set; } = false;

    private static string? NormalizePhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return new string(value.Where(char.IsDigit).ToArray());
    }
}