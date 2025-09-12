using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;

namespace TimeRegistration.Models
{
    public record ForceCheckoutRequest(DateTime? When, DateTime? CheckIn);
    public record AdminLoginRequest(string Phone, string Secret, string Password);

    public record UserRecord(int Id, User user);

    // addded
    public record DeleteAdminRecord(User user);
    public record UpdateRegistrationRecord(Registration Registration); // new apporch

    /*
    public class Models
    {

    }
    */
}