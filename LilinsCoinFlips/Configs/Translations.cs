using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace LilinsCoinFlips.Configs
{
    public class Translations : ITranslation
    {
        [Description("This is added to the effect message if the coin breaks.")]
        public string CoinBreaksMessage { get; set; } = "\nAlso your coin was used too much and it broke down.";

        [Description("The broadcast message when a coin is registered with no uses.")]
        public string CoinNoUsesMessage { get; set; } = "Your coin had no uses to begin with!";

        public List<string> HintMessages { get; set; } = new()
        {
            "Your coin landed on tails.",
            "Your coin landed on heads."
        };
        [Description("Messages that will be send when the coin is on cooldown.")]
        public string TossOnCooldownMessage { get; set; } = "You can't throw the coin yet.";

        [Description("Info for the player if no SCP exists to teleport to.")]
        public string NoSCPToTeleportToMessage { get; set; } = "No SCP was found to teleport to.";
    }
}
