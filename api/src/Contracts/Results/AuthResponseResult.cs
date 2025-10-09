using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeRegistration.Contracts.Results
{
    public class AuthResponseResult
    {
        public string Token { get; set; } = "";
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; }
    }

}