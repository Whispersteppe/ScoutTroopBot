using NetCord;
using NetCord.Rest;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutTroopBot.Configuration;

public class ChannelTemplateConfiguration
{
    public string Name { get; set; }
    public ChannelType Type { get; set; }
    public string? Topic { get; set; }

    // this stuff is from the GuildChannelProperties
    //public int? Bitrate { get; set; }
    //public int? UserLimit { get; set; }
    //public int? Slowmode { get; set; }
    //public int? Position { get; set; }
    public bool? Nsfw { get; set; }
    //public string? RtcRegion { get; set; }
    //public VideoQualityMode? VideoQualityMode { get; set; }
    //public ThreadArchiveDuration? DefaultAutoArchiveDuration { get; set; }
    //public EmojiProperties? DefaultReactionEmoji { get; set; }
    //public IEnumerable<ForumTagProperties>? AvailableTags { get; set; }
    //public SortOrderType? DefaultSortOrder { get; set; }
    //public ForumLayoutType? DefaultForumLayout { get; set; }
    //public int? DefaultThreadSlowmode { get; set; }

    public List<PermissionOverwriteTemplateConfiguration> PermissionOverwrites { get; set; }

    public List<MessageTemplateConfiguration>? InitialMessages { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }

}

