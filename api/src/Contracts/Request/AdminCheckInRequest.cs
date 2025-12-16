using System;

namespace TimeRegistration.Contracts.Request
{
    public class AdminCheckInRequest
    {
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
    }
}
