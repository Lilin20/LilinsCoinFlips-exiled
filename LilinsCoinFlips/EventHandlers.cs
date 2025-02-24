using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using LilinsCoinFlips.Configs;
using LilinsCoinFlips.Types;
using UnityEngine;

namespace LilinsCoinFlips
{
    public class EventHandlers
    {
        private static Config Config => Plugin.Instance.Config;
        private static Configs.Translations Translations => Plugin.Instance.Translation;
        private readonly System.Random _rd = new();


        public static Dictionary<ushort, int> CoinUses = new();



        private readonly Dictionary<int, int> _goodEffectChances = new()
        {
            { 0, Config.MedkitChance },
            { 1, Config.RandomEffectChance },
            { 2, Config.SilentStepChance },
            { 3, Config.KeyCardChance },
            { 4, Config.EscapeTPChance },
            { 5, Config.HealChance },
            { 6, Config.MaxHPChance },
            { 7, Config.RandomItemChance },
            { 8, Config.RandomCustomItemChance },
        };

        private readonly Dictionary<int, int> _badEffectChances = new()
        {
            { 0, Config.ShitPantsChance },
            { 1, Config.RandomRoomTPChance },
            { 2, Config.InstaGrenadeChance },
            { 3, Config.PlayerSwapChance },
            { 4, Config.FentanylChance },
            { 5, Config.RandomEffectChance },
            { 6, Config.BouncyBallEffectChance },
            { 7, Config.HpReductionChance },
            { 8, Config.HugeDamageChance },
            { 9, Config.PrimedVaseChance },
            { 10, Config.FakeScpKillChance },
            { 11, Config.InventoryResetChance },
            { 12, Config.SpectSwapChance },
            { 13, Config.InventorySwapChance },
            { 14, Config.JSChance },
        };

        private readonly Dictionary<string, DateTime> _cooldownDict = new();

        public static void SendHint(Player player, string message, bool isTails = false)
        {
            player.ShowHint(message, 5);
        }

        public void OnCoinFlip(FlippingCoinEventArgs ev)
        {
            string message = "";

            bool helper = false;

            bool flag = _cooldownDict.ContainsKey(ev.Player.RawUserId)
                && (DateTime.UtcNow - _cooldownDict[ev.Player.RawUserId]).TotalSeconds < Config.CoinCooldown;

            if (flag)
            {
                ev.IsAllowed = false;
                SendHint(ev.Player, Translations.TossOnCoodownMessage);
                Log.Debug($"{ev.Player.Nickname} tried to throw a coin on cooldown.");
                return;
            }

            _cooldownDict[ev.Player.RawUserId] = DateTime.UtcNow;

            if (!CoinUses.ContainsKey(ev.Player.CurrentItem.Serial))
            {
                CoinUses.Add(ev.Player.CurrentItem.Serial, _rd.Next(Config.MinMaxDefaultCoins[0], Config.MinMaxDefaultCoins[1]));
                Log.Debug($"Registered a coin, uses left: {CoinUses[ev.Player.CurrentItem.Serial]}");

                if (CoinUses[ev.Player.CurrentItem.Serial] < 1)
                {
                    CoinUses.Remove(ev.Player.CurrentItem.Serial);
                    Log.Debug("Removed a coin.");
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
                int totalChance = _goodEffectChances.Values.Sum();
                int randomNum = _rd.Next(1, totalChance + 1);

                int headsEvent = 2;

                foreach (KeyValuePair<int, int> kvp in _goodEffectChances)
                {
                    if (randomNum <= kvp.Value)
                    {
                        headsEvent = kvp.Key;
                        break;
                    }

                    randomNum -= kvp.Value;
                }

                Log.Debug($"headsEvent = {headsEvent}");

                var effect = CoinFlipEffect.GoodEffects[headsEvent];
                effect.Execute(ev.Player);
                message = effect.Message;
            }

            if (ev.IsTails)
            {
                int totalChance = _badEffectChances.Values.Sum();
                int randomNum = _rd.Next(2, totalChance + 1);

                int tailsEvent = 13;

                foreach (KeyValuePair<int, int> kvp in _badEffectChances)
                {
                    if (randomNum <= kvp.Value)
                    {
                        tailsEvent = kvp.Key;
                        break;
                    }

                    randomNum -= kvp.Value;
                }

                Log.Debug($"tailsEvent = {tailsEvent}");

                var effect = CoinFlipEffect.BadEffects[tailsEvent];
                effect.Execute(ev.Player);
                message = effect.Message;
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

        public void OnSpawningItem(SpawningItemEventArgs ev)
        {
            if (Config.DefaultCoinsAmount != 0 && ev.Pickup.Type == ItemType.Coin)
            {
                Log.Debug($"Removed a coin, coins left to remove {Config.DefaultCoinsAmount}");
                ev.IsAllowed = false;
                Config.DefaultCoinsAmount--;
            }
        }

        public void OnFillingLocker(FillingLockerEventArgs ev)
        {
            if (ev.Pickup.Type == ItemType.Coin && Config.DefaultCoinsAmount != 0)
            {
                Log.Debug($"Removed a locker coin, coins left to remove {Config.DefaultCoinsAmount}");
                ev.IsAllowed = false;
                Config.DefaultCoinsAmount--;
            }
            else if (ev.Pickup.Type == Config.ItemToReplace.ElementAt(0).Key
                && Config.ItemToReplace.ElementAt(0).Value != 0)
            {
                Log.Debug($"Placed a coin, coins left to place: {Config.ItemToReplace.ElementAt(0).Value}. Replaced item: {ev.Pickup.Type}");
                ev.IsAllowed = false;
                Pickup.CreateAndSpawn(ItemType.Coin, ev.Pickup.Position, new Quaternion());
                Config.ItemToReplace[Config.ItemToReplace.ElementAt(0).Key]--;
            }
        }
    }
}
