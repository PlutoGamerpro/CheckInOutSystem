using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeRegistration.Contracts.Requests
{
    public record ForceCheckoutRequest(DateTime? When, DateTime? CheckIn);
}
