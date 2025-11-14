using Discord;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutBot.Configuration;

public class PermissionOverwriteTemplateConfiguration
{
    public string RoleName { get; set; }
    public string permissionTypeType { get; set; }
    public List<GuildPermission> Allowed { get; set; }
    public List<GuildPermission> Denied { get; set; }

    /*
        CreateInstantInvite
        KickMembers
        BanMembers 
        Administrator
        ManageChannels
        ManageGuild
        ViewGuildInsights
        AddReactions
        ViewAuditLog
        ViewChannel 
        SendMessages 
        SendTTSMessages 
        ManageMessages 
        EmbedLinks
        AttachFiles 
        ReadMessageHistory
        MentionEveryone 
        UseExternalEmojis 
        Connect
        Speak
        MuteMembers 
        DeafenMembers
        MoveMembers 
        UseVAD
        PrioritySpeaker
        Stream 
        ChangeNickname
        ManageNicknames 
        ManageRoles 
        ManageWebhooks 
        ManageEmojisAndStickers 
        UseApplicationCommands 
        RequestToSpeak
        ManageEvents
        ManageThreads
        CreatePublicThreads 
        CreatePrivateThreads 
        UseExternalStickers 
        SendMessagesInThreads 
        StartEmbeddedActivities 
        ModerateMembers
        ViewMonetizationAnalytics 
        UseSoundboard
        CreateGuildExpressions
        CreateEvents 
        UseExternalSounds 
        SendVoiceMessages 
        UseClydeAI 
        SetVoiceChannelStatus
        SendPolls
        UseExternalApps 
     
     */
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }

}
