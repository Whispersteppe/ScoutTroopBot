using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using ScoutBot.Database;
using Xunit.Abstractions;

namespace ScoutBot_Test.Database;

[Collection("Shared Database Container")]
public class SbotDbContextTests : SbotDatabaseBase
{

    public SbotDbContextTests(SbotDbContextContainer dbContextContainer, ITestOutputHelper output)
        :base(dbContextContainer, output)
    {
    }

    [Fact]
    public async Task FirstTest()
    {
        var logger = GetLogger<SbotDbContextTests>(Output);

        logger.LogInformation("this is a test logging");

        Output.WriteLine("here");
    }

    [Fact]
    public async Task GenerateToSqlServer()
    {

        var optionsBuilder = new DbContextOptionsBuilder<SbotDbContext>();
        optionsBuilder.UseSqlServer("Server=FlamingoHome;Database=DiscordBot;encrypt=false;Trusted_connection=true;");

        var dbContextOptions = optionsBuilder.Options;
        var context = new SbotDbContext(dbContextOptions);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

    }

}
