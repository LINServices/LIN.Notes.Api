using Microsoft.EntityFrameworkCore;

namespace LIN.Notes.Persistence.Access;

public class Tasks(DataContext context)
{
    public async Task<CreateResponse> Create(TaskDataModel data)
    {
        try
        {
            data.Id = 0;
            data.Note = null!;
            context.Tasks.Add(data);
            await context.SaveChangesAsync();
            return new(Responses.Success, data.Id);
        }
        catch (Exception) { }
        return new();
    }

    public async Task<ResponseBase> Update(TaskDataModel data)
    {
        try
        {
            await context.Tasks
                .Where(t => t.Id == data.Id)
                .ExecuteUpdateAsync(t => t
                    .SetProperty(a => a.Description, data.Description)
                    .SetProperty(a => a.IsCompleted, data.IsCompleted));
            return new(Responses.Success);
        }
        catch (Exception) { }
        return new();
    }

    public async Task<ReadOneResponse<int>> GetNoteId(int taskId)
    {
        try
        {
            var noteId = await context.Tasks
                .Where(t => t.Id == taskId)
                .Select(t => t.NoteId)
                .FirstOrDefaultAsync();
            return noteId > 0 ? new(Responses.Success, noteId) : new(Responses.NotRows);
        }
        catch (Exception) { }
        return new();
    }

    public async Task<ResponseBase> Delete(int id)
    {
        try
        {
            var res = await context.Tasks.Where(t => t.Id == id).ExecuteDeleteAsync();
            return res <= 0 ? new(Responses.NotRows) : new(Responses.Success);
        }
        catch (Exception) { }
        return new();
    }
}
