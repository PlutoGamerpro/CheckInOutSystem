using System;
using System.Collections.Generic;
using TimeRegistration.Classes;

namespace TimeRegistration.Services
{
    // DTO returned by the service to prevent the controller from searching again
    public record CheckInResult(int CheckInId, string? Name, string Phone);

    public interface ICheckInService
    {
        IEnumerable<CheckIn> GetAllCheckIns();
        CheckIn GetCheckInById(int id);
        CheckInResult CreateCheckInByPhone(string tlf);
        CheckIn? UpdateCheckIn(int id, CheckIn checkIn);
        void DeleteCheckIn(int id);
        bool GetCheckInStatus(string tlf);
    }
}