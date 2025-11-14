using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DbModels;

public class DbProperty
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public List<DbGuildUserProperty> GuildUserProperties { get; set; }
}
