namespace LIN.Notes.Services.Model;

public class IamRequest
{

    public int Id { get; set; }

    public int Profile { get; set; }

    public IamBy IamBy { get; set; }

}

public enum IamBy
{
    Note,
    Access
}