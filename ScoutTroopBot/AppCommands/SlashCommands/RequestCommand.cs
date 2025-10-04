using NetCord.Services.ApplicationCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutTroopBot.AppCommands.SlashCommands;

/// <summary>
/// various request commands (BOR, Merit Badge, Scoutmaster Conference)
/// </summary>
/// <remarks>
/// these are currently placeholders and need to be implemented.  they may become dialogs and buttons someplace.
/// </remarks>
[SlashCommand("request", "Handles various requests")]
public class RequestCommand : ApplicationCommandModule<ApplicationCommandContext>
{
    [SubSlashCommand("bor", "Request a Board of Review")]
    public async Task<string> RequestBorAsync()
    {
        // TODO Implement the logic to handle the BOR request
        return "Board of Review request received. Further instructions will be sent via DM.";
    }

    [SubSlashCommand("meritbadge", "Request a Merit Badge")]
    public async Task<string> RequestMeritBadgeAsync()
    {
        // TODO Implement the logic to handle the Merit Badge request
        return "Merit Badge request received. Further instructions will be sent via DM.";
    }

    [SubSlashCommand("sm", "Request a Scoutmaster Conference")]
    public async Task<string> RequestScoutmasterConferenceAsync()
    {
        // TODO Implement the logic to handle the Scoutmaster Conference request
        return "Scoutmaster Conference request received. Further instructions will be sent via DM.";
    }
}
