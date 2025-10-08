using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutTroopBot.AppCommands;

/*
 this will probably be reswizzled as I figure out modal dialogs and a better generial approach
  - button to start registration
  - modal dialog to collect info
  - log info to file or database
  - maybe assign roles based on info (like "Eagle Scout" role)
 */
public class RegisterScoutCommand(ILogger<RegisterScoutCommand> logger, RestClient restClient) : ApplicationCommandModule<ApplicationCommandContext>
{

    [SlashCommand("register", "Collects scout information and logs it")]
    public async Task<InteractionCallbackProperties<InteractionMessageProperties>> BorkAsync()
    {

        //var messageX = new MessageProperties()
        //{
        //    Content = "Please register as a scout by clicking the button below.",
        //    Components = new IMessageComponentProperties[]
        //    {
        //        new ActionRowProperties()
        //        {
        //            new ButtonProperties("register_scout_modal", "Register Scout", ButtonStyle.Primary)
        //        },
        //    }, 
        //};

        var message = new InteractionMessageProperties()
        {
            Content = "Please register as a scout by clicking the button below.",
            Components = new IMessageComponentProperties[]
            {
                new ActionRowProperties()
                {
                    new ButtonProperties("register_scout_modal", "Register Scout", ButtonStyle.Primary)
                }
            }
        };

        var callback = InteractionCallback.Message(message);
        return callback;

    }

}


public class RegisterScoutButtonHandler(ILogger<RegisterScoutHandler> logger) : ComponentInteractionModule<ButtonInteractionContext>
{

    //TODO i'm gonna need a dynamic class thingie to make this really sing
    [ComponentInteraction("register_scout_modal")]
    public async Task<InteractionCallbackProperties<ModalProperties>> BorkAsync()
    {
        // Show modal dialog to user
        //TODO these explode if there is no options in the list
        var modal = new ModalProperties("register_scout", "Scout Information")
        {
            new LabelProperties("Name", new TextInputProperties("name", TextInputStyle.Short)),
            new LabelProperties("Rank", CreateRankMenu()),
            new LabelProperties("Position", CreatePositionMenu()),
            new LabelProperties("Unit", CreateUnitMenu()),
            new LabelProperties("Patrol", CreatePatrolMenu()),
            //new LabelProperties("Patrol", new TextInputProperties("patrol", TextInputStyle.Short)),
            //new LabelProperties("Rank", new TextInputProperties("rank", TextInputStyle.Short)),
            //new LabelProperties("Position", new TextInputProperties("position", TextInputStyle.Short)),
        };

        var callback = InteractionCallback.Modal(modal);
        return callback;
    }

    private StringMenuProperties CreateUnitMenu()
    {
        var unitRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Unit")).Select(r => r.Name.Replace(" Unit", "")).ToList();
        List<StringMenuSelectOptionProperties> options = new List<StringMenuSelectOptionProperties>();
        foreach (var unitName in unitRoles)
        {
            options.Add(new StringMenuSelectOptionProperties(unitName, unitName.Replace(" ", "_")));
        }
        var unitMenu = new StringMenuProperties("unit", options)
        {
            Placeholder = "Select Unit",
            MinValues = 1,
            MaxValues = 1
        };
        return unitMenu;
    }

    private StringMenuProperties CreatePatrolMenu()
    {
        var patrolRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Patrol")).Select(r => r.Name.Replace(" Patrol", "")).ToList();

        List<StringMenuSelectOptionProperties> options = new List<StringMenuSelectOptionProperties>();
        foreach (var patrolName in patrolRoles)
        {
            options.Add(new StringMenuSelectOptionProperties(patrolName, patrolName.Replace(" ", "_")));
        }

        var patrolMenu = new StringMenuProperties("patrol", options)
        {
            Placeholder = "Select Patrol",
            MinValues = 1,
            MaxValues = 1
        };

        return patrolMenu;
    }
    private StringMenuProperties CreatePositionMenu()
    {
        //TODO pull from the config
        var positionMenu = new StringMenuProperties("position", new[]
        {
            new StringMenuSelectOptionProperties("Patrol Leader", "patrol_leader"),
            new StringMenuSelectOptionProperties("Assistant Patrol Leader", "assistant_patrol_leader"),
            new StringMenuSelectOptionProperties("Scribe", "scribe"),
            new StringMenuSelectOptionProperties("Quartermaster", "quartermaster"),
            new StringMenuSelectOptionProperties("Historian", "historian"),
            new StringMenuSelectOptionProperties("Den Chief", "den_chief"),
            new StringMenuSelectOptionProperties("Webelos Leader", "webelos_leader"),

            new StringMenuSelectOptionProperties("Scoutmaster", "scoutmaster"),
            new StringMenuSelectOptionProperties("Assistant Scoutmaster", "assistant-scoutmaster"),
            new StringMenuSelectOptionProperties("Committee", "committee"),


            new StringMenuSelectOptionProperties("None", "none"),
        })
        {
            Placeholder = "Select Position",
            MinValues = 1,
            MaxValues = 1
        };
        return positionMenu;
    }

    private StringMenuProperties CreateRankMenu()
    {
        //TODO pull from the config

        var rankMenu = new StringMenuProperties("rank", new[]
        {
            new StringMenuSelectOptionProperties("Scout", "scout"),
            new StringMenuSelectOptionProperties("Tenderfoot", "tenderfoot"),
            new StringMenuSelectOptionProperties("Second Class", "second_class"),
            new StringMenuSelectOptionProperties("First Class", "first_class"),
            new StringMenuSelectOptionProperties("Star", "star"),
            new StringMenuSelectOptionProperties("Life", "life"),
            new StringMenuSelectOptionProperties("Eagle", "eagle"),
        })
        {
            Placeholder = "Select Rank",
            MinValues = 1,
            MaxValues = 1
        };
        return rankMenu;
    }

}

public class RegisterScoutHandler(ILogger<RegisterScoutHandler> logger) : ComponentInteractionModule<ModalInteractionContext>
{

    private (string name, string value) GetComponentValue(IModalComponent component)
    {
        string internalName = "";
        string internalValue = "";
        if (component is NetCord.Label label)
        {
            if (label.Component is TextInput text)
            {
                internalName = text.CustomId;
                internalValue = text.Value;
            }
            else if (label.Component is StringMenu menu)
            {
                internalName = menu.CustomId;
                internalValue = menu.SelectedValues.FirstOrDefault("Scout");
            }
            else
            {

            }
        }
        else
        {

        }

        return (internalName, internalValue);
    }

    [ComponentInteraction("register_scout")]
    public async Task<string> RegisterScoutModalHandlerAsync()
    {
        // Extract values from modal components by their custom IDs
        var namevalues = new Dictionary<string, string>();

        foreach (var component in Context.Components)
        {
            var nvp = GetComponentValue(component);
            namevalues.Add(nvp.name, nvp.value);
        }

        //TODO do more here

        JsonSerializerOptions settings = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var rslt = JsonSerializer.Serialize(namevalues, settings);

        logger.LogInformation("Register Scout Submitted: {Result}", rslt);

        return $"Thank you! Your information has been logged.{rslt}";
    }

}


