using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScoutBot.Database.DbModels;
using ScoutBot.Database.DtoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.Repositories;
public interface  IUnitRepository
{
    Task<DtoUnit?> GetUnit(int unitId);
    Task<DtoUnit?> GetUnit(ulong guildId, string name);
    Task<int> CreateUnit(DtoUnit dtoUnit);
    Task RemoveUnit(int unitId);
}

public class UnitRepository : IUnitRepository
{
    ILogger<UnitRepository> _logger;
    SbotDbContext _botDbContext;
    public UnitRepository(ILogger<UnitRepository> logger, SbotDbContext botDbContext)
    {
        _logger = logger;
        _botDbContext = botDbContext;
    }

    public async Task RemoveUnit(int unitId)
    {
        await _botDbContext.Units.Where(x => x.Id == unitId).ExecuteDeleteAsync();
    }

    public async Task<DtoUnit?> GetUnit(int unitId)
    {
        var unit = _botDbContext.Units
            .Where(x => x.Id == unitId)
            .Select(x => new DtoUnit()
            { 
                GuildId = x.GuildId, 
                Id = x.Id, 
                Name = x.Name
            })
            .FirstOrDefault();

        return unit;
    }

    public async Task<DtoUnit?> GetUnit(ulong guildId, string name)
    {
        var unit = await _botDbContext.Units
            .Where(x => x.GuildId == guildId && x.Name == name)
            .Select(x => new DtoUnit()
            {
                GuildId = x.GuildId,
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync();

        return unit;
    }

    public async Task<int> CreateUnit(DtoUnit dtoUnit)
    {
        var unit = new DbUnit()
        {
            GuildId = dtoUnit.GuildId,
            Name = dtoUnit.Name
        };

        _botDbContext.Units.Add(unit);
        await _botDbContext.SaveChangesAsync();

        return unit.Id;
    }

}
