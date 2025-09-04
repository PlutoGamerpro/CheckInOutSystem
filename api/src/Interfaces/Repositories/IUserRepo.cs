using TimeRegistration.Classes;

namespace TimeRegistration.Interfaces;

public interface IUserRepo
{
    public List<User> GetAll();
    public void Create(User user);


}
