using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Interfaces;

namespace LilinsCoinFlips.Configs
{
    public class Config : IConfig
    {
        [Description("Whether or not the plugin should be enabled. Default: true")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not debug logs should be shown. Default: false")]
        public bool Debug { get; set; } = false;

        public string goodEffectsPath { get; set; } = "none";
        public string badEffectsPath { get; set; } = "none";

        [Description("Sets the coin cooldown.")]
        public double CoinCooldown { get; set; } = 5;

        public List<int> MinMaxDefaultCoins { get; set; } = new()
        {
            1,
            4
        };
    }
}
