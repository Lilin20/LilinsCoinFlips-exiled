using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Exiled.API.Features;

namespace LilinsCoinFlips.Types
{
    public static class EffectLoader
    {
        public static List<CoinFlipEffect> GoodEffects { get; set; }
        public static List<CoinFlipEffect> BadEffects { get; set; }

        public static List<CoinFlipEffect> LoadEffectsFromYaml(string yamlPath)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            if (!File.Exists(yamlPath))
            {
                Log.Debug($"YAML file not found: {yamlPath}");
                return new List<CoinFlipEffect>();
            }

            try
            {
                using var reader = new StreamReader(yamlPath);
                var yamlData = deserializer.Deserialize<List<YamlEffect>>(reader);

                Log.Debug($"Successfully loaded YAML file: {yamlPath}");
                foreach (var effect in yamlData)
                {
                    Log.Debug($"Loaded Effect - Message: {effect.Message}, Actions: [{string.Join(", ", effect.Actions.Select(a => a.Type))}], Chance: {effect.Chance}");
                }

                return yamlData.Select(yamlEffect =>
                {
                    var actionList = new List<Action<Player>>();

                    foreach (var action in yamlEffect.Actions)
                    {
                        var playerAction = CoinFlipEffect.GetActionFromName(action.Type, action.Parameters);
                        if (playerAction != null)
                            actionList.Add(playerAction);
                    }

                    // Kombiniere alle Aktionen zu einer einzigen
                    Action<Player> combinedAction = player =>
                    {
                        foreach (var act in actionList)
                        {
                            act(player);
                        }
                    };

                    return new CoinFlipEffect(yamlEffect.Message, combinedAction, yamlEffect.Chance);
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load effects from YAML file: {yamlPath}. Exception: {ex}");
                throw;
            }

        }
        public class YamlEffect
        {
            public string Message { get; set; }
            public int Chance { get; set; }
            public List<YamlAction> Actions { get; set; }
        }

        public class YamlAction
        {
            public string Type { get; set; }
            public Dictionary<string, object> Parameters { get; set; }
        }
    }
}
