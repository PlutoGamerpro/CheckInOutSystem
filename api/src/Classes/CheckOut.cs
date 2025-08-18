namespace TimeRegistration.Classes;

public class CheckOut
{
    public int Id { get; set; }
    public DateTime TimeEnd { get; set; }
    public int FkUserId { get; set; }
}
