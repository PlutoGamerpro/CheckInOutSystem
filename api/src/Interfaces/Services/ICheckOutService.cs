using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;

namespace TimeRegistration.Services
{
    public record CheckOutResult(int CheckOutId, string? Name, string Phone);
    
    public interface ICheckOutService
    {
        IEnumerable<CheckOut> GetAllCheckOuts();
        CheckOut GetCheckOutById(int id);
        CheckOutResult CreateCheckOut(string? tlf);
        CheckOut UpdateCheckOut(int id, CheckOut checkOut);
        void DeleteCheckOut(int id);


    }
}