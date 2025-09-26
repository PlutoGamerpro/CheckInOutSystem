using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Contracts.Results;
namespace TimeRegistration.Services
{
   
    
    public interface ICheckOutService
    {    
        IEnumerable<CheckOut> GetAllCheckOuts();
        CheckOut GetCheckOutById(int id);
        CheckOutResult CreateCheckOut(string? tlf);
        CheckOut UpdateCheckOut(int id, CheckOut checkOut);
        void DeleteCheckOut(int id); 
    }
}