using Microsoft.Extensions.Logging;
using ScoutBot.Database;
using Xunit.Abstractions;

namespace ScoutBot_Test.Database;

[Collection("Shared Database Container")]
public class SbotDatabaseBase
{
    SbotDbContextContainer _dbContextContainer;
    ITestOutputHelper _output;
    public SbotDatabaseBase(SbotDbContextContainer dbContextContainer, ITestOutputHelper output)
    {
        _dbContextContainer = dbContextContainer;
        _output = output;
    }

    public ITestOutputHelper Output => _output;

    public SbotDbContext GetContext()
    {
        return _dbContextContainer.GetContext();
    }

    public ILogger<T> GetLogger<T>(ITestOutputHelper output)
    {
        return _dbContextContainer.GetLogger<T>(output);
    }
}