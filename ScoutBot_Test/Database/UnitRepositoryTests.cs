using FluentAssertions;
using ScoutBot.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ScoutBot_Test.Database;

[Collection("Shared Database Container")]
public class UnitRepositoryTests : SbotDatabaseBase
{
    public UnitRepositoryTests(SbotDbContextContainer dbContextContainer, ITestOutputHelper output) 
        : base(dbContextContainer, output)
    {
    }

    [Fact]
    public async Task CRUDUnit()
    {
        GuildRepository guildRepository = new GuildRepository(GetLogger<GuildRepository>(Output), GetContext());

        await guildRepository.CreateGuild(2, "test2");
        var guild = await guildRepository.GetGuild(2);


        UnitRepository unitRepository = new UnitRepository(GetLogger<UnitRepository>(Output), GetContext());
        int id = await unitRepository.CreateUnit(new ScoutBot.Database.DtoModels.DtoUnit() { GuildId = 2, Name = "test name"});

        var unit = await unitRepository.GetUnit(id);

        unit.Should().NotBeNull();

        await unitRepository.RemoveUnit(id);

        var unit2 = await unitRepository.GetUnit(id);
        unit2.Should().BeNull();

    }
}
