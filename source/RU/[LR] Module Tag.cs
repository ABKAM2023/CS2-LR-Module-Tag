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
    public override string ModuleDescription => "Обновляет клан-тег каждый раунд в зависимости от ранга";
 
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
                        {0, "[С-I]"}, {1, "[С-II]"}, {2, "[С-III]"}, {3, "[С-IV]"}, {4, "[СЭ]"},
                        {5, "[СВМ]"}, {6, "[ЗЗ-I]"}, {7, "[ЗЗ-II]"}, {8, "[ЗЗ-III]"}, {9, "[ЗЗМ]"},
                        {10, "[МХ-I]"}, {11, "[МХ-II]"}, {12, "[МХЭ]"}, {13, "[ЗМХ]"},
                        {14, "[ЛБ]"}, {15, "[ЛБМ]"}, {16, "[ВМВР]"}, {17, "[ВЭ]"}
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
