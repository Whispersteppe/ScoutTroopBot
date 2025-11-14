using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScoutBot.Database;
using ScoutBot_Test.Database;
using Xunit.Abstractions;


namespace ScoutBot_Test.Database;

[CollectionDefinition("Shared Database Container")]
public class SharedDatabaseContainer : ICollectionFixture<SbotDbContextContainer>
{
    // This class has no code; its purpose is to define the collection and link to the fixture.
}

public class SbotDbContextContainer : IDisposable
{
    public SbotDbContextContainer()
    {
        var dbContext = GetContext();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }


    private DbContextOptions<SbotDbContext> _dbContextOptions;
    private DbContextOptions<SbotDbContext> DbContextOptions
    {
        get
        {
            if (_dbContextOptions == null)
            {

                SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder()
                {
                    DataSource = "testDb.sqlite",
                    Mode = SqliteOpenMode.ReadWriteCreate
                };

                var optionsBuilder = new DbContextOptionsBuilder<SbotDbContext>();
                optionsBuilder.UseSqlite(sqliteConnectionStringBuilder.ToString());

                _dbContextOptions = optionsBuilder.Options;
            }
            return _dbContextOptions;
        }
    }

    public SbotDbContext GetContext()
    {
        var context = new SbotDbContext(DbContextOptions);

        return context;
    }

    XUnitLoggerProvider _loggerProvider;

    public ILogger<T> GetLogger<T>(ITestOutputHelper output)
    {
        if (_loggerProvider == null)
        {
            _loggerProvider = new XUnitLoggerProvider(output);
        }

        var logger = _loggerProvider.CreateLogger<T>();

        return logger;
    }

    public void Dispose()
    {
    }
}