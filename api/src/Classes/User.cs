using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TimeRegistration.Classes;

public class User 
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    //The ONLY phone field exposed via API.
    // The physical column is still called "Phone" in the database (legacy). Perform a rename migration if you want to merge.
    [JsonPropertyName("phone")]
    [Column("Tlf")] // Fysisk kolonne forbliver Tlf
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

    [JsonPropertyName("isManager")]
    public bool IsManager { get; set; } = false;

    public string? Password { get; set; } // only admins and managers need a password 

    private static string? NormalizePhone(string? value) // this method also appers in other class dont rememeber the name
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return new string(value.Where(char.IsDigit).ToArray());
    }
}

