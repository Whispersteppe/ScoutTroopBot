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

public interface IGuildRepository
{
    Task<DtoGuild?> GetGuild(ulong guildId);
    Task CreateGuild(DtoGuild dtoGuild);
    Task RemoveGuild(ulong discordGuildId);
}
public class GuildRepository : IGuildRepository
{
    ILogger<GuildRepository> _logger;
    SbotDbContext _botDbContext;

    public GuildRepository(ILogger<GuildRepository> logger, SbotDbContext botDbContext)
    {
        _logger = logger;
        _botDbContext = botDbContext;
    }

    public async Task CreateGuild(ulong discordGuildId, string guildName)
    {
        var guild = new DbGuild()
        {
            DiscordGuildID = discordGuildId,
            GuildName = guildName
        };

        _botDbContext.Guilds.Add(guild);
        await _botDbContext.SaveChangesAsync();
    }

    public async Task RemoveGuild(ulong discordGuildId)
    {
        await _botDbContext.Guilds.Where(x => x.DiscordGuildID == discordGuildId).ExecuteDeleteAsync();
    }

    public async Task<DtoGuild?> GetGuild(ulong guildId)
    {
        var guild = await _botDbContext.Guilds
            .Where(x => x.DiscordGuildID == guildId)
            .Select(x => new DtoGuild()
                {
                    DiscordGuildID = x.DiscordGuildID,
                    GuildName = x.GuildName
                })
            .FirstOrDefaultAsync();

        return guild;
    }

    public async Task CreateGuild(DtoGuild dtoGuild)
    {
        var guild = new DbGuild()
        {
            DiscordGuildID = dtoGuild.DiscordGuildID,
            GuildName = dtoGuild.GuildName,
        };

        _botDbContext.Guilds.Add(guild);
        await _botDbContext.SaveChangesAsync();
    }
}
