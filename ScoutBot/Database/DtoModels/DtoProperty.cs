using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DtoModels;

public class DtoProperty
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public List<DtoGuildUserProperty> GuildUserProperties { get; set; }
}
