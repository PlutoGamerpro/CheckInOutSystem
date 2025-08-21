namespace TimeRegistration.Classes;

public class Registration
{
    public int Id { get; set; }
    public int FkCheckInId { get; set; }
    public int? FkCheckOutId { get; set; } // NULLable for at tillade identifikation af session stadig åben
    public int FkUserId { get; set; } 
    public DateTime TimeStart { get; set; } = DateTime.UtcNow;

    // Navegação para permitir Include(r => r.User)
    public User? User { get; set; } // probably error this should not be added
}
