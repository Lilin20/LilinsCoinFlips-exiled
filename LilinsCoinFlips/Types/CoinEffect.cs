using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.CustomItems.API.Features;
using LilinsCoinFlips.Configs;
using PlayerRoles;
using RemoteAdmin.Communication;
using UnityEngine;

namespace LilinsCoinFlips.Types
{
    public class CoinFlipEffect
    {
        private static Config Config => Plugin.Instance.Config;
        private static Configs.Translations Translations => Plugin.Instance.Translation;
        private static readonly System.Random Rd = new();

        public Action<Player> Execute { get; set; }
        public string Message { get; set; }

        public CoinFlipEffect(string message, Action<Player> execute)
        {
            Execute = execute;
            Message = message;
        }

        private static readonly Dictionary<string, string> _scpNames = new()
        {
            { "1 7 3", "SCP-173"},
            { "9 3 9", "SCP-939"},
            { "0 9 6", "SCP-096"},
            { "0 7 9", "SCP-079"},
            { "0 4 9", "SCP-049"},
            { "1 0 6", "SCP-106"}
        };

        private static bool flag1 = Config.RedCardChance > Rd.Next(1, 101);

        public static List<CoinFlipEffect> GoodEffects = new()
        {
            //0
            new CoinFlipEffect(Translations.MedikitMessage, player =>
            {
                Pickup.CreateAndSpawn(ItemType.Medkit, player.Position, new UnityEngine.Quaternion());
                Pickup.CreateAndSpawn(ItemType.Painkillers, player.Position, new UnityEngine.Quaternion());
            }),

            //1
            new CoinFlipEffect(Translations.RandomGoodEffectMessage, player =>
            {
                var effect = Config.GoodEffects.ToList().RandomItem();
                player.EnableEffect(effect, 5, true);
                Log.Debug($"Chosen random effect: {effect}");
            }),

            //2
            new CoinFlipEffect(Translations.SiletStepMessage, player =>
            {
                player.EnableEffect(Exiled.API.Enums.EffectType.SilentWalk, 255, 20f, false);
            }),

            //3
            new CoinFlipEffect(flag1 ? Translations.RedCardMessage : Translations.ContainmentEngineerCardMessage, player =>
            {
                Pickup.CreateAndSpawn(flag1 ? ItemType.KeycardFacilityManager : ItemType.KeycardContainmentEngineer, player.Position, new Quaternion());
            }),

            //4
            new CoinFlipEffect(Translations.TpToEscapeMessage, player =>
            {
                player.Teleport(Door.Get(DoorType.EscapePrimary));
            }),

            //5
            new CoinFlipEffect(Translations.MagicHealMessage, player =>
            {
                player.Heal(25);
            }),

            //6
            new CoinFlipEffect(Translations.HealthIncreaseMessage, player =>
            {
                player.Health *= 1.1f;
            }),

            //7
            new CoinFlipEffect(Translations.RandomItemMessage, player =>
            {
                Item.Create(Config.ItemsToGive.ToList().RandomItem()).CreatePickup(player.Position);
            }),

            //8
            new CoinFlipEffect(Translations.RandomCustomItemMessage, player =>
            {
                CustomItem.Get(Config.CustomItemIDs.GetRandomValue()).Spawn(player);
            }),
        };

        public static List<CoinFlipEffect> BadEffects = new()
        {
            //0
            new CoinFlipEffect(Translations.ShitPantsMessage, player =>
            {
                player.PlaceTantrum();
            }),

            //1
            new CoinFlipEffect(Translations.RandomRoomTPMessage, player =>
            {
                if (Warhead.IsDetonated)
                {
                    Scp330 candy = (Scp330) Item.Create(ItemType.SCP330);
                    candy.AddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Red);
                    candy.CreatePickup(player.Position);
                    return;
                }
                Room room = Room.Get(Config.RoomsToTeleport.GetRandomValue());
                Log.Debug($"Chosen room: {room.name}");
                player.Teleport(room);
            }),

            //2
            new CoinFlipEffect(Translations.InstaGrenadeMessage, player =>
            {
                float randomValue = UnityEngine.Random.Range(0.1f, 5f);

                ExplosiveGrenade instaBoom = (ExplosiveGrenade) Item.Create(ItemType.GrenadeHE);
                instaBoom.FuseTime = 0.1f;
                instaBoom.MaxRadius = randomValue;
                instaBoom.SpawnActive(player.Position, player);
            }),

            //3
            new CoinFlipEffect(Player.List.Count(x => x.IsAlive && !Config.PlayerSwapIgnoredRoles.Contains(x.Role.Type)) == 1 ? Translations.PlayerSwapMessage : Translations.PlayerSwapMessage, player =>
            {
                var playerList = Player.List.Where(x => x.IsAlive && !Config.PlayerSwapIgnoredRoles.Contains(x.Role.Type)).ToList();
                playerList.Remove(player);
                
                if (playerList.IsEmpty())
                {
                    return;
                }

                var targetPlayer = playerList.RandomItem();
                var pos = targetPlayer.Position;
                
                targetPlayer.Teleport(player.Position);
                player.Teleport(pos);
                
                EventHandlers.SendHint(targetPlayer, Translations.PlayerSwapMessage);
            }),
            
            //4
            new CoinFlipEffect(Translations.FentanylMessage, player =>
            {
                CustomItem.Get(100).Spawn(player);
            }),

            //5
            new CoinFlipEffect(Translations.RandomBadEffectMessage, player =>
            {
                var effect = Config.BadEffects.ToList().RandomItem();
                
                //prevents players from staying in PD infinitely
                if (effect == EffectType.PocketCorroding)
                    player.EnableEffect(EffectType.PocketCorroding);
                else
                    player.EnableEffect(effect, 5, true);

                Log.Debug($"Chosen random effect: {effect}");
            }),

            //6
            new CoinFlipEffect(Translations.BouncyBallMessage, player =>
            {
                int randomNumber = Rd.Next(3, 6);

                for (int i = 0; i < randomNumber; i++)
                {
                    Scp018Projectile test = (Scp018Projectile) Projectile.Create(ItemType.SCP018);
                    test.Spawn(player.Position + Vector3.up);
                    test.Activate();
                }
            }),

            //7
            new CoinFlipEffect(Translations.HpReductionMessage, player =>
            {
                if ((int) player.Health == 1)
                    player.Kill(DamageType.CardiacArrest);
                else
                    player.Health *= 0.7f;
            }),

            //8
            new CoinFlipEffect(Translations.HugeDamageMessage, player =>
            {
                if ((int) player.Health == 1)
                    player.Kill(DamageType.CardiacArrest);
                else
                    player.Health = 1;
            }),

            //9
            new CoinFlipEffect(Translations.PrimedVaseMessage, player =>
            {
                Scp244 vase = (Scp244)Item.Create(ItemType.SCP244a);
                vase.Primed = true;
                vase.CreatePickup(player.Position);
            }),

            //10
            new CoinFlipEffect(Translations.FakeScpKillMessage, player =>
            {
                var scpName = _scpNames.ToList().RandomItem();

                Cassie.MessageTranslated($"scp {scpName.Key} successfully terminated by automatic security system",
                    $"{scpName.Value} successfully terminated by Automatic Security System.");
            }),

            //11
            new CoinFlipEffect(Translations.InventoryResetMessage, player =>
            {
                player.DropHeldItem();
                player.ClearInventory();
            }),

            //12
            new CoinFlipEffect(Player.List.Where(x => x.Role.Type == RoleTypeId.Spectator).IsEmpty() ? Translations.SpectSwapNoSpectsMessage : Translations.SpectSwapPlayerMessage, player =>
            {
                var spectList = Player.List.Where(x => x.Role.Type == RoleTypeId.Spectator).ToList();

                if (spectList.IsEmpty())
                {
                    return;
                }

                var spect = spectList.RandomItem();

                spect.Role.Set(player.Role.Type, RoleSpawnFlags.None);
                spect.Teleport(player);
                spect.Health = player.Health;

                List<ItemType> playerItems = player.Items.Select(item => item.Type).ToList();

                foreach (var item in playerItems)
                {
                    spect.AddItem(item);
                }
                
                
                //give spect the players ammo, has to be done before ClearInventory() or else ammo will fall on the floor
                for (int i = 0; i < player.Ammo.Count; i++)
                {
                    spect.AddAmmo(player.Ammo.ElementAt(i).Key.GetAmmoType(), player.Ammo.ElementAt(i).Value);
                    player.SetAmmo(player.Ammo.ElementAt(i).Key.GetAmmoType(), 0);
                }

                player.ClearInventory();
                player.Role.Set(RoleTypeId.Spectator);

                EventHandlers.SendHint(spect, Translations.SpectSwapSpectMessage);
            }),

            //13
            new CoinFlipEffect(Player.List.Where(x => !Config.InventorySwapIgnoredRoles.Contains(x.Role.Type)).Count(x => x.IsAlive) <= 1 ? Translations.InventorySwapOnePlayerMessage : Translations.InventorySwapMessage, player =>
            {
                List<Player> playerList = Player.List.Where(x => x != player && !Config.InventorySwapIgnoredRoles.Contains(x.Role.Type)).ToList();

                if (playerList.Count(x => x.IsAlive) <= 1)
                {
                    player.Hurt(50);
                    return;
                }

                var target = playerList.Where(x => x != player).ToList().RandomItem();

                // Saving items
                List<ItemType> items1 = player.Items.Select(item => item.Type).ToList();
                List<ItemType> items2 = target.Items.Select(item => item.Type).ToList();

                // Saving and removing ammo
                Dictionary<AmmoType, ushort> ammo1 = new();
                Dictionary<AmmoType, ushort> ammo2 = new();
                for (int i = 0; i < player.Ammo.Count; i++)
                {
                    ammo1.Add(player.Ammo.ElementAt(i).Key.GetAmmoType(), player.Ammo.ElementAt(i).Value);
                    player.SetAmmo(ammo1.ElementAt(i).Key, 0);
                }
                for (int i = 0; i < target.Ammo.Count; i++)
                {
                    ammo2.Add(target.Ammo.ElementAt(i).Key.GetAmmoType(), target.Ammo.ElementAt(i).Value);
                    target.SetAmmo(ammo2.ElementAt(i).Key, 0);
                }

                // setting items
                target.ResetInventory(items1);
                player.ResetInventory(items2);

                // setting ammo
                foreach (var ammo in ammo2)
                {
                    player.SetAmmo(ammo.Key, ammo.Value);
                }
                foreach (var ammo in ammo1)
                {
                    target.SetAmmo(ammo.Key, ammo.Value);
                }

                EventHandlers.SendHint(target, Translations.InventorySwapMessage);
            }),
        
            //14
            new CoinFlipEffect("Jumpscare :3", player =>
            {
                
            }),
        };
    }
}
