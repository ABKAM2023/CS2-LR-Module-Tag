using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Utils; 
using LevelsRanks.API;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LevelsRanksModuleTag;

[MinimumApiVersion(80)]
public class LevelsRanksModuleTag : BasePlugin
{
    public override string ModuleName => "[LR] Module Tag";
    public override string ModuleVersion => "1.0";
    public override string ModuleAuthor => "ABKAM designed by RoadSide Romeo & Wend4r";
    public override string ModuleDescription => "Updates the clan tag every round based on rank";
    
    private readonly PluginCapability<IPointsManager> _pointsManagerCapability = new("levelsranks");
    private IPointsManager? _pointsManager;
    private Dictionary<int, string>? _tagsConfig;

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        base.Load(hotReload);

        _pointsManager = _pointsManagerCapability.Get();
        
        if (_pointsManager == null)
        {
            Server.PrintToConsole("Points management system is currently unavailable.");
            return;
        }

        CreateTagsConfig();
        _tagsConfig = LoadTagsConfig();

        RegisterEventHandler<EventRoundStart>(OnRoundStart);
    }

    private HookResult OnRoundStart(EventRoundStart roundStartEvent, GameEventInfo info)
    {
        foreach (var player in Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller"))
        {
            if (player != null && player.IsValid && !player.IsBot && player.TeamNum != (int)CsTeam.Spectator)
            {
                var currentRank = _pointsManager.GetCurrentRank(player.SteamID.ToString());
                if (currentRank != null && _tagsConfig.TryGetValue(currentRank.Id, out var tag))
                {
                    player.Clan = tag;
                    Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
                }
            }
        }
        return HookResult.Continue;
    }

    private void CreateTagsConfig()
    {
        var filePath = Path.Combine(ModuleDirectory, "settings_tags.yml");

        if (!File.Exists(filePath))
        {
            var tagsConfig = new Dictionary<string, Dictionary<int, string>>
            {
                {
                    "tags", new Dictionary<int, string>
                    {
                        {0, "[S-I]"}, {1, "[S-II]"}, {2, "[S-III]"}, {3, "[S-IV]"}, {4, "[SE]"},
                        {5, "[SEM]"}, {6, "[GN-I]"}, {7, "[GN-II]"}, {8, "[GN-III]"}, {9, "[GNM]"},
                        {10, "[MG-I]"}, {11, "[MG-II]"}, {12, "[MGE]"}, {13, "[DMG]"},
                        {14, "[LE]"}, {15, "[LEM]"}, {16, "[SMFC]"}, {17, "[GE]"}
                    }
                }
            };
            
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var yaml = serializer.Serialize(tagsConfig);
            File.WriteAllText(filePath, yaml);
        }
    }

    private Dictionary<int, string> LoadTagsConfig()
    {
        var filePath = Path.Combine(ModuleDirectory, "settings_tags.yml");
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var yaml = File.ReadAllText(filePath);
        var tagsConfig = deserializer.Deserialize<Dictionary<string, Dictionary<int, string>>>(yaml);
        return tagsConfig["tags"];
    }
}
