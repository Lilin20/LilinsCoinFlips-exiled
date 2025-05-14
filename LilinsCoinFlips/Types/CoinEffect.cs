using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CustomPlayerEffects;
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
using PlayerStatsSystem;
using RemoteAdmin.Communication;
using UnityEngine;
using Utf8Json.Internal;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LilinsCoinFlips.Types
{
    public class CoinFlipEffect
    {
        public Action<Player> Execute { get; set; }
        public string Message { get; set; }
        public int Chance { get; set; }

        public CoinFlipEffect(string message, Action<Player> execute, int chance)
        {
            Execute = execute;
            Message = message;
            Chance = chance;
        }

        public static Action<Player> GetActionFromName(string actionName, Dictionary<string, object> parameters)
        {
            Log.Debug($"GetActionFromName called with actionName: {actionName}");

            return actionName switch
            {
                "SpawnItems" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("items", out var items) && items is IEnumerable<object> rawList)
                    {
                        Log.Debug($"Found 'items' parameter with {rawList.Count()} items.");

                        foreach (var itemObj in rawList)
                        {
                            var itemStr = itemObj.ToString();
                            Log.Debug($"Attempting to spawn item: {itemStr}");

                            Log.Debug($"Player details: {player.ToString()}");

                            if (Enum.TryParse<ItemType>(itemStr, out var itemEnum))
                            {
                                Log.Debug($"Successfully parsed ItemType: {itemEnum}");
                                Pickup.CreateAndSpawn(itemEnum, player.Position, UnityEngine.Quaternion.identity);
                                Log.Debug($"Spawned {itemEnum} at {player.Position}");
                            }
                            else
                            {
                                Log.Debug($"Invalid ItemType: {itemStr}");
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("'items' parameter not found or invalid format.");
                    }
                },
                "SpawnCustomItems" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("items", out var items) && items is IEnumerable<object> rawList)
                    {
                        Log.Debug($"Found 'customitems' parameter with {rawList.Count()} items.");

                        foreach (var itemObj in rawList)
                        {
                            var itemStr = itemObj.ToString();
                            Log.Debug($"Attempting to spawn item: {itemStr}");

                            Log.Debug($"Player details: {player.ToString()}");

                            if (Enum.TryParse<ItemType>(itemStr, out var itemEnum))
                            {
                                Log.Debug($"Successfully parsed CustomItem: {itemEnum}");
                                CustomItem.TrySpawn(itemStr, player.Position, out Pickup pickup);
                                Log.Debug($"Spawned {itemEnum} at {player.Position}");
                            }
                            else
                            {
                                Log.Debug($"Invalid CustomItem: {itemStr}");
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("'customitems' parameter not found or invalid format.");
                    }
                },
                "TeleportToRoom" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("room", out var location) && location is string loc)
                    {
                        Log.Debug($"Teleporting player to location: {loc}");
                        player.Teleport(Room.Get((RoomType)Enum.Parse(typeof(RoomType), loc)));
                        Log.Debug("Player teleported.");
                    }
                    else
                    {
                        Log.Debug("'location' parameter not found or invalid format.");
                    }
                },
                "TeleportRandom" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("possiblerooms", out var roomListRaw) && roomListRaw is IEnumerable<object> roomList)
                    {
                        var random = new System.Random();
                        var roomArray = roomList.ToArray();

                        if (roomArray.Length == 0)
                        {
                            Log.Debug("Room list is empty.");
                            return;
                        }

                        var randomRoom = roomArray[random.Next(roomArray.Length)];

                        if (randomRoom != null)
                        {
                            var roomStr = randomRoom.ToString();
                            Log.Debug($"Selected random room string: {roomStr}");

                            if (Enum.TryParse<RoomType>(roomStr, out var roomType))
                            {
                                var targetRoom = Room.Get(roomType);
                                if (targetRoom != null)
                                {
                                    player.Teleport(targetRoom);
                                    Log.Debug($"Player {player.Nickname} teleported to room: {roomType}");
                                }
                                else
                                {
                                    Log.Debug($"RoomType {roomType} resolved, but Room.Get returned null.");
                                }
                            }
                            else
                            {
                                Log.Debug($"Invalid RoomType string: {roomStr}");
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("'rooms' parameter not found or invalid format.");
                    }
                },
                "RandomEffect" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("effects", out var effects) && effects is IEnumerable<object> effectList)
                    {
                        Log.Debug($"Found 'effects' parameter with {effectList.Count()} items.");

                        // Zufälligen Effekt aus der Liste auswählen
                        var random = new System.Random();
                        var randomEffect = effectList.ElementAtOrDefault(random.Next(effectList.Count()));

                        if (randomEffect != null)
                        {
                            var effectStr = randomEffect.ToString();
                            Log.Debug($"Attempting to apply effect: {effectStr}");

                            Log.Debug($"Player details: {player.ToString()}");

                            if (Enum.TryParse<EffectType>(effectStr, out var effectEnum))
                            {
                                Log.Debug($"Successfully parsed EffectType: {effectEnum}");
                                player.EnableEffect(effectEnum, 5, true);
                                Log.Debug($"Given effect {effectEnum} to {player.DisplayNickname}");
                            }
                            else
                            {
                                Log.Debug($"Invalid EffectType: {effectStr}");
                            }
                        }
                        else
                        {
                            Log.Warn("No valid effects found in the list.");
                        }
                    }
                    else
                    {
                        Log.Warn("'effects' parameter not found or invalid format.");
                    }
                },
                "Hurt" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    if (parameters.TryGetValue("amount", out var ha) && ha is float hurtAmount)
                    {
                        Log.Debug($"Hurting player {player.DisplayNickname}");
                        player.Hurt(hurtAmount);
                    }
                    else
                    {
                        Log.Debug("'amount' parameter not found or invalid format.");
                    }
                },
                "Handcuff" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    Log.Debug($"Trying to handcuff {player.DisplayNickname}");
                    player.Handcuff();
                },
                "Warhead" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    Log.Debug($"Trying to activate warhead...");
                    if (Warhead.IsDetonated || !Warhead.IsInProgress)
                    {
                        Warhead.Start();
                    }
                },
                "SwapPosition" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    var playerList = Player.List.Where(p => p.IsAlive).ToList();
                    playerList.Remove(player);
                    playerList.Remove(playerList.FirstOrDefault(p => p.Role.Type == RoleTypeId.Scp079));

                    var randomPlayer = playerList.RandomItem();
                    var randomPlayerPos = randomPlayer.Position;

                    randomPlayer.Teleport(player.Position);
                    player.Teleport(randomPlayerPos);
                },
                "Explode" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                    grenade.FuseTime = 0.1f;
                    grenade.SpawnActive(player.Position, player);
                },
                "SpawnTantrum" => player =>
                {
                    Log.Debug($"Handling '{actionName}' action...");

                    player.PlaceTantrum();
                },
                _ => player => Log.Debug($"Unknown action: {actionName}")
            };
        }
    }
}
