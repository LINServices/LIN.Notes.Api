namespace LIN.Notes.Services;


public static class ServerLogger
{


    /// <summary>
    /// Hora de inicio del servidor
    /// </summary>
    public static DateTime OpenDate { get; set; } = new();



    /// <summary>
    /// Lista de conexiones abiertas
    /// </summary>
    public readonly static List<ConnectionLogger> OpenConnections = new();



    /// <summary>
    /// Lista de errores generados
    /// </summary>
    public readonly static List<string> Errors = new();




    /// <summary>
    /// Registra una colección
    /// </summary>
    public static void LogConnection(ConnectionLogger connectionLogger)
    {
        OpenConnections.Add(connectionLogger);
    }



    /// <summary>
    /// Registra una colección
    /// </summary>
    public static void LogError(string error)
    {
        Errors.Add(error);
    }



}

public class ConnectionLogger
{

    public int Number { get; set; }

    public ConnectionLogger(int number = -1)
    {
        Number = number;
    }


}
