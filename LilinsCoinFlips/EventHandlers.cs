using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using LilinsCoinFlips.Configs;
using LilinsCoinFlips.Types;

namespace LilinsCoinFlips
{
    public class EventHandlers
    {
        private static Config Config => Plugin.Instance.Config;
        private readonly System.Random _rd = new();
        public static Dictionary<ushort, int> CoinUses = new();
        private static Configs.Translations Translations => Plugin.Instance.Translation;

        private readonly Dictionary<string, DateTime> _cooldownDict = new();

        public static void SendHint(Player player, string message, bool isTails = false)
        {
            player.ShowHint(message, 5);
        }

        public void OnCoinFlip(FlippingCoinEventArgs ev)
        {
            string message = "";

            bool helper = false;

            bool onCd = _cooldownDict.ContainsKey(ev.Player.RawUserId)
                && (DateTime.UtcNow - _cooldownDict[ev.Player.RawUserId]).TotalSeconds < Config.CoinCooldown;

            if (onCd)
            {
                ev.IsAllowed = false;
                SendHint(ev.Player, Translations.TossOnCooldownMessage);
                Log.Debug($"{ev.Player.Nickname} tried to throw a coin on cooldown.");
                return;
            }

            _cooldownDict[ev.Player.RawUserId] = DateTime.UtcNow;

            if (!CoinUses.ContainsKey(ev.Player.CurrentItem.Serial))
            {
                CoinUses.Add(ev.Player.CurrentItem.Serial, _rd.Next(Config.MinMaxDefaultCoins[0], Config.MinMaxDefaultCoins[1]));
                Log.Debug($"Coin registered. Uses left: {CoinUses[ev.Player.CurrentItem.Serial]}");

                if (CoinUses[ev.Player.CurrentItem.Serial] < 1)
                {
                    CoinUses.Remove(ev.Player.CurrentItem.Serial);
                    Log.Debug("Coin was removed due to throw limit.");
                    if (ev.Player.CurrentItem != null)
                    {
                        ev.Player.RemoveHeldItem();
                    }
                    SendHint(ev.Player, Translations.CoinNoUsesMessage);
                    return;
                }
            }

            CoinUses[ev.Player.CurrentItem.Serial]--;
            Log.Debug($"Uses left: {CoinUses[ev.Player.CurrentItem.Serial]}");

            if (CoinUses[ev.Player.CurrentItem.Serial] < 1)
            {
                helper = true;
            }

            Log.Debug($"Is tails: {ev.IsTails}");

            if (!ev.IsTails)
            {
                int totalChance = EffectLoader.GoodEffects.Sum(effect => effect.Chance);
                int randomNum = _rd.Next(1, totalChance + 1);

                var selectedEffect = EffectLoader.GoodEffects.FirstOrDefault(effect => (randomNum -= effect.Chance) <= 0);

                selectedEffect?.Execute(ev.Player);
                message = selectedEffect?.Message;
            }

            if (ev.IsTails)
            {
                int totalChance = EffectLoader.BadEffects.Sum(effect => effect.Chance);
                int randomNum = _rd.Next(1, totalChance + 1);

                var selectedEffect = EffectLoader.BadEffects.FirstOrDefault(effect => (randomNum -= effect.Chance) <= 0);

                selectedEffect?.Execute(ev.Player);
                message = selectedEffect?.Message;
            }

            if (helper)
            {
                if (ev.Player.CurrentItem != null)
                {
                    ev.Player.RemoveHeldItem();
                }
                message += Translations.CoinBreaksMessage;
            }

            if (message != null)
            {
                SendHint(ev.Player, message);
            }
        }
    }
}
