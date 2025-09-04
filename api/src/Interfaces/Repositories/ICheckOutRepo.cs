using TimeRegistration.Classes;

namespace TimeRegistration.Interfaces;

public interface ICheckOutRepo
{
    IEnumerable<CheckOut> GetAll();
    CheckOut? Get(int id);
    CheckOut Create(CheckOut checkOut);
    CheckOut? Update(int id, CheckOut checkOut);
    CheckOut? Delete(int id);

}
