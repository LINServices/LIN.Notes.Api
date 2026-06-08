using Microsoft.EntityFrameworkCore;

namespace LIN.Notes.Persistence.Access;

public class Movements(DataContext context)
{
    public async Task<CreateResponse> Create(MovementDataModel data)
    {
        try
        {
            data.Id = 0;
            data.Profile = null!;
            context.Movements.Add(data);
            await context.SaveChangesAsync();
            return new(Responses.Success, data.Id);
        }
        catch (Exception) { }
        return new();
    }

    public async Task<ReadAllResponse<MovementDataModel>> ReadAll(int profileId)
    {
        try
        {
            var movements = await context.Movements
                .Where(m => m.ProfileId == profileId)
                .Select(m => new MovementDataModel
                {
                    Id = m.Id,
                    Amount = m.Amount,
                    Type = m.Type,
                    Date = m.Date,
                    ProfileId = m.ProfileId
                })
                .ToListAsync();
            return new(Responses.Success, movements);
        }
        catch (Exception) { }
        return new();
    }

    public async Task<ReadOneResponse<int>> GetProfileId(int id)
    {
        try
        {
            var profileId = await context.Movements
                .Where(m => m.Id == id)
                .Select(m => m.ProfileId)
                .FirstOrDefaultAsync();
            return profileId > 0 ? new(Responses.Success, profileId) : new(Responses.NotRows);
        }
        catch (Exception) { }
        return new();
    }

    public async Task<ResponseBase> Delete(int id)
    {
        try
        {
            var res = await context.Movements.Where(m => m.Id == id).ExecuteDeleteAsync();
            return res <= 0 ? new(Responses.NotRows) : new(Responses.Success);
        }
        catch (Exception) { }
        return new();
    }
}
