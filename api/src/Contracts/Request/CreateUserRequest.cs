using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace TimeRegistration.Contracts.Requests
{
   public record CreateUserRequest(
      [property: JsonPropertyName("name")] string? Name, 
      [property: JsonPropertyName("phone")] string? Phone, 
      [property: JsonPropertyName("countryCode")] string? CountryCode, 
      [property: JsonPropertyName("isAdmin")] bool? IsAdmin, 
    //  [property: JsonPropertyName("isManager")] bool IsManager, 
      [property: JsonPropertyName("password")] string? Password
   );
}