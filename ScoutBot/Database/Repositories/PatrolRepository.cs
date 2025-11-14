using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.Repositories;

public interface  IPatrolRepository
{
    
}

public class PatrolRepository : IPatrolRepository
{
    ILogger<PatrolRepository> _logger;
    SbotDbContext _botDbContext;

    public PatrolRepository(ILogger<PatrolRepository> logger, SbotDbContext botDbContext)
    {
        _logger = logger;
        _botDbContext = botDbContext;
    }
}
