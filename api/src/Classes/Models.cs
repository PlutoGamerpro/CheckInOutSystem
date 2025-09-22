using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;

namespace TimeRegistration.Models
{
    public record ForceCheckoutRequest(DateTime? When, DateTime? CheckIn);
    public record AdminLoginRequest(string Phone, string Secret, string Password); 
    public record DateRange(DateTime? startInclusiveUtc, DateTime? endExclusiveUtc); // ...not implemented
    public record UserRecordRequest(int Id, string Name, string Phone, bool IsAdmin);       
    public record CreateRegistrationRequest(int Id, int UserId, DateTime CheckIn, DateTime? CheckOut); // addded 
    public record DeleteAdminRecord(User user);
    public record UpdateRegistrationRecord(Registration Registration); // new apporch

    
}