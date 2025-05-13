using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;

namespace LilinsCoinFlips.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ReloadEffects : ICommand
    {
        public string Command => "lilinscoinflipsreload";
        public string[] Aliases => new string[] { "lcfr" };
        public string Description => "Used to reload effects. Type: lcf reload";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count >= 1)
            {
                response = "This command takes no arguments.";
                return false;
            }

            //C:\Users\Lilin\AppData\Roaming\EXILED\Configs\Plugins\lilins_coin_flips\bad.yaml
            var good = LilinsCoinFlips.Types.EffectLoader.LoadEffectsFromYaml(Plugin.Instance.Config.goodEffectsPath);
            var bad = LilinsCoinFlips.Types.EffectLoader.LoadEffectsFromYaml(Plugin.Instance.Config.badEffectsPath);

            LilinsCoinFlips.Types.EffectLoader.GoodEffects = good;
            LilinsCoinFlips.Types.EffectLoader.BadEffects = bad;

            response = $"[LCF] Coinflip effects reloaded.";
            return true;
        }
    }
}
