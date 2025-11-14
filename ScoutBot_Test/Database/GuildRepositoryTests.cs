using FluentAssertions;
using ScoutBot.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ScoutBot_Test.Database;

[Collection("Shared Database Container")]
public class GuildRepositoryTests : SbotDatabaseBase
{
    public GuildRepositoryTests(SbotDbContextContainer dbContextContainer, ITestOutputHelper output) 
        : base(dbContextContainer, output)
    {
    }

    [Fact]
    public async Task CRUDGuild()
    {
        GuildRepository repository = new GuildRepository(GetLogger<GuildRepository>(Output), GetContext());

        await repository.CreateGuild(1, "test");

        var guild = await repository.GetGuild(1);

        guild.DiscordGuildID.Should().Be(1);
        guild.GuildName.Should().Be("test");

        await repository.RemoveGuild(1);

        var guild2 = await repository.GetGuild(1);
        guild2.Should().BeNull();
    }
}
