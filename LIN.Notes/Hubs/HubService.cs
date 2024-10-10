namespace LIN.Notes.Hubs;

public class HubService(IHubContext<NotesHub> hubContext)
{

    /// <summary>
    /// Notificar nueva nota.   
    /// </summary>
    /// <param name="profile">Id del perfil.</param>
    /// <param name="noteId">Id de la nota.</param>
    public async Task NewNote(int profile, int noteId)
    {
        // Enviar.
        await hubContext.Clients.Group($"group.{profile}").SendAsync("#command", new CommandModel()
        {
            Command = $"add({noteId})"
        });
    }


    /// <summary>
    /// Actualizar nota.
    /// </summary>
    /// <param name="profile">Id del perfil.</param>
    /// <param name="noteId">Id de la nota.</param>
    public async Task UpdateNote(int profile, int noteId)
    {
        // Enviar.
        await hubContext.Clients.Group($"group.{profile}").SendAsync("#command", new CommandModel()
        {
            Command = $"update({noteId})"
        });
    }


    /// <summary>
    /// Actualizar nota.
    /// </summary>
    /// <param name="profile">Id del perfil.</param>
    /// <param name="noteId">Id de la nota.</param>
    public async Task UpdateNoteColor(int profile, int noteId, int color)
    {
        // Enviar.
        await hubContext.Clients.Group($"group.{profile}").SendAsync("#command", new CommandModel()
        {
            Command = $"updateColor({noteId}, {color})"
        });
    }


    /// <summary>
    /// Eliminar nota.
    /// </summary>
    /// <param name="profile">Id del perfil.</param>
    /// <param name="noteId">Id de la nota.</param>
    public async Task DeleteNote(int profile, int noteId)
    {
        // Enviar.
        await hubContext.Clients.Group($"group.{profile}").SendAsync("#command", new CommandModel()
        {
            Command = $"delete({noteId})"
        });
    }

}