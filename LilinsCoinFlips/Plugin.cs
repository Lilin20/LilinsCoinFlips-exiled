using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LilinsCoinFlips.Configs;
using Exiled.API.Features;
using Player = Exiled.Events.Handlers.Player;
using LilinsCoinFlips.Types;

namespace LilinsCoinFlips
{
    public class Plugin : Plugin<Config, Configs.Translations>
    {
        public override Version RequiredExiledVersion => new(9, 5, 0);
        public override Version Version => new(1, 0, 0);
        public override string Author => "Lilin";
        public override string Name => "LilinsCoinFlips";

        public static Plugin Instance;
        private EventHandlers _eventHandler;

        public override void OnEnabled()
        {
            Instance = this;

            try
            {
                EffectLoader.GoodEffects = EffectLoader.LoadEffectsFromYaml(Instance.Config.goodEffectsPath);
                EffectLoader.BadEffects = EffectLoader.LoadEffectsFromYaml(Instance.Config.badEffectsPath);

                Log.Info("Effekte erfolgreich geladen!");
            }
            catch (Exception ex)
            {
                Log.Error($"Fehler beim Laden der Effekte: {ex}");
            }

            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            Instance = null;
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            _eventHandler = new EventHandlers();
            Player.FlippingCoin += _eventHandler.OnCoinFlip;
        }

        private void UnregisterEvents()
        {
            Player.FlippingCoin -= _eventHandler.OnCoinFlip;
            _eventHandler = null;
        }
    }
}
