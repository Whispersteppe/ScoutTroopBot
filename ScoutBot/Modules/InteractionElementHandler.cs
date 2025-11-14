using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoutBot.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScoutBot.Modules.SbotCommandContainer;

namespace ScoutBot.Modules;

public class InteractionElementHandler : InteractionModuleBase<SocketInteractionContext>
{
    private InteractionService _commands { get; set; }
    private InteractionHandler _handler;
    private readonly ILogger<UtilityCommands> _logger;
    private readonly RootConfiguration _config;

    public InteractionElementHandler(
        InteractionHandler handler,
        ILogger<UtilityCommands> logger, 
        InteractionService commands, 
        IOptions<RootConfiguration> config)
    {
        _handler = handler;
        _logger = logger;
        _commands = commands;
        _config = config.Value;
    }

    [ComponentInteraction("sbot_interaction_begin:*")]
    public async Task TriggerInteraction(string interactionName)
    {
        //TODO this works.
        //  also, a few rude gestures towards the non-support of dropdowns in modals.  guess I'll go with the commucation interaction model.
        //TODO need a better way of doing a communication sequence.  vlaargh
        var messageContent = "What is your rank?";

        var builder = new ComponentBuilder();
        string triggerInteractionName = "rank_selection";//this would come from configuration in a real scenario

        ActionRowBuilder rowBuilder = new ActionRowBuilder();
        rowBuilder.AddComponent(new SelectMenuBuilder()
        {
            CustomId = $"sbot_interaction:{triggerInteractionName}:",//  this will need to be a compound key to identify the interaction element and the context of the interaction
            Placeholder = "Select your rank",
            MinValues = 1,
            MaxValues = 1, 
            Options = new List<SelectMenuOptionBuilder>()
            {
                new SelectMenuOptionBuilder() { Label = "None", Value = "rank_none" },
                new SelectMenuOptionBuilder() { Label = "Scout", Value = "rank_scout" },
                new SelectMenuOptionBuilder() { Label = "Tenderfoot", Value = "rank_tenderfoot" },
                new SelectMenuOptionBuilder() { Label = "Second Class", Value = "rank_secondclass" },
                new SelectMenuOptionBuilder() { Label = "First Class", Value = "rank_firstclass" },
                new SelectMenuOptionBuilder() { Label = "Star", Value = "rank_star" },
                new SelectMenuOptionBuilder() { Label = "Life", Value = "rank_life" },
                new SelectMenuOptionBuilder() { Label = "Eagle", Value = "rank_eagle" },
            }
        });


        builder.AddRow(rowBuilder);

        await Context.Interaction.RespondAsync(messageContent, components: builder.Build());
    }

    [ComponentInteraction("sbot_interaction:*:")]
    public async Task HandleInteractionResponse(string interactionName, string selection)
    {
        // Process the modal input here
        string response = $"{interactionName}: {selection}";
        await RespondAsync(response, ephemeral: true);
    }
}
