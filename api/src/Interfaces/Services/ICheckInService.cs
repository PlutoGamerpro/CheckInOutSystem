using System;
using System.Collections.Generic;
using TimeRegistration.Classes;
using TimeRegistration.Contracts.Results;
namespace TimeRegistration.Services
{
  
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