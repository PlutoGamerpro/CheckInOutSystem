namespace TimeRegistration.Classes;

public class Registration
{
    public int Id { get; set; }
    public int FkCheckInId { get; set; }
    public int? FkCheckOutId { get; set; } // nullable para permitir identificar sessão ainda aberta
}
