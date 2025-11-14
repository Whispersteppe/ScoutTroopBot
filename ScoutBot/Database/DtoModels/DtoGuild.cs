using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DtoModels;

public class DtoGuild
{
    public required ulong DiscordGuildID { get; set; }
    public string GuildName { get; set; }

    public List<DtoGuildUser> Users { get; set; }
    public List<DtoUnit> Units { get; set; }
}

