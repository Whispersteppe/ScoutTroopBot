using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.Repositories;

public interface IUserRepository
{
    // Define user-related data operations here
}

public class UserRepository : IUserRepository
{
    ILogger<UserRepository> _logger;
    SbotDbContext _botDbContext;
    public UserRepository(ILogger<UserRepository> logger, SbotDbContext botDbContext)
    {
        _logger = logger;
        _botDbContext = botDbContext;
    }
}
