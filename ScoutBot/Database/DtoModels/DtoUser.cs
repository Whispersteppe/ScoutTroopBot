using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DtoModels;

public class DtoUser
{
    public required ulong DiscordUserId { get; set; }
    public string Name { get; set; }

    public List<DtoGuildUser> GuildUsers { get; set; }
}
