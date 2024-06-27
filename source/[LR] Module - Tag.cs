using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Utils;
using LevelsRanksApi;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Microsoft.Extensions.Logging;

namespace LevelsRanksModuleTag;

[MinimumApiVersion(80)]
public class LevelsRanksModuleTag : BasePlugin
{
    public override string ModuleName => "[LR] Module - Module Tag";
    public override string ModuleVersion => "1.1";
    public override string ModuleAuthor => "ABKAM designed by RoadSide Romeo & Wend4r";

    private Dictionary<int, string>? _tagsConfig;
    private bool _allowDisableTag = false;
    private HashSet<ulong> _disabledTags = new();
    private string _disabledTagsFilePath;

    private ILevelsRanksApi? _api;
    private readonly PluginCapability<ILevelsRanksApi> _apiCapability = new("levels_ranks");

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        base.OnAllPluginsLoaded(hotReload);

        _api = _apiCapability.Get();

        if (_api == null)
        {
            Server.PrintToConsole("Levels Ranks API is currently unavailable.");
            return;
        }

        CreateTagsConfig();
        _tagsConfig = LoadTagsConfig();
        _disabledTagsFilePath = Path.Combine(ModuleDirectory, "player_disable_tags.json");
        LoadDisabledTags();

        if (_tagsConfig != null && _allowDisableTag)
        {
            _allowDisableTag = true;
        }

        RegisterEventHandler<EventRoundStart>(OnRoundStart);

        if (_allowDisableTag)
        {
            _api.RegisterMenuOption(Localizer["menu.toggle_tag"], ToggleTagMenu);
        }
    }

    private Dictionary<int, string> LoadTagsConfig()
    {
        var configDirectory = Path.Combine(Application.RootDirectory, "configs/plugins/LevelsRanks");
        var filePath = Path.Combine(configDirectory, "settings_tags.json");

        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(json);

            if (config != null && config.TryGetValue("LR_Tags", out var tagsSection) && tagsSection.TryGetValue("Tags", out var tags))
            {
                var parsedTags = new Dictionary<int, string>();

                if (tags is JsonElement tagsElement)
                {
                    if (tagsElement.TryGetProperty("access", out var accessValue) && accessValue.GetString() == "1")
                    {
                        _allowDisableTag = true;
                    }

                    foreach (var tag in tagsElement.EnumerateObject())
                    {
                        if (int.TryParse(tag.Name, out var rank))
                        {
                            if (tag.Value.TryGetProperty("tag", out var tagValue))
                            {
                                parsedTags[rank] = tagValue.GetString();
                            }
                        }
                    }

                    return parsedTags;
                }
            }
        }

        return null;
    }

    private void CreateTagsConfig()
    {
        var configDirectory = Path.Combine(Application.RootDirectory, "configs/plugins/LevelsRanks");
        var filePath = Path.Combine(configDirectory, "settings_tags.json");

        if (!File.Exists(filePath))
        {
            var defaultConfig = new
            {
                LR_Tags = new
                {
                    Tags = new Dictionary<string, object>
                    {
                        { "access", "1" },
                        { "1", new { tag = "[Rank 1]" } },
                        { "2", new { tag = "[Rank 2]" } },
                        { "3", new { tag = "[Rank 3]" } },
                        { "4", new { tag = "[Rank 4]" } },
                        { "5", new { tag = "[Rank 5]" } },
                        { "6", new { tag = "[Rank 6]" } },
                        { "7", new { tag = "[Rank 7]" } },
                        { "8", new { tag = "[Rank 8]" } },
                        { "9", new { tag = "[Rank 9]" } },
                        { "10", new { tag = "[Rank 10]" } },
                        { "11", new { tag = "[Rank 11]" } },
                        { "12", new { tag = "[Rank 12]" } },
                        { "13", new { tag = "[Rank 13]" } },
                        { "14", new { tag = "[Rank 14]" } },
                        { "15", new { tag = "[Rank 15]" } },
                        { "16", new { tag = "[Rank 16]" } },
                        { "17", new { tag = "[Rank 17]" } },
                        { "18", new { tag = "[Rank 18]" } }
                    }
                }
            };

            Directory.CreateDirectory(configDirectory);
            var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }

    private void LoadDisabledTags()
    {
        if (File.Exists(_disabledTagsFilePath))
        {
            var json = File.ReadAllText(_disabledTagsFilePath);
            _disabledTags = JsonSerializer.Deserialize<HashSet<ulong>>(json) ?? new HashSet<ulong>();
        }
    }

    private void SaveDisabledTags()
    {
        var json = JsonSerializer.Serialize(_disabledTags, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_disabledTagsFilePath, json);
    }

    private HookResult OnRoundStart(EventRoundStart roundStartEvent, GameEventInfo info)
    {
        var currentRanksTask = _api.GetCurrentRanksAsync();
        currentRanksTask.Wait();
        var currentRanks = currentRanksTask.Result;

        foreach (var player in Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller"))
        {
            if (player != null && player.IsValid && !player.IsBot && player.TeamNum != (int)CsTeam.Spectator)
            {
                var steamId64 = player.SteamID;

                if (_disabledTags.Contains(steamId64))
                {
                    player.Clan = "";
                    Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
                    continue;
                }

                var steamId = _api.ConvertToSteamId(steamId64);

                if (currentRanks.TryGetValue(steamId, out var rankId))
                {
                    if (_tagsConfig.TryGetValue(rankId, out var tag))
                    {
                        player.Clan = tag;
                        Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
                        
                    }
                }
                else
                {
                    Logger.LogWarning($"No rank found for player {steamId} (SteamID64: {steamId64})");
                }
            }
        }

        return HookResult.Continue;
    }

    private void ToggleTagMenu(CCSPlayerController player)
    {
        var steamId64 = player.SteamID;

        if (_disabledTags.Contains(steamId64))
        {
            _disabledTags.Remove(steamId64);
            player.PrintToChat(ReplaceColorPlaceholders(Localizer["chat.tag_enabled"]));
        }
        else
        {
            _disabledTags.Add(steamId64);
            player.Clan = "";
            Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
            player.PrintToChat(ReplaceColorPlaceholders(Localizer["chat.tag_disabled"]));
        }
        
        SaveDisabledTags();
    }

    public override void Unload(bool hotReload)
    {
        if (_allowDisableTag)
        {
            _api.UnregisterMenuOption(Localizer["menu.toggle_tag"]);
        }
    }
    [ConsoleCommand("css_lvl_reload", "Reloads the configuration files")]
    public void ReloadConfigsCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            try
            {
                LoadTagsConfig();
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"Error reloading configuration: {ex.Message}");
            }
        }
    }  
    public string ReplaceColorPlaceholders(string message)
    {
        if (message.Contains('{'))
        {
            var modifiedValue = message;
            foreach (var field in typeof(ChatColors).GetFields())
            {
                var pattern = $"{{{field.Name}}}";
                if (message.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    modifiedValue = modifiedValue.Replace(pattern, field.GetValue(null).ToString(),
                        StringComparison.OrdinalIgnoreCase);
            }

            return modifiedValue;
        }

        return message;
    }
}
