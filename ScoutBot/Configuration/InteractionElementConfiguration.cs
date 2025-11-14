using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Configuration;

public class InteractionElementConfiguration
{
    public string name { get; set; }
    public string Prompt { get; set; }
    public PromptTypeEnum PromptType { get; set; }
    public string DefaultValue { get; set; }

    //TODO - these will get manipulated a bit later.  for a select list it will be one of the following, not all of them.
    public List<string> SelectValues { get; set; }//  a fixed set of select values
    public string SelectDatabase { get; set; } //  a database to pull select values from, like patrol names
    public string SelectFixedData { get; set; } // pointing to a table in the configuration, like ranks, or badges, etc.


    public List<InteractionElementPostAction> PostActions { get; set; }
    public List<string> NextPrompt { get; set; }

}

public class InteractionElementPostAction
{
    public string ActionType { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}

public enum PromptTypeEnum
{
    Text,
    Paragraph,
    SelectMenu
}
