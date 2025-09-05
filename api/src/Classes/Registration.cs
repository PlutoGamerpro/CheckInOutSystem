namespace TimeRegistration.Classes;

public class Registration
{
    public int Id { get; set; }
    public int FkCheckInId { get; set; }
    public int? FkCheckOutId { get; set; } // NULLable to allow identification of session still open
    public int FkUserId { get; set; } 
    public DateTime TimeStart { get; set; } = DateTime.UtcNow;

    // Navigation to allow Include(r => r.User)
    public User? User { get; set; } // probably error this should not be added
}
