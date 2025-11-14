using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutBot.Configuration;

/// <summary>
/// Represents the configuration for a template, including item type, associated categories, roles, and additional
/// properties.
/// </summary>
/// <remarks>This class is used to define the structure and metadata for a template configuration. It includes
/// properties for specifying the type of item, associated categories and roles, and any additional properties that are
/// not explicitly defined.
/// This is used for units, patrols, merit badges, and common items.
/// </remarks>
public class TemplateConfiguration
{
    /// <summary>
    /// the type of item this template represents (e.g., "unit", "patrol", "merit badge", "common").
    /// </summary>
    public string ItemType { get; set; }
    /// <summary>
    /// The set of categories and channels to create for this template.
    /// </summary>
    public List<CategoryTemplateConfiguration> Categories { get; set; }
    /// <summary>
    /// the set of roles to create for this template.
    /// </summary>
    public List<RoleTemplateConfiguration> Roles { get; set; }

    /// <summary>
    /// extra properties that just end up in the JSON but we don't have a specific property for.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; } = new Dictionary<string, JsonElement>();

}
