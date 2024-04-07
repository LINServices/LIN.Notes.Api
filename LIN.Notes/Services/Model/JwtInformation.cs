namespace LIN.Notes.Services.Model;

public class JwtInformation
{
    public bool IsAuthenticated { get; set; }
    public int AccountId { get; set; }
    public int ProfileId { get; set; }
}