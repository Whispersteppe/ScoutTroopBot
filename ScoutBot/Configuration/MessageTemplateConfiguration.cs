using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutBot.Configuration;

public class MessageTemplateConfiguration
{
    public string Content { get; set; }
    //public bool Tts { get; set; }
    //public List<EmbedTemplateConfiguration> Embeds { get; set; }
    public List<ActionRowTemplateConfiguration> ActionRowItems { get; set; }
    //public List<AttachmentTemplateConfiguration> Attachments { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }
}

public class ActionRowTemplateConfiguration
{
    public List<ActionRowItemTemplateConfiguration> Items { get; set; }
}

public class ActionRowItemTemplateConfiguration
{
    public string CustomIDName { get; set; }
    public string Label { get; set; }
    public string Style { get; set; }
}
