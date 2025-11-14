using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DtoModels;

public class DtoPatrol
{
    public required int Id { get; set; }
    public required int UnitId { get; set; }
    public string Name { get; set; }

    public DtoUnit Unit { get; set; }
    public List<DtoPatrolMember> PatrolMembers { get; set; }

}
