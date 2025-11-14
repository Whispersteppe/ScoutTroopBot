using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DbModels;

public class DbPatrol
{
    public required int Id { get; set; }
    public required int UnitId { get; set; }
    public string Name { get; set; }

    public DbUnit Unit { get; set; }
    public List<DbPatrolMember> PatrolMembers { get; set; }

}
