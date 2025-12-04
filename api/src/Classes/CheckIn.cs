namespace TimeRegistration.Classes;

public class CheckIn
{

    public int Id { get; set; }
    public DateTime TimeStart { get; set; }
    
    public DateTime AllowedCheckOutTime { get; set; }
    public int FkUserId { get; set; }
}
