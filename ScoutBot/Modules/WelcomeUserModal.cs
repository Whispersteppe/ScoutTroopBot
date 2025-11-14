using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScoutBot.Modules.SbotCommandContainer;

namespace ScoutBot.Modules;

public class WelcomeUserInteraction : InteractionModuleBase<SocketInteractionContext>
{

    private InteractionService _commands { get; set; }
    private InteractionHandler _handler;
    private readonly ILogger<UtilityCommands> _logger;

    public WelcomeUserInteraction(InteractionHandler handler, ILogger<UtilityCommands> logger, InteractionService commands)
    {
        _handler = handler;
        _logger = logger;
        _commands = commands;
    }

    [SlashCommand("welcome_user", "Opens a modal to welcome a new user")]
    public async Task Command()
    {
        await Context.Interaction.RespondWithModalAsync<WelcomeUserModal>("welcome_user_modal");
    }

    [SlashCommand("welcome_user3", "Opens a modal to welcome a new user")]
    public async Task Command3()
    {
        //TODO this works.
        //  also, a few rude gestures towards the non-support of dropdowns in modals.  guess I'll go with the commucation interaction model.
        //TODO need a better way of doing a communication sequence.  vlaargh
        var messageContent = "What is your rank?";

        var builder = new ComponentBuilder();

        ActionRowBuilder rowBuilder = new ActionRowBuilder();
        rowBuilder.AddComponent(new SelectMenuBuilder()
        {
            CustomId = "rank_value",
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

    [ComponentInteraction("rank_value")]
    public async Task HandleWelcomeUserModal(string rank)
    {
        // Process the modal input here
        string response = $"Rank: {rank}";
        await RespondAsync(response, ephemeral: true);
    }


    [SlashCommand("welcome_user2", "Opens a modal to welcome a new user")]
    public async Task AnotherCommand()
    {
        SelectMenuBuilder rankBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a rank")
            .WithMinValues(1)
            .WithMaxValues(1)
            .WithCustomId("rank_input")
            .AddOption("None", "rank_none")
            .AddOption("Scout", "rank_scout")
            .AddOption("Tenderfoot", "rank_tenderfoot")
            .AddOption("Second Class", "rank_secondclass")
            .AddOption("First Class", "rank_firstclass")
            .AddOption("Star", "rank_star")
            .AddOption("Life", "rank_life")
            .AddOption("Eagle", "rank_eagle");

        SelectMenuBuilder patrolBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a patrol")
            .WithMinValues(1)
            .WithMaxValues(1)
            .WithCustomId("patrol_input")
            .AddOption("Alpha", "patrol_alpha")
            .AddOption("Bravo", "patrol_bravo")
            .AddOption("Charlie", "patrol_charlie")
            .AddOption("Delta", "patrol_delta");

        var positionBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a position")
            .WithMinValues(1)
            .WithMaxValues(1)
            .WithCustomId("position_input")
            .AddOption("Patrol Leader", "position_leader")
            .AddOption("Assistant Patrol Leader", "position_assistant")
            .AddOption("Scribe", "position_scribe")
            .AddOption("Quartermaster", "position_quartermaster")
            .AddOption("Historian", "position_historian");
       
        var patrolRow = new ActionRowBuilder()
            .AddComponent(patrolBuilder)
            ;

        var mb = new ModalBuilder()
            .WithTitle("Welcome to our Server")
            .WithCustomId("welcome_user_modal")
            .AddTextInput("Enter your full name", "name_input", TextInputStyle.Short, maxLength: 100)
            //.AddComponents([patrolRow], 0)
//            .AddComponents([patrolBuilder.Build()], 0)
//            .AddComponents([positionBuilder.Build()], 0)
            ;

        mb.Components.ActionRows.Add(patrolRow);

        await Context.Interaction.RespondWithModalAsync(mb.Build());
    }

    [ModalInteraction("welcome_user_modal")]
    public async Task HandleWelcomeUserModal(WelcomeUserModal modal)
    { 
        // Process the modal input here
        string response = $"Welcome, {modal.Name}!\n" +
                          $"Patrol: {modal.Patrol}\n" +
                          $"Rank: {modal.Rank}\n" +
                          $"Position: {modal.Position}";
        await RespondAsync(response, ephemeral: true);
    }

    //TODO this is kind of limiting.  may have to go with the ModalBuilder approach above
    public class WelcomeUserModal : IModal
    {
        public string Title => "Welcome to Our Server";


        [ModalTextInput("name_input", TextInputStyle.Short, "Enter your full name", maxLength: 100)]
        public string Name { get; set; }

        [ModalTextInput("patrol_input", TextInputStyle.Short, "Enter your patrol name", maxLength: 100)]
        public string Patrol { get; set; }

        [ModalTextInput("rank_input", TextInputStyle.Short, "Enter your rank", maxLength: 100)]
        public string Rank { get; set; }

        [ModalTextInput("position_input", TextInputStyle.Short, "Enter your position", maxLength: 100)]
        public string Position { get; set; }
    }
}
