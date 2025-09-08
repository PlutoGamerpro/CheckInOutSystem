using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeRegistration.Models
{
    public record ForceCheckoutRequest(DateTime? When, DateTime? CheckIn);
    public record AdminLoginRequest(string Phone, string Secret, string Password);

    /*
    public class Models
    {

    }
    */
}