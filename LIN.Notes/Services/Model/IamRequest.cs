namespace LIN.Notes.Services.Model;

internal class IamRequest
{

    public int Id { get; set; }

    public int Profile { get; set; }

    public IamBy IamBy { get; set; }

}

internal enum IamBy
{
    Note,
    Access
}